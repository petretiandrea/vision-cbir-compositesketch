using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Model.Extractor
{
    public abstract class AbstractLBPFeatureExtractor : AbstractFeatureExtrator
    {
        public int NumberOfCell { get; private set; }

        public AbstractLBPFeatureExtractor(int numberOfCell)
        {
            this.NumberOfCell = numberOfCell;
        }

        protected Rectangle[] ComputePatches<TColor, TDepth>(Image<TColor, TDepth> img, int k)
            where TColor : struct, IColor
            where TDepth : new()
        {
            // TODO: compute overlapped patch
            var patches = new Rectangle[k * k];
            var patchSize = new Size(img.Width / k, img.Height / k);
            var overlap = new Size(img.Width % k, img.Height % k);
            for (int i = 0; i < patches.Length; i++)
            {
                var row = i / k;
                var col = i % k;
                var r = new Rectangle(
                    col * patchSize.Width,
                    row * patchSize.Height,
                    patchSize.Width + overlap.Width,
                    patchSize.Height + overlap.Height);
                patches[i] = r;
            }
            return patches;
        }
    }

    public class LBPFeatureExtractor : AbstractLBPFeatureExtractor
    {
        private LBP lbp;
        public LBPFeatureExtractor(LBP lbp, int numberOfCell) : base(numberOfCell) { }

        public override float[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
        {
            var lbpImage = lbp.Apply(image);
            var patches = ComputePatches(lbpImage, NumberOfCell);
            return patches
                //.AsParallel()
                .Select(patch => lbpImage.GetSubRect(patch))
                .Select(imgPatch => LBPUtils.CalculateHistogramFromLBP(imgPatch.Convert<Gray, byte>()))
                .Aggregate(new List<float>(), (acc, hist) => { acc.AddRange(hist); return acc; })
                .ToArray();
        }
    }

    public class MLBPFeatureExtractor : AbstractLBPFeatureExtractor
    {
        private MultiscaleLBP lbp;

        public MLBPFeatureExtractor(MultiscaleLBP lbp, int numberOfCell) : base(numberOfCell)
        {
            this.lbp = lbp;
        }

        public override float[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
        {
            var multiscaleImages = lbp.Apply(image);

            var patches = ComputePatches(image, NumberOfCell);
            return patches
                //.AsParallel()
                .Select(patch => multiscaleImages.Select(lbpImage => lbpImage.GetSubRect(patch)).ToArray())
                .Select(multiscalePatches => LBPUtils.CalculateHistogramFromMLBP(multiscalePatches))
                .Aggregate(new List<float>(), (acc, hist) => { acc.AddRange(hist); return acc; })
                .ToArray();
        }
    }
}
