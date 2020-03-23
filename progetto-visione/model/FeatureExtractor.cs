using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace Vision.Model
{
    using Feature = System.Double;

    public interface FeatureExtractor
    {
        double[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();

        List<double[]> ExtractDescriptors<TColor, TDepth>(Image<TColor, TDepth>[] images)
            where TColor : struct, IColor
            where TDepth : new();
    }

    public abstract class AbstractFeatureExtrator : FeatureExtractor
    {
        public abstract double[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();

        public List<double[]> ExtractDescriptors<TColor, TDepth>(Image<TColor, TDepth>[] images)
            where TColor : struct, IColor
            where TDepth : new()
        {
            return images.Select(img => ExtractDescriptor(img)).ToList();
        }
    }
}
