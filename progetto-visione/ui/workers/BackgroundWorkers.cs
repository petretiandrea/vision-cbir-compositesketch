using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vision.CBR;
using Vision.Model;

namespace Vision.UI.Workers
{
    public class TaskSearchSketch : BackgroundWorker
    {
        public EventHandler<Tuple<string[], string[]>> OnSearchCompleted;

        public static TaskSearchSketch NewSearch(PhotoSketchCBIR cbr, string sketchPath, Gender gender, int rankSize)
        {
            return new TaskSearchSketch(cbr, sketchPath, gender, rankSize);
        }

        protected TaskSearchSketch(PhotoSketchCBIR cbr, string sketchPath, Gender gender, int rankSize)
        {
            Func<Tuple<PhotoMetadata, double>, string> formatLabel = t =>
                string.Format("Id: {0}, Gender: {1}, Score: {2}", t.Item1.Id, t.Item1.Gender, t.Item2);

            DoWork += (s, e) =>
            {
                // background work compute a search
                var rank = cbr.Search(sketchPath, gender, rankSize);
                var images = rank.Select(r => r.Item1.AbsolutePath).ToArray();
                var labels = rank.Select(formatLabel).ToArray();
                e.Result = Tuple.Create(images, labels);
            };

            RunWorkerCompleted += (s, e) => OnSearchCompleted.Invoke(this, e.Result as Tuple<string[], string[]>);
        }
    }

    public class TaskLoadGalleryDatabase : BackgroundWorker
    {
        public EventHandler<FaceFeaturesDB> OnLoadCompleted;

        public TaskLoadGalleryDatabase(string path)
        {
            DoWork += (s, e) => e.Result = FaceFeaturesDB.CreateFromDump(path);
            RunWorkerCompleted += (s, e) => {
                OnLoadCompleted.Invoke(this, e.Result as FaceFeaturesDB);
            };
        }
    }

    public class TaskExtractGalleryFeatures : BackgroundWorker
    {
        public TaskExtractGalleryFeatures(PhotoSketchFeatureExtractor extractor, string galleryPath)
        {
            WorkerReportsProgress = true;
            DoWork += (s, e) =>
            {
                var db = new FaceFeaturesDB();
                var photos = PhotoMetadataCsv.FromCSV(galleryPath).ToList();
                var totalPhotos = photos.Count;
                var currentProgress = 0;

                Parallel.ForEach(photos, photo =>
                {
                    try {
                        var feature = extractor.ExtractFaceFeatures(photo.AbsolutePath);
                        db.AddPhotoFeatures(photo, feature);
                    } catch (ArgumentException) { }
                    
                    Interlocked.Increment(ref currentProgress);
                    ReportProgress((int) (currentProgress / (float) totalPhotos * 100));
                });

                e.Result = db;
            };
        }
    }
}
