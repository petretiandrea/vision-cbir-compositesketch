using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace Vision.Model
{
    using Feature = System.Single;

    public interface FeatureExtractor<TColor, TDepth> where TColor : struct, IColor where TDepth : new()
    {
        Feature[] ExtractDescriptor(Image<TColor, TDepth> image);
        List<Feature[]> ExtractDescriptors(Image<TColor, TDepth>[] images);
    }

    public abstract class AbstractFeatureExtrator<TColor, TDepth> : FeatureExtractor<TColor, TDepth> where TColor : struct, IColor where TDepth : new()
    {
        public List<Feature[]> ExtractDescriptors(Image<TColor, TDepth>[] images)
        {
            return images.Select(img => ExtractDescriptor(img)).ToList();
        }

        public abstract Feature[] ExtractDescriptor(Image<TColor, TDepth> image);
    }
}
