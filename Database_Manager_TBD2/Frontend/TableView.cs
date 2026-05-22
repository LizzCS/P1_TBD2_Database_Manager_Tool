using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Database_Manager_TBD2
{
    public class TableView : Form
    {
        private SplitContainer mainSplit;
        private SplitContainer rightSplit;

        private TreeView treeDatabase;

        private RichTextBox txtQueryEditor;

        private DataGridView dgvResults;

        private Button btnExecute;

        public TableView ()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // FORM

            this.Text = "Database Manager";
            this.Width = 1400;
            this.Height = 800;

            this.BackColor =
                Color.FromArgb(18, 18, 18);

            this.ForeColor = Color.White;

            this.Font =
                new Font("Segoe UI", 10);

            // MAIN SPLIT

            mainSplit = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 5,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            mainSplit.Panel1.BackColor =
                Color.FromArgb(28, 28, 28);

            mainSplit.Panel2.BackColor =
                Color.FromArgb(18, 18, 18);

            // DATABASE TREE

            treeDatabase = new TreeView()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };

            TreeNode serverNode =
                new TreeNode("localhost")
                {
                    ForeColor = Color.White
                };

            TreeNode dbNode =
                new TreeNode("Databases");

            dbNode.Nodes.Add("master");
            dbNode.Nodes.Add("tempdb");
            dbNode.Nodes.Add("MyDatabase");

            TreeNode tablesNode =
                new TreeNode("Tables");

            tablesNode.Nodes.Add("Users");
            tablesNode.Nodes.Add("Orders");
            tablesNode.Nodes.Add("Products");

            serverNode.Nodes.Add(dbNode);
            serverNode.Nodes.Add(tablesNode);
            serverNode.Nodes.Add("Views");
            serverNode.Nodes.Add("Functions");

            treeDatabase.Nodes.Add(serverNode);

            serverNode.Expand();

            mainSplit.Panel1.Controls.Add(treeDatabase);

            // RIGHT SPLIT

            rightSplit = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 320,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            // QUERY PANEL

            Panel queryPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            // EXECUTE BUTTON

            btnExecute = new Button()
            {
                Text = "Execute",
                Height = 42,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnExecute.FlatAppearance.BorderSize = 0;

            btnExecute.Click += BtnExecute_Click;

            // QUERY EDITOR

            txtQueryEditor = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 12),
                Text =
@"SELECT *
FROM Users;"
            };

            queryPanel.Controls.Add(txtQueryEditor);
            queryPanel.Controls.Add(btnExecute);

            // RESULTS GRID

            dgvResults = new DataGridView()
            {
                Dock = DockStyle.Fill,
                BackgroundColor =
                    Color.FromArgb(35, 35, 35),

                BorderStyle = BorderStyle.None,

                EnableHeadersVisualStyles = false,

                GridColor =
                    Color.FromArgb(60, 60, 60),

                RowHeadersVisible = false,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvResults.DefaultCellStyle =
                new DataGridViewCellStyle()
                {
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    SelectionBackColor =
                        Color.FromArgb(0, 120, 215),

                    SelectionForeColor = Color.White
                };

            dgvResults.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle()
                {
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White
                };

            // ADD SPLITS

            rightSplit.Panel1.Controls.Add(queryPanel);

            rightSplit.Panel2.Controls.Add(dgvResults);

            mainSplit.Panel2.Controls.Add(rightSplit);

            // FORM

            this.Controls.Add(mainSplit);
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            // DEMO DATA

            DataTable table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("Email");

            table.Rows.Add("1", "John", "john@test.com");
            table.Rows.Add("2", "Maria", "maria@test.com");
            table.Rows.Add("3", "David", "david@test.com");

            dgvResults.DataSource = table;
        }
    }
}