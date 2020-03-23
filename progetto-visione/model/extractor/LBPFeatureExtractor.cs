using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Model.Extractor
{
    public struct BlockExtraction
    {
        public int Size { get; set; }
        public int Stride { get; set; }
    }

    public class MLBPFeatureExtractor : AbstractFeatureExtrator
    {
        private MultiscaleLBP lbp;
        public BlockExtraction Block { get; private set; }

        public MLBPFeatureExtractor(MultiscaleLBP lbp, BlockExtraction block)
        {
            this.lbp = lbp;
            this.Block = block;
        }
        
        public override double[] ExtractDescriptor<TColor, TDepth>(Image<TColor, TDepth> image)
        {
            var multiscaleImages = lbp.Apply(image);

            var patches = ImageUtils.SplitIntoBlocks(image, Block.Size, Block.Stride);
            return patches
                .Select(patch => multiscaleImages.Select(lbpImage => lbpImage.GetSubRect(patch)).ToArray())
                .Select(multiscalePatches => lbp.HistogramFromMLBP(multiscalePatches))
                .SelectMany(hist => hist)
                .ToArray();
        }
    }
}
