using Vision.UI;
namespace Vision
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.imageGallery = new Vision.UI.ImageGallery();
            this.label1 = new System.Windows.Forms.Label();
            this.boxSketchPath = new System.Windows.Forms.TextBox();
            this.btnSettings = new System.Windows.Forms.Button();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnSearchSketch = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.imageGallery);
            this.groupBox1.Location = new System.Drawing.Point(12, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(762, 542);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Results";
            // 
            // imageGallery
            // 
            this.imageGallery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageGallery.Location = new System.Drawing.Point(3, 16);
            this.imageGallery.Name = "imageGallery";
            this.imageGallery.Size = new System.Drawing.Size(756, 523);
            this.imageGallery.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sketch Path";
            // 
            // boxSketchPath
            // 
            this.boxSketchPath.Location = new System.Drawing.Point(12, 25);
            this.boxSketchPath.Name = "boxSketchPath";
            this.boxSketchPath.Size = new System.Drawing.Size(609, 20);
            this.boxSketchPath.TabIndex = 2;
            this.boxSketchPath.Click += new System.EventHandler(this.OnSketchPathClick);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(708, 23);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(66, 23);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.OnBtnSettingsClick);
            // 
            // fileDialog
            // 
            this.fileDialog.FileName = "openFileDialog1";
            // 
            // btnSearchSketch
            // 
            this.btnSearchSketch.Location = new System.Drawing.Point(627, 23);
            this.btnSearchSketch.Name = "btnSearchSketch";
            this.btnSearchSketch.Size = new System.Drawing.Size(75, 23);
            this.btnSearchSketch.TabIndex = 5;
            this.btnSearchSketch.Text = "Search";
            this.btnSearchSketch.UseVisualStyleBackColor = true;
            this.btnSearchSketch.Click += new System.EventHandler(this.OnBtnSearchSketchClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 603);
            this.Controls.Add(this.btnSearchSketch);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.boxSketchPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private ImageGallery imageGallery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox boxSketchPath;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.Button btnSearchSketch;
    }
}

