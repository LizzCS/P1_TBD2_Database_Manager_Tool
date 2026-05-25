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
        private ContextMenuStrip objectMenu;
        private ContextMenuStrip indexMenu;
        private ContextMenuStrip categoryMenu;  

        private TreeView treeDatabase;
        private RichTextBox txtQueryEditor;
        private DataGridView dgvResults;

        private Button btnExecute;
        private Button btnNewQuery;
        private Button btnRefresh;

        private Label lblCurrentView;

        private Conexion con;
        private Backend.TableView.Metadata metadata;
        private Backend.TableView.DDL ddl;

        private TreeNode selectedTableNode;

        public Main(Conexion connection)
        {
            con = connection;
            metadata = new Backend.TableView.Metadata(con);
            ddl = new Backend.TableView.DDL(con);

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

            mainSplit = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 5
            };

            mainSplit.Panel1.BackColor = Color.FromArgb(28, 28, 28);
            mainSplit.Panel2.BackColor = Color.FromArgb(18, 18, 18);

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

            rightSplit = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 320
            };

            Panel queryPanel = new Panel()
            {
                Dock = DockStyle.Fill
            };

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

            objectMenu = new ContextMenuStrip();
            objectMenu.Items.Add("View DDL", null, (s, e) => ShowDDL());

            indexMenu = new ContextMenuStrip();
            indexMenu.Items.Add("View DDL", null, (s, e) => ShowDDL());
            
            dbMenu.Items.Add(new ToolStripSeparator());

            dbMenu.Items.Add("New Table...", null, (s, e) =>
            {
                using (var designer = new CreateTable(con))
                {
                    if (designer.ShowDialog() == DialogResult.OK)
                        RefreshTree();
                }
            });

            dbMenu.Items.Add("New View...", null, (s, e) =>
            {
                using (var designer = new CreateView(con))
                {
                    if (designer.ShowDialog() == DialogResult.OK)
                        RefreshTree();
                }
            });

            categoryMenu = new ContextMenuStrip();

            categoryMenu.Items.Add("New Table...", null, (s, e) =>
            {
                using (var designer = new CreateTable(con))
                {
                    if (designer.ShowDialog() == DialogResult.OK)
                        RefreshTree();
                }
            });

            categoryMenu.Items.Add("New View...", null, (s, e) =>
            {
                using (var designer = new CreateView(con))
                {
                    if (designer.ShowDialog() == DialogResult.OK)
                        RefreshTree();
                }
            });

            this.FormClosing += Main_FormClosing;
        }

        private void LoadTree()
        {
            treeDatabase.Nodes.Clear();

            TreeNode root = new TreeNode(con.Database);
            treeDatabase.Nodes.Add(root);

            TreeNode EmptyNode(string text = "(vacio)") =>
                new TreeNode(text) { ForeColor = Color.Gray };

            TreeNode tablesNode = new TreeNode("Tables") { Tag = "CATEGORY_TABLES" };
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
                    tableNode.Tag = new Tuple<string, string, string>("TABLE", schema, name);
                    tablesNode.Nodes.Add(tableNode);

                    DataTable cols = metadata.GetColumns(schema, name);

                    if (cols.Rows.Count == 0)
                    {
                        tableNode.Nodes.Add(EmptyNode());
                    }
                    else
                    {
                        foreach (DataRow c in cols.Rows)
                            tableNode.Nodes.Add($"   {c["ColumnName"]} ({c["DataType"]})");
                    }
                }
            }

            TreeNode viewsNode = new TreeNode("Views") { Tag = "CATEGORY_VIEWS" };
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
                    string schema = r["SchemaName"].ToString();
                    string name = r["ObjectName"].ToString();

                    TreeNode node = new TreeNode($"{schema}.{name}");
                    node.Tag = new Tuple<string, string, string>("VIEW", schema, name);
                    viewsNode.Nodes.Add(node);
                }
            }

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
                    string schema = r["SchemaName"].ToString();
                    string name = r["ObjectName"].ToString();

                    TreeNode node = new TreeNode($"{schema}.{name}");
                    node.Tag = new Tuple<string, string, string>("PROC", schema, name);
                    procNode.Nodes.Add(node);
                }
            }

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
                    string schema = r["SchemaName"].ToString();
                    string name = r["ObjectName"].ToString();

                    TreeNode node = new TreeNode($"{schema}.{name}");
                    node.Tag = new Tuple<string, string, string>("FUNC", schema, name);
                    funcNode.Nodes.Add(node);
                }
            }

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
                    string schema = r["SchemaName"].ToString();
                    string name = r["ObjectName"].ToString();

                    TreeNode node = new TreeNode($"{schema}.{name}");
                    node.Tag = new Tuple<string, string, string>("TRIGGER", schema, name);
                    trigNode.Nodes.Add(node);
                }
            }

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
                    string schema = r["SchemaName"].ToString();
                    string tableName = r["TableName"].ToString();
                    string indexName = r["IndexName"].ToString();

                    TreeNode node = new TreeNode($"{schema}.{tableName}.{indexName}");
                    node.Tag = new Tuple<string, string, string, string>("INDEX", schema, tableName, indexName);
                    indexNode.Nodes.Add(node);
                }
            }

            TreeNode seqNode = new TreeNode("Sequences");
            root.Nodes.Add(seqNode);

            DataTable seqs = metadata.GetSequences();

            if (seqs.Rows.Count == 0)
            {
                seqNode.Nodes.Add(EmptyNode());
            }
            else
            {
                foreach (DataRow r in seqs.Rows)
                {
                    string schema = r["SchemaName"].ToString();
                    string name = r["SequenceName"].ToString();

                    TreeNode node = new TreeNode($"{schema}.{name}");

                    node.Tag = new Tuple<string, string, string>(
                        "SEQUENCE",
                        schema,
                        name);

                    seqNode.Nodes.Add(node);
                }
            }

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
                    userNode.Nodes.Add($"{r["UserName"]} ({r["UserType"]})");
            }

            root.Expand();
        }


        private void TreeDatabase_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            selectedTableNode = e.Node;
            treeDatabase.SelectedNode = e.Node;

            if (e.Node.Parent == null)
            {
                treeDatabase.ContextMenuStrip = dbMenu;
                return;
            }

            switch (e.Node.Tag)
            {
                case "CATEGORY_TABLES":
                    categoryMenu.Items[0].Visible = true;    
                    categoryMenu.Items[1].Visible = false;
                    treeDatabase.ContextMenuStrip = categoryMenu;
                    break;

                case "CATEGORY_VIEWS":
                    categoryMenu.Items[0].Visible = false;  
                    categoryMenu.Items[1].Visible = true;   
                    treeDatabase.ContextMenuStrip = categoryMenu;
                    break;

                case Tuple<string, string, string> t when t.Item1 == "TABLE":
                    treeDatabase.ContextMenuStrip = tableMenu;
                    break;

                case Tuple<string, string, string> t
                    when t.Item1 == "VIEW"
                      || t.Item1 == "PROC"
                      || t.Item1 == "FUNC"
                      || t.Item1 == "TRIGGER"
                      || t.Item1 == "SEQUENCE":
                    treeDatabase.ContextMenuStrip = objectMenu;
                    break;

                case Tuple<string, string, string, string>:
                    treeDatabase.ContextMenuStrip = indexMenu;
                    break;

                default:
                    treeDatabase.ContextMenuStrip = null;
                    break;
            }
        }


        private void TreeDatabase_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is Tuple<string, string, string> t && t.Item1 == "TABLE")
                OpenTableData(t.Item2, t.Item3);
        }

        private void ShowStructure()
        {
            if (selectedTableNode?.Tag is not Tuple<string, string, string> t || t.Item1 != "TABLE")
                return;

            try
            {
                DataTable cols = metadata.GetColumns(t.Item2, t.Item3);
                lblCurrentView.Text = $"Structure: [{t.Item2}].[{t.Item3}]";
                dgvResults.DataSource = cols;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenTableData()
        {
            if (selectedTableNode?.Tag is not Tuple<string, string, string> t || t.Item1 != "TABLE")
                return;

            OpenTableData(t.Item2, t.Item3);
        }

        private void OpenTableData(string schema, string table)
        {
            try
            {
                lblCurrentView.Text = $"Table Data: [{schema}].[{table}]";
                dgvResults.DataSource =
                    con.ExecuteSelect($"SELECT TOP 100 * FROM [{schema}].[{table}]");
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
            if (selectedTableNode?.Tag == null) return;

            try
            {
                string ddlText;
                string label;

                switch (selectedTableNode.Tag)
                {
                    case Tuple<string, string, string> t when t.Item1 == "TABLE":
                        ddlText = ddl.GetTableDDL(t.Item2, t.Item3);
                        label = $"DDL: [{t.Item2}].[{t.Item3}]";
                        break;

                    case Tuple<string, string, string> t when t.Item1 == "VIEW":
                        ddlText = ddl.GetViewDDL(t.Item2, t.Item3);
                        label = $"DDL: [{t.Item2}].[{t.Item3}]";
                        break;

                    case Tuple<string, string, string> t when t.Item1 == "PROC":
                        ddlText = ddl.GetProcedureDDL(t.Item2, t.Item3);
                        label = $"DDL: [{t.Item2}].[{t.Item3}]";
                        break;

                    case Tuple<string, string, string> t when t.Item1 == "FUNC":
                        ddlText = ddl.GetFunctionDDL(t.Item2, t.Item3);
                        label = $"DDL: [{t.Item2}].[{t.Item3}]";
                        break;

                    case Tuple<string, string, string> t when t.Item1 == "TRIGGER":
                        ddlText = ddl.GetTriggerDDL(t.Item2, t.Item3);
                        label = $"DDL: [{t.Item2}].[{t.Item3}]";
                        break;

                    case Tuple<string, string, string, string> t:
                        ddlText = ddl.GetIndexDDL(t.Item2, t.Item3, t.Item4);
                        label = $"DDL: [{t.Item2}].[{t.Item3}].[{t.Item4}]";
                        break;

                    default:
                        return;
                }

                txtQueryEditor.Text = ddlText ?? "-- No DDL found for this object.";
                lblCurrentView.Text = label;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                lblCurrentView.Text = "Query Results";

                dgvResults.DataSource = con.ExecuteSelect(txtQueryEditor.Text);

                MessageBox.Show("Query executed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Desconectarse de la base de datos y volver al menu principal?",
                "Desconectar",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel || result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                this.Hide();
                Menu menu = new Menu();
                menu.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                e.Cancel = true;
            }
        }
    }
}