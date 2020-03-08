using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Vision.Model;
using Vision.ui;
using Vision.ui.controller;

namespace Vision
{
    public partial class Form1 : Form
    {
        private PhotoSketchCBRController controller;
        private TrainingForm trainingForm;

        public Form1(PhotoSketchCBRController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.controller.SearchCompleted += SearchCompleted;
            this.trainingForm = new TrainingForm(controller);
        }

        private void SearchCompleted(object sender, Rank<string, double> results)
        {
            if (trainingForm != null) trainingForm.Enabled = true;
            ShowResults(results.Select(r => r.Item1).ToArray(), results.Select(r => string.Format("Name: {0}, Score: {1}", Path.GetFileName(r.Item1), r.Item2)).ToArray());
        }

        // UI Events
        private void OnBtnSearchSketchClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(boxSketchPath.Text)) return;
            if (trainingForm != null) trainingForm.Enabled = false;

            controller.StartSearch(boxSketchPath.Text);
        }

        private void OnSketchPathClick(object sender, EventArgs e)
        {
            boxSketchPath.Text = RequireSketchFileSelection();
        }

        private void OnBtnSettingsClick(object sender, EventArgs e)
        {
            if (trainingForm != null && trainingForm.Visible) return;
            trainingForm.StartPosition = FormStartPosition.Manual;
            trainingForm.Location = new Point(Location.X + Size.Width, Location.Y);
            trainingForm.ShowDialog(this);
        }

        public void ShowResults(string[] imageResults, string[] labels)
        {
            imageGallery.ClearGallery();
            for (int i = 0; i < imageResults.Length; i++)
            {
                imageGallery.AddImage(imageResults[i], labels[i]);
            }
        }

        private string RequireSketchFileSelection()
        {
            fileDialog.Title = "Select a sketch";
            fileDialog.FileName = null;
            fileDialog.Filter = "Image Files(*.BMP;*.PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
            return fileDialog.ShowDialog(this) == DialogResult.OK ? fileDialog.FileName : null;
        }

        /*
         * weightEyes.Text = string.Format("{0}", controller.Weigths[0]);
            weightEyebrows.Text = string.Format("{0}", controller.Weigths[1]);
            weightMouth.Text = string.Format("{0}", controller.Weigths[2]);
            weightNose.Text = string.Format("{0}", controller.Weigths[3]);
            weightHair.Text = string.Format("{0}", controller.Weigths[4]);*/
    }
}
