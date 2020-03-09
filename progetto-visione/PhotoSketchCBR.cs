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
using Vision.Preprocess;


using FaceFeatures = Vision.Model.Face<float[]>;

namespace Vision.CBR
{
    public class PhotoSketchCBR /*: CBR*/ 
    {
        static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";
        
        private FaceComponentsExtractor faceComponentsExtractor = new DefaultFaceComponentsExtractor(
                new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL)
            );

        // PATCH Dimensions
        private const int PATCH_EYES = 5;
        private const int PATCH_EYEBROWS = 5;
        private const int PATCH_NOSE = 5;
        private const int PATCH_MOUTH = 5;
        private const int PATCH_HAIR = 5;

        // METRICS for SIMILARITY
        private static readonly FeatureCompareMetric COMPONENT_METRIC = Metrics.Intersection;
        private static readonly FeatureCompareMetric SHAPE_METRIC = (f1, f2) => 1 - Metrics.CosineDistance(f1, f2);
        
        // Feature extraction and fusion algorithms
        private MultiscaleLBP lbp = CircularMultiscaleLBP.Create(1);
        private Dictionary<FaceComponent, FeatureExtractor> featureExtractors;
        public FusionStrategy SearchFusionStrategy { get; set; }

        // Database
        public FaceFeaturesDB Database { get; private set; }

        public PhotoSketchCBR()
        {
            Database = new FaceFeaturesDB();
            this.SearchFusionStrategy = new BordaCount(1, 1, 1, 1, 1, 1);
            this.featureExtractors = new Dictionary<FaceComponent, FeatureExtractor>
            {
                { FaceComponent.EYES, new MLBPFeatureExtractor(lbp, PATCH_EYES) },
                { FaceComponent.EYEBROWS, new MLBPFeatureExtractor(lbp, PATCH_EYEBROWS) },
                { FaceComponent.MOUTH, new MLBPFeatureExtractor(lbp, PATCH_NOSE) },
                { FaceComponent.NOSE, new MLBPFeatureExtractor(lbp, PATCH_MOUTH) },
                { FaceComponent.HAIR, new MLBPFeatureExtractor(lbp, PATCH_HAIR) }
            };
        }

        public void Train(string[] photoDb)
        {
            foreach (var photo in photoDb)
            {
                var normPhoto = Preprocessing.PreprocessImage<byte>(photo);
                var faceComponents = faceComponentsExtractor.ExtractComponentsFromImage(normPhoto);
                var faceFeatures = ExtractFaceFeatures(faceComponents, featureExtractors);
                Database.AddPhoto(photo, Gender.MALE, faceFeatures.Eyes, faceFeatures.Eyebrows, faceFeatures.Nose, faceFeatures.Mouth, faceFeatures.Hair, faceFeatures.Shape);
            }
        }

        public Rank<string, double> Search(string queryImagePath, Gender gender = default(Gender), int maxImageToRetrive = 10)
        {
            if (SearchFusionStrategy == null) throw new InvalidOperationException("No search fusion strategy is set.");

            var normPhoto = Preprocessing.PreprocessImage<byte>(queryImagePath);
            var componentsImages = faceComponentsExtractor.ExtractComponentsFromImage(normPhoto);
            var sketchFeature = ExtractFaceFeatures(componentsImages, featureExtractors);

            var rankEye = Rank.FromMetric(Database.FeaturesEyes(gender), sketchFeature.Eyes, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_EYES, COMPONENT_METRIC));
            var rankBrows = Rank.FromMetric(Database.FeaturesEyebrows(gender), sketchFeature.Eyebrows, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_EYEBROWS, COMPONENT_METRIC));
            var rankNose = Rank.FromMetric(Database.FeaturesNose(gender), sketchFeature.Nose, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_NOSE, COMPONENT_METRIC));
            var rankMouth = Rank.FromMetric(Database.FeaturesMouth(gender), sketchFeature.Mouth, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_MOUTH, COMPONENT_METRIC));
            var rankHair = Rank.FromMetric(Database.FeaturesHair(gender), sketchFeature.Hair, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_HAIR, COMPONENT_METRIC));
            var rankShape = Rank.FromMetric(Database.FeatureShape(gender), sketchFeature.Shape, SHAPE_METRIC);

            rankEye.NormalizeScore(ScoreNormalization.Tanh);
            rankBrows.NormalizeScore(ScoreNormalization.Tanh);
            rankNose.NormalizeScore(ScoreNormalization.Tanh);
            rankMouth.NormalizeScore(ScoreNormalization.Tanh);
            rankHair.NormalizeScore(ScoreNormalization.Tanh);
            rankShape.NormalizeScore(ScoreNormalization.Tanh);

            return SearchFusionStrategy.Fusion(rankEye, rankBrows, rankNose, rankMouth, rankHair, rankShape).Take(maxImageToRetrive).ToRank();
        }

        private FaceFeatures ExtractFaceFeatures(Face<Image<Gray, byte>> face, Dictionary<FaceComponent, FeatureExtractor> featureExtractors)
        {
            var componentsFeatures = featureExtractors
                .AsParallel()
                .Select(extractor => new { extractor.Key, Features = extractor.Value.ExtractDescriptor(face.GetComponent(extractor.Key)) })
                .ToDictionary(kv => kv.Key, kv => kv.Features);
            
            return Face.FromDictionary(componentsFeatures, face.Shape);
        }
    }
}
