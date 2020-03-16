using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Detector;
using Vision.Model;
using Vision.Model.Extractor;
using Vision.performance;
using Vision.Preprocess;

namespace Vision.CBR
{
    public class PhotoSketchCBR
    {
        // METRICS for SIMILARITY
        private static readonly FeatureCompareMetric COMPONENT_METRIC = Metrics.Intersection;
        private static readonly FeatureCompareMetric SHAPE_METRIC = (f1, f2) => 1 - Metrics.CosineDistance(f1, f2);

        private PhotoSketchAlgorithm photoSketchAlgo;
        public FusionStrategy SearchFusionStrategy { get; set; }

        // Database
        public FaceFeaturesDB Database { get; set; }
        
        public PhotoSketchCBR(PhotoSketchAlgorithm featureExtractionAlgorithm)
        {
            photoSketchAlgo = featureExtractionAlgorithm;
            SearchFusionStrategy = new BordaCount(1, 1, 1, 1, 1, 1);
            Database = new FaceFeaturesDB();
        }

        public Rank<PhotoMetadata, double> Search(string sketchPath, Gender gender, int maxImageToRetrive = 10)
        {
            if (Database.Entries.Length <= 0) return Rank.Empty<PhotoMetadata, double>();
            if (SearchFusionStrategy == null) throw new InvalidOperationException("No search fusion strategy is set.");

            var sketchFeature = photoSketchAlgo.ExtractFaceFeatures(sketchPath);

            var ranks = new Rank<PhotoMetadata, double>[6];

            Task.WaitAll(
                Task.Run(() => ranks[0] = MakeComponentRank(Database.FeaturesHair(gender), sketchFeature.Hair, photoSketchAlgo.Options.Hair.TotalPatch)),
                Task.Run(() => ranks[1] = MakeComponentRank(Database.FeaturesEyebrows(gender), sketchFeature.Eyebrows, photoSketchAlgo.Options.EyeBrows.TotalPatch)),
                //Task.Run(() => ranks[2] = MakeComponentRank(Database.FeaturesEyes(gender), sketchFeature.Eyes, photoSketchAlgo.Options.Eyes.TotalPatch)),
                Task.Run(() => ranks[3] = MakeComponentRank(Database.FeaturesNose(gender), sketchFeature.Nose, photoSketchAlgo.Options.Nose.TotalPatch)),
                Task.Run(() => ranks[4] = MakeComponentRank(Database.FeaturesMouth(gender), sketchFeature.Mouth, photoSketchAlgo.Options.Mouth.TotalPatch))
                //Task.Run(() => ranks[5] = Rank.FromMetric(Database.FeaturesShape(gender), sketchFeature.Shape, SHAPE_METRIC).NormalizeScore(ScoreNormalization.Tanh))
                );
            
            return SearchFusionStrategy.Fusion(ranks[0], ranks[1]/*, ranks[2]*/, ranks[3], ranks[4]/*, ranks[5]*/).Take(maxImageToRetrive).ToRank();
        }
        
        private Rank<PhotoMetadata, double> MakeComponentRank(Tuple<PhotoMetadata, float[]>[] dbFeatures, float[] features, int numberOfPatch)
        {
            var rank =  Rank.FromMetric(dbFeatures, features, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, numberOfPatch, COMPONENT_METRIC));
            return rank.NormalizeScore(ScoreNormalization.Tanh);
        }
        
    }
}
