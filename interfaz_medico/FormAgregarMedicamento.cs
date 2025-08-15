using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace interfaz_medico
{
    public partial class FormAgregarMedicamento : Form
    {

        private readonly string _cs;
        private bool _silenciarEventos = false;

        public FormAgregarMedicamento()
        {
            InitializeComponent();

            var cs = ConfigurationManager.ConnectionStrings["MySqlConnection"];
            _cs = cs != null ? cs.ConnectionString : "";

            CargarDosisFrecuencia(CbDosis, CbFrecuencia);
        }

        private void FormAgregarMedicamento_Load(object sender, EventArgs e)
        {

        }

        private void CargarDosisFrecuencia(ComboBox cbDosis, ComboBox cbFrecuencia)
        {

            DataTable dosisTabla = new DataTable();
            DataTable frecTabla = new DataTable();

            try
            {
                using (var conexion = NuevaConexion())
                {
                    using (var cmdDosis = new MySqlCommand(
                        "SELECT MIN(id) AS id, dosis, stock FROM medicamento_dosis GROUP BY dosis ORDER BY dosis ASC", conexion))
                    {
                        using (var da = new MySqlDataAdapter(cmdDosis)) da.Fill(dosisTabla);
                    }

                    using (var cmdFrecuencia = new MySqlCommand(
                        "SELECT DISTINCT frecuencia FROM medicamento_frecuencia ORDER BY frecuencia DESC", conexion))
                    {
                        using (var da2 = new MySqlDataAdapter(cmdFrecuencia)) da2.Fill(frecTabla);
                    }
                }
            }
            catch { }

            if (!dosisTabla.Columns.Contains("id")) dosisTabla.Columns.Add("id", typeof(int));
            if (!dosisTabla.Columns.Contains("dosis")) dosisTabla.Columns.Add("dosis", typeof(string));
            if (!dosisTabla.Columns.Contains("stock")) dosisTabla.Columns.Add("stock", typeof(int));

            _silenciarEventos = true;

            cbDosis.DrawItem -= DosisComboBox_DrawItem;

            cbDosis.BeginUpdate();
            cbDosis.DataSource = null;
            cbDosis.Items.Clear();
            cbDosis.DisplayMember = string.Empty;
            cbDosis.ValueMember = string.Empty;

            cbDosis.DataSource = dosisTabla;
            cbDosis.DisplayMember = "dosis";
            cbDosis.ValueMember = "dosis";

            cbDosis.SelectedIndex = -1;
            cbDosis.Text = string.Empty;
            cbDosis.DrawMode = DrawMode.OwnerDrawFixed;
            cbDosis.DrawItem += DosisComboBox_DrawItem;
            cbDosis.EndUpdate();

            cbFrecuencia.BeginUpdate();
            cbFrecuencia.Items.Clear();
            for (int i = 0; i < frecTabla.Rows.Count; i++)
                cbFrecuencia.Items.Add(Convert.ToString(frecTabla.Rows[i]["frecuencia"]));
            cbFrecuencia.SelectedIndex = -1;
            cbFrecuencia.Text = string.Empty;
            cbFrecuencia.EndUpdate();

            _silenciarEventos = false;
        }

        private void DosisComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (e.Index < 0) return;

            DataRowView row = cb.Items[e.Index] as DataRowView;
            string texto = Convert.ToString(row["dosis"]);
            int stock = 0; try { stock = Convert.ToInt32(row["stock"]); } catch { }

            e.DrawBackground();
            using (var brush = new SolidBrush(stock <= 0 ? Color.Black : e.ForeColor))
                e.Graphics.DrawString(texto, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void AgregarMedicamento()
        {
            string nombreMedicamento = TxtNombre.Text;
            string dosis = CbDosis.Text;
            string frecuencia = CbFrecuencia.SelectedItem.ToString();
            int nombreLote = int.Parse(TxtLote.Text);
            int cantidadLote = int.Parse(TxtCantidadLote.Text);
            DateTime caducidad = dtpCaducidad.Value;
            int stock = int.Parse(TxtCantidad.Text);
            bool success = false;

            using (var conexion = NuevaConexion())
            {
                var transaction = conexion.BeginTransaction();

                try
                {
                    // Insertar medicamento
                    long medicamentoId;
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO medicamentos (nombre, cantidad) VALUES (@nombre, @cantidad); SELECT LAST_INSERT_ID();", conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombreMedicamento);
                        cmd.Parameters.AddWithValue("@cantidad", stock);
                        medicamentoId = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // Insertar medicamento_dosis
                    long medicamentoDosisId;
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO medicamento_dosis (medicamento_id, dosis, stock) VALUES (@medicamento_id, @dosis, @stock); SELECT LAST_INSERT_ID();", conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@medicamento_id", medicamentoId);
                        cmd.Parameters.AddWithValue("@dosis", dosis);
                        cmd.Parameters.AddWithValue("@stock", cantidadLote);
                        medicamentoDosisId = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // Insertar medicamento_frecuencia
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO medicamento_frecuencia (medicamento_id, frecuencia) VALUES (@medicamento_id, @frecuencia);", conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@medicamento_id", medicamentoId);
                        cmd.Parameters.AddWithValue("@frecuencia", frecuencia);
                        cmd.ExecuteNonQuery();
                    }

                    // Insertar lote
                    long loteId;
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO lotes (numero, caducidad) VALUES (@nombre, @caducidad); SELECT LAST_INSERT_ID();", conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@nombre", @"L00" + nombreLote);
                        cmd.Parameters.AddWithValue("@caducidad", caducidad);
                        loteId = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // Insertar en md_stock_lote
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO md_stock_lote (medicamento_dosis_id, lote_id, stock) VALUES (@medicamento_dosis_id, @lote_id, @stock);", conexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@medicamento_dosis_id", medicamentoDosisId);
                        cmd.Parameters.AddWithValue("@lote_id", loteId);
                        cmd.Parameters.AddWithValue("@stock", cantidadLote);
                        cmd.ExecuteNonQuery();
                    }

                    // Confirmar transacción
                    transaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    // Revertir si algo falla
                    transaction.Rollback();
                    MessageBox.Show("Error al registrar medicamento: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (success)
            {
                MessageBox.Show("Registro completado correctamente", "Registro exitoso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private MySqlConnection NuevaConexion()
        {
            var cn = new MySqlConnection(_cs);
            if (cn.State != System.Data.ConnectionState.Open) cn.Open();
            return cn;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AgregarMedicamento();
        }

        private void TxtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtLote_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtCantidadLote_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
