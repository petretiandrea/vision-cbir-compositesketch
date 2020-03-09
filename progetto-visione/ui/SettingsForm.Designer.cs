namespace Vision.ui
{
    partial class SettingsForm
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
            this.btnStartTrain = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.boxDbPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.weightEyes = new System.Windows.Forms.TextBox();
            this.weightEyebrows = new System.Windows.Forms.TextBox();
            this.weightMouth = new System.Windows.Forms.TextBox();
            this.weightNose = new System.Windows.Forms.TextBox();
            this.weightHair = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelTraining = new System.Windows.Forms.Label();
            this.progressBox = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.progressBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartTrain
            // 
            this.btnStartTrain.Location = new System.Drawing.Point(366, 23);
            this.btnStartTrain.Name = "btnStartTrain";
            this.btnStartTrain.Size = new System.Drawing.Size(94, 23);
            this.btnStartTrain.TabIndex = 0;
            this.btnStartTrain.Text = "Extract Features";
            this.btnStartTrain.UseVisualStyleBackColor = true;
            this.btnStartTrain.Click += new System.EventHandler(this.OnBtnTrainClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database Photo Path";
            // 
            // boxDbPath
            // 
            this.boxDbPath.Location = new System.Drawing.Point(12, 25);
            this.boxDbPath.Name = "boxDbPath";
            this.boxDbPath.Size = new System.Drawing.Size(348, 20);
            this.boxDbPath.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 111);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 150);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.weightEyes, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.weightEyebrows, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.weightMouth, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.weightNose, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.weightHair, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(442, 131);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 26);
            this.label6.TabIndex = 6;
            this.label6.Text = "Weight Mouth";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "Weight Eyes";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // weightEyes
            // 
            this.weightEyes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightEyes.Location = new System.Drawing.Point(99, 3);
            this.weightEyes.Name = "weightEyes";
            this.weightEyes.Size = new System.Drawing.Size(340, 20);
            this.weightEyes.TabIndex = 7;
            // 
            // weightEyebrows
            // 
            this.weightEyebrows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightEyebrows.Location = new System.Drawing.Point(99, 29);
            this.weightEyebrows.Name = "weightEyebrows";
            this.weightEyebrows.Size = new System.Drawing.Size(340, 20);
            this.weightEyebrows.TabIndex = 8;
            // 
            // weightMouth
            // 
            this.weightMouth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightMouth.Location = new System.Drawing.Point(99, 55);
            this.weightMouth.Name = "weightMouth";
            this.weightMouth.Size = new System.Drawing.Size(340, 20);
            this.weightMouth.TabIndex = 9;
            // 
            // weightNose
            // 
            this.weightNose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightNose.Location = new System.Drawing.Point(99, 81);
            this.weightNose.Name = "weightNose";
            this.weightNose.Size = new System.Drawing.Size(340, 20);
            this.weightNose.TabIndex = 10;
            // 
            // weightHair
            // 
            this.weightHair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightHair.Location = new System.Drawing.Point(99, 107);
            this.weightHair.Name = "weightHair";
            this.weightHair.Size = new System.Drawing.Size(340, 20);
            this.weightHair.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 26);
            this.label7.TabIndex = 12;
            this.label7.Text = "Weight Nose";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 27);
            this.label3.TabIndex = 13;
            this.label3.Text = "Weight Hair";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(3, 16);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(442, 23);
            this.progressBar.TabIndex = 4;
            // 
            // labelTraining
            // 
            this.labelTraining.AutoSize = true;
            this.labelTraining.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTraining.Location = new System.Drawing.Point(0, 0);
            this.labelTraining.Name = "labelTraining";
            this.labelTraining.Size = new System.Drawing.Size(35, 13);
            this.labelTraining.TabIndex = 5;
            this.labelTraining.Text = "label4";
            // 
            // progressBox
            // 
            this.progressBox.Controls.Add(this.progressBar);
            this.progressBox.Controls.Add(this.labelTraining);
            this.progressBox.Location = new System.Drawing.Point(12, 52);
            this.progressBox.Name = "progressBox";
            this.progressBox.Size = new System.Drawing.Size(448, 53);
            this.progressBox.TabIndex = 7;
            this.progressBox.Visible = false;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 469);
            this.Controls.Add(this.progressBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.boxDbPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStartTrain);
            this.Name = "SettingsForm";
            this.Text = "TrainingForm";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.progressBox.ResumeLayout(false);
            this.progressBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartTrain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox boxDbPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox weightEyes;
        private System.Windows.Forms.TextBox weightEyebrows;
        private System.Windows.Forms.TextBox weightMouth;
        private System.Windows.Forms.TextBox weightNose;
        private System.Windows.Forms.TextBox weightHair;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelTraining;
        private System.Windows.Forms.Panel progressBox;
    }
}