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

namespace Vision.ui.controller
{
    public class PhotoSketchCBRController
    {
        public event EventHandler<Rank<string, double>> SearchCompleted;
        public event EventHandler TrainCompleted;

        private PhotoSketchCBR CBR { get; set; }
        public double[] Weigths { get; }
        public int SearchSize { get; set; }

        private BackgroundWorker searchTask;
        private BackgroundWorker trainTask;

        public PhotoSketchCBRController(PhotoSketchCBR cbr)
        {
            CBR = cbr;
            SearchSize = 50;
        }

        public void StartSearch(string pathImage)
        {
            if (searchTask != null && searchTask.IsBusy) return;
            searchTask = new BackgroundWorker();
            searchTask.DoWork += (sender, e) => e.Result = CBR.Search(pathImage, SearchSize);
            searchTask.RunWorkerCompleted += (sender, e) => SearchCompleted.Invoke(this, e.Result as Rank<string, double>);
            searchTask.RunWorkerAsync();
        }

        public int StartTraining(string dbPath)
        {
            if (trainTask != null && trainTask.IsBusy) return -1;

            var files = Directory.GetFiles(dbPath);
            trainTask = new BackgroundWorker();
            trainTask.DoWork += (sender, e) => CBR.Train(files);
            trainTask.RunWorkerCompleted += (sender, e) => TrainCompleted.Invoke(this, EventArgs.Empty);
            trainTask.RunWorkerAsync();
            return files.Length;
        }

        public void ChangeWeight(params double[] weights)
        {
            CBR.SearchFusionStrategy = new BordaCount<string>(weights);
        }
    }
}
