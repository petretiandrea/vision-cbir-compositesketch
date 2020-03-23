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

namespace Vision.UI
{
    public partial class FormDashboardCBIR : Form, CBRView
    {
        public event EventHandler OnSearchClick;
        public event EventHandler OnLoadDbClick;

        public string SketchPath { get => boxSketchPath.Text; }
        public string GalleryPath { get => boxDatabasePath.Text; }
        public int RankSize { get => int.Parse(rankDimension.Text); set => rankDimension.Text = string.Format("{0}", value); }
        public double WeightHair { get => double.Parse(boxWeightHair.Text); set => boxWeightHair.Text = string.Format("{0}", value); }
        public double WeightEyebrows { get => double.Parse(boxWeightEyebrows.Text); set => boxWeightEyebrows.Text = string.Format("{0}", value); }
        public double WeightEyes { get => double.Parse(boxWeightEyes.Text); set => boxWeightEyes.Text = string.Format("{0}", value); }
        public double WeightNose { get => double.Parse(boxWeightNose.Text); set => boxWeightNose.Text = string.Format("{0}", value); }
        public double WeightMouth { get => double.Parse(boxWeightMouth.Text); set => boxWeightMouth.Text = string.Format("{0}", value); }
        public double WeightShape { get => double.Parse(boxWeightShape.Text); set => boxWeightShape.Text = string.Format("{0}", value); }

        public string LoadingLabel { get; set; }
        public bool BackgroundLoading { get => loadingBox.Visible; set => TriggerLoading(value); }

        public Gender SketchGender {
            get => (Gender)Enum.Parse(typeof(Gender), genderBox.SelectedItem as string);
            private set => genderBox.SelectedItem = Enum.GetName(typeof(Gender), value);
        }

        
        public FormDashboardCBIR()
        {
            InitializeComponent();
            this.genderBox.Items.AddRange(Enum.GetNames(typeof(Gender)));
            this.genderBox.SelectedItem = Enum.GetName(typeof(Gender), default(Gender));
        }

        public void ShowPhotoResults(string[] images, string[] labels)
        {
            imageGallery.ClearGallery();
            imageGallery.AddImages(images, labels);
        }

        private void TriggerLoading(bool value)
        {
            if(!loadingBox.Visible && value)
            {
                Enabled = false;
                loadingLabel.Text = LoadingLabel;
                loadingBar.Style = ProgressBarStyle.Marquee;
                loadingBox.Visible = true;
            } else
            {
                loadingBox.Visible = false;
                Enabled = true;
            }
        }

        // UI Events
        private void OnBtnSearchSketchClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(boxSketchPath.Text)) return;
            OnSearchClick.Invoke(this, EventArgs.Empty);
        }

        private void OnSketchPathClick(object sender, EventArgs e)
        {
            boxSketchPath.Text = RequireSketchFileSelection();
        }

        private string RequireSketchFileSelection()
        {
            fileDialog.Title = "Select a sketch";
            fileDialog.FileName = boxSketchPath.Text ?? "";
            fileDialog.Filter = "Image Files(*.BMP;*.PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
            return fileDialog.ShowDialog(this) == DialogResult.OK ? fileDialog.FileName : "";
        }

        private void OnDatabasePathClick(object sender, EventArgs e)
        {
            boxDatabasePath.Text = RequireDatabaseFileSelection();
        }

        private void OnBtnLoadDbClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(boxDatabasePath.Text)) return;
            OnLoadDbClick.Invoke(this, EventArgs.Empty);
        }

        private string RequireDatabaseFileSelection()
        {
            fileDialog.Title = "Select a gallery features csv";
            fileDialog.FileName = boxDatabasePath.Text ?? "";
            fileDialog.Filter = "Csv Files(*.csv)|*.csv";
            return fileDialog.ShowDialog(this) == DialogResult.OK ? fileDialog.FileName : "";
        }
    }
}
