using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Vision.CBR;
using Vision.Detector;
using Vision.Model;
using Vision.Model.Extractor;
using Vision.Preprocess;

using FaceFeatureContainer = Vision.Model.FaceComponentContainer<float[], float[]>;

namespace Vision.Model
{
    public struct FaceComponentParams
    {
        public int Size { get; set; }
        public int Stride { get; set; }
        public BoundingBoxParam BoundingBox { get; set; }
        public int TotalPatch {
            get {
                var m = (BoundingBox.TargetWidth - Size) / Stride + 1;
                var n = ((int)(BoundingBox.TargetWidth * BoundingBox.AspectRatio) - Size) / Stride + 1;
                return n * m;
            }
        }
    }

    public struct PhotoSketchAlgorithmOptions
    {
        public FaceComponentParams Eyes;
        public FaceComponentParams EyeBrows;
        public FaceComponentParams Mouth;
        public FaceComponentParams Nose;
        public FaceComponentParams Hair;
        public float[] Scales;

        public static PhotoSketchAlgorithmOptions Default => new PhotoSketchAlgorithmOptions()
        {
            /*Eyes = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.3, TargetWidth = 300 } },
            EyeBrows = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.15, TargetWidth = 320 } },
            Mouth = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.6, TargetWidth = 200 } },
            Nose = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 1, TargetWidth = 120 } },
            Hair = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.5, TargetWidth = 150 } },
            Scales = new int[] { 1, 3, 5, 7 }*/
            Eyes = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.2, TargetWidth = 160 } },
            EyeBrows = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.2, TargetWidth = 160 } },
            Mouth = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.6, TargetWidth = 160 } },
            Nose = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 1, TargetWidth = 96 } },
            Hair = new FaceComponentParams { Size = 32, Stride = 16, BoundingBox = new BoundingBoxParam { AspectRatio = 0.5, TargetWidth = 160 } },
            Scales = new float[] { 1, 3, 5, 7 }
        };
    }

    public class PhotoSketchAlgorithm
    {
        public static PhotoSketchAlgorithm Default => new PhotoSketchAlgorithm(PhotoSketchAlgorithmOptions.Default);

        private static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        private static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";

        private FaceComponentsExtractor faceComponentsExtractor;

        // FEATURE EXTRACTION
        private MultiscaleLBP lbp;
        private Dictionary<FaceComponent, MLBPFeatureExtractor> featureExtractors;

        public PhotoSketchAlgorithmOptions Options { get; private set; }

        public PhotoSketchAlgorithm(PhotoSketchAlgorithmOptions options)
        {
            Options = options;
            lbp = CircularMultiscaleLBP.Create(options.Scales);
            faceComponentsExtractor = new PhotoSketchFaceComponentsExtractor(
                new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL),
                options.Hair.BoundingBox,
                options.EyeBrows.BoundingBox,
                options.Eyes.BoundingBox,
                options.Nose.BoundingBox,
                options.Mouth.BoundingBox);

            featureExtractors = new Dictionary<FaceComponent, MLBPFeatureExtractor>()
            {
                { FaceComponent.EYES, new MLBPFeatureExtractor(lbp, options.Eyes.Size, options.Eyes.Stride) },
                { FaceComponent.EYEBROWS, new MLBPFeatureExtractor(lbp, options.EyeBrows.Size, options.EyeBrows.Stride) },
                { FaceComponent.MOUTH, new MLBPFeatureExtractor(lbp, options.Mouth.Size, options.Mouth.Stride) },
                { FaceComponent.NOSE, new MLBPFeatureExtractor(lbp, options.Nose.Size, options.Nose.Stride) },
                { FaceComponent.HAIR, new MLBPFeatureExtractor(lbp, options.Hair.Size, options.Hair.Stride) }
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
                Task.Run(() => features[0] = featureExtractors[FaceComponent.HAIR].ExtractDescriptor(face.Hair)),
                Task.Run(() => features[1] = featureExtractors[FaceComponent.EYEBROWS].ExtractDescriptor(face.Eyebrows)),
                Task.Run(() => features[2] = featureExtractors[FaceComponent.EYES].ExtractDescriptor(face.Eyes)),
                Task.Run(() => features[3] = featureExtractors[FaceComponent.NOSE].ExtractDescriptor(face.Nose)),
                Task.Run(() => features[4] = featureExtractors[FaceComponent.MOUTH].ExtractDescriptor(face.Mouth))
                );

            return FaceComponentContainer.Create(features[0], features[1], features[2], features[3], features[4], face.Shape);
        }

        public void ComputeGalleryAverageImage(List<string> photos)
        {
            var norms = new List<Image<Gray, byte>>();
            foreach(var photo in photos)
            {
                try
                {
                    norms.Add(Preprocessing.PreprocessImage<byte>(photo));
                }
                catch (Exception e) { }
            }
            var photosComponents = norms.Select(photo => faceComponentsExtractor.ExtractComponentsFromImage(photo)).ToArray();

            var hair = photosComponents.Select(c => c.Hair).ToArray();
            var eyes = photosComponents.Select(c => c.Eyes).ToArray();
            var eyebrows = photosComponents.Select(c => c.Eyebrows).ToArray();
            var nose = photosComponents.Select(c => c.Nose).ToArray();
            var mouth = photosComponents.Select(c => c.Mouth).ToArray();
            ImageViewer.Show(Avg(160, 80, hair));
            ImageViewer.Show(Avg(160, 32, eyebrows));
            ImageViewer.Show(Avg(160, 48, eyes));
            ImageViewer.Show(Avg(96, 96, nose));
            ImageViewer.Show(Avg(160, 96, mouth));
        }

        private Image<Gray, byte> Avg(int width, int height, IEnumerable<Image<Gray, byte>> images)
        {
            var avg = new Image<Gray, double>(width, height);
            var count = 0;
            foreach (var img  in images)
            {
                avg += img.Convert<Gray, double>();
                count++;
            }
            avg /= count;
            return avg.Convert<Gray, byte>();
        }
    }
}
