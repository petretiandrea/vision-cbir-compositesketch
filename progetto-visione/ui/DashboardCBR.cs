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
    public interface CBRView
    {
        event EventHandler OnSearchClick;
        string SketchPath { get; }
        Gender SketchGender { get; }

        int RankSize { get; set; }
        double WeightHair { get; set; }
        double WeightEyebrows { get; set; }
        double WeightEyes { get; set; }
        double WeightNose { get; set; }
        double WeightMouth { get; set; }
        double WeightShape { get; set; }

        void ShowPhotoResults(string[] images, string[] labels);
        bool BackgroundLoading { get; set; }
        string LoadingLabel { get; set; }
    }

    public partial class DashboardCBR : Form, CBRView
    {
        public event EventHandler OnSearchClick;

        public string SketchPath { get => boxSketchPath.Text; }
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

        public DashboardCBR()
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
            if(!loadingBox.Visible)
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
            fileDialog.FileName = null;
            fileDialog.Filter = "Image Files(*.BMP;*.PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
            return fileDialog.ShowDialog(this) == DialogResult.OK ? fileDialog.FileName : null;
        }
    }
}
