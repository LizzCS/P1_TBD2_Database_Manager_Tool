using Database_Manager_TBD2.Backend;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Database_Manager_TBD2
{
    public class Menu : Form
    {
        private ListBox lstConnections;

        private TextBox txtName;
        private TextBox txtServer;
        private TextBox txtOriginalDatabase;
        private TextBox txtUsername;
        private TextBox txtPassword;

        private Label lblUsername;
        private Label lblPassword;
        
        private Panel pnlStatus;
        private Label lblStatus;

        private RadioButton rbWindowsAuth;
        private RadioButton rbSqlAuth;

        private Button btnTest;
        private Button btnSave;
        private Button btnConnect;

        private Panel leftPanel;
        private Panel rightPanel;

        private List<Conexion> connections =
            new List<Conexion>();

        public Menu()
        {
            InitializeComponent();

            LoadConnections();
        }

        private void InitializeComponent()
        {
            // FORM

            this.Text = "Database Manager";
            this.Width = 1200;
            this.Height = 700;

            this.BackColor =
                Color.FromArgb(18, 18, 18);

            this.ForeColor = Color.White;

            this.Font =
                new Font("Segoe UI", 10);

            // PANELS

            leftPanel = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 350,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(28, 28, 28)
            };

            rightPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(18, 18, 18)
            };

            // LISTBOX

            lstConnections = new ListBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11),
                ItemHeight = 30
            };

            lstConnections.SelectedIndexChanged +=
                LstConnections_SelectedIndexChanged;

            leftPanel.Controls.Add(lstConnections);

            // LAYOUT VALUES

            int left = 20;
            int width = 520;

            int labelTop = 20;
            int textboxOffset = 28;
            int sectionSpacing = 80;

            Label lblName = new Label()
            {
                Text = "Nombre de Conexion",
                Top = labelTop,
                Left = left,
                Width = 250,
                ForeColor = Color.LightGray
            };

            txtName = new TextBox()
            {
                Top = labelTop + textboxOffset,
                Left = left,
                Width = width,
                Height = 30,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            labelTop += sectionSpacing;

            Label lblServer = new Label()
            {
                Text = "Servidor",
                Top = labelTop,
                Left = left,
                Width = 250,
                ForeColor = Color.LightGray
            };

            txtServer = new TextBox()
            {
                Top = labelTop + textboxOffset,
                Left = left,
                Width = width,
                Height = 30,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            labelTop += sectionSpacing;

            Label lblOriginal = new Label()
            {
                Text = "Base de Datos",
                Top = labelTop,
                Left = left,
                Width = 250,
                ForeColor = Color.LightGray
            };

            txtOriginalDatabase = new TextBox()
            {
                Top = labelTop + textboxOffset,
                Left = left,
                Width = width,
                Height = 30,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            labelTop += sectionSpacing;

            rbWindowsAuth = new RadioButton()
            {
                Text = "Autenticación de Windows",
                Top = labelTop,
                Left = left,
                Width = 220,
                ForeColor = Color.White,
                Checked = true
            };

            rbSqlAuth = new RadioButton()
            {
                Text = "Autenticación SQL",
                Top = labelTop,
                Left = 280,
                Width = 220,
                ForeColor = Color.White
            };

            rbWindowsAuth.CheckedChanged += AuthChanged;

            rbSqlAuth.CheckedChanged += AuthChanged;

            labelTop += sectionSpacing;

            lblUsername = new Label()
            {
                Text = "ID Usuario",
                Top = labelTop,
                Left = left,
                Width = 200,
                ForeColor = Color.LightGray,
                Visible = false
            };

            txtUsername = new TextBox()
            {
                Top = labelTop + textboxOffset,
                Left = left,
                Width = 240,
                Height = 30,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            lblPassword = new Label()
            {
                Text = "Contraseña",
                Top = labelTop,
                Left = 300,
                Width = 200,
                ForeColor = Color.LightGray,
                Visible = false
            };

            txtPassword = new TextBox()
            {
                Top = labelTop + textboxOffset,
                Left = 300,
                Width = 240,
                Height = 30,
                PasswordChar = '*',
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            labelTop += 110;

            btnTest = new Button()
            {
                Text = "Probar Conexion",
                Top = labelTop,
                Left = left,
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnTest.FlatAppearance.BorderSize = 0;

            pnlStatus = new Panel()
            {
                Width = 18,
                Height = 18,
                Left = left,
                Top = labelTop + -40,
                BackColor = Color.Gray
            };

            lblStatus = new Label()
            {
                Text = "Idle",
                Left = left + 40,
                Top = labelTop + -43,
                Width = 120,
                ForeColor = Color.LightGray
            };

            btnSave = new Button()
            {
                Text = "Guardar",
                Top = labelTop,
                Left = 210,
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnSave.FlatAppearance.BorderSize = 0;

            btnConnect = new Button()
            {
                Text = "Conectar",
                Top = labelTop,
                Left = 390,
                Width = 150,
                Height = 45,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnConnect.FlatAppearance.BorderSize = 0;


            btnTest.Click += BtnTest_Click;
            btnSave.Click += BtnSave_Click;
            btnConnect.Click += BtnConnect_Click;

            rightPanel.Controls.Add(lblName);
            rightPanel.Controls.Add(txtName);

            rightPanel.Controls.Add(lblServer);
            rightPanel.Controls.Add(txtServer);

            rightPanel.Controls.Add(lblOriginal);
            rightPanel.Controls.Add(txtOriginalDatabase);

            rightPanel.Controls.Add(rbWindowsAuth);
            rightPanel.Controls.Add(rbSqlAuth);

            rightPanel.Controls.Add(lblUsername);
            rightPanel.Controls.Add(txtUsername);

            rightPanel.Controls.Add(lblPassword);
            rightPanel.Controls.Add(txtPassword);

            rightPanel.Controls.Add(btnTest);

            rightPanel.Controls.Add(pnlStatus);
            rightPanel.Controls.Add(lblStatus);

            rightPanel.Controls.Add(btnSave);
            rightPanel.Controls.Add(btnConnect);

            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);

            this.FormClosing += Main_FormClosing;
        }

        private void AuthChanged(
            object sender,
            EventArgs e)
        {
            bool sqlAuth = rbSqlAuth.Checked;

            lblUsername.Visible = sqlAuth;
            txtUsername.Visible = sqlAuth;

            lblPassword.Visible = sqlAuth;
            txtPassword.Visible = sqlAuth;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtServer.Text) ||
                string.IsNullOrWhiteSpace(txtOriginalDatabase.Text))
            {
                SetErrorState();
                MessageBox.Show("Name, Server, and Database are required.");
                return;
            }

            if (rbSqlAuth.Checked &&
                (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                 string.IsNullOrWhiteSpace(txtPassword.Text)))
            {
                SetErrorState();
                MessageBox.Show("Username and Password are required for SQL Authentication.");
                return;
            }

            Conexion con = new Conexion()
            {
                Name = txtName.Text,

                Server = txtServer.Text,

                Database = txtOriginalDatabase.Text,

                UseWindowsAuth = rbWindowsAuth.Checked,

                Username = txtUsername.Text,

                Password = txtPassword.Text
            };

            connections.Add(con);

            ConnectionStorage.Save(connections);

            RefreshList();

            MessageBox.Show("Conexión guardada.");
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                Conexion con = BuildConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Conexion con = BuildConnection();

                if (con == null)
                    return;

                Main dashboard = new Main(con);

                dashboard.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Conexion BuildConnection()
        {
            SetTestingState();

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtServer.Text) ||
                string.IsNullOrWhiteSpace(txtOriginalDatabase.Text))
            {
                SetErrorState();
                MessageBox.Show("Nombre, Servidor, y Base de Datos son requeridos.");
                return null;
            }

            if (rbSqlAuth.Checked &&
                (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                 string.IsNullOrWhiteSpace(txtPassword.Text)))
            {
                SetErrorState();
                MessageBox.Show("Nombre de usuario y contraseña son requeridos para la autenticación SQL.");
                return null;
            }

            Conexion con = new Conexion()
            {
                Name = txtName.Text,
                Server = txtServer.Text,
                Database = txtOriginalDatabase.Text,
                
                UseWindowsAuth = rbWindowsAuth.Checked,

                Username = txtUsername.Text,
                Password = txtPassword.Text
            };

            try
            {
                con.TestConnection();

                SetSuccessState();
                MessageBox.Show("Conexión exitosa.");

                return con;
            }
            catch (Exception ex)
            {
                SetErrorState();
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        private void SetTestingState()
        {
            pnlStatus.BackColor = Color.Goldenrod;
            lblStatus.Text = "Probando...";
        }

        private void SetSuccessState()
        {
            pnlStatus.BackColor = Color.LimeGreen;
            lblStatus.Text = "Conectado";
        }

        private void SetErrorState()
        {
            pnlStatus.BackColor = Color.Red;
            lblStatus.Text = "Error";
        }

        private void SetIdleState()
        {
            pnlStatus.BackColor = Color.Gray;
            lblStatus.Text = "Inactivo";
        }

        private void LoadConnections()
        {
            connections =
                ConnectionStorage.Load();

            RefreshList();
        }

        private void RefreshList()
        {
            lstConnections.Items.Clear();

            foreach (var item in connections)
            {
                lstConnections.Items.Add(item);
            }
        }

        private void LstConnections_SelectedIndexChanged(object sender,EventArgs e)
        {
            if (lstConnections.SelectedItem is not Conexion con)
                return;

            txtName.Text = con.Name;
            txtServer.Text = con.Server;
            txtOriginalDatabase.Text = con.Database;

            rbWindowsAuth.Checked = con.UseWindowsAuth;
            rbSqlAuth.Checked = !con.UseWindowsAuth;

            txtUsername.Text = con.Username;
            txtPassword.Text = con.Password;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}