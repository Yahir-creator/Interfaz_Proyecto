// FormAlmacen.cs
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace interfaz_medico
{
    public partial class FormAlmacen : Form
    {
        // NO leer la cadena aquí: en diseñador puede ser null y rompe.
        private readonly string _connStr;

        // Contexto de selección
        private int? _selMedId, _selMdId, _selLoteId;
        private string _selNombre = "", _selDosis = "", _selLoteNum = "", _selEstado = "";
        private DateTime? _selCad;
        private int _selStock = 0;

        public FormAlmacen()
        {
            InitializeComponent();

            // Si es diseñador, no toques BD ni eventos.
            if (IsDesignMode())
            {
                _connStr = string.Empty;
                return;
            }

            // Leer cadena de conexión de forma segura
            var cs = ConfigurationManager.ConnectionStrings["MySqlConnection"];
            _connStr = (cs != null) ? cs.ConnectionString : string.Empty;

            // Estilo grilla y eventos UI (solo en ejecución)
            dgvMedicamentos.ReadOnly = true;
            dgvMedicamentos.MultiSelect = false;
            dgvMedicamentos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMedicamentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMedicamentos.SelectionChanged += (s, e) => TomarSeleccionDeGrilla();

            btnAgregar.Click += BtnAgregar_Click;
            btnBaja.Click += BtnBaja_Click;
            btnEliminarDef.Click += BtnEliminarDefinitivo_Click;

            // Cerrar sesión → volver a FormLogin (no cerrar toda la app)
            BtnCerrarSesion.Click += BtnCerrarSesion_Click;

            // Carga inicial de la grilla
            CargarMedicamentosGrid();
        }

        private static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode ==
                   System.ComponentModel.LicenseUsageMode.Designtime
                   || System.Diagnostics.Process.GetCurrentProcess().ProcessName.Equals("devenv", StringComparison.OrdinalIgnoreCase);
        }

        private MySqlConnection NuevaConexion() => new MySqlConnection(_connStr);

        private void CargarMedicamentosGrid()
        {
            if (IsDesignMode()) return;

            try
            {
                using (var cn = NuevaConexion())
                using (var da = new MySqlDataAdapter(
                    @"SELECT id_med, id_dosis, id_lote, nombre, dosis, n_lote, caducidad, stock, total_med, estado
                      FROM v_almacen
                      ORDER BY nombre, dosis, n_lote;", cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvMedicamentos.DataSource = dt;

                    // Encabezados
                    if (dgvMedicamentos.Columns.Contains("nombre")) dgvMedicamentos.Columns["nombre"].HeaderText = "nombre";
                    if (dgvMedicamentos.Columns.Contains("dosis")) dgvMedicamentos.Columns["dosis"].HeaderText = "dosis";
                    if (dgvMedicamentos.Columns.Contains("n_lote")) dgvMedicamentos.Columns["n_lote"].HeaderText = "n° lote";
                    if (dgvMedicamentos.Columns.Contains("caducidad")) dgvMedicamentos.Columns["caducidad"].HeaderText = "caducidad";
                    if (dgvMedicamentos.Columns.Contains("stock")) dgvMedicamentos.Columns["stock"].HeaderText = "stock";
                    if (dgvMedicamentos.Columns.Contains("total_med")) dgvMedicamentos.Columns["total_med"].HeaderText = "total_med";
                    if (dgvMedicamentos.Columns.Contains("estado")) dgvMedicamentos.Columns["estado"].HeaderText = "estado";

                    // IDs ocultos
                    if (dgvMedicamentos.Columns.Contains("id_med")) dgvMedicamentos.Columns["id_med"].Visible = false;
                    if (dgvMedicamentos.Columns.Contains("id_dosis")) dgvMedicamentos.Columns["id_dosis"].Visible = false;
                    if (dgvMedicamentos.Columns.Contains("id_lote")) dgvMedicamentos.Columns["id_lote"].Visible = false;
                }

                TomarSeleccionDeGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar grilla: " + ex.Message);
            }
        }

        private void TomarSeleccionDeGrilla()
        {
            if (IsDesignMode()) return;

            if (dgvMedicamentos.CurrentRow == null)
            {
                _selMedId = _selMdId = _selLoteId = null;
                _selNombre = _selDosis = _selLoteNum = _selEstado = "";
                _selCad = null; _selStock = 0;
                HabilitarAcciones(false, false);
                return;
            }

            var r = dgvMedicamentos.CurrentRow;
            _selMedId = Convert.ToInt32(r.Cells["id_med"].Value);
            _selMdId = Convert.ToInt32(r.Cells["id_dosis"].Value);
            _selLoteId = Convert.ToInt32(r.Cells["id_lote"].Value);
            _selNombre = r.Cells["nombre"].Value?.ToString() ?? "";
            _selDosis = r.Cells["dosis"].Value?.ToString() ?? "";
            _selLoteNum = r.Cells["n_lote"].Value?.ToString() ?? "";
            _selEstado = r.Cells["estado"].Value?.ToString() ?? "";
            _selStock = Convert.ToInt32(r.Cells["stock"].Value ?? 0);
            _selCad = r.Cells["caducidad"].Value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r.Cells["caducidad"].Value);

            //bool caducado = string.Equals(_selEstado, "caducado", StringComparison.OrdinalIgnoreCase);
            //bool puedeAgregar = true;
            bool puedeBajar = _selStock > 0;

            HabilitarAcciones(true, puedeBajar);
        }


        private void dgvMedicamentos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvMedicamentos_Paint(object sender, PaintEventArgs e)
        {
            if (dgvMedicamentos.Rows.Count == 0)
            {
                string mensaje = "No hay datos para mostrar";
                using (Font font = new Font("Segoe UI", 12, FontStyle.Italic))
                {
                    TextRenderer.DrawText(
                        e.Graphics,
                        mensaje,
                        font,
                        dgvMedicamentos.ClientRectangle,
                        Color.Gray,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
                }
            }
        }

        private void lblCant_Click(object sender, EventArgs e)
        {

        }

        private void txtAltaCantidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void gbAlta_Enter(object sender, EventArgs e)
        {

        }

        private void FormAlmacen_Load(object sender, EventArgs e)
        {

        }

        private void dgvMedicamentos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvMedicamentos.Columns["estado"].Index == e.ColumnIndex && e.Value != null)
            {
                string estado = e.Value.ToString().ToLower();

                if (estado == "caducado")
                {
                    dgvMedicamentos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue; // Azul bajito
                    dgvMedicamentos.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (estado == "agotado")
                {
                    dgvMedicamentos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    dgvMedicamentos.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }
            }
        }

        private void btnAgregarMedicamento_Click(object sender, EventArgs e)
        {
            FormAgregarMedicamento formAgregar = new FormAgregarMedicamento();
            formAgregar.ShowDialog();
        }

        private void HabilitarAcciones(bool puedeAgregar, bool puedeBajar)
        {
            btnAgregar.Enabled = puedeAgregar && _selMdId.HasValue && _selLoteId.HasValue;
            btnBaja.Enabled = puedeBajar && _selMdId.HasValue && _selLoteId.HasValue;
            btnEliminarDef.Enabled = _selMdId.HasValue && _selLoteId.HasValue;
        }

        // ---------------- Acciones ----------------

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (!_selMdId.HasValue || !_selLoteId.HasValue)
            {
                MessageBox.Show("Selecciona una fila.");
                return;
            }
            if (!int.TryParse(txtAltaCantidad.Text, out int cant) || cant <= 0)
            {
                MessageBox.Show("Cantidad inválida (> 0).");
                return;
            }

            try
            {
                using (var cn = NuevaConexion())
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        using (var cmd = new MySqlCommand(
                            @"
                                UPDATE md_stock_lote
                                SET stock = COALESCE(stock, 0) + @c
                                WHERE medicamento_dosis_id = @md AND lote_id = @lote;
                            ", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@c", cant);
                            cmd.Parameters.AddWithValue("@md", _selMdId.Value);
                            cmd.Parameters.AddWithValue("@lote", _selLoteId.Value);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                }

                MessageBox.Show("Stock agregado.");
                txtAltaCantidad.Clear();
                CargarMedicamentosGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void BtnBaja_Click(object sender, EventArgs e)
        {
            if (!_selMdId.HasValue || !_selLoteId.HasValue)
            {
                MessageBox.Show("Selecciona una fila.");
                return;
            }
            if (_selStock <= 0)
            {
                MessageBox.Show("Este lote ya está con stock 0.");
                return;
            }

            try
            {
                using (var cn = NuevaConexion())
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        using (var cmd = new MySqlCommand(
                            @"UPDATE md_stock_lote
                              SET stock = 0
                              WHERE medicamento_dosis_id = @md AND lote_id = @lote;", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@md", _selMdId.Value);
                            cmd.Parameters.AddWithValue("@lote", _selLoteId.Value);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                }

                MessageBox.Show("Baja aplicada (stock = 0).");
                CargarMedicamentosGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en baja: " + ex.Message);
            }
        }

        private void BtnEliminarDefinitivo_Click(object sender, EventArgs e)
        {
            if (!_selMdId.HasValue || string.IsNullOrWhiteSpace(_selLoteNum))
            {
                MessageBox.Show("Selecciona una fila.");
                return;
            }

            var r = MessageBox.Show(
                $"¿Eliminar DEFINITIVAMENTE?\n\n{_selNombre} — {_selDosis}\nLote: {_selLoteNum}\nCad.: {(_selCad.HasValue ? _selCad.Value.ToString("dd/MM/yyyy") : "--")}\n\n" +
                "Esta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (r != DialogResult.Yes) return;

            try
            {
                using (var cn = NuevaConexion())
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        using (var cmd = new MySqlCommand("CALL sp_baja_definitiva_dosis_lote(@md, @loteNum);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@md", _selMdId.Value);
                            cmd.Parameters.AddWithValue("@loteNum", _selLoteNum);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                }

                MessageBox.Show("Eliminación definitiva realizada.");
                CargarMedicamentosGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        // Cerrar sesión → volver a FormLogin
        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            var login = new FormLogin();
            login.Show();
            this.Close();
        }
    }
}
