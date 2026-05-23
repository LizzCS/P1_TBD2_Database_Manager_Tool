using Database_Manager_TBD2.Backend;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Database_Manager_TBD2
{
    public class Main : Form
    {
        private SplitContainer mainSplit;
        private SplitContainer rightSplit;

        private ContextMenuStrip dbMenu;
        private ContextMenuStrip tableMenu;

        private TreeView treeDatabase;
        private RichTextBox txtQueryEditor;
        private DataGridView dgvResults;

        private Button btnExecute;
        private Button btnNewQuery;
        private Button btnRefresh;

        private Label lblCurrentView;

        private Conexion con;
        private Backend.TableView.Metadata metadata;

        private TreeNode selectedTableNode;

        public Main(Conexion connection)
        {
            con = connection;
            metadata = new Backend.TableView.Metadata(con);

            InitializeComponent();
            LoadTree();
        }

        private void InitializeComponent()
        {
            Text = "Database Manager";
            Width = 1400;
            Height = 800;

            BackColor = Color.FromArgb(18, 18, 18);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10);

            // MAIN SPLIT
            mainSplit = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 300
            };

            mainSplit.Panel1.BackColor = Color.FromArgb(28, 28, 28);
            mainSplit.Panel2.BackColor = Color.FromArgb(18, 18, 18);

            // TREE
            treeDatabase = new TreeView()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            treeDatabase.NodeMouseClick += TreeDatabase_NodeMouseClick;
            treeDatabase.NodeMouseDoubleClick += TreeDatabase_NodeMouseDoubleClick;

            mainSplit.Panel1.Controls.Add(treeDatabase);

            // RIGHT SPLIT
            rightSplit = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 320
            };

            // QUERY PANEL
            Panel queryPanel = new Panel()
            {
                Dock = DockStyle.Fill
            };

            // BUTTON BAR
            FlowLayoutPanel buttonBar = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = 45,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(25, 25, 25),
                Padding = new Padding(6)
            };

            btnNewQuery = new Button()
            {
                Text = "New Query",
                Width = 120,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White
            };

            btnNewQuery.FlatAppearance.BorderSize = 0;
            btnNewQuery.Click += (s, e) =>
            {
                txtQueryEditor.Text = "";
                lblCurrentView.Text = "New Query";
            };

            btnExecute = new Button()
            {
                Text = "Execute",
                Width = 120,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };

            btnExecute.FlatAppearance.BorderSize = 0;
            btnExecute.Click += BtnExecute_Click;

            btnRefresh = new Button()
            {
                Text = "Refresh",
                Width = 120,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White
            };

            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => RefreshTree();

            buttonBar.Controls.Add(btnNewQuery);
            buttonBar.Controls.Add(btnExecute);
            buttonBar.Controls.Add(btnRefresh);

            // EDITOR
            txtQueryEditor = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 11),
                Text = "SELECT * FROM Users;"
            };

            queryPanel.Controls.Add(txtQueryEditor);
            queryPanel.Controls.Add(buttonBar);

            // CURRENT VIEW LABEL
            lblCurrentView = new Label()
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "Ready",
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(25, 25, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // GRID
            dgvResults = new DataGridView()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(35, 35, 35),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvResults.DefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(0, 120, 215),
                SelectionForeColor = Color.White
            };

            rightSplit.Panel1.Controls.Add(queryPanel);

            rightSplit.Panel2.Controls.Add(dgvResults);
            rightSplit.Panel2.Controls.Add(lblCurrentView);

            dgvResults.BringToFront();

            mainSplit.Panel2.Controls.Add(rightSplit);

            Controls.Add(mainSplit);

            // CONTEXT MENUS

            dbMenu = new ContextMenuStrip();
            dbMenu.Items.Add("New Query", null, (s, e) =>
            {
                txtQueryEditor.Text = "";
                lblCurrentView.Text = "New Query";
            });

            dbMenu.Items.Add("Refresh", null, (s, e) => RefreshTree());

            tableMenu = new ContextMenuStrip();

            tableMenu.Items.Add("View Table", null, (s, e) => OpenTableData());

            tableMenu.Items.Add("View DDL", null, (s, e) => ShowDDL());

            tableMenu.Items.Add("View Structure", null, (s, e) => ShowStructure());
        }

        // =========================================================
        // TREE LOAD
        // =========================================================
        private void LoadTree()
        {
            treeDatabase.Nodes.Clear();

            TreeNode root = new TreeNode(con.Database);

            treeDatabase.Nodes.Add(root);

            TreeNode EmptyNode(string text = "(vacio)")
            {
                return new TreeNode(text)
                {
                    ForeColor = Color.Gray
                };
            }

            // TABLES
            TreeNode tablesNode = new TreeNode("Tables");

            root.Nodes.Add(tablesNode);

            DataTable tables = metadata.GetTables();

            if (tables.Rows.Count == 0)
            {
                tablesNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in tables.Rows)
                {
                    string schema = r["SchemaName"].ToString();
                    string name = r["ObjectName"].ToString();

                    TreeNode tableNode = new TreeNode($"{schema}.{name}");

                    tableNode.Tag = new Tuple<string, string>(schema, name);

                    tablesNode.Nodes.Add(tableNode);

                    DataTable cols = metadata.GetColumns(schema, name);

                    if (cols.Rows.Count == 0)
                    {
                        tableNode.Nodes.Add(EmptyNode());
                    }
                    else
                    {
                        foreach (DataRow c in cols.Rows)
                        {
                            tableNode.Nodes.Add(
                                $"   {c["ColumnName"]} ({c["DataType"]})");
                        }
                    }
                }
            }

            // VIEWS
            TreeNode viewsNode = new TreeNode("Views");

            root.Nodes.Add(viewsNode);

            DataTable views = metadata.GetViews();

            if (views.Rows.Count == 0)
            {
                viewsNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in views.Rows)
                {
                    viewsNode.Nodes.Add(
                        $"{r["SchemaName"]}.{r["ObjectName"]}");
                }
            }

            // PROCEDURES
            TreeNode procNode = new TreeNode("Procedures");

            root.Nodes.Add(procNode);

            DataTable procs = metadata.GetProcedures();

            if (procs.Rows.Count == 0)
            {
                procNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in procs.Rows)
                {
                    procNode.Nodes.Add(
                        $"{r["SchemaName"]}.{r["ObjectName"]}");
                }
            }

            // FUNCTIONS
            TreeNode funcNode = new TreeNode("Functions");

            root.Nodes.Add(funcNode);

            DataTable funcs = metadata.GetFunctions();

            if (funcs.Rows.Count == 0)
            {
                funcNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in funcs.Rows)
                {
                    funcNode.Nodes.Add(
                        $"{r["SchemaName"]}.{r["ObjectName"]}");
                }
            }

            // TRIGGERS
            TreeNode trigNode = new TreeNode("Triggers");

            root.Nodes.Add(trigNode);

            DataTable trigs = metadata.GetTriggers();

            if (trigs.Rows.Count == 0)
            {
                trigNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in trigs.Rows)
                {
                    trigNode.Nodes.Add(
                        $"{r["SchemaName"]}.{r["ObjectName"]}");
                }
            }

            // INDEXES
            TreeNode indexNode = new TreeNode("Indexes");

            root.Nodes.Add(indexNode);

            DataTable idx = metadata.GetIndexes();

            if (idx.Rows.Count == 0)
            {
                indexNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in idx.Rows)
                {
                    indexNode.Nodes.Add(
                        $"{r["SchemaName"]}.{r["TableName"]}.{r["IndexName"]}");
                }
            }

            // USERS
            TreeNode userNode = new TreeNode("Users");

            root.Nodes.Add(userNode);

            DataTable users = metadata.GetUsers();

            if (users.Rows.Count == 0)
            {
                userNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in users.Rows)
                {
                    userNode.Nodes.Add(
                        $"{r["UserName"]} ({r["UserType"]})");
                }
            }

            root.Expand();
        }

        // =========================================================
        // TREE EVENTS
        // =========================================================
        private void TreeDatabase_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            selectedTableNode = e.Node;

            treeDatabase.SelectedNode = e.Node;

            if (e.Node.Parent == null)
            {
                treeDatabase.ContextMenuStrip = dbMenu;
                return;
            }

            if (e.Node.Tag is Tuple<string, string>)
                treeDatabase.ContextMenuStrip = tableMenu;
            else
                treeDatabase.ContextMenuStrip = null;
        }

        private void TreeDatabase_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is Tuple<string, string> t)
                OpenTableData(t.Item1, t.Item2);
        }

        // =========================================================
        // ACTIONS
        // =========================================================

        private void ShowStructure()
        {
            if (selectedTableNode?.Tag is not Tuple<string, string> t)
                return;

            try
            {
                string schema = t.Item1;
                string table = t.Item2;

                DataTable cols = metadata.GetColumns(schema, table);

                lblCurrentView.Text =
                    $"Structure: [{schema}].[{table}]";

                dgvResults.DataSource = cols;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenTableData()
        {
            if (selectedTableNode?.Tag is not Tuple<string, string> t)
                return;

            OpenTableData(t.Item1, t.Item2);
        }

        private void OpenTableData(string schema, string table)
        {
            try
            {
                lblCurrentView.Text =
                    $"Table Data: [{schema}].[{table}]";

                dgvResults.DataSource =
                    con.ExecuteSelect(
                        $"SELECT TOP 100 * FROM [{schema}].[{table}]");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshTree()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                metadata = new Backend.TableView.Metadata(con);

                LoadTree();

                lblCurrentView.Text = "Database Tree Refreshed";

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;

                MessageBox.Show(ex.Message);
            }
        }

        private void ShowDDL()
        {
            if (selectedTableNode?.Tag is not Tuple<string, string> t)
                return;

            txtQueryEditor.Text =
                $"-- DDL not implemented yet for {t.Item1}.{t.Item2}";

            lblCurrentView.Text =
                $"DDL: [{t.Item1}].[{t.Item2}]";
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                lblCurrentView.Text = "Query Results";

                dgvResults.DataSource =
                    con.ExecuteSelect(txtQueryEditor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}