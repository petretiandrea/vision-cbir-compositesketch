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
        public int Size { get; private set; }
        public int Stride { get; private set; }

        public AbstractLBPFeatureExtractor(int size, int stride)
        {
            this.Size = size;
            this.Stride = stride;
        }

        protected Rectangle[] ComputePatches<TColor, TDepth>(Image<TColor, TDepth> img, int size, int stride)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var n = (img.Width - size) / stride + 1;
            var m = (img.Height - size) / stride + 1;
            var rectangles = new List<Rectangle>(n * m);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var r = new Rectangle(
                        j * stride,
                        i * stride,
                        size,
                        size);
                    rectangles.Add(r);
                }
            }
            return rectangles.ToArray();
        }

        /*protected Rectangle[] ComputePatches<TColor, TDepth>(Image<TColor, TDepth> img, int k)
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
        }*/
    }

    public class LBPFeatureExtractor : AbstractLBPFeatureExtractor
    {
        private LBP lbp;
        public LBPFeatureExtractor(LBP lbp, int size, int stride) : base(size, stride) {
            this.lbp = lbp;
        }

        public override float[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
        {
            /* var lbpImage = lbp.Apply(image);
             var patches = ComputePatches(lbpImage, NumberOfCell);
             return patches
                 .Select(patch => lbpImage.GetSubRect(patch))
                 .Select(imgPatch => LBPUtils.CalculateHistogramFromLBP(imgPatch.Convert<Gray, byte>()))
                 .SelectMany(hist => hist)
                 .ToArray();*/
            return null;
        }
    }

    public class MLBPFeatureExtractor : AbstractLBPFeatureExtractor
    {
        private MultiscaleLBP lbp;

        public MLBPFeatureExtractor(MultiscaleLBP lbp, int size, int stride) : base(size, stride)
        {
            this.lbp = lbp;
        }
        
        public override float[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
        {
            var multiscaleImages = lbp.Apply(image);

            var patches = ComputePatches(image, Size, Stride);
            return patches
                .Select(patch => multiscaleImages.Select(lbpImage => lbpImage.GetSubRect(patch)).ToArray())
                .Select(multiscalePatches => LBPUtils.CalculateHistogramFromMLBP(multiscalePatches))
                .SelectMany(hist => hist)
                .ToArray();
        }
    }
}
