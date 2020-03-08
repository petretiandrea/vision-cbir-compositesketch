using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace Vision.Model
{
    using Feature = System.Single;

    public interface FeatureExtractor
    {
        Feature[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();

        List<Feature[]> ExtractDescriptors<TColor, TDepth>(Image<TColor, TDepth>[] images)
            where TColor : struct, IColor
            where TDepth : new();
    }

    public abstract class AbstractFeatureExtrator : FeatureExtractor
    {
        public abstract float[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();

        public List<float[]> ExtractDescriptors<TColor, TDepth>(Image<TColor, TDepth>[] images)
            where TColor : struct, IColor
            where TDepth : new()
        {
            return images.Select(img => ExtractDescriptor(img)).ToList();
        }
    }
}
