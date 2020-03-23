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
using Vision.Model;
using Vision.UI.Workers;

namespace Vision.UI
{
    public partial class FormGalleryExtraction : Form
    {
        private PhotoSketchFeatureExtractor extractor;
        private TaskExtractGalleryFeatures extractionTask;

        public FormGalleryExtraction(PhotoSketchFeatureExtractor extractor)
        {
            InitializeComponent();
            this.extractor = extractor;
        }

        private void OnBoxGalleryClick(object sender, EventArgs e)
        {
            boxGallery.Text = RequireGalleryFileSelection();
        }

        private void OnBtnExtractClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(boxGallery.Text)) return;

            if (extractionTask != null && extractionTask.IsBusy) return;
            progressBar.Value = 0;
            progressLabel.Text = "Extracting Features...";
            extractionTask = new TaskExtractGalleryFeatures(extractor, boxGallery.Text);
            extractionTask.ProgressChanged += (s, p) =>
            {
                Console.WriteLine("aaaa" + p.ProgressPercentage);
                progressBar.Value = p.ProgressPercentage;
            };
            extractionTask.RunWorkerCompleted += (s, ev) =>
            {
                progressLabel.Text = "Saving gallery features database";
                SaveFeaturesExtracted(ev.Result as FaceFeaturesDB, boxGallery.Text);
            };
            extractionTask.RunWorkerAsync();
        }

        private void SaveFeaturesExtracted(FaceFeaturesDB db, string csvGalleryPath)
        {
            var dbName = string.Format("GalleryFeatures-{0}.csv", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            var folderCsvGallery = Path.GetDirectoryName(Path.GetFullPath(csvGalleryPath));
            var path = Path.Combine(folderCsvGallery, dbName);
            db.Dump(path);
            progressLabel.Text = string.Format("Saved on {0}", path);
        }

        private string RequireGalleryFileSelection()
        {
            fileDialog.Title = "Select a gallery csv";
            fileDialog.FileName = boxGallery.Text ?? "";
            fileDialog.Filter = "Csv Files(*.csv)|*.csv";
            return fileDialog.ShowDialog(this) == DialogResult.OK ? fileDialog.FileName : "";
        }
    }
}
