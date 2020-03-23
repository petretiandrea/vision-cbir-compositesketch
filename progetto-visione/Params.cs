using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;
using Vision.Model.Extractor;
using Vision.Normalization;

namespace Vision
{
    // The main parameters are collected here
    public static class Params
    {
        // Paramters for each bounding box
        public static Dictionary<FaceComponent, BoundingBoxParam> GetComponentBoundingBoxParams()
        {
            return new Dictionary<FaceComponent, BoundingBoxParam>()
            {
                { FaceComponent.HAIR, new BoundingBoxParam { AspectRatio = 0.5, CropWidth = 160 } },
                { FaceComponent.EYEBROWS, new BoundingBoxParam { AspectRatio = 0.2, CropWidth = 144,  Padding = 3 } },
                { FaceComponent.EYES, new BoundingBoxParam { AspectRatio = 0.2, CropWidth = 144, Padding = 3 } },
                { FaceComponent.NOSE, new BoundingBoxParam { AspectRatio = 1, CropWidth = 96, Padding = 3 } },
                { FaceComponent.MOUTH, new BoundingBoxParam { AspectRatio = 0.5, CropWidth = 160, Padding = 3 } }
            };
        }

        // Paramters used for split image into n overlapped blocks.
        public static Dictionary<FaceComponent, BlockExtraction> GetComponentBlockParams()
        {
            return new Dictionary<FaceComponent, BlockExtraction>()
            {
                { FaceComponent.HAIR, new BlockExtraction { Size = 32, Stride = 16 } },
                { FaceComponent.EYEBROWS, new BlockExtraction { Size = 16, Stride = 8 } },
                { FaceComponent.EYES, new BlockExtraction { Size = 16, Stride = 8 } },
                { FaceComponent.NOSE, new BlockExtraction { Size = 32, Stride = 16 } },
                { FaceComponent.MOUTH, new BlockExtraction { Size = 32, Stride = 16 } }
            };
        }

        // This the reference shape used for Procruestes analisys. 
        // It will be serialized on file, for better usage.
        public static PointF[][] GetReferenceShape()
        {
            var eyebrows = new float[] { 47.04848f, 116.0241f, 52.90615f, 105.9396f, 64.64134f, 102.0683f, 77.38805f, 103.2246f, 88.72691f, 108.6733f, 111.6032f, 108.9392f, 122.7954f, 103.2422f, 135.5818f, 101.7269f, 147.5632f, 105.1349f, 154.6555f, 115.5083f };
            var eyes = new float[] { 60.12034f, 126.4593f, 66.66933f, 122.3419f, 75.44972f, 122.3553f, 83.38912f, 127.7777f, 75.0155f, 130.0433f, 66.32563f, 130.0507f, 118.2833f, 127.5629f, 125.9639f, 122.552f, 134.8607f, 122.5263f, 142.0542f, 126.5789f, 135.1928f, 129.9111f, 126.155f, 129.8041f };
            var nose = new float[] { 101.5908f, 142.2801f, 101.665f, 153.4808f, 88.68618f, 165.9935f, 94.96569f, 167.5934f, 101.6543f, 168.8412f, 108.1938f, 167.5843f, 114.5462f, 165.8784f };
            var mouth = new float[] { 79.14745f, 191.1652f, 88.13f, 185.9446f, 95.44063f, 182.8241f, 101.4747f, 184.1124f, 106.8169f, 182.441f, 114.3614f, 185.4994f, 123.1896f, 190.6765f, 114.2386f, 191.5269f, 106.901f, 191.2176f, 101.5371f, 191.2309f, 95.45079f, 190.9854f, 88.11995f, 191.5645f, 82.59917f, 190.7474f, 95.35546f, 188.6547f, 101.5993f, 189.1832f, 106.9801f, 188.32f, 119.3962f, 190.1967f, 106.7337f, 183.3982f, 101.5805f, 183.9244f, 95.52496f, 183.3945f };
            var shape = new float[] { 34.74339f, 130.8541f, 35.60175f, 147.7792f, 37.87053f, 165.1158f, 41.18313f, 182.3461f, 46.77122f, 198.618f, 56.22302f, 212.7667f, 69.43761f, 223.0963f, 85.41431f, 229.9267f, 103.2233f, 231.3049f, 120.7917f, 229.3943f, 135.6541f, 221.8094f, 148.3354f, 211.7014f, 157.5546f, 197.8477f, 162.433f, 181.8351f, 165.0439f, 165.4344f, 166.6458f, 148.5753f, 166.2711f, 131.8215f, 47.04848f, 116.0241f, 52.90615f, 105.9396f, 64.64134f, 102.0683f, 77.38805f, 103.2246f, 88.72691f, 108.6733f, 111.6032f, 108.9392f, 122.7954f, 103.2422f, 135.5818f, 101.7269f, 147.5632f, 105.1349f, 154.6555f, 115.5083f, 101.3202f, 121.397f, 101.4772f, 131.8522f, 101.5908f, 142.2801f, 101.665f, 153.4808f, 88.68618f, 165.9935f, 94.96569f, 167.5934f, 101.6543f, 168.8412f, 108.1938f, 167.5843f, 114.5462f, 165.8784f, 60.12034f, 126.4593f, 66.66933f, 122.3419f, 75.44972f, 122.3553f, 83.38912f, 127.7777f, 75.0155f, 130.0433f, 66.32563f, 130.0507f, 118.2833f, 127.5629f, 125.9639f, 122.552f, 134.8607f, 122.5263f, 142.0542f, 126.5789f, 135.1928f, 129.9111f, 126.155f, 129.8041f, 79.14745f, 191.1652f, 88.13f, 185.9446f, 95.44063f, 182.8241f, 101.4747f, 184.1124f, 106.8169f, 182.441f, 114.3614f, 185.4994f, 123.1896f, 190.6765f, 114.2386f, 191.5269f, 106.901f, 191.2176f, 101.5371f, 191.2309f, 95.45079f, 190.9854f, 88.11995f, 191.5645f, 82.59917f, 190.7474f, 95.35546f, 188.6547f, 101.5993f, 189.1832f, 106.9801f, 188.32f, 119.3962f, 190.1967f, 106.7337f, 183.3982f, 101.5805f, 183.9244f, 95.52496f, 183.3945f };

            var points = new PointF[5][];
            points[0] = CreatePoints(eyebrows);
            points[1] = CreatePoints(eyes);
            points[2] = CreatePoints(nose);
            points[3] = CreatePoints(mouth);
            points[4] = CreatePoints(shape);
            return points;
        }

        private static PointF[] CreatePoints(float[] points)
        {
            var pointsF = new PointF[(int)(points.Length / 2f)];
            for (int i = 0, j = 0; i < points.Length; i += 2, j++)
            {
                pointsF[j] = new PointF(points[i], points[i + 1]);
            }
            return pointsF;
        }
    }
}
