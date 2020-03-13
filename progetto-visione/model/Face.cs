using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Vision.Detector;

namespace Vision.Model
{
    public enum FaceComponent
    {
        EYES,
        EYEBROWS,
        NOSE,
        MOUTH,
        HAIR
    }

    public static class FaceComponentContainer
    {
        public static FaceComponentContainer<T, S> Create<T, S>(T hair, T eyebrows, T eyes, T nose, T mouth, S shape)
        {
            return new FaceComponentContainer<T, S>(new Dictionary<FaceComponent, T>()
            {
                { FaceComponent.HAIR, hair },
                { FaceComponent.EYEBROWS, eyebrows },
                { FaceComponent.EYES, eyes },
                { FaceComponent.NOSE, nose },
                { FaceComponent.MOUTH, mouth }
            }, shape);
        }
        public static FaceComponentContainer<T, S> FromDictionary<T,S>(Dictionary<FaceComponent, T> faceComponents, S shape)
        {
            return new FaceComponentContainer<T, S>(faceComponents, shape);   
        }
        public static FaceComponentContainer<T, S> Empty<T, S>()
        {
            return new FaceComponentContainer<T, S>(new Dictionary<FaceComponent, T>(), default(S));
        }
    }

    public class FaceComponentContainer<ComponentType, ShapeType>
    {
        private Dictionary<FaceComponent, ComponentType> components;
        public FaceComponentContainer(Dictionary<FaceComponent, ComponentType> faceComponents, ShapeType shape)
        {
            components = faceComponents;
            Shape = shape;
        }

        public ComponentType Hair { get => components[FaceComponent.HAIR]; }
        public ComponentType Eyebrows { get => components[FaceComponent.EYEBROWS]; }
        public ComponentType Eyes { get => components[FaceComponent.EYES]; }
        public ComponentType Nose { get => components[FaceComponent.NOSE]; }
        public ComponentType Mouth { get => components[FaceComponent.MOUTH]; }
        public ShapeType Shape { get; }

        public void SetComponent(FaceComponent component, ComponentType value) => components.Add(component, value);
        public ComponentType GetComponent(FaceComponent component) => components[component];
        
    }

    public interface FaceComponentsExtractor
    {
        FaceComponentsDetector Detector { get; }
        FaceComponentContainer<Image<TColor, TDepth>, float[]> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();
    }
}
