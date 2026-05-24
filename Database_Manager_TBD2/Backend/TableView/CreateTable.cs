using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Database_Manager_TBD2.Backend;

namespace Database_Manager_TBD2
{

    public class CreateTable : Form
    {
        private DataGridView dgvColumns;
        private Panel pnlBottom;
        private Panel pnlProperties;
        private Label lblColProps;
        private TextBox txtDefaultValue;
        private Label lblDefault;
        private CheckBox chkIdentity;
        private TextBox txtIdentitySeed;
        private TextBox txtIdentityIncrement;
        private Label lblSeed;
        private Label lblIncrement;
        private Button btnSave;
        private Button btnCancel;
        private Button btnAddRow;
        private Button btnDeleteRow;
        private TextBox txtTableName;
        private TextBox txtSchema;
        private Label lblTableName;
        private Label lblSchema;
        private Panel pnlTop;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        private readonly Conexion con;
        private readonly string originalSchema;
        private readonly string originalTable;
        private readonly bool isEdit;

        private const int COL_NAME = 0;
        private const int COL_TYPE = 1;
        private const int COL_LENGTH = 2;
        private const int COL_PRECISION = 3;
        private const int COL_SCALE = 4;
        private const int COL_NULLABLE = 5;
        private const int COL_PK = 6;

        private static readonly string[] SqlTypes =
        {
            "int", "bigint", "smallint", "tinyint",
            "bit",
            "decimal", "numeric", "float", "real", "money", "smallmoney",
            "char", "varchar", "nchar", "nvarchar", "text", "ntext",
            "date", "datetime", "datetime2", "smalldatetime", "datetimeoffset", "time",
            "binary", "varbinary", "image",
            "uniqueidentifier", "xml", "geography", "geometry",
            "rowversion", "timestamp", "sql_variant", "hierarchyid"
        };

        public CreateTable(Conexion connection, string schema = null, string tableName = null)
        {
            con = connection;
            originalSchema = schema;
            originalTable = tableName;
            isEdit = schema != null && tableName != null;

            InitializeComponent();

            if (isEdit)
                LoadExistingTable();
            else
                AddEmptyRow();
        }
        
        private void InitializeComponent()
        {
            Text = isEdit ? $"Table Designer — [{originalSchema}].[{originalTable}]": "New Table Designer";
            Width = 920;
            Height = 680;
            MinimumSize = new Size(750, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(18, 18, 18);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);

            // ── TOP BAR (schema / table name) ──────────────────────
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(28, 28, 28),
                Padding = new Padding(8, 8, 8, 0)
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

            lblTableName = new Label
            {
                Text = "Table Name:",
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(190, 16)
            };

            txtTableName = new TextBox
            {
                Text = isEdit ? originalTable : "",
                Width = 220,
                Location = new Point(285, 12),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnAddRow = new Button
            {
                Text = "+ Add Column",
                Width = 110,
                Height = 26,
                Location = new Point(540, 11),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };
            btnAddRow.FlatAppearance.BorderSize = 0;
            btnAddRow.Click += (s, e) => AddEmptyRow();

            btnDeleteRow = new Button
            {
                Text = "✕ Delete",
                Width = 80,
                Height = 26,
                Location = new Point(658, 11),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White
            };
            btnDeleteRow.FlatAppearance.BorderSize = 0;
            btnDeleteRow.Click += BtnDeleteRow_Click;

            pnlTop.Controls.AddRange(new Control[]
            {
                lblSchema, txtSchema, lblTableName, txtTableName,
                btnAddRow, btnDeleteRow
            });

            // ── COLUMN GRID ────────────────────────────────────────
            dgvColumns = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = true,
                RowHeadersWidth = 28,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                GridColor = Color.FromArgb(55, 55, 55),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 28
            };

            dgvColumns.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(0, 84, 166),
                SelectionForeColor = Color.White
            };

            dgvColumns.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            dgvColumns.RowHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.LightGray
            };

            // Column Name
            dgvColumns.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColName",
                HeaderText = "Column Name",
                Width = 180,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Data Type (combo)
            var typeCol = new DataGridViewComboBoxColumn
            {
                Name = "DataType",
                HeaderText = "Data Type",
                Width = 140,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                FlatStyle = FlatStyle.Flat
            };
            typeCol.Items.AddRange(SqlTypes);
            dgvColumns.Columns.Add(typeCol);

