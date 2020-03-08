using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.model
{
    public interface FeatureVector
    {
        float[] Features { get; }
        float Compare(FeatureVector toCompare, FeatureComparator comparator);
    }

    public interface FeatureComparator
    {
        float Compare(float[] features1, float[] features2);
    }

    public abstract class AbstractFeatureVector : FeatureVector
    {
        public float[] Features { get; private set; }

        public AbstractFeatureVector(float[] features)
        {
            Features = features;
        }

        public abstract float Compare(FeatureVector toCompare, FeatureComparator comparator);
    }

    public class PatchFeatureVector : AbstractFeatureVector
    {
        public int NumberOfPatch { get; private set; }
        public PatchFeatureVector(int numberOfPatch, float[] features) : base(features)
        {
            NumberOfPatch = numberOfPatch;
        }

        public override float Compare(FeatureVector toCompare, FeatureComparator comparator)
        {
            var patchFeatureVector = toCompare as PatchFeatureVector;
            if (NumberOfPatch == patchFeatureVector.NumberOfPatch)
            {
                //Features.Length / NumberOfPatch;
            }
            return 0;
        }
    }
}
