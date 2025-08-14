using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;


namespace ClinicaMedica
{
    public partial class FormMedico : Form
    {
        private readonly string _cs;
        private DataTable _medicamentosTabla;
        private bool _silenciarEventos = false;

        // Carpeta donde están tus plantillas/imágenes de recetas
        private readonly string[] _carpetasRecetas = new[]
        {
            @"C:\Users\jesus\OneDrive\Desktop\ACADEMICO\Recetas",
            @"C:\Users\jesus\OneDrive\Desktop\Recetas" // opcional como respaldo
        };

        // *** ÚNICA PLANTILLA A UTILIZAR ***
        // (Ajustado a "Sofia" sin acento para coincidir con el archivo que mostraste)
        private readonly string _plantillaBase = "RECETA_Sofía Isabel Ramírez López";

        // Ya no usamos login para decidir plantilla, pero dejo el campo por si tu UI lo establece en otro lado
        private string _loginActual = null;

        private PictureBox _pbReceta = null;
        private Bitmap _ultimaImagenReceta = null;
        private PrintDocument _printDoc;

        // Coordenadas de escritura sobre la imagen-plantilla
        // Coordenadas de escritura sobre la imagen-plantilla (ajústalas si lo ves desplazado)
        private readonly Rectangle _rcFecha = new Rectangle(990, 205, 520, 48);
        private readonly Rectangle _rcPaciente = new Rectangle(990, 265, 520, 48);
        private readonly Rectangle _rcMedicamentos = new Rectangle(990, 330, 520, 390);


        public FormMedico()
        {
            InitializeComponent();

            var cs = ConfigurationManager.ConnectionStrings["MySqlConnection"];
            _cs = cs != null ? cs.ConnectionString : "";

            ConfigurarAutoComplete();
            CargarPacientes();
            CargarMedicamentos();

            btnNuevaReceta.Click += BtnNuevaReceta_Click;
            btnGenerarReceta.Click += BtnGenerarReceta_Click;
            btnEnviarReceta.Click += BtnEnviarReceta_Click;
            btnCerrarSesion.Click += BtnCerrarSesion_Click;

            cbMedicamento1.SelectedIndexChanged += CbMedicamento_SelectedIndexChanged;
            cbMedicamento2.SelectedIndexChanged += CbMedicamento_SelectedIndexChanged;
            cbMedicamento3.SelectedIndexChanged += CbMedicamento_SelectedIndexChanged;
            cbMedicamento4.SelectedIndexChanged += CbMedicamento_SelectedIndexChanged;

            cbDosis1.SelectedIndexChanged += CbDosis_SelectedIndexChanged;
            cbDosis2.SelectedIndexChanged += CbDosis_SelectedIndexChanged;
            cbDosis3.SelectedIndexChanged += CbDosis_SelectedIndexChanged;
            cbDosis4.SelectedIndexChanged += CbDosis_SelectedIndexChanged;

            cbCantidad1.SelectedIndexChanged += CbCantidad_SelectedIndexChanged;
            cbCantidad2.SelectedIndexChanged += CbCantidad_SelectedIndexChanged;
            cbCantidad3.SelectedIndexChanged += CbCantidad_SelectedIndexChanged;
            cbCantidad4.SelectedIndexChanged += CbCantidad_SelectedIndexChanged;

            _printDoc = new PrintDocument();
            _printDoc.DocumentName = "Receta";
            _printDoc.PrintPage += PrintDoc_PrintPage;
        }

        public void SetLoginUsuario(string login) { _loginActual = login; }

        private MySqlConnection NuevaConexion()
        {
            var cn = new MySqlConnection(_cs);
            if (cn.State != System.Data.ConnectionState.Open) cn.Open();
            return cn;
        }

        private void AsegurarVisorImagen()
        {
            if (_pbReceta != null) return;
            _pbReceta = new PictureBox();
            _pbReceta.Bounds = txtReceta.Bounds;
            _pbReceta.Anchor = txtReceta.Anchor;
            _pbReceta.SizeMode = PictureBoxSizeMode.Zoom;
            _pbReceta.Visible = false;
            this.Controls.Add(_pbReceta);
            _pbReceta.BringToFront();
        }

