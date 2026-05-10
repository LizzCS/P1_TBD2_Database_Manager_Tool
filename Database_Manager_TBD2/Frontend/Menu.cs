using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Database_Manager_TBD2
{
    public class Menu : Form
    {
        private ListBox lstConnections;
        private TextBox txtServer;
        private TextBox txtDatabase;
        private TextBox txtOriginalDatabase;

        private Button btnTest;
        private Button btnSave;
        private Button btnConnect;

        private Panel leftPanel;
        private Panel rightPanel;

        public Menu()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // FORM
            this.Text = "Database Manager";
            this.Width = 1200;
            this.Height = 700;

            // DARK THEME FORM
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;

            // PANELS
            leftPanel = new Panel();
            rightPanel = new Panel();

            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 350;
            leftPanel.Padding = new Padding(10);
            leftPanel.BackColor = Color.FromArgb(28, 28, 28);

            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Padding = new Padding(20);
            rightPanel.BackColor = Color.FromArgb(18, 18, 18);

            // LISTBOX
            lstConnections = new ListBox();
            lstConnections.Dock = DockStyle.Fill;
            lstConnections.BackColor = Color.FromArgb(35, 35, 35);
            lstConnections.ForeColor = Color.White;
            lstConnections.BorderStyle = BorderStyle.None;

            leftPanel.Controls.Add(lstConnections);

            // LABEL SERVER
            Label lblServer = new Label()
            {
                Text = "Server",
                Top = 20,
                Left = 20,
                Width = 200,
                ForeColor = Color.LightGray
            };

            txtServer = new TextBox()
            {
                Top = 45,
                Left = 20,
                Width = 500,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // LABEL DATABASE
            Label lblDatabase = new Label()
            {
                Text = "Database - Destino",
                Top = 100,
                Left = 20,
                Width = 200,
                ForeColor = Color.LightGray
            };

            txtDatabase = new TextBox()
            {
                Top = 125,
                Left = 20,
                Width = 500,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // LABEL ORIGINAL
            Label lblOriginal = new Label()
            {
                Text = "Database - Origen",
                Top = 180,
                Left = 20,
                Width = 250,
                ForeColor = Color.LightGray
            };

            txtOriginalDatabase = new TextBox()
            {
                Top = 205,
                Left = 20,
                Width = 500,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // BUTTON TEST
            btnTest = new Button()
            {
                Text = "Probar Conexion",
                Top = 280,
                Left = 20,
                Width = 150,
                Height = 50,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTest.FlatAppearance.BorderSize = 0;
            btnTest.Click += BtnTest_Click;

            // BUTTON SAVE
            btnSave = new Button()
            {
                Text = "Guardar",
                Top = 280,
                Left = 190,
                Width = 120,
                Height = 50,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // BUTTON CONNECT (ACCENT)
            btnConnect = new Button()
            {
                Text = "Conectar",
                Top = 280,
                Left = 330,
                Width = 120,
                Height = 50,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += BtnConnect_Click;

            // ADD TO RIGHT PANEL
            rightPanel.Controls.Add(lblServer);
            rightPanel.Controls.Add(txtServer);

            rightPanel.Controls.Add(lblDatabase);
            rightPanel.Controls.Add(txtDatabase);

            rightPanel.Controls.Add(lblOriginal);
            rightPanel.Controls.Add(txtOriginalDatabase);

            rightPanel.Controls.Add(btnTest);
            rightPanel.Controls.Add(btnSave);
            rightPanel.Controls.Add(btnConnect);

            // ADD PANELS TO FORM
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
        }

        private string BuildConnectionString()
        {
            return $"Server={txtServer.Text};Database={txtDatabase.Text};Trusted_Connection=True;";
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtServer.Text) ||
                    string.IsNullOrWhiteSpace(txtDatabase.Text))
                {
                    MessageBox.Show("Favor ingresar tanto el servidor como la base de datos.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServer.Text) ||
                string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                MessageBox.Show("Favor ingresar tanto el servidor como la base de datos.");
                return;
            }

            lstConnections.Items.Add($"{txtServer.Text} - {txtDatabase.Text}");
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtServer.Text) ||
                    string.IsNullOrWhiteSpace(txtDatabase.Text))
                {
                    MessageBox.Show("Favor ingresar tanto el servidor como la base de datos.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}