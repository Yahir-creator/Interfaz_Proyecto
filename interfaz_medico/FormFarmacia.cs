// FormFarmacia.cs
using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace interfaz_medico
{
    public partial class FormFarmacia : Form
    {
        // Usa la cadena del app.config (MySqlConnection)
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public FormFarmacia()
        {
            InitializeComponent();

            // Ajustes de la grilla
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private MySqlConnection NuevaConexion() => new MySqlConnection(_connStr);

        private void FormFarmacia_Load(object sender, EventArgs e)
        {
            // AHORA: cargar TODAS las recetas (incluye Entregado)
            CargarRecetas(soloPendientes: false);
        }

        private void CargarRecetas(bool soloPendientes)
        {
            try
            {
                using (var conn = NuevaConexion())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            r.id,
                            p.nombre AS paciente,
                            m1.nombre AS medicamento1, r.dosis1, r.frecuencia1,
                            m2.nombre AS medicamento2, r.dosis2, r.frecuencia2,
                            m3.nombre AS medicamento3, r.dosis3, r.frecuencia3,
                            m4.nombre AS medicamento4, r.dosis4, r.frecuencia4,
                            r.estado
                        FROM recetas r
                        JOIN pacientes p ON r.id_paciente = p.id
                        LEFT JOIN medicamentos m1 ON r.id_medicamento1 = m1.id
                        LEFT JOIN medicamentos m2 ON r.id_medicamento2 = m2.id
                        LEFT JOIN medicamentos m3 ON r.id_medicamento3 = m3.id
                        LEFT JOIN medicamentos m4 ON r.id_medicamento4 = m4.id";

                    if (soloPendientes)
                        query += " WHERE r.estado = 'Pendiente'";

                    using (var da = new MySqlDataAdapter(query, conn))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }

                FormatearEncabezados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar recetas: " + ex.Message);
            }
        }

        private void FormatearEncabezados()
        {
            void setHeader(string name, string header)
            {
                if (dataGridView1.Columns.Contains(name))
                    dataGridView1.Columns[name].HeaderText = header;
            }

            setHeader("id", "Folio");
            setHeader("paciente", "Paciente");

            for (int i = 1; i <= 4; i++)
            {
                setHeader($"medicamento{i}", $"Medicamento {i}");
                setHeader($"dosis{i}", $"Dosis {i}");
                setHeader($"frecuencia{i}", $"Frecuencia {i}");
            }

            setHeader("estado", "Estado");
        }

        private string ConstruirDetalleRecetaDeFila(DataGridViewRow fila, string estadoFinal)
        {
            var sb = new StringBuilder();

            if (fila.HasColumn("paciente"))
                sb.AppendLine($"Paciente: {fila.Cells["paciente"].Value?.ToString()}");

            for (int i = 1; i <= 4; i++)
            {
                string colMed = $"medicamento{i}";
                string colDosis = $"dosis{i}";
                string colFrec = $"frecuencia{i}";

                string med = fila.HasColumn(colMed) ? fila.Cells[colMed]?.Value?.ToString() : null;
                string dosis = fila.HasColumn(colDosis) ? fila.Cells[colDosis]?.Value?.ToString() : null;
                string frec = fila.HasColumn(colFrec) ? fila.Cells[colFrec]?.Value?.ToString() : null;

                if (!string.IsNullOrWhiteSpace(med))
                    sb.AppendLine($"Medicamento {i}: {med} - {dosis}, {frec}");
            }

            sb.Append("Estado: ").Append(estadoFinal);
            return sb.ToString();
        }

        // (Requerido porque el diseñador enlaza este evento)
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // noop
        }

        private void btnCerrarSesion_Click_1(object sender, EventArgs e)
        {
            var login = new FormLogin();
            login.Show();
            this.Close();
        }

        private void btnEntregado_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una receta antes de marcar como entregada.");
                return;
            }

            var fila = dataGridView1.SelectedRows[0];
            if (!fila.HasColumn("id") || fila.Cells["id"]?.Value == null)
            {
                MessageBox.Show("La receta seleccionada no tiene un ID válido.");
                return;
            }

            int recetaId = Convert.ToInt32(fila.Cells["id"].Value);

            try
            {
                using (var conn = NuevaConexion())
                {
                    conn.Open();
                    const string update = "UPDATE recetas SET estado = 'Entregado' WHERE id = @id";
                    using (var cmd = new MySqlCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recetaId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Receta marcada como entregada.");

                // Mostrar detalle en el cuadro de texto
                txtRecetaEntregada.Text = ConstruirDetalleRecetaDeFila(fila, estadoFinal: "Entregado");

                // AHORA: recargar mostrando TODAS para que la receta siga visible
                CargarRecetas(soloPendientes: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado: " + ex.Message);
            }
        }

        private void btnActualizar_Click_1(object sender, EventArgs e)
        {
            CargarRecetas(soloPendientes: false);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var login = new FormLogin();
            login.Show();
            this.Close();
        }
    }

    // Extensión segura para comprobar columnas sin provocar el error de colección
    internal static class DataGridViewRowExtensions
    {
        public static bool HasColumn(this DataGridViewRow row, string columnName)
            => row?.DataGridView?.Columns?.Contains(columnName) == true;
    }
}
