using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Vision.Detector;
using Emgu.CV.Structure;

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

    public class FaceComponentContainer
    {
        public Image<Gray, byte> Hair { get; private set; }
        public Image<Gray, byte> Eyes { get; private set; }
        public Image<Gray, byte> Eyebrows { get; private set; }
        public Image<Gray, byte> Nose { get; private set; }
        public Image<Gray, byte> Mouth { get; private set; }
        public double[] Shape { get; private set; }

        public FaceComponentContainer(Image<Gray, byte> hair, Image<Gray, byte> eyebrows, Image<Gray, byte> eyes,
            Image<Gray, byte> nose, Image<Gray, byte> mouth, double[] shape)
        {
            Hair = hair;
            Eyebrows = eyebrows;
            Eyes = eyes;
            Nose = nose;
            Mouth = mouth;
            Shape = shape;
        }
    }

    public interface FaceComponentsExtractor
    {
        FaceComponentContainer ExtractComponentsFromImage(Image<Gray, byte> image);
    }
}
