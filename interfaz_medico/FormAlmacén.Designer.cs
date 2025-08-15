// FormAlmacen.Designer.cs
using System.Windows.Forms;

namespace interfaz_medico
{
    partial class FormAlmacen
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvMedicamentos;

        private GroupBox gbAlta;
        private TextBox txtAltaCantidad;
        private Button btnAgregar;

        private GroupBox gbBaja;
        private Button btnBaja;

        private GroupBox gbEliminar;
        private Button btnEliminarDef;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.dgvMedicamentos = new System.Windows.Forms.DataGridView();
            this.gbAlta = new System.Windows.Forms.GroupBox();
            this.lblCant = new System.Windows.Forms.Label();
            this.txtAltaCantidad = new System.Windows.Forms.TextBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.gbBaja = new System.Windows.Forms.GroupBox();
            this.btnBaja = new System.Windows.Forms.Button();
            this.gbEliminar = new System.Windows.Forms.GroupBox();
            this.btnEliminarDef = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnCerrarSesion = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAgregarMedicamento = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicamentos)).BeginInit();
            this.gbAlta.SuspendLayout();
            this.gbBaja.SuspendLayout();
            this.gbEliminar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCerrarSesion)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMedicamentos
            // 
            this.dgvMedicamentos.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvMedicamentos.Location = new System.Drawing.Point(12, 113);
            this.dgvMedicamentos.Name = "dgvMedicamentos";
            this.dgvMedicamentos.Size = new System.Drawing.Size(1880, 730);
            this.dgvMedicamentos.TabIndex = 0;
            this.dgvMedicamentos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvMedicamentos_CellFormatting);
            this.dgvMedicamentos.Paint += new System.Windows.Forms.PaintEventHandler(this.dgvMedicamentos_Paint);
            // 
            // gbAlta
            // 
            this.gbAlta.Controls.Add(this.lblCant);
            this.gbAlta.Controls.Add(this.txtAltaCantidad);
            this.gbAlta.Controls.Add(this.btnAgregar);
            this.gbAlta.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbAlta.ForeColor = System.Drawing.Color.White;
            this.gbAlta.Location = new System.Drawing.Point(491, 870);
            this.gbAlta.Name = "gbAlta";
            this.gbAlta.Size = new System.Drawing.Size(420, 108);
            this.gbAlta.TabIndex = 2;
            this.gbAlta.TabStop = false;
            this.gbAlta.Text = "Agregar stock por dosis (según fila)";
            this.gbAlta.Enter += new System.EventHandler(this.gbAlta_Enter);
            // 
            // lblCant
            // 
            this.lblCant.AutoSize = true;
            this.lblCant.Location = new System.Drawing.Point(15, 49);
            this.lblCant.Name = "lblCant";
            this.lblCant.Size = new System.Drawing.Size(78, 22);
            this.lblCant.TabIndex = 1;
            this.lblCant.Text = "Cantidad:";
            this.lblCant.Click += new System.EventHandler(this.lblCant_Click);
            // 
            // txtAltaCantidad
            // 
            this.txtAltaCantidad.Location = new System.Drawing.Point(99, 46);
            this.txtAltaCantidad.Name = "txtAltaCantidad";
            this.txtAltaCantidad.Size = new System.Drawing.Size(153, 29);
            this.txtAltaCantidad.TabIndex = 2;
            this.txtAltaCantidad.TextChanged += new System.EventHandler(this.txtAltaCantidad_TextChanged);
            // 
            // btnAgregar
            // 
            this.btnAgregar.BackColor = System.Drawing.Color.White;
            this.btnAgregar.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregar.ForeColor = System.Drawing.Color.Teal;
            this.btnAgregar.Location = new System.Drawing.Point(278, 39);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(120, 43);
            this.btnAgregar.TabIndex = 3;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = false;
            // 
            // gbBaja
            // 
            this.gbBaja.Controls.Add(this.btnBaja);
            this.gbBaja.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbBaja.ForeColor = System.Drawing.Color.White;
            this.gbBaja.Location = new System.Drawing.Point(982, 870);
            this.gbBaja.Name = "gbBaja";
            this.gbBaja.Size = new System.Drawing.Size(420, 108);
            this.gbBaja.TabIndex = 3;
            this.gbBaja.TabStop = false;
            this.gbBaja.Text = "Dar de baja por dosis (según fila)";
            // 
            // btnBaja
            // 
            this.btnBaja.BackColor = System.Drawing.Color.White;
            this.btnBaja.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBaja.ForeColor = System.Drawing.Color.Teal;
            this.btnBaja.Location = new System.Drawing.Point(28, 38);
            this.btnBaja.Name = "btnBaja";
            this.btnBaja.Size = new System.Drawing.Size(366, 43);
            this.btnBaja.TabIndex = 1;
            this.btnBaja.Text = "Baja (stock = 0)";
            this.btnBaja.UseVisualStyleBackColor = false;
            // 
            // gbEliminar
            // 
            this.gbEliminar.Controls.Add(this.btnEliminarDef);
            this.gbEliminar.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbEliminar.ForeColor = System.Drawing.Color.White;
            this.gbEliminar.Location = new System.Drawing.Point(1472, 870);
            this.gbEliminar.Name = "gbEliminar";
            this.gbEliminar.Size = new System.Drawing.Size(420, 108);
            this.gbEliminar.TabIndex = 1;
            this.gbEliminar.TabStop = false;
            this.gbEliminar.Text = "Baja definitiva (eliminar)";
            // 
            // btnEliminarDef
            // 
            this.btnEliminarDef.BackColor = System.Drawing.Color.White;
            this.btnEliminarDef.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarDef.ForeColor = System.Drawing.Color.Teal;
            this.btnEliminarDef.Location = new System.Drawing.Point(27, 38);
            this.btnEliminarDef.Name = "btnEliminarDef";
            this.btnEliminarDef.Size = new System.Drawing.Size(368, 43);
            this.btnEliminarDef.TabIndex = 1;
            this.btnEliminarDef.Text = "Eliminar";
            this.btnEliminarDef.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.BtnCerrarSesion);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 100);
            this.panel1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nunito", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(406, 33);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ventana Administrador de almacén";
            // 
            // BtnCerrarSesion
            // 
            this.BtnCerrarSesion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCerrarSesion.Image = global::interfaz_medico.Properties.Resources.logout_red;
            this.BtnCerrarSesion.Location = new System.Drawing.Point(1829, 22);
            this.BtnCerrarSesion.Name = "BtnCerrarSesion";
            this.BtnCerrarSesion.Size = new System.Drawing.Size(54, 54);
            this.BtnCerrarSesion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BtnCerrarSesion.TabIndex = 0;
            this.BtnCerrarSesion.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAgregarMedicamento);
            this.groupBox1.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 870);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(420, 108);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Agregar medicamento";
            // 
            // btnAgregarMedicamento
            // 
            this.btnAgregarMedicamento.Font = new System.Drawing.Font("Nunito", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarMedicamento.ForeColor = System.Drawing.Color.Teal;
            this.btnAgregarMedicamento.Location = new System.Drawing.Point(25, 39);
            this.btnAgregarMedicamento.Name = "btnAgregarMedicamento";
            this.btnAgregarMedicamento.Size = new System.Drawing.Size(368, 43);
            this.btnAgregarMedicamento.TabIndex = 0;
            this.btnAgregarMedicamento.Text = "Agregar";
            this.btnAgregarMedicamento.UseVisualStyleBackColor = true;
            this.btnAgregarMedicamento.Click += new System.EventHandler(this.btnAgregarMedicamento_Click);
            // 
            // FormAlmacen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvMedicamentos);
            this.Controls.Add(this.gbEliminar);
            this.Controls.Add(this.gbAlta);
            this.Controls.Add(this.gbBaja);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormAlmacen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Almacén";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormAlmacen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicamentos)).EndInit();
            this.gbAlta.ResumeLayout(false);
            this.gbAlta.PerformLayout();
            this.gbBaja.ResumeLayout(false);
            this.gbEliminar.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCerrarSesion)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private Label lblCant;
        private Panel panel1;
        private PictureBox BtnCerrarSesion;
        private Label label1;
        private GroupBox groupBox1;
        private Button btnAgregarMedicamento;
    }
}
