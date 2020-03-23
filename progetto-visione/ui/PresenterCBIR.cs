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
using Vision.UI.Workers;

namespace Vision.UI
{

    public class PresenterCBIR
    {
        private CBRView View { get; set; }
        private PhotoSketchCBIR CBR { get; set; }

        private TaskSearchSketch searchTask;
        private TaskLoadGalleryDatabase galleryTask;

        public PresenterCBIR(CBRView view, PhotoSketchCBIR cbr)
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
            View.OnLoadDbClick += OnLoadGalleryClick;
        }

        public void OnSearchClick(object sender, EventArgs eventArg)
        {
            bool searching = StartSearch(View.SketchPath, View.SketchGender, View.RankSize);
            if (searching) {
                View.LoadingLabel = "Searching...";
                View.BackgroundLoading = true;
            }
        }

        public void OnLoadGalleryClick(object sender, EventArgs eventArg)
        {
            if (galleryTask != null && galleryTask.IsBusy) return;

            View.BackgroundLoading = true;
            galleryTask = new TaskLoadGalleryDatabase(View.GalleryPath);
            galleryTask.OnLoadCompleted += (s, e) => {
                CBR.Database = e;
                View.BackgroundLoading = false;
            };
            galleryTask.RunWorkerAsync();
            View.LoadingLabel = "Loading photo database...";
        }

        private bool StartSearch(string sketchPath, Gender gender, int rankSize)
        {
            if (searchTask != null && searchTask.IsBusy) return false;
            searchTask = TaskSearchSketch.NewSearch(CBR, sketchPath, gender, rankSize);
            searchTask.OnSearchCompleted += SearchCompleted;
            searchTask.RunWorkerAsync();
            return true;
        }

        private void SearchCompleted(object sender, Tuple<string[], string[]> imagesLabeled)
        {
            // when background work complete show the results from UI thread
            if(imagesLabeled != null) View.ShowPhotoResults(imagesLabeled.Item1, imagesLabeled.Item2);
            View.BackgroundLoading = false;
        }

        private void UpdateWeigthsUI()
        {
            if (CBR.SearchFusionStrategy is WeightedSum)
            {
                var bordaCount = CBR.SearchFusionStrategy as WeightedSum;
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
            CBR.SearchFusionStrategy = new WeightedSum(View.WeightHair, View.WeightEyebrows, View.WeightEyes, View.WeightNose, View.WeightMouth, View.WeightShape);
        }
    }
}
