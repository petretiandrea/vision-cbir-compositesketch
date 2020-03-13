using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Vision.CBR;
using Vision.Detector;
using Vision.Model;
using Vision.Model.Extractor;
using Vision.Preprocess;

using FaceFeatureContainer = Vision.Model.FaceComponentContainer<float[], float[]>;

namespace Vision.Model
{
    public struct PhotoSketchAlgorithmOptions
    {
        public int EyesPatches;
        public int EyeBrowsPatches;
        public int MouthPatches;
        public int NosePatches;
        public int HairPatches;
        public int[] Scales;

        public static PhotoSketchAlgorithmOptions Default => new PhotoSketchAlgorithmOptions()
        {
            EyesPatches = 5,
            EyeBrowsPatches = 5,
            MouthPatches = 5,
            NosePatches = 5,
            HairPatches = 5,
            Scales = new int[] { 1, 3, 5, 7 }
        };
    }

    public class PhotoSketchAlgorithm
    {
        public static PhotoSketchAlgorithm Default => new PhotoSketchAlgorithm(PhotoSketchAlgorithmOptions.Default);

        private static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        private static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";

        private FaceComponentsExtractor faceComponentsExtractor = new DefaultFaceComponentsExtractor(
                new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL)
            );

        // FEATURE EXTRACTION
        private MultiscaleLBP lbp;
        private Dictionary<FaceComponent, MLBPFeatureExtractor> featureExtractors;

        public PhotoSketchAlgorithmOptions Options { get; private set; }

        public PhotoSketchAlgorithm(PhotoSketchAlgorithmOptions options)
        {
            Options = options;
            lbp = CircularMultiscaleLBP.Create(options.Scales);
            featureExtractors = new Dictionary<FaceComponent, MLBPFeatureExtractor>()
            {
                { FaceComponent.EYES, new MLBPFeatureExtractor(lbp, options.EyesPatches) },
                { FaceComponent.EYEBROWS, new MLBPFeatureExtractor(lbp, options.EyeBrowsPatches) },
                { FaceComponent.MOUTH, new MLBPFeatureExtractor(lbp, options.MouthPatches) },
                { FaceComponent.NOSE, new MLBPFeatureExtractor(lbp, options.NosePatches) },
                { FaceComponent.HAIR, new MLBPFeatureExtractor(lbp, options.HairPatches) }
            };
        }

        public FaceFeatureContainer ExtractFaceFeatures(string photoPath)
        {
            var normPhoto = Preprocessing.PreprocessImage<byte>(photoPath);
            var faceComponents = faceComponentsExtractor.ExtractComponentsFromImage(normPhoto);
            return ExtractFaceFeatures(faceComponents, featureExtractors);
        }

        public FaceFeatureContainer[] ExtractFacesFeatures(params string[] photos)
        {
            return photos.Select(photo => ExtractFaceFeatures(photo)).ToArray();
        }

        private FaceFeatureContainer ExtractFaceFeatures(FaceComponentContainer<Image<Gray, byte>, float[]> face, 
            Dictionary<FaceComponent, MLBPFeatureExtractor> featureExtractors)
        {
            var features = new float[featureExtractors.Count][];
            Task.WaitAll(
                Task.Run(() => features[0] = featureExtractors[FaceComponent.EYES].ExtractDescriptor(face.Eyes)),
                Task.Run(() => features[1] = featureExtractors[FaceComponent.EYEBROWS].ExtractDescriptor(face.Eyebrows)),
                Task.Run(() => features[2] = featureExtractors[FaceComponent.MOUTH].ExtractDescriptor(face.Mouth)),
                Task.Run(() => features[3] = featureExtractors[FaceComponent.NOSE].ExtractDescriptor(face.Nose)),
                Task.Run(() => features[4] = featureExtractors[FaceComponent.HAIR].ExtractDescriptor(face.Hair)));

            return FaceComponentContainer.Create(features[0], features[1], features[2], features[3], features[4], face.Shape);
        }
    }
}
