using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Database_Manager_TBD2.Backend;

namespace Database_Manager_TBD2
{

    public class CreateView : Form
    {

        private SplitContainer mainSplit;  
        private RichTextBox txtQuery;
        private DataGridView dgvPreview;
        private Panel pnlTop;
        private Panel pnlBottom;
        private TextBox txtSchema;
        private TextBox txtViewName;
        private Label lblSchema;
        private Label lblViewName;
        private Button btnPreview;
        private Button btnSave;
        private Button btnCancel;
        private Label lblPreviewStatus;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        private readonly Conexion con;
        private readonly string originalSchema;
        private readonly string originalView;
        private readonly bool isEdit;

        public CreateView(Conexion connection, string schema = null, string viewName = null)
        {
            con = connection;
            originalSchema = schema;
            originalView = viewName;
            isEdit = schema != null && viewName != null;

            InitializeComponent();

            if (isEdit)
                LoadExistingView();
            else
                txtQuery.Text = BuildTemplate();
        }

        private void InitializeComponent()
        {
            Text = isEdit ? $"View Designer — [{originalSchema}].[{originalView}]": "New View Designer";
            Width = 940;
            Height = 700;
            MinimumSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(18, 18, 18);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);

            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(28, 28, 28),
                Padding = new Padding(8)
            };

            lblSchema = new Label
            {
                Text = "Schema:",
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(10, 16)
            };

            txtSchema = new TextBox
            {
                Text = isEdit ? originalSchema : "dbo",
                Width = 100,
                Location = new Point(80, 12),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblViewName = new Label
            {
                Text = "View Name:",
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(188, 16)
            };

            txtViewName = new TextBox
            {
                Text = isEdit ? originalView : "",
                Width = 220,
                Location = new Point(288, 12),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnPreview = new Button
            {
                Text = "▶  Preview (F5)",
                Width = 130,
                Height = 26,
                Location = new Point(520, 11),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 140, 70),
                ForeColor = Color.White
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Click += BtnPreview_Click;

            pnlTop.Controls.AddRange(new Control[]
            {
                lblSchema, txtSchema, lblViewName, txtViewName, btnPreview
            });

            mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 320,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            txtQuery = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(220, 220, 170),
                Font = new Font("Consolas", 11f),
                BorderStyle = BorderStyle.None
            };
            txtQuery.KeyDown += TxtQuery_KeyDown;

            mainSplit.Panel1.Controls.Add(txtQuery);
            mainSplit.Panel1.BackColor = Color.FromArgb(30, 30, 30);

            lblPreviewStatus = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                Text = "Results",
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(28, 28, 28),
                Padding = new Padding(8, 4, 0, 0)
            };

