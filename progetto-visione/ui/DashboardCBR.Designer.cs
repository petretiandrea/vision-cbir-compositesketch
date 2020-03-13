using Vision.UI;
namespace Vision.UI
{
    partial class DashboardCBR
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
            this.loadingBox = new System.Windows.Forms.Panel();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.loadingBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.boxSketchPath = new System.Windows.Forms.TextBox();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnSearchSketch = new System.Windows.Forms.Button();
            this.genderBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.boxWeightHair = new System.Windows.Forms.TextBox();
            this.boxWeightEyebrows = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.boxWeightNose = new System.Windows.Forms.TextBox();
            this.boxWeightMouth = new System.Windows.Forms.TextBox();
            this.boxWeightEyes = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.boxWeightShape = new System.Windows.Forms.TextBox();
            this.rankDimension = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.loadingBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.loadingBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 154);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(762, 440);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Results";
            // 
            // loadingBox
            // 
            this.loadingBox.Controls.Add(this.loadingLabel);
            this.loadingBox.Controls.Add(this.loadingBar);
            this.loadingBox.Location = new System.Drawing.Point(174, 135);
            this.loadingBox.Name = "loadingBox";
            this.loadingBox.Size = new System.Drawing.Size(436, 45);
            this.loadingBox.TabIndex = 0;
            this.loadingBox.Visible = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.Location = new System.Drawing.Point(3, 0);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(35, 13);
            this.loadingLabel.TabIndex = 1;
            this.loadingLabel.Text = "label9";
            // 
            // loadingBar
            // 
            this.loadingBar.Location = new System.Drawing.Point(3, 16);
            this.loadingBar.Name = "loadingBar";
            this.loadingBar.Size = new System.Drawing.Size(430, 23);
            this.loadingBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.loadingBar.TabIndex = 0;
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
            this.boxSketchPath.Size = new System.Drawing.Size(471, 20);
            this.boxSketchPath.TabIndex = 2;
            this.boxSketchPath.Click += new System.EventHandler(this.OnSketchPathClick);
            // 
            // fileDialog
            // 
            this.fileDialog.FileName = "openFileDialog1";
            // 
            // btnSearchSketch
            // 
            this.btnSearchSketch.Location = new System.Drawing.Point(699, 23);
            this.btnSearchSketch.Name = "btnSearchSketch";
            this.btnSearchSketch.Size = new System.Drawing.Size(75, 23);
            this.btnSearchSketch.TabIndex = 5;
            this.btnSearchSketch.Text = "Search";
            this.btnSearchSketch.UseVisualStyleBackColor = true;
            this.btnSearchSketch.Click += new System.EventHandler(this.OnBtnSearchSketchClick);
            // 
            // genderBox
            // 
            this.genderBox.DisplayMember = "UNKNOWN";
            this.genderBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.genderBox.FormattingEnabled = true;
            this.genderBox.Location = new System.Drawing.Point(583, 25);
            this.genderBox.Name = "genderBox";
            this.genderBox.Size = new System.Drawing.Size(110, 21);
            this.genderBox.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(580, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Gender";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(12, 51);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(762, 97);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.boxWeightHair, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.boxWeightEyebrows, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.boxWeightNose, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.boxWeightMouth, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.boxWeightEyes, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.boxWeightShape, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(756, 78);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 26);
            this.label3.TabIndex = 0;
            this.label3.Text = "Weight Eyes";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 26);
            this.label5.TabIndex = 6;
            this.label5.Text = "Weight Eyebrows";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boxWeightHair
            // 
            this.boxWeightHair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxWeightHair.Location = new System.Drawing.Point(99, 3);
            this.boxWeightHair.Name = "boxWeightHair";
            this.boxWeightHair.Size = new System.Drawing.Size(283, 20);
            this.boxWeightHair.TabIndex = 7;
            // 
            // boxWeightEyebrows
            // 
            this.boxWeightEyebrows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxWeightEyebrows.Location = new System.Drawing.Point(99, 29);
            this.boxWeightEyebrows.Name = "boxWeightEyebrows";
            this.boxWeightEyebrows.Size = new System.Drawing.Size(283, 20);
            this.boxWeightEyebrows.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(388, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 26);
            this.label6.TabIndex = 6;
            this.label6.Text = "Weight Mouth";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boxWeightNose
            // 
            this.boxWeightNose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxWeightNose.Location = new System.Drawing.Point(469, 3);
            this.boxWeightNose.Name = "boxWeightNose";
            this.boxWeightNose.Size = new System.Drawing.Size(284, 20);
            this.boxWeightNose.TabIndex = 9;
            // 
            // boxWeightMouth
            // 
            this.boxWeightMouth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxWeightMouth.Location = new System.Drawing.Point(469, 29);
            this.boxWeightMouth.Name = "boxWeightMouth";
            this.boxWeightMouth.Size = new System.Drawing.Size(284, 20);
            this.boxWeightMouth.TabIndex = 10;
            // 
            // boxWeightEyes
            // 
            this.boxWeightEyes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxWeightEyes.Location = new System.Drawing.Point(99, 55);
            this.boxWeightEyes.Name = "boxWeightEyes";
            this.boxWeightEyes.Size = new System.Drawing.Size(283, 20);
            this.boxWeightEyes.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 26);
            this.label4.TabIndex = 13;
            this.label4.Text = "Weight Hair";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(388, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 26);
            this.label7.TabIndex = 12;
            this.label7.Text = "Weight Nose";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(388, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 26);
            this.label8.TabIndex = 14;
            this.label8.Text = "Weight Shape";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boxWeightShape
            // 
            this.boxWeightShape.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxWeightShape.Location = new System.Drawing.Point(469, 55);
            this.boxWeightShape.Name = "boxWeightShape";
            this.boxWeightShape.Size = new System.Drawing.Size(284, 20);
            this.boxWeightShape.TabIndex = 15;
            // 
            // rankDimension
            // 
            this.rankDimension.Location = new System.Drawing.Point(489, 25);
            this.rankDimension.Name = "rankDimension";
            this.rankDimension.Size = new System.Drawing.Size(88, 20);
            this.rankDimension.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(486, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Rank Size";
            // 
            // DashboardCBR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 603);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.rankDimension);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.genderBox);
            this.Controls.Add(this.btnSearchSketch);
            this.Controls.Add(this.boxSketchPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "DashboardCBR";
            this.Text = "Photo Sketch CBR";
            this.groupBox1.ResumeLayout(false);
            this.loadingBox.ResumeLayout(false);
            this.loadingBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private ImageGallery imageGallery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox boxSketchPath;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.Button btnSearchSketch;
        private System.Windows.Forms.ComboBox genderBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox rankDimension;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox boxWeightHair;
        private System.Windows.Forms.TextBox boxWeightEyebrows;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox boxWeightNose;
        private System.Windows.Forms.TextBox boxWeightMouth;
        private System.Windows.Forms.TextBox boxWeightEyes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel loadingBox;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.ProgressBar loadingBar;
        private System.Windows.Forms.TextBox boxWeightShape;
        private System.Windows.Forms.Label label9;
    }
}

