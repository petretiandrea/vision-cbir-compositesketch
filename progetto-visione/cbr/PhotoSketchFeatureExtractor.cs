using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Vision;
using Vision.Model;
using Vision.Model.Extractor;
using Vision.Normalization;

namespace Vision.Model
{

    public struct ComponentFeature
    {
        public double[] Features { get; private set; }
        public int Blocks { get; set; }

        public ComponentFeature(double[] features, int blocks) : this()
        {
            Features = features ?? throw new ArgumentNullException(nameof(features));
            Blocks = blocks;
        }
    }

    public struct FaceFeatures
    {
        public ComponentFeature Hair { get; private set; }
        public ComponentFeature Eyebrows { get; private set; }
        public ComponentFeature Eyes { get; private set; }
        public ComponentFeature Nose { get; private set; }
        public ComponentFeature Mouth { get; private set; }
        public double[] Shape { get; private set; }

        public FaceFeatures(ComponentFeature hair, ComponentFeature eyebrows, ComponentFeature eyes, ComponentFeature nose, ComponentFeature mouth, double[] shape) : this()
        {
            Hair = hair;
            Eyebrows = eyebrows;
            Eyes = eyes;
            Nose = nose;
            Mouth = mouth;
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
        }
    }

    public static class PhotoSketchFeatureExtractorFactory
    {
        public static PhotoSketchFeatureExtractor Default(FaceComponentsExtractor componentExtractor, Dictionary<FaceComponent, BlockExtraction> blockParams)
        {
            var lbp = CircularMultiscaleLBP.CreateUniform(1, 3, 5, 7);
            var extractors = new Dictionary<FaceComponent, MLBPFeatureExtractor>()
            {
                { FaceComponent.HAIR, new MLBPFeatureExtractor(lbp, blockParams[FaceComponent.HAIR]) },
                { FaceComponent.EYEBROWS, new MLBPFeatureExtractor(lbp, blockParams[FaceComponent.EYEBROWS]) },
                { FaceComponent.EYES, new MLBPFeatureExtractor(lbp, blockParams[FaceComponent.EYES]) },
                { FaceComponent.NOSE, new MLBPFeatureExtractor(lbp, blockParams[FaceComponent.NOSE]) },
                { FaceComponent.MOUTH, new MLBPFeatureExtractor(lbp, blockParams[FaceComponent.MOUTH]) }
            };
            return new PhotoSketchFeatureExtractor(componentExtractor, extractors);
        }
    }

    public class PhotoSketchFeatureExtractor
    {
        private FaceComponentsExtractor componentExtractor;
        private Dictionary<FaceComponent, MLBPFeatureExtractor> featureExtractors;

        public PhotoSketchFeatureExtractor(FaceComponentsExtractor componentExtractor, Dictionary<FaceComponent, MLBPFeatureExtractor> featureExtractors)
        {
            this.componentExtractor = componentExtractor;
            this.featureExtractors = featureExtractors;
        }

        public FaceFeatures ExtractFaceFeatures(string photoPath)
        {
            var normPhoto = Preprocessing.PreprocessImage(photoPath);
            var faceComponents = componentExtractor.ExtractComponentsFromImage(normPhoto);
            return ExtractFaceFeatures(faceComponents, featureExtractors);
        }

        public FaceFeatures[] ExtractFacesFeatures(params string[] photos)
        {
            return photos.Select(photo => ExtractFaceFeatures(photo)).ToArray();
        }

        private FaceFeatures ExtractFaceFeatures(FaceComponentContainer faceComponents, 
            Dictionary<FaceComponent, MLBPFeatureExtractor> featureExtractors)
        {
            var features = new ComponentFeature[featureExtractors.Count];
            
            Task.WaitAll(
                Task.Run(() => features[0] = ExtractComponentFeature(faceComponents.Hair, featureExtractors[FaceComponent.HAIR])),
                Task.Run(() => features[1] = ExtractComponentFeature(faceComponents.Eyebrows, featureExtractors[FaceComponent.EYEBROWS])),
                Task.Run(() => features[2] = ExtractComponentFeature(faceComponents.Eyes, featureExtractors[FaceComponent.EYES])),
                Task.Run(() => features[3] = ExtractComponentFeature(faceComponents.Nose, featureExtractors[FaceComponent.NOSE])),
                Task.Run(() => features[4] = ExtractComponentFeature(faceComponents.Mouth, featureExtractors[FaceComponent.MOUTH]))
                );
            

            return new FaceFeatures(features[0], features[1], features[2], features[3], features[4], faceComponents.Shape);
        }

        private ComponentFeature ExtractComponentFeature(Image<Gray, byte> component, MLBPFeatureExtractor featureExtractor)
        {
            var features = featureExtractor.ExtractDescriptor(component);
            return new ComponentFeature(features, ImageUtils.CountBlocks(component, featureExtractor.Block.Size, featureExtractor.Block.Stride));
        }
    }
}