        private void ConfigurarAutoComplete()
        {
            ComboBox[] combos = new ComboBox[] {
                cbMedicamento1, cbMedicamento2, cbMedicamento3, cbMedicamento4,
                cbDosis1, cbDosis2, cbDosis3, cbDosis4,
                cbFrecuencia1, cbFrecuencia2, cbFrecuencia3, cbFrecuencia4,
                cbCantidad1, cbCantidad2, cbCantidad3, cbCantidad4
            };

            foreach (ComboBox cb in combos)
            {
                try { cb.AutoCompleteSource = AutoCompleteSource.ListItems; cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend; }
                catch { cb.AutoCompleteMode = AutoCompleteMode.None; cb.AutoCompleteSource = AutoCompleteSource.None; }
            }

            cbCantidad1.Enabled = cbCantidad2.Enabled = cbCantidad3.Enabled = cbCantidad4.Enabled = false;
        }

        private void CargarPacientes()
        {
            try
            {
                using (var conexion = NuevaConexion())
                using (var cmd = new MySqlCommand("SELECT id, nombre FROM pacientes", conexion))
                using (var da = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cbPacientes.DataSource = dt;
                    cbPacientes.DisplayMember = "nombre";
                    cbPacientes.ValueMember = "id";
                }
            }
            catch { }
        }

        private void CargarMedicamentos()
        {
            try
            {
                using (var conexion = NuevaConexion())
                using (var cmd = new MySqlCommand("SELECT id, nombre, estado, cantidad FROM medicamentos", conexion))
                using (var da = new MySqlDataAdapter(cmd))
                {
                    _medicamentosTabla = new DataTable();
                    da.Fill(_medicamentosTabla);
                }

                _silenciarEventos = true;

                foreach (ComboBox cb in new ComboBox[] { cbMedicamento1, cbMedicamento2, cbMedicamento3, cbMedicamento4 })
                {
                    cb.DrawItem -= MedicamentoComboBox_DrawItem;

                    cb.DataSource = null;
                    cb.Items.Clear();
                    cb.DisplayMember = string.Empty;
                    cb.ValueMember = string.Empty;

                    cb.DisplayMember = "nombre";
                    cb.ValueMember = "id";
                    cb.DataSource = _medicamentosTabla.Copy();

                    cb.SelectedIndex = -1;
                    cb.Text = string.Empty;

                    cb.DrawMode = DrawMode.OwnerDrawFixed;
                    cb.DrawItem += MedicamentoComboBox_DrawItem;
                }
            }
            catch { }
            finally { _silenciarEventos = false; }
        }

        private void MedicamentoComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (e.Index < 0) return;

            DataRowView row = cb.Items[e.Index] as DataRowView;
            string nombre = Convert.ToString(row["nombre"]);
            int cantidad = 0; try { cantidad = Convert.ToInt32(row["cantidad"]); } catch { }

