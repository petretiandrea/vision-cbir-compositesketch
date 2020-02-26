using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace Vision.Model
{
    using Feature = System.Single;

    interface FeatureExtractor<TColor, TDepth> where TColor : struct, IColor where TDepth : new()
    {
        Feature[] ExtractDescriptor(Image<TColor, TDepth> image);
    }

    static class Extensions
    {
        public static List<Feature[]> ExtractDescriptors<TColor, TDepth>(this FeatureExtractor<TColor, TDepth> e, Image<TColor, TDepth>[] images) where TColor : struct, IColor where TDepth : new()
        {
            return images.Select(img => e.ExtractDescriptor(img)).ToList();
        }
    }
}
