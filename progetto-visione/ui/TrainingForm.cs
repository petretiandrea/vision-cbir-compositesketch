using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.ui.controller;

namespace Vision.ui
{
    public partial class TrainingForm : Form
    {
        private PhotoSketchCBRController controller;

        public TrainingForm(PhotoSketchCBRController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.controller.TrainCompleted += TrainCompleted;
        }

        private void OnBtnTrainClick(object sender, EventArgs e)
        {
            // TODO: show dialog for choose folder
            if (string.IsNullOrWhiteSpace(boxDbPath.Text)) return;

            var numberOfPhoto = controller.StartTraining(boxDbPath.Text);
            progressBar.Style = ProgressBarStyle.Marquee;
            labelTraining.Text = string.Format("Found {0} photos, extracting features...", numberOfPhoto);
            progressBox.Show();
        }

        private void TrainCompleted(object sender, EventArgs e)
        {
            labelTraining.Text = "Features extracted";
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;
        }
    }
}