            e.DrawBackground();
            using (var brush = new SolidBrush(cantidad <= 0 ? Color.Red : e.ForeColor))
                e.Graphics.DrawString(nombre, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void DosisComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (e.Index < 0) return;

            DataRowView row = cb.Items[e.Index] as DataRowView;
            string texto = Convert.ToString(row["dosis"]);
            int stock = 0; try { stock = Convert.ToInt32(row["stock"]); } catch { }

            e.DrawBackground();
            using (var brush = new SolidBrush(stock <= 0 ? Color.Red : e.ForeColor))
                e.Graphics.DrawString(texto, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void CantidadComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (e.Index < 0) return;

            int valor = Convert.ToInt32(cb.Items[e.Index]);
            int stock = (cb.Tag is int) ? (int)cb.Tag : 0;

            e.DrawBackground();
            using (var brush = new SolidBrush(valor > stock ? Color.Red : e.ForeColor))
                e.Graphics.DrawString(valor.ToString(), e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void CargarCantidades(ComboBox cbCantidad, int stock)
        {
            cbCantidad.BeginUpdate();
            cbCantidad.Items.Clear();

            int extras = 5;
            int maxMostrar = Math.Max(stock, 0) + extras;
            for (int i = 1; i <= maxMostrar; i++) cbCantidad.Items.Add(i);

            cbCantidad.Tag = stock;
            cbCantidad.DrawMode = DrawMode.OwnerDrawFixed;
            cbCantidad.DrawItem -= CantidadComboBox_DrawItem;
            cbCantidad.DrawItem += CantidadComboBox_DrawItem;

            cbCantidad.SelectedIndex = -1;
            cbCantidad.Text = string.Empty;
            cbCantidad.Enabled = stock > 0;
            cbCantidad.EndUpdate();
        }

        private void CargarDosisFrecuencia(ComboBox cbMedicamento, ComboBox cbDosis, ComboBox cbFrecuencia)
        {
            if (cbMedicamento == null || cbMedicamento.SelectedIndex == -1) return;

            int idMedicamento;
            if (!int.TryParse(Convert.ToString(cbMedicamento.SelectedValue), out idMedicamento)) return;

            DataTable dosisTabla = new DataTable();
            DataTable frecTabla = new DataTable();

            try
            {
                using (var conexion = NuevaConexion())
                {
                    using (var cmdDosis = new MySqlCommand(
                        "SELECT id, dosis, stock FROM medicamento_dosis WHERE medicamento_id = @id ORDER BY id", conexion))
                    {
                        cmdDosis.Parameters.AddWithValue("@id", idMedicamento);
                        using (var da = new MySqlDataAdapter(cmdDosis)) da.Fill(dosisTabla);
                    }

                    using (var cmdFrecuencia = new MySqlCommand(
                        "SELECT frecuencia FROM medicamento_frecuencia WHERE medicamento_id = @id", conexion))
                    {
                        cmdFrecuencia.Parameters.AddWithValue("@id", idMedicamento);
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

            cbDosis.DisplayMember = "dosis";
            cbDosis.ValueMember = "id";
            cbDosis.DataSource = dosisTabla;

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

            if (cbDosis == cbDosis1) CargarCantidades(cbCantidad1, 0);
            else if (cbDosis == cbDosis2) CargarCantidades(cbCantidad2, 0);
            else if (cbDosis == cbDosis3) CargarCantidades(cbCantidad3, 0);
            else if (cbDosis == cbDosis4) CargarCantidades(cbCantidad4, 0);

            _silenciarEventos = false;
        }

        private void CbMedicamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_silenciarEventos) return;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == -1) return;

            if (EsMedicamentoDuplicado(cb))
            {
                MessageBox.Show("Este medicamento ya fue seleccionado en la receta. Elige uno diferente.");
                _silenciarEventos = true; cb.SelectedIndex = -1; cb.Text = string.Empty; _silenciarEventos = false;
                return;
            }

            DataRowView row = cb.SelectedItem as DataRowView;
            int cantidadStock = 0; try { cantidadStock = Convert.ToInt32(row["cantidad"]); } catch { cantidadStock = 0; }
            if (cantidadStock <= 0)
            {
                MessageBox.Show("Este medicamento está agotado.");
                _silenciarEventos = true; cb.SelectedIndex = -1; cb.Text = string.Empty; _silenciarEventos = false;
                return;
            }

            if (cb == cbMedicamento1) CargarDosisFrecuencia(cbMedicamento1, cbDosis1, cbFrecuencia1);
            else if (cb == cbMedicamento2) CargarDosisFrecuencia(cbMedicamento2, cbDosis2, cbFrecuencia2);
            else if (cb == cbMedicamento3) CargarDosisFrecuencia(cbMedicamento3, cbDosis3, cbFrecuencia3);
            else if (cb == cbMedicamento4) CargarDosisFrecuencia(cbMedicamento4, cbDosis4, cbFrecuencia4);
        }

        private void CbDosis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_silenciarEventos) return;
            ComboBox cbDosis = sender as ComboBox;
            if (cbDosis.SelectedIndex == -1) return;

            DataRowView row = cbDosis.SelectedItem as DataRowView;
            if (row == null) return;

            int stockDosis = 0; try { stockDosis = Convert.ToInt32(row["stock"]); } catch { stockDosis = 0; }

            if (stockDosis <= 0)
            {
                MessageBox.Show("La dosis seleccionada no tiene stock disponible. Elige otra dosis.");
                _silenciarEventos = true; cbDosis.SelectedIndex = -1; cbDosis.Text = string.Empty; _silenciarEventos = false;
                return;
            }

            if (cbDosis == cbDosis1) CargarCantidades(cbCantidad1, stockDosis);
            else if (cbDosis == cbDosis2) CargarCantidades(cbCantidad2, stockDosis);
            else if (cbDosis == cbDosis3) CargarCantidades(cbCantidad3, stockDosis);
            else if (cbDosis == cbDosis4) CargarCantidades(cbCantidad4, stockDosis);
        }

        private void CbCantidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_silenciarEventos) return;

