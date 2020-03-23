namespace Vision.UI
{
    partial class FormGalleryExtraction
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.boxGallery = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExtractFeatures = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // boxGallery
            // 
            this.boxGallery.Location = new System.Drawing.Point(12, 25);
            this.boxGallery.Name = "boxGallery";
            this.boxGallery.Size = new System.Drawing.Size(373, 20);
            this.boxGallery.TabIndex = 0;
            this.boxGallery.Click += new System.EventHandler(this.OnBoxGalleryClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Gallery CSV";
            // 
            // btnExtractFeatures
            // 
            this.btnExtractFeatures.Location = new System.Drawing.Point(391, 23);
            this.btnExtractFeatures.Name = "btnExtractFeatures";
            this.btnExtractFeatures.Size = new System.Drawing.Size(104, 23);
            this.btnExtractFeatures.TabIndex = 2;
            this.btnExtractFeatures.Text = "Extract Features";
            this.btnExtractFeatures.UseVisualStyleBackColor = true;
            this.btnExtractFeatures.Click += new System.EventHandler(this.OnBtnExtractClick);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 105);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(483, 23);
            this.progressBar.TabIndex = 3;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 89);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(0, 13);
            this.progressLabel.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(483, 35);
            this.label2.TabIndex = 5;
            this.label2.Text = "The CSV Gallery file must be in the same folder where is extracted. It contains a" +
    " relative path to each mugshot, gender and an Id.";
            // 
            // fileDialog
            // 
            this.fileDialog.FileName = "openFileDialog1";
            // 
            // GalleryExtractionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 139);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnExtractFeatures);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.boxGallery);
            this.Name = "GalleryExtractionForm";
            this.Text = "GalleryExtractionForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox boxGallery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExtractFeatures;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog fileDialog;
    }
}