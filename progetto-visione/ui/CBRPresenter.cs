using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vision.CBR;
using Vision.Model;

namespace Vision.UI
{

    public class PhotoSketchCBRPresenter
    {
        private CBRView View { get; set; }
        private PhotoSketchCBR CBR { get; set; }

        private BackgroundWorker searchTask;
        private BackgroundWorker databaseTask;

        private static string DEFAULT_GALLERY_CSV = @"resources/dataset/photo_gallery.csv"; 

        public PhotoSketchCBRPresenter(CBRView view, PhotoSketchCBR cbr)
        {
            View = view;
            CBR = cbr;
            Init();
        }

        private void Init()
        {
            UpdateWeigthsUI();
            View.RankSize = 10;
            View.OnSearchClick += OnSearchClick;
            LoadDatabase(DEFAULT_GALLERY_CSV);
        }

        public void OnSearchClick(object sender, EventArgs eventArg)
        {
            bool searching = StartSearch(View.SketchPath, View.SketchGender, View.RankSize);
            if (searching) {
                View.LoadingLabel = "Searching...";
                View.BackgroundLoading = true;
            }
        }

        private bool StartSearch(string sketchPath, Gender gender, int rankSize)
        {
            if (searchTask != null && searchTask.IsBusy) return false;
            searchTask = new BackgroundWorker();
            searchTask.DoWork += (s, e) =>
            {
                // background work computer a search
                var rank = CBR.Search(sketchPath, gender, rankSize);
                var images = rank.Select(r => r.Item1.Path).ToArray();
                var labels = rank.Select(r => string.Format("Name: {0}, Gender: {1}, Score: {2}", r.Item1.Path, r.Item1.Gender, r.Item2).ToArray());
                e.Result = Tuple.Create(images, labels);
            };
            searchTask.RunWorkerCompleted += SearchCompleted;
            searchTask.RunWorkerAsync();
            return true;
        }

        private void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // when background work complete show the results from UI thread
            var tuple = e.Result as Tuple<string[], string[]>;
            if(tuple != null) View.ShowPhotoResults(tuple.Item1, tuple.Item2);
            View.BackgroundLoading = false;
        }

        private void LoadDatabase(string databaseFilename)
        {
            if (databaseTask != null && databaseTask.IsBusy) return;
            databaseTask = new BackgroundWorker();
            databaseTask.DoWork += (s, e) => e.Result = FaceFeaturesDBDumper.ReadCSV(databaseFilename);
            databaseTask.RunWorkerCompleted += (s, e) => { CBR.Database = e.Result as FaceFeaturesDB; View.BackgroundLoading = false; };
            View.BackgroundLoading = true;
            View.LoadingLabel = "Loading photo database...";
            databaseTask.RunWorkerAsync();
        }

        private void UpdateWeigthsUI()
        {
            if (CBR.SearchFusionStrategy is BordaCount)
            {
                var bordaCount = CBR.SearchFusionStrategy as BordaCount;
                View.WeightHair = bordaCount.Weights[0];
                View.WeightEyebrows = bordaCount.Weights[1];
                View.WeightEyes = bordaCount.Weights[2];
                View.WeightNose = bordaCount.Weights[3];
                View.WeightMouth = bordaCount.Weights[4];
                View.WeightShape = bordaCount.Weights[5];
            }
        }
        private void UpdateFusionStrategy()
        {
            CBR.SearchFusionStrategy = new BordaCount(View.WeightHair, View.WeightEyebrows, View.WeightEyes, View.WeightNose, View.WeightMouth, View.WeightShape);
        }
    }
}