            // Length
            dgvColumns.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Length",
                HeaderText = "Length / Max",
                Width = 90,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Precision
            dgvColumns.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Precision",
                HeaderText = "Precision",
                Width = 75,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Scale
            dgvColumns.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Scale",
                HeaderText = "Scale",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Allow Nulls (checkbox)
            dgvColumns.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "AllowNulls",
                HeaderText = "Allow Nulls",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // PK (checkbox)
            dgvColumns.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsPK",
                HeaderText = "PK",
                Width = 42,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvColumns.CellValueChanged += DgvColumns_CellValueChanged;
            dgvColumns.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvColumns.IsCurrentCellDirty)
                    dgvColumns.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            dgvColumns.SelectionChanged += DgvColumns_SelectionChanged;
            dgvColumns.RowPostPaint += DgvColumns_RowPostPaint;

            // ── PROPERTIES PANEL (bottom-left SSMS style) ──────────
            pnlProperties = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 130,
                BackColor = Color.FromArgb(28, 28, 28),
                Padding = new Padding(10)
            };

            lblColProps = new Label
            {
                Text = "Column Properties",
                ForeColor = Color.FromArgb(0, 150, 255),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 8)
            };

            lblDefault = new Label
            {
                Text = "Default Value:",
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(10, 34)
            };

            txtDefaultValue = new TextBox
            {
                Width = 160,
                Location = new Point(120, 30),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            chkIdentity = new CheckBox
            {
                Text = "Identity",
                ForeColor = Color.LightGray,
                Location = new Point(290, 32),
                AutoSize = true
            };
            chkIdentity.CheckedChanged += ChkIdentity_CheckedChanged;

            lblSeed = new Label
            {
                Text = "Seed:",
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(370, 34)
            };

            txtIdentitySeed = new TextBox
            {
                Text = "1",
                Width = 50,
                Location = new Point(418, 30),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false
            };

            lblIncrement = new Label
            {
                Text = "Increment:",
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(468, 34)
            };

            txtIdentityIncrement = new TextBox
            {
                Text = "1",
                Width = 50,
                Location = new Point(560, 30),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false
            };

            pnlProperties.Controls.AddRange(new Control[]
            {
                lblColProps, lblDefault, txtDefaultValue,
                chkIdentity, lblSeed, txtIdentitySeed,
                lblIncrement, txtIdentityIncrement
            });

            // ── BOTTOM BUTTON BAR ──────────────────────────────────
            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                BackColor = Color.FromArgb(25, 25, 25)
            };

            btnSave = new Button
            {
                Text = "Save Table",
                Width = 120,
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
                Location = new Point(136, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            pnlBottom.Controls.AddRange(new Control[] { btnSave, btnCancel });

            // ── STATUS BAR ────────────────────────────────────────
            statusStrip = new StatusStrip { BackColor = Color.FromArgb(25, 25, 25) };
            lblStatus = new ToolStripStatusLabel
            {
                Text = "Ready",
                ForeColor = Color.LightGray
            };
            statusStrip.Items.Add(lblStatus);

            // ── LAYOUT ────────────────────────────────────────────
            Controls.Add(dgvColumns);
            Controls.Add(pnlProperties);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Controls.Add(statusStrip);
        }

        // =========================================================
        // LOAD EXISTING TABLE
        // =========================================================
        private void LoadExistingTable()
        {
            try
            {
                string sql = $@"
                SELECT
                    c.name AS ColumnName,
                    t.name AS DataType,
                    c.max_length,
                    c.precision,
                    c.scale,
                    c.is_nullable,
                    c.is_identity,
                    ic.seed_value,
                    ic.increment_value,
                    dc.definition AS DefaultValue,
                    CASE WHEN pk.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsPK
                    FROM sys.tables tbl
                    INNER JOIN sys.schemas s  
                        ON tbl.schema_id    = s.schema_id
                    INNER JOIN sys.columns  c  
                        ON tbl.object_id    = c.object_id
                    INNER JOIN sys.types    t  
                        ON c.user_type_id   = t.user_type_id
                    LEFT  JOIN sys.identity_columns ic
                           ON c.object_id = ic.object_id 
                            AND c.column_id = ic.column_id
                    LEFT  JOIN sys.default_constraints dc
                           ON c.default_object_id = dc.object_id
                    LEFT  JOIN (
                    SELECT ic2.object_id, ic2.column_id
                    FROM sys.index_columns ic2
                    INNER JOIN sys.indexes i2
                           ON ic2.object_id = i2.object_id AND ic2.index_id = i2.index_id
                    WHERE i2.is_primary_key = 1
                ) pk ON c.object_id = pk.object_id AND c.column_id = pk.column_id
                WHERE s.name   = '{EscapeSql(originalSchema)}'
                  AND tbl.name = '{EscapeSql(originalTable)}'
                ORDER BY c.column_id";

                DataTable dt = con.ExecuteSelect(sql);

                foreach (DataRow r in dt.Rows)
                {
                    string typeName = r["DataType"].ToString();
                    int maxLen = Convert.ToInt32(r["max_length"]);
                    byte prec = Convert.ToByte(r["precision"]);
                    byte scale = Convert.ToByte(r["scale"]);
                    bool identity = Convert.ToBoolean(r["is_identity"]);

                    string lenDisplay = GetLengthDisplay(typeName, maxLen);

                    int idx = dgvColumns.Rows.Add(
                        r["ColumnName"].ToString(),
                        typeName,
                        lenDisplay,
                        NeedsDecimal(typeName) ? prec.ToString() : "",
                        NeedsDecimal(typeName) ? scale.ToString() : "",
                        Convert.ToBoolean(r["is_nullable"]),
                        Convert.ToBoolean(r["IsPK"])
                    );

                    dgvColumns.Rows[idx].Tag = new RowMeta
                    {
                        DefaultValue = r["DefaultValue"]?.ToString() ?? "",
                        IsIdentity = identity,
                        IdentitySeed = identity ? r["seed_value"]?.ToString() ?? "1" : "1",
                        IdentityIncrement = identity ? r["increment_value"]?.ToString() ?? "1" : "1"
                    };

                    if (Convert.ToBoolean(r["IsPK"]))
                        MarkPKRow(idx);
                }

                SetStatus($"Loaded {dt.Rows.Count} columns.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading table: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvColumns_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == COL_TYPE)
                UpdateTypeHints(e.RowIndex);

            if (e.ColumnIndex == COL_PK && e.RowIndex >= 0)
            {
                bool isPK = Convert.ToBoolean(dgvColumns.Rows[e.RowIndex].Cells[COL_PK].Value);
                if (isPK) MarkPKRow(e.RowIndex);
                else UnmarkPKRow(e.RowIndex);
            }
        }

        private void DgvColumns_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvColumns.SelectedRows.Count == 0) return;

            var row = dgvColumns.SelectedRows[0];
            var meta = row.Tag as RowMeta ?? new RowMeta();

            txtDefaultValue.Text = meta.DefaultValue;
            chkIdentity.Checked = meta.IsIdentity;
            txtIdentitySeed.Text = meta.IdentitySeed;
            txtIdentityIncrement.Text = meta.IdentityIncrement;
            txtIdentitySeed.Enabled = meta.IsIdentity;
            txtIdentityIncrement.Enabled = meta.IsIdentity;
        }

        private void ChkIdentity_CheckedChanged(object sender, EventArgs e)
        {
            txtIdentitySeed.Enabled = chkIdentity.Checked;
            txtIdentityIncrement.Enabled = chkIdentity.Checked;

            if (dgvColumns.SelectedRows.Count == 0) return;
            SyncMetaFromProperties(dgvColumns.SelectedRows[0].Index);
        }

        private void DgvColumns_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            bool isPK = Convert.ToBoolean(dgvColumns.Rows[e.RowIndex].Cells[COL_PK].Value);
            if (!isPK) return;

            string key = "🔑";
            var sf = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center,
                LineAlignment = System.Drawing.StringAlignment.Center
            };
            e.Graphics.DrawString(key, new Font("Segoe UI", 8f), Brushes.Gold,
                new RectangleF(e.RowBounds.Left, e.RowBounds.Top,
                               dgvColumns.RowHeadersWidth, e.RowBounds.Height), sf);
        }

        private void AddEmptyRow()
        {
            int idx = dgvColumns.Rows.Add("", "int", "", "", "", true, false);
            dgvColumns.Rows[idx].Tag = new RowMeta();
            dgvColumns.ClearSelection();
            dgvColumns.Rows[idx].Selected = true;
            dgvColumns.CurrentCell = dgvColumns.Rows[idx].Cells[COL_NAME];
        }

        private void BtnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dgvColumns.SelectedRows.Count == 0) return;
            int idx = dgvColumns.SelectedRows[0].Index;
            dgvColumns.Rows.RemoveAt(idx);
        }

        private void UpdateTypeHints(int rowIndex)
        {
            string type = dgvColumns.Rows[rowIndex].Cells[COL_TYPE].Value?.ToString() ?? "";

            // Clear hints first
            dgvColumns.Rows[rowIndex].Cells[COL_LENGTH].Value = "";
            dgvColumns.Rows[rowIndex].Cells[COL_PRECISION].Value = "";
            dgvColumns.Rows[rowIndex].Cells[COL_SCALE].Value = "";

            switch (type.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "varbinary":
                case "binary":
                    dgvColumns.Rows[rowIndex].Cells[COL_LENGTH].Value = "50";
                    break;
                case "decimal":
                case "numeric":
                    dgvColumns.Rows[rowIndex].Cells[COL_PRECISION].Value = "18";
                    dgvColumns.Rows[rowIndex].Cells[COL_SCALE].Value = "2";
                    break;
            }
        }

        private void SyncMetaFromProperties(int rowIndex)
        {
            var meta = dgvColumns.Rows[rowIndex].Tag as RowMeta ?? new RowMeta();
            meta.DefaultValue = txtDefaultValue.Text;
            meta.IsIdentity = chkIdentity.Checked;
            meta.IdentitySeed = txtIdentitySeed.Text;
            meta.IdentityIncrement = txtIdentityIncrement.Text;
            dgvColumns.Rows[rowIndex].Tag = meta;
        }

        private void MarkPKRow(int idx)
        {
            dgvColumns.Rows[idx].DefaultCellStyle.ForeColor = Color.Gold;
            dgvColumns.Rows[idx].Cells[COL_NULLABLE].Value = false;
        }

        private void UnmarkPKRow(int idx)
        {
            dgvColumns.Rows[idx].DefaultCellStyle.ForeColor = Color.White;
        }

        private string GetLengthDisplay(string type, int maxLen)
        {
            switch (type.ToLower())
            {
                case "nvarchar":
                case "nchar":
                    return maxLen == -1 ? "MAX" : (maxLen / 2).ToString();
                case "varchar":
                case "char":
                case "varbinary":
                case "binary":
                    return maxLen == -1 ? "MAX" : maxLen.ToString();
                default:
                    return "";
            }
        }

        private bool NeedsDecimal(string type) =>
            type.ToLower() == "decimal" || type.ToLower() == "numeric";

        private void SetStatus(string msg) => lblStatus.Text = msg;

        private string EscapeSql(string v) => v?.Replace("'", "''") ?? "";

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (dgvColumns.SelectedRows.Count > 0)
                SyncMetaFromProperties(dgvColumns.SelectedRows[0].Index);

            string schema = txtSchema.Text.Trim();
            string table = txtTableName.Text.Trim();

            if (string.IsNullOrWhiteSpace(schema) || string.IsNullOrWhiteSpace(table))
            {
                MessageBox.Show("Schema and table name are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvColumns.Rows.Count == 0)
            {
                MessageBox.Show("Add at least one column.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string ddl = BuildDDL(schema, table);

                using (var preview = new DdlPreviewForm(ddl, schema, table, con, isEdit))
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
                MessageBox.Show($"Error generating DDL:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildDDL(string schema, string table)
        {
            var sb = new System.Text.StringBuilder();

            var pkCols = new List<string>();

            if (isEdit)
            {
                sb.AppendLine($"-- Alter existing table [{schema}].[{table}]");
                sb.AppendLine($"-- NOTE: Dropping and recreating. Back up data first.");
                sb.AppendLine($"DROP TABLE [{schema}].[{table}];");
                sb.AppendLine("GO");
                sb.AppendLine();
            }

            sb.AppendLine($"CREATE TABLE [{schema}].[{table}]");
            sb.AppendLine("(");

            var lines = new List<string>();

            foreach (DataGridViewRow row in dgvColumns.Rows)
            {
                string colName = row.Cells[COL_NAME].Value?.ToString()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(colName)) continue;

                string type = row.Cells[COL_TYPE].Value?.ToString() ?? "int";
                string len = row.Cells[COL_LENGTH].Value?.ToString()?.Trim() ?? "";
                string prec = row.Cells[COL_PRECISION].Value?.ToString()?.Trim() ?? "";
                string scale = row.Cells[COL_SCALE].Value?.ToString()?.Trim() ?? "";
                bool nullable = Convert.ToBoolean(row.Cells[COL_NULLABLE].Value);
                bool isPK = Convert.ToBoolean(row.Cells[COL_PK].Value);
                var meta = row.Tag as RowMeta ?? new RowMeta();

                if (isPK) pkCols.Add($"[{colName}]");

                var col = new System.Text.StringBuilder();
                col.Append($"    [{colName}] ");
                col.Append(FormatType(type, len, prec, scale));

                if (meta.IsIdentity)
                    col.Append($" IDENTITY({meta.IdentitySeed},{meta.IdentityIncrement})");

                col.Append(nullable ? " NULL" : " NOT NULL");

                if (!string.IsNullOrWhiteSpace(meta.DefaultValue))
                    col.Append($" DEFAULT {meta.DefaultValue}");

                lines.Add(col.ToString());
            }

            if (pkCols.Count > 0)
                lines.Add($"    CONSTRAINT [PK_{table}] PRIMARY KEY ({string.Join(", ", pkCols)})");

            sb.AppendLine(string.Join(",\n", lines));
            sb.AppendLine(");");

            return sb.ToString();
        }

        private string FormatType(string type, string len, string prec, string scale)
        {
            switch (type.ToLower())
            {
                case "varchar":
                case "char":
                case "varbinary":
                case "binary":
                    return string.IsNullOrEmpty(len) ? type
                           : len.ToUpper() == "MAX" ? $"{type}(MAX)"
                           : $"{type}({len})";

                case "nvarchar":
                case "nchar":
                    return string.IsNullOrEmpty(len) ? type
                           : len.ToUpper() == "MAX" ? $"{type}(MAX)"
                           : $"{type}({len})";

                case "decimal":
                case "numeric":
                    return (string.IsNullOrEmpty(prec) && string.IsNullOrEmpty(scale))
                           ? type : $"{type}({prec},{scale})";

                case "datetime2":
                case "datetimeoffset":
                case "time":
                    return string.IsNullOrEmpty(scale) ? type : $"{type}({scale})";

                default:
                    return type;
            }
        }

        private class RowMeta
        {
            public string DefaultValue { get; set; } = "";
            public bool IsIdentity { get; set; } = false;
            public string IdentitySeed { get; set; } = "1";
            public string IdentityIncrement { get; set; } = "1";
        }
    }

    public class DdlPreviewForm : Form
    {
        private RichTextBox txtDDL;
        private Button btnExecute;
        private Button btnCancel;
        private Label lblTitle;

        private readonly Conexion con;
        private readonly string schema;
        private readonly string name;
        private readonly bool isEdit;

        public DdlPreviewForm(string ddl, string schema, string name,
                              Conexion connection, bool isEdit = false)
        {
            this.con = connection;
            this.schema = schema;
            this.name = name;
            this.isEdit = isEdit;

            Text = "Preview DDL";
            Width = 700;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(18, 18, 18);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);

            lblTitle = new Label
            {
                Text = "Review the DDL below. Click Execute to apply.",
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = Color.LightGray,
                Padding = new Padding(8, 8, 0, 0),
                BackColor = Color.FromArgb(28, 28, 28)
            };

            txtDDL = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(220, 220, 170),
                Font = new Font("Consolas", 10.5f),
                BorderStyle = BorderStyle.None,
                Text = ddl,
                ReadOnly = false   
            };

            var pnlBtn = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                BackColor = Color.FromArgb(25, 25, 25)
            };

            btnExecute = new Button
            {
                Text = "Execute",
                Width = 110,
                Height = 30,
                Location = new Point(8, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };
            btnExecute.FlatAppearance.BorderSize = 0;
            btnExecute.Click += BtnExecute_Click;

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

            pnlBtn.Controls.AddRange(new Control[] { btnExecute, btnCancel });

            Controls.Add(txtDDL);
            Controls.Add(lblTitle);
            Controls.Add(pnlBtn);
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                con.ExecuteNonQuery(txtDDL.Text);
                MessageBox.Show("Executed successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Execution failed:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}