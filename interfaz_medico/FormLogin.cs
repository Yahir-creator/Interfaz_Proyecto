using ClinicaMedica;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace interfaz_medico
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void ButtonEntrar_Click(object sender, EventArgs e)
        {
            string usuario = textBox1.Text.Trim();
            string contrasena = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Ingrese usuario y contraseña.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener cadena de conexión desde app.config
            string cadenaConexion = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conexion.Open();

                    string consulta = "SELECT rol FROM usuarios WHERE usuario = @usuario AND contrasena = @contrasena";
                    using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", usuario);
                        comando.Parameters.AddWithValue("@contrasena", contrasena);

                        object resultado = comando.ExecuteScalar();

                        if (resultado != null)
                        {
                            string rol = resultado.ToString();
                            this.Hide();

                            if (rol == "medico")
                            {
                                FormMedico f = new FormMedico();
                                f.Show();
                            }
                            else if (rol == "farmacia")
                            {
                                FormFarmacia f = new FormFarmacia();
                                f.Show();
                            }
                            else if (rol == "almacen")
                            {
                                FormAlmacen f = new FormAlmacen();
                                f.Show();
                            }
                            else
                            {
                                MessageBox.Show("Rol no reconocido en el sistema.");
                                this.Show();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión a la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            // Sepración
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