            ComboBox cbCantidad = sender as ComboBox;
            if (cbCantidad.SelectedIndex == -1) return;

            int stock = (cbCantidad.Tag is int) ? (int)cbCantidad.Tag : 0;
            int valor = Convert.ToInt32(cbCantidad.SelectedItem);

            if (valor > stock)
            {
                MessageBox.Show("La cantidad seleccionada excede el stock disponible para la dosis elegida.");
                _silenciarEventos = true; cbCantidad.SelectedIndex = -1; cbCantidad.Text = string.Empty; _silenciarEventos = false;
            }
        }

        private void BtnNuevaReceta_Click(object sender, EventArgs e)
        {
            _silenciarEventos = true;

            foreach (ComboBox cb in new ComboBox[] {
                cbMedicamento1, cbMedicamento2, cbMedicamento3, cbMedicamento4,
                cbDosis1, cbDosis2, cbDosis3, cbDosis4,
                cbFrecuencia1, cbFrecuencia2, cbFrecuencia3, cbFrecuencia4,
                cbCantidad1, cbCantidad2, cbCantidad3, cbCantidad4
            })
            {
                cb.DataSource = (cb == cbMedicamento1 || cb == cbMedicamento2 || cb == cbMedicamento3 || cb == cbMedicamento4)
                                ? cb.DataSource : null;
                cb.SelectedIndex = -1;
                cb.Text = string.Empty;
            }

            cbCantidad1.Enabled = cbCantidad2.Enabled = cbCantidad3.Enabled = cbCantidad4.Enabled = false;

            txtReceta.Clear();
            if (_pbReceta != null) _pbReceta.Visible = false;
            txtReceta.Visible = true;

            if (_ultimaImagenReceta != null) { _ultimaImagenReceta.Dispose(); _ultimaImagenReceta = null; }

            _silenciarEventos = false;

            CargarMedicamentos();
        }

        private void BtnGenerarReceta_Click(object sender, EventArgs e)
        {
            if (cbPacientes.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un paciente.");
                return;
            }

            int bloqueIncompleto = BloqueIncompleto();
            if (bloqueIncompleto > 0)
            {
                MessageBox.Show("Faltan datos en el medicamento #" + bloqueIncompleto + ". Seleccione dosis, frecuencia y cantidad.");
                return;
            }

            // Vista en texto
            StringBuilder receta = new StringBuilder();
            receta.AppendLine("CLÍNICA MÉDICA GENERAL");
            receta.AppendLine("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy"));
            receta.AppendLine("Paciente: " + cbPacientes.Text);
            receta.AppendLine("Receta:");
            AgregarLineaReceta(receta, cbMedicamento1, cbCantidad1, cbDosis1, cbFrecuencia1);
            AgregarLineaReceta(receta, cbMedicamento2, cbCantidad2, cbDosis2, cbFrecuencia2);
            AgregarLineaReceta(receta, cbMedicamento3, cbCantidad3, cbDosis3, cbFrecuencia3);
            AgregarLineaReceta(receta, cbMedicamento4, cbCantidad4, cbDosis4, cbFrecuencia4);
            receta.AppendLine().AppendLine().AppendLine();