            dgvPreview = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(35, 35, 35),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            dgvPreview.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(0, 84, 166),
                SelectionForeColor = Color.White
            };

            dgvPreview.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            mainSplit.Panel2.Controls.Add(dgvPreview);
            mainSplit.Panel2.Controls.Add(lblPreviewStatus);
            mainSplit.Panel2.BackColor = Color.FromArgb(28, 28, 28);

            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                BackColor = Color.FromArgb(25, 25, 25)
            };

            btnSave = new Button
            {
                Text = "Save View",
                Width = 110,
                Height = 30,
                Location = new Point(8, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Width = 90,
                Height = 30,
                Location = new Point(126, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            pnlBottom.Controls.AddRange(new Control[] { btnSave, btnCancel });

            statusStrip = new StatusStrip { BackColor = Color.FromArgb(25, 25, 25) };
            lblStatus = new ToolStripStatusLabel
            {
                Text = "Ready  —  F5 to preview",
                ForeColor = Color.LightGray
            };
            statusStrip.Items.Add(lblStatus);

            Controls.Add(mainSplit);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Controls.Add(statusStrip);
        }

        private void LoadExistingView()
        {
            try
            {
                string sql = $@"
                SELECT OBJECT_DEFINITION(OBJECT_ID(
                    QUOTENAME('{EscapeSql(originalSchema)}') + '.' + QUOTENAME('{EscapeSql(originalView)}')
                )) AS Definition";

                DataTable dt = con.ExecuteSelect(sql);
                string def = dt.Rows.Count > 0 ? dt.Rows[0]["Definition"]?.ToString() : null;

                txtQuery.Text = string.IsNullOrWhiteSpace(def)
                    ? $"-- Could not load definition for [{originalSchema}].[{originalView}]"
                    : def;

                SetStatus($"Loaded view [{originalSchema}].[{originalView}]");
            }
            catch (Exception ex)
            {
                txtQuery.Text = $"-- Error: {ex.Message}";
            }
        }

        private void TxtQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.SuppressKeyPress = true;
                BtnPreview_Click(sender, e);
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            string selectSql = ExtractSelectForPreview(txtQuery.Text);

            if (string.IsNullOrWhiteSpace(selectSql))
            {
                MessageBox.Show("Could not extract a SELECT from the query.",
                    "Preview", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Wrap in TOP 200 to be safe
                string wrapped = WrapWithTop200(selectSql);
                DataTable result = con.ExecuteSelect(wrapped);
                dgvPreview.DataSource = result;
                lblPreviewStatus.Text = $"Preview — {result.Rows.Count} row(s)  (TOP 200)";
                SetStatus($"Preview OK — {result.Rows.Count} row(s)");
            }
            catch (Exception ex)
            {
                lblPreviewStatus.Text = "Preview error";
                SetStatus($"Error: {ex.Message}");
                MessageBox.Show($"Preview error:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string schema = txtSchema.Text.Trim();
            string viewName = txtViewName.Text.Trim();

            if (string.IsNullOrWhiteSpace(schema) || string.IsNullOrWhiteSpace(viewName))
            {
                MessageBox.Show("Schema and view name are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string ddl = BuildViewDDL(schema, viewName);

                using (var preview = new DdlPreviewForm(ddl, schema, viewName, con, isEdit))
                {
                    if (preview.ShowDialog() == DialogResult.OK)
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error building DDL:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildViewDDL(string schema, string viewName)
        {
            var sb = new System.Text.StringBuilder();

            if (isEdit)
            {
                sb.AppendLine($"ALTER VIEW [{schema}].[{viewName}]");
            }
            else
            {
                sb.AppendLine($"CREATE VIEW [{schema}].[{viewName}]");
            }

            sb.AppendLine("AS");

            string selectPart = ExtractSelectForPreview(txtQuery.Text);
            sb.AppendLine(string.IsNullOrWhiteSpace(selectPart) ? txtQuery.Text : selectPart);

            return sb.ToString();
        }

        private string ExtractSelectForPreview(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            string upper = text.ToUpperInvariant();
            int asIdx = -1;

            if (upper.TrimStart().StartsWith("CREATE") || upper.TrimStart().StartsWith("ALTER"))
            {
                int viewIdx = upper.IndexOf("VIEW", StringComparison.Ordinal);
                if (viewIdx >= 0)
                {
                    asIdx = upper.IndexOf("\nAS", viewIdx, StringComparison.Ordinal);
                    if (asIdx < 0)
                        asIdx = upper.IndexOf(" AS ", viewIdx, StringComparison.Ordinal);
                }
            }

            string selectPart = asIdx >= 0
                ? text.Substring(asIdx).TrimStart().TrimStart('A', 'S', 'a', 's').Trim()
                : text.Trim();

            return selectPart;
        }

        private string WrapWithTop200(string select)
        {
            string upper = select.TrimStart().ToUpperInvariant();

            if (upper.StartsWith("SELECT TOP"))
                return select;

            if (upper.StartsWith("SELECT"))
                return "SELECT TOP 200 " + select.TrimStart().Substring(6);

            return select;
        }

        private string BuildTemplate() =>
            "SELECT\r\n    t.Column1,\r\n    t.Column2\r\nFROM dbo.YourTable t\r\nWHERE 1=1;";

        private void SetStatus(string msg) => lblStatus.Text = msg;

        private string EscapeSql(string v) => v?.Replace("'", "''") ?? "";
    }
}