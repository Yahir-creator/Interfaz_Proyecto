// FormAlmacen.Designer.cs
using System.Windows.Forms;

namespace interfaz_medico
{
    partial class FormAlmacen
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvMedicamentos;

        private GroupBox gbAlta;
        private Label lblSelAlta;
        private TextBox txtAltaCantidad;
        private Button btnAgregar;

        private GroupBox gbBaja;
        private Label lblSelBaja;
        private Button btnBaja;

        private GroupBox gbEliminar;
        private Label lblSelEliminar;
        private Button btnEliminarDef;

        private Button btnCerrarSesion;

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
            this.lblSelAlta = new System.Windows.Forms.Label();
            this.lblCant = new System.Windows.Forms.Label();
            this.txtAltaCantidad = new System.Windows.Forms.TextBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.gbBaja = new System.Windows.Forms.GroupBox();
            this.lblSelBaja = new System.Windows.Forms.Label();
            this.btnBaja = new System.Windows.Forms.Button();
            this.gbEliminar = new System.Windows.Forms.GroupBox();
            this.lblSelEliminar = new System.Windows.Forms.Label();
            this.btnEliminarDef = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicamentos)).BeginInit();
            this.gbAlta.SuspendLayout();
            this.gbBaja.SuspendLayout();
            this.gbEliminar.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMedicamentos
            // 
            this.dgvMedicamentos.Location = new System.Drawing.Point(12, 12);
            this.dgvMedicamentos.Name = "dgvMedicamentos";
            this.dgvMedicamentos.Size = new System.Drawing.Size(860, 360);
            this.dgvMedicamentos.TabIndex = 0;
            // 
            // gbAlta
            // 
            this.gbAlta.Controls.Add(this.lblSelAlta);
            this.gbAlta.Controls.Add(this.lblCant);
            this.gbAlta.Controls.Add(this.txtAltaCantidad);
            this.gbAlta.Controls.Add(this.btnAgregar);
            this.gbAlta.Location = new System.Drawing.Point(12, 385);
            this.gbAlta.Name = "gbAlta";
            this.gbAlta.Size = new System.Drawing.Size(560, 180);
            this.gbAlta.TabIndex = 2;
            this.gbAlta.TabStop = false;
            this.gbAlta.Text = "Agregar stock por dosis (según fila)";
            // 
            // lblSelAlta
            // 
            this.lblSelAlta.Location = new System.Drawing.Point(15, 28);
            this.lblSelAlta.Name = "lblSelAlta";
            this.lblSelAlta.Size = new System.Drawing.Size(520, 23);
            this.lblSelAlta.TabIndex = 0;
            this.lblSelAlta.Text = "Fila seleccionada: —";
            // 
            // lblCant
            // 
            this.lblCant.AutoSize = true;
            this.lblCant.Location = new System.Drawing.Point(15, 70);
            this.lblCant.Name = "lblCant";
            this.lblCant.Size = new System.Drawing.Size(52, 13);
            this.lblCant.TabIndex = 1;
            this.lblCant.Text = "Cantidad:";
            // 
            // txtAltaCantidad
            // 
            this.txtAltaCantidad.Location = new System.Drawing.Point(85, 66);
            this.txtAltaCantidad.Name = "txtAltaCantidad";
            this.txtAltaCantidad.Size = new System.Drawing.Size(100, 20);
            this.txtAltaCantidad.TabIndex = 2;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(200, 64);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(120, 23);
            this.btnAgregar.TabIndex = 3;
            this.btnAgregar.Text = "Agregar";
            // 
            // gbBaja
            // 
            this.gbBaja.Controls.Add(this.lblSelBaja);
            this.gbBaja.Controls.Add(this.btnBaja);
            this.gbBaja.Location = new System.Drawing.Point(578, 385);
            this.gbBaja.Name = "gbBaja";
            this.gbBaja.Size = new System.Drawing.Size(294, 180);
            this.gbBaja.TabIndex = 3;
            this.gbBaja.TabStop = false;
            this.gbBaja.Text = "Dar de baja por dosis (según fila)";
            // 
            // lblSelBaja
            // 
            this.lblSelBaja.Location = new System.Drawing.Point(15, 28);
            this.lblSelBaja.Name = "lblSelBaja";
            this.lblSelBaja.Size = new System.Drawing.Size(260, 23);
            this.lblSelBaja.TabIndex = 0;
            this.lblSelBaja.Text = "Fila seleccionada: —";
            // 
            // btnBaja
            // 
            this.btnBaja.Location = new System.Drawing.Point(15, 64);
            this.btnBaja.Name = "btnBaja";
            this.btnBaja.Size = new System.Drawing.Size(120, 23);
            this.btnBaja.TabIndex = 1;
            this.btnBaja.Text = "Baja (stock = 0)";
            // 
            // gbEliminar
            // 
            this.gbEliminar.Controls.Add(this.lblSelEliminar);
            this.gbEliminar.Controls.Add(this.btnEliminarDef);
            this.gbEliminar.Location = new System.Drawing.Point(880, 12);
            this.gbEliminar.Name = "gbEliminar";
            this.gbEliminar.Size = new System.Drawing.Size(280, 160);
            this.gbEliminar.TabIndex = 1;
            this.gbEliminar.TabStop = false;
            this.gbEliminar.Text = "Baja definitiva (eliminar)";
            // 
            // lblSelEliminar
            // 
            this.lblSelEliminar.Location = new System.Drawing.Point(15, 25);
            this.lblSelEliminar.Name = "lblSelEliminar";
            this.lblSelEliminar.Size = new System.Drawing.Size(250, 23);
            this.lblSelEliminar.TabIndex = 0;
            this.lblSelEliminar.Text = "Fila seleccionada: —";
            // 
            // btnEliminarDef
            // 
            this.btnEliminarDef.Location = new System.Drawing.Point(15, 90);
            this.btnEliminarDef.Name = "btnEliminarDef";
            this.btnEliminarDef.Size = new System.Drawing.Size(250, 23);
            this.btnEliminarDef.TabIndex = 1;
            this.btnEliminarDef.Text = "Eliminar definitivamente";
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.Location = new System.Drawing.Point(988, 525);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(172, 40);
            this.btnCerrarSesion.TabIndex = 4;
            this.btnCerrarSesion.Text = "Cerrar sesión";
            // 
            // FormAlmacen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1172, 580);
            this.Controls.Add(this.dgvMedicamentos);
            this.Controls.Add(this.gbEliminar);
            this.Controls.Add(this.gbAlta);
            this.Controls.Add(this.gbBaja);
            this.Controls.Add(this.btnCerrarSesion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormAlmacen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Almacén";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicamentos)).EndInit();
            this.gbAlta.ResumeLayout(false);
            this.gbAlta.PerformLayout();
            this.gbBaja.ResumeLayout(false);
            this.gbEliminar.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private Label lblCant;
    }
}