            txtReceta.Font = new Font("Courier New", 10);
            txtReceta.Text = receta.ToString();

            try
            {
                // Resolver plantilla única
                string rutaPlantilla = ResolverPlantillaAbsoluta(_plantillaBase);
                if (rutaPlantilla == null)
                {
                    MessageBox.Show(
                        "No se encontró la plantilla:\n" + _plantillaBase +
                        "\nSe buscó en:\n" + RutasBusquedaTexto() +
                        "\n\nArchivos válidos: .png, .jpg, .jpeg, .bmp");
                    if (_pbReceta != null) _pbReceta.Visible = false;
                    txtReceta.Visible = true;
                    if (_ultimaImagenReceta != null) { _ultimaImagenReceta.Dispose(); _ultimaImagenReceta = null; }
                    return;
                }

                Bitmap bmp = RenderRecetaImagen(
                    rutaPlantilla,
                    DateTime.Now.ToString("dd/MM/yyyy"),
                    cbPacientes.Text,
                    FormatearSoloMedicamentosParaPlantilla()
                );

                if (_ultimaImagenReceta != null) _ultimaImagenReceta.Dispose();
                _ultimaImagenReceta = bmp;

                AsegurarVisorImagen();
                if (_pbReceta.Image != null) _pbReceta.Image.Dispose();
                _pbReceta.Image = (Bitmap)bmp.Clone();
                _pbReceta.Visible = true;
                txtReceta.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar la imagen de la receta: " + ex.Message);
                if (_pbReceta != null) _pbReceta.Visible = false;
                txtReceta.Visible = true;
                if (_ultimaImagenReceta != null) { _ultimaImagenReceta.Dispose(); _ultimaImagenReceta = null; }
            }
        }

        private void BtnEnviarReceta_Click(object sender, EventArgs e)
        {
            if (cbPacientes.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un paciente.");
                return;
            }

            int bloqueIncompleto = BloqueIncompleto();
            if (bloqueIncompleto > 0)
            {
                MessageBox.Show("Faltan datos en el medicamento #" + bloqueIncompleto + ". Seleccione dosis, frecuencia y cantidad.");
                return;
            }

            bool exito = false;

            try
            {
                using (var conexion = NuevaConexion())
                using (var tx = conexion.BeginTransaction())
                {
                    using (var cmd = new MySqlCommand(@"
                        INSERT INTO recetas 
                        (id_paciente, 
                         id_medicamento1, dosis1, frecuencia1, 
                         id_medicamento2, dosis2, frecuencia2, 
                         id_medicamento3, dosis3, frecuencia3,
                         id_medicamento4, dosis4, frecuencia4,
                         estado)
                        VALUES 
                        (@paciente, 
                         @med1, @d1, @f1, 
                         @med2, @d2, @f2,
                         @med3, @d3, @f3,
                         @med4, @d4, @f4,
                         'Pendiente')", conexion, tx))
                    {
                        cmd.Parameters.AddWithValue("@paciente", cbPacientes.SelectedValue);
                        cmd.Parameters.AddWithValue("@med1", cbMedicamento1.SelectedValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@d1", cbDosis1.Text);
                        cmd.Parameters.AddWithValue("@f1", cbFrecuencia1.Text);
                        cmd.Parameters.AddWithValue("@med2", cbMedicamento2.SelectedValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@d2", cbDosis2.Text);
                        cmd.Parameters.AddWithValue("@f2", cbFrecuencia2.Text);
                        cmd.Parameters.AddWithValue("@med3", cbMedicamento3.SelectedValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@d3", cbDosis3.Text);
                        cmd.Parameters.AddWithValue("@f3", cbFrecuencia3.Text);
                        cmd.Parameters.AddWithValue("@med4", cbMedicamento4.SelectedValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@d4", cbDosis4.Text);
                        cmd.Parameters.AddWithValue("@f4", cbFrecuencia4.Text);
                        cmd.ExecuteNonQuery();
                    }

                    DescontarTx(cbMedicamento1, cbDosis1, cbCantidad1, conexion, tx);
                    DescontarTx(cbMedicamento2, cbDosis2, cbCantidad2, conexion, tx);
                    DescontarTx(cbMedicamento3, cbDosis3, cbCantidad3, conexion, tx);
                    DescontarTx(cbMedicamento4, cbDosis4, cbCantidad4, conexion, tx);

                    tx.Commit();
                    exito = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo enviar la receta a farmacia. Inténtalo de nuevo.\n\nDetalle: " + ex.Message);
            }

            CargarMedicamentos();
            if (exito) MessageBox.Show("Receta enviada a farmacia.");
        }

        private void BtnImprimirReceta_Click(object sender, EventArgs e)
        {
            if (_ultimaImagenReceta == null)
            {
                MessageBox.Show("No hay una receta generada para imprimir.");
                return;
            }

            using (var dlg = new PrintDialog { Document = _printDoc, UseEXDialog = true })
                if (dlg.ShowDialog(this) == DialogResult.OK) _printDoc.Print();
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_ultimaImagenReceta == null) { e.HasMorePages = false; return; }

            Rectangle m = e.MarginBounds;
            Size imgSize = _ultimaImagenReceta.Size;
            float ratio = Math.Min((float)m.Width / imgSize.Width, (float)m.Height / imgSize.Height);
            int w = (int)(imgSize.Width * ratio);
            int h = (int)(imgSize.Height * ratio);
            Rectangle dest = new Rectangle(m.Left + (m.Width - w) / 2, m.Top + (m.Height - h) / 2, w, h);
            e.Graphics.DrawImage(_ultimaImagenReceta, dest);
            e.HasMorePages = false;
        }

        // Cerrar sesión y regresar a FormLogin (sin cerrar la app)
        private void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            var login = new interfaz_medico.FormLogin();
            login.Show();
            this.Close();
        }

        private void FormMedico_Load(object sender, EventArgs e) { }

        private bool EsMedicamentoDuplicado(ComboBox quien)
        {
            object val = quien.SelectedValue;
            if (val == null || val == DBNull.Value) return false;

            ComboBox[] todos = new ComboBox[] { cbMedicamento1, cbMedicamento2, cbMedicamento3, cbMedicamento4 };
            foreach (ComboBox cb in todos)
            {
                if (cb == quien) continue;
                if (cb.SelectedIndex != -1 && cb.SelectedValue != null && Equals(cb.SelectedValue, val))
                    return true;
            }
            return false;
        }

        private int BloqueIncompleto()
        {
            if (cbMedicamento1.SelectedIndex != -1 &&
                (cbDosis1.SelectedIndex == -1 || cbFrecuencia1.SelectedIndex == -1 || cbCantidad1.SelectedIndex == -1)) return 1;

            if (cbMedicamento2.SelectedIndex != -1 &&
                (cbDosis2.SelectedIndex == -1 || cbFrecuencia2.SelectedIndex == -1 || cbCantidad2.SelectedIndex == -1)) return 2;

            if (cbMedicamento3.SelectedIndex != -1 &&
                (cbDosis3.SelectedIndex == -1 || cbFrecuencia3.SelectedIndex == -1 || cbCantidad3.SelectedIndex == -1)) return 3;

            if (cbMedicamento4.SelectedIndex != -1 &&
                (cbDosis4.SelectedIndex == -1 || cbFrecuencia4.SelectedIndex == -1 || cbCantidad4.SelectedIndex == -1)) return 4;

            return 0;
        }

        private void AgregarLineaReceta(StringBuilder receta, ComboBox med, ComboBox cant, ComboBox dosis, ComboBox frecuencia)
        {
            if (med.SelectedIndex != -1 && dosis.SelectedIndex != -1 && frecuencia.SelectedIndex != -1)
            {
                string cantidadTxt = (cant.SelectedIndex != -1) ? (" x " + Convert.ToString(cant.SelectedItem)) : "";
                receta.AppendLine("- " + med.Text + cantidadTxt + ": " + dosis.Text + ", " + frecuencia.Text);
            }
        }

        private void AgregarLineaRecetaPlano(StringBuilder sb, ComboBox med, ComboBox cant, ComboBox dosis, ComboBox frecuencia)
        {
            if (med.SelectedIndex != -1 && dosis.SelectedIndex != -1 && frecuencia.SelectedIndex != -1)
            {
                string cantidadTxt = (cant.SelectedIndex != -1) ? (" x " + Convert.ToString(cant.SelectedItem)) : "";
                sb.AppendLine(med.Text + cantidadTxt + ": " + dosis.Text + ", " + frecuencia.Text);
            }
        }

        private string FormatearSoloMedicamentosParaPlantilla()
        {
            StringBuilder sb = new StringBuilder();
            AgregarLineaRecetaPlano(sb, cbMedicamento1, cbCantidad1, cbDosis1, cbFrecuencia1);
            AgregarLineaRecetaPlano(sb, cbMedicamento2, cbCantidad2, cbDosis2, cbFrecuencia2);
            AgregarLineaRecetaPlano(sb, cbMedicamento3, cbCantidad3, cbDosis3, cbFrecuencia3);
            AgregarLineaRecetaPlano(sb, cbMedicamento4, cbCantidad4, cbDosis4, cbFrecuencia4);
            return sb.ToString().TrimEnd();
        }

        private string ResolverPlantillaAbsoluta(string nombreBase)
        {
            string[] exts = new[] { ".png", ".jpg", ".jpeg", ".bmp" };

            foreach (var carpeta in _carpetasRecetas)
            {
                if (!Directory.Exists(carpeta)) continue;

                // 1) Intento directo por cada extensión
                foreach (var ext in exts)
                {
                    string p = Path.Combine(carpeta, nombreBase + ext);
                    if (File.Exists(p)) return p;
                }

                // 2) Búsqueda recursiva en subcarpetas (por si la imagen no está en la raíz)
                try
                {
                    foreach (var ruta in Directory.EnumerateFiles(carpeta, "*", SearchOption.AllDirectories))
                    {
                        var sinExt = Path.GetFileNameWithoutExtension(ruta);
                        var ext = Path.GetExtension(ruta);
                        if (sinExt.Equals(nombreBase, StringComparison.OrdinalIgnoreCase) &&
                            exts.Contains(ext, StringComparer.OrdinalIgnoreCase))
                        {
                            return ruta;
                        }
                    }
                }
                catch { /* sin bloqueo si alguna subcarpeta no se puede leer */ }
            }

            return null;
        }

        private string RutasBusquedaTexto()
        {
            return string.Join(Environment.NewLine, _carpetasRecetas);
        }

        private Bitmap RenderRecetaImagen(string plantillaAbs, string fecha, string paciente, string medicamentosTexto)
        {
            Bitmap baseImg = new Bitmap(plantillaAbs);
            try
            {
                Bitmap bmp = new Bitmap(baseImg.Width, baseImg.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.DrawImage(baseImg, 0, 0, baseImg.Width, baseImg.Height);

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    // Fuentes base (se ajustan si no caben)
                    using (var fontFechaBase = new Font("Arial", 22, FontStyle.Regular, GraphicsUnit.Point))
                    using (var fontPacienteBase = new Font("Arial", 22, FontStyle.Regular, GraphicsUnit.Point))
                    using (var fontMedBase = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Point))
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        // Formatos
                        using (var formatoCentro = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                        using (var formatoWrap = new StringFormat(StringFormatFlags.LineLimit) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near, Trimming = StringTrimming.EllipsisWord })
                        {
                            // Fecha y Paciente centrados verticalmente en su línea
                            DrawTextFitted(g, fecha, fontFechaBase, brush, _rcFecha, formatoCentro, minSize: 14f, maxSize: 28f);
                            DrawTextFitted(g, paciente, fontPacienteBase, brush, _rcPaciente, formatoCentro, minSize: 14f, maxSize: 28f);

                            // Medicamentos con viñetas y salto de línea
                            string medsConViñetas = ToBullets(medicamentosTexto);
                            DrawTextFitted(g, medsConViñetas, fontMedBase, brush, _rcMedicamentos, formatoWrap, minSize: 12f, maxSize: 22f);
                        }
                    }
                }
                return bmp;
            }
            finally
            {
                baseImg.Dispose();
            }
        }

        // Convierte "A\nB\nC" en "• A\n• B\n• C"
        private static string ToBullets(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            var lineas = texto
                .Replace("\r\n", "\n")
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => "• " + l.Trim());
            return string.Join(Environment.NewLine, lineas);
        }

        // Dibuja el texto ajustando el tamaño de la fuente para que quepa en el rectángulo
        private static void DrawTextFitted(Graphics g, string text, Font baseFont, Brush brush, Rectangle rect, StringFormat sf, float minSize, float maxSize)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // Buscamos el mayor tamaño que quepa (búsqueda binaria)
            float low = minSize, high = maxSize, best = minSize;
            for (int i = 0; i < 12; i++)
            {
                float mid = (low + high) / 2f;
                using (var testFont = new Font(baseFont.FontFamily, mid, baseFont.Style, baseFont.Unit))
                {
                    var sz = g.MeasureString(text, testFont, rect.Size, sf);
                    bool cabe = (sz.Width <= rect.Width + 0.5f) && (sz.Height <= rect.Height + 0.5f);
                    if (cabe) { best = mid; low = mid; } else { high = mid; }
                }
                if (Math.Abs(high - low) < 0.5f) break;
            }

            using (var finalFont = new Font(baseFont.FontFamily, best, baseFont.Style, baseFont.Unit))
            {
                g.DrawString(text, finalFont, brush, rect, sf);
            }
        }


        // Descuento por dosis + recálculo del total por medicamento
        private void DescontarTx(ComboBox cbMedicamento, ComboBox cbDosis, ComboBox cbCantidad, MySqlConnection conn, MySqlTransaction tx)
        {
            if (cbMedicamento.SelectedIndex == -1 || cbDosis.SelectedIndex == -1 || cbCantidad.SelectedIndex == -1) return;

            int idMed = Convert.ToInt32(cbMedicamento.SelectedValue);
            int idDosis = Convert.ToInt32(((DataRowView)cbDosis.SelectedItem)["id"]);
            int cantidadDescontar = Convert.ToInt32(cbCantidad.SelectedItem);

            using (var cmdDosis = new MySqlCommand(
                "UPDATE medicamento_dosis SET stock = stock - @cant WHERE id = @idDosis", conn, tx))
            {
                cmdDosis.Parameters.AddWithValue("@cant", cantidadDescontar);
                cmdDosis.Parameters.AddWithValue("@idDosis", idDosis);
                cmdDosis.ExecuteNonQuery();
            }

            // Recalcular totales del medicamento a partir de las dosis
            RecalcularTotalesMedicamento(conn, tx, idMed);
        }

        private void RecalcularTotalesMedicamento(MySqlConnection conn, MySqlTransaction tx, int idMed)
        {
            using (var cmd = new MySqlCommand(@"
                UPDATE medicamentos m
                JOIN (SELECT medicamento_id, COALESCE(SUM(stock),0) AS total
                      FROM medicamento_dosis
                      WHERE medicamento_id=@id
                      GROUP BY medicamento_id) x
                  ON x.medicamento_id = m.id
                SET m.cantidad = x.total,
                    m.estado = CASE WHEN x.total > 0 THEN 'disponible' ELSE 'agotado' END
                WHERE m.id = @id;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", idMed);
                cmd.ExecuteNonQuery();
            }
        }

        // Stubs del diseñador
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void BtnEscanearQr_Click(object sender, EventArgs e) { }
        private void TxtReceta_TextChanged(object sender, EventArgs e) { }
    }
}
