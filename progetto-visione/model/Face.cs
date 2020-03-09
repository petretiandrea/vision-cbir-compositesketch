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
    public enum Gender
    {
        UNKOWN, MALE, FEMALE
    };

    public enum FaceComponent
    {
        EYES,
        EYEBROWS,
        NOSE,
        MOUTH,
        HAIR
    }

    public static class Face
    {
        public static Face<T> FromDictionary<T>(Dictionary<FaceComponent, T> faceComponents, float[] shape)
        {
            return new Face<T>(faceComponents, shape);   
        }

        public static Face<T> Empty<T>()
        {
            return new Face<T>(new Dictionary<FaceComponent, T>(), new float[] { });
        }
    }

    public class Face<ComponentType>
    {
        private Dictionary<FaceComponent, ComponentType> components;
        public Face(Dictionary<FaceComponent, ComponentType> faceComponents, float[] shape)
        {
            components = faceComponents;
            Shape = shape;
        }

        public float[] Shape { get; }
        public ComponentType Eyes { get => components[FaceComponent.EYES]; }
        public ComponentType Eyebrows { get => components[FaceComponent.EYEBROWS]; }
        public ComponentType Nose { get => components[FaceComponent.NOSE]; }
        public ComponentType Mouth { get => components[FaceComponent.MOUTH]; }
        public ComponentType Hair { get => components[FaceComponent.HAIR]; }
        public void SetComponent(FaceComponent component, ComponentType value) => components.Add(component, value);
        public ComponentType GetComponent(FaceComponent component) => components[component];
        
    }

    public interface FaceComponentsExtractor
    {
        FaceComponentsDetector Detector { get; }
        Face<Image<TColor, TDepth>> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();
    }
}
