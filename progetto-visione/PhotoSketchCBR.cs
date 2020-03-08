using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Detector;
using Vision.Model;
using Vision.Model.Extractor;
using Vision.Preprocess;
using Vision.Utils;

namespace Vision.CBR
{
    public class PhotoSketchCBR : CBR
    {
        static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";
        
        private FaceComponentsExtractor faceComponentsExtractor = new DefaultFaceComponentsExtractor(
                new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL)
            );
        
        private List<Tuple<string, float[]>> featuresEyes;
        private List<Tuple<string, float[]>> featuresEyebrows;
        private List<Tuple<string, float[]>> featuresNose;
        private List<Tuple<string, float[]>> featuresMouth;
        private List<Tuple<string, float[]>> featuresHair;
        
        private MultiscaleLBP lbp = CircularMultiscaleLBP.Create(1);
        private Dictionary<FaceComponent, FeatureExtractor> featureExtractors;

        private const int PATCH_EYES = 5;
        private const int PATCH_EYEBROWS = 5;
        private const int PATCH_NOSE = 5;
        private const int PATCH_MOUTH = 5;
        private const int PATCH_HAIR = 5;

        public FusionStrategy<string> SearchFusionStrategy { get; set; }

        public PhotoSketchCBR()
        {
            this.SearchFusionStrategy = new BordaCount<string>(1, 1, 1, 1, 1);
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
            var features = photoDb
                //.AsParallel()
                //.AsOrdered()
                .Select(photoPath =>
                {
                    var normPhoto = Preprocessing.PreprocessImage<byte>(photoPath);
                    var componentsImage = faceComponentsExtractor.ExtractComponentsFromImage(normPhoto);
                    return Tuple.Create(photoPath, ExtractFaceFeatures(componentsImage, featureExtractors));
                })
                .ToList();
            
            featuresEyes = features.Select(item => Tuple.Create(item.Item1, item.Item2.Eyes)).ToList();
            featuresEyebrows = features.Select(item => Tuple.Create(item.Item1, item.Item2.Eyebrows)).ToList();
            featuresHair = features.Select(item => Tuple.Create(item.Item1, item.Item2.Hair)).ToList();
            featuresNose = features.Select(item => Tuple.Create(item.Item1, item.Item2.Nose)).ToList();
            featuresMouth = features.Select(item => Tuple.Create(item.Item1, item.Item2.Mouth)).ToList();
        }

        public Rank<string, double> Search(string queryImagePath, int maxImageToRetrive = 10)
        {
            if (SearchFusionStrategy == null) throw new InvalidOperationException("No search fusion strategy is set.");

            var normPhoto = Preprocessing.PreprocessImage<byte>(queryImagePath);
            var componentsImages = faceComponentsExtractor.ExtractComponentsFromImage(normPhoto);
            var sketchFeature = ExtractFaceFeatures(componentsImages, featureExtractors);

            var metric = HistogramUtils.Intersection;

            var rankEye = Rank.FromMetric(featuresEyes, sketchFeature.Eyes, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_EYES, metric));
            var rankBrows = Rank.FromMetric(featuresEyebrows, sketchFeature.Eyebrows, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_EYEBROWS, metric));
            var rankNose = Rank.FromMetric(featuresNose, sketchFeature.Nose, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_NOSE, metric));
            var rankMouth = Rank.FromMetric(featuresMouth, sketchFeature.Mouth, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_MOUTH, metric));
            var rankHair = Rank.FromMetric(featuresHair, sketchFeature.Hair, (f1, f2) => LBPUtils.CalculareSimilarity(f1, f2, PATCH_HAIR, metric));

            return SearchFusionStrategy.Fusion(rankEye, rankMouth, rankNose, rankBrows, rankHair).Take(maxImageToRetrive).ToRank();
        }

        private FaceFeatures ExtractFaceFeatures(Dictionary<FaceComponent, Image<Gray, byte>> components, Dictionary<FaceComponent, FeatureExtractor> featureExtractors)
        {
            var faceFeature = new FaceFeatures();
            var componentsFeatures = components
                .AsParallel()
                .Select(component => new { component.Key, Features = featureExtractors[component.Key].ExtractDescriptor(component.Value) })
                .AsSequential();

            foreach (var componentFeatures in componentsFeatures)
            {
                faceFeature.SetFeaturesOf(componentFeatures.Key, componentFeatures.Features);
            }
            return faceFeature;
        }
    }

    public class FaceFeatures
    {
       private Dictionary<FaceComponent, float[]> componentsFeatures = new Dictionary<FaceComponent, float[]>();

       public float[] Eyes { get => componentsFeatures[FaceComponent.EYES]; }
       public float[] Eyebrows { get => componentsFeatures[FaceComponent.EYEBROWS]; }
       public float[] Hair { get => componentsFeatures[FaceComponent.HAIR]; }
       public float[] Nose { get => componentsFeatures[FaceComponent.NOSE]; }
       public float[] Mouth { get => componentsFeatures[FaceComponent.MOUTH]; }

       public void SetFeaturesOf(FaceComponent component, float[] features) => componentsFeatures.Add(component, features);
       public float[] GetFeaturesOf(FaceComponent component) => componentsFeatures[component];
   }
}
