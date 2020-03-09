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
    public partial class MainForm : Form
    {
        private PhotoSketchCBRController controller;
        private SettingsForm settings;

        public MainForm(PhotoSketchCBRController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.controller.SearchCompleted += SearchCompleted;
            this.settings = new SettingsForm(controller);
        }

        private void SearchCompleted(object sender, Rank<string, double> results)
        {
            if (settings != null) settings.Enabled = true;
            ShowResults(results.Select(r => r.Item1).ToArray(), results.Select(r => string.Format("Name: {0}, Score: {1}", Path.GetFileName(r.Item1), r.Item2)).ToArray());
        }

        // UI Events
        private void OnBtnSearchSketchClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(boxSketchPath.Text)) return;
            if (settings != null) settings.Enabled = false;

            controller.StartSearch(boxSketchPath.Text, Gender.UNKOWN);
        }

        private void OnSketchPathClick(object sender, EventArgs e)
        {
            boxSketchPath.Text = RequireSketchFileSelection();
        }

        private void OnBtnSettingsClick(object sender, EventArgs e)
        {
            if (settings != null && settings.Visible) return;
            settings.StartPosition = FormStartPosition.Manual;
            settings.Location = new Point(Location.X + Size.Width, Location.Y);
            settings.ShowDialog(this);
        }

        public void ShowResults(string[] imageResults, string[] labels)
        {
            imageGallery.ClearGallery();
            imageGallery.AddImages(imageResults, labels);
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
