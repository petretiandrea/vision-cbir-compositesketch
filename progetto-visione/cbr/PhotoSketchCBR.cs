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
using Vision.Normalization;

namespace Vision.CBR
{
    public class PhotoSketchCBIR
    {
        // METRICS for SIMILARITY
        private static readonly FeatureCompareMetric COMPONENT_METRIC = Metrics.Intersection;
        private static readonly FeatureCompareMetric SHAPE_METRIC = (f1, f2) => 1 - Metrics.CosineDistance(f1, f2);

        private PhotoSketchFeatureExtractor featureExtractor;
        public FusionStrategy SearchFusionStrategy { get; set; }

        // Database
        public FaceFeaturesDB Database { get; set; }
        
        public PhotoSketchCBIR(PhotoSketchFeatureExtractor featureExtractor)
        {
            this.featureExtractor = featureExtractor;
            SearchFusionStrategy = new WeightedSum(1, 1, 1, 1, 1, 1);
            Database = new FaceFeaturesDB();
        }
        

        public Rank<PhotoMetadata, double> Search(string sketchPath, Gender gender, int maxImageToRetrive = 10)
        {
            if (Database.Entries.Length <= 0) return Rank.Empty<PhotoMetadata, double>();
            if (SearchFusionStrategy == null) throw new InvalidOperationException("No search fusion strategy is set.");

            var ranks = ComputeComponentRanks(sketchPath, gender);

            return SearchFusionStrategy.Fusion(ranks[0], ranks[1], ranks[2], ranks[3], ranks[4], ranks[5]).Take(maxImageToRetrive).ToRank();
        }

        public Rank<PhotoMetadata, double>[] ComputeComponentRanks(string sketchPath, Gender gender)
        {
            var sketchFeature = featureExtractor.ExtractFaceFeatures(sketchPath);
            var ranks = new Rank<PhotoMetadata, double>[6];
            
            Task.WaitAll(
                Task.Run(() => ranks[0] = MakeComponentRank(Database.FeaturesHair(gender), sketchFeature.Hair)),
                Task.Run(() => ranks[1] = MakeComponentRank(Database.FeaturesEyebrows(gender), sketchFeature.Eyebrows)),
                Task.Run(() => ranks[2] = MakeComponentRank(Database.FeaturesEyes(gender), sketchFeature.Eyes)),
                Task.Run(() => ranks[3] = MakeComponentRank(Database.FeaturesNose(gender), sketchFeature.Nose)),
                Task.Run(() => ranks[4] = MakeComponentRank(Database.FeaturesMouth(gender), sketchFeature.Mouth)),
                Task.Run(() => ranks[5] = Rank.FromMetric(Database.FeaturesShape(gender), sketchFeature.Shape, SHAPE_METRIC).NormalizeScore(ScoreNormalization.Tanh))
                );

            return ranks;
        }

        private Rank<PhotoMetadata, double> MakeComponentRank(Tuple<PhotoMetadata, double[]>[] dbFeatures, ComponentFeature componentFeatures)
        {
            var rank =  Rank.FromMetric(dbFeatures, componentFeatures.Features, (f1, f2) => ComputeBlockSimilarity(f1, f2, componentFeatures.Blocks, COMPONENT_METRIC));
            return rank.NormalizeScore(ScoreNormalization.Tanh);
        }

        public static double ComputeBlockSimilarity(double[] features1, double[] features2, int numberOfPatch, FeatureCompareMetric metric)
        {
            if (features1.Length != features2.Length) throw new ArgumentException("The features vector must be to same length");

            var featuresChunckLength = features1.Length / numberOfPatch;
            return Enumerable.Range(0, numberOfPatch)
                .AsParallel()
                .Select(cellIndex => new
                {
                    PhotoPatchFeatures = features1.SubArray(cellIndex * featuresChunckLength, featuresChunckLength),
                    SketchPatchFeature = features2.SubArray(cellIndex * featuresChunckLength, featuresChunckLength)
                })
                .Select(chunkFeatures => metric(chunkFeatures.PhotoPatchFeatures, chunkFeatures.SketchPatchFeature))
                .Aggregate(0d, (acc, sim) => acc + sim);
        }

    }
}
