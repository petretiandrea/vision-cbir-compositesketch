using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using System.Drawing;
using Vision.Model;
using Vision.Detector;

namespace Vision.Normalization
{
    /// <summary>
    /// Pre-processing purpose. Detect a face into photo and scale, cut and rotate it.
    /// The rotation is based on horizon line of the eyes.
    /// </summary>
    public class FaceNormalizer
    {
        private EyesDetector eyesDector;

        /// percentage that control how much of the face is visible
        private const double DESIDERED_LEFT_EYE_X = 0.35;
        private const double DESIDERED_LEFT_EYE_Y = 0.5;

        public FaceNormalizer(EyesDetector eyesDector)
        {
            this.eyesDector = eyesDector;
        }

        public Image<Gray, byte> Normalize(Image<Bgr, byte> originalImage, int targetWidth=512, int targetHeight = 512)
        {
            var eyes = eyesDector.DetectEyes(originalImage);
            //DrawEyesRect(originalImage, eyes);
            // detect angle respect to horizontal line of eyes
            var dy = eyes.Right.GetCenter().Y - eyes.Left.GetCenter().Y;
            var dx = eyes.Right.GetCenter().X - eyes.Left.GetCenter().X;
            var rotationAngle = RadToDegree(Math.Atan2(dy, dx)); // angle
            var eyesCenterX = (eyes.Left.GetCenter().X + eyes.Right.GetCenter().X) / 2;
            var eyesCenterY = (eyes.Left.GetCenter().Y + eyes.Right.GetCenter().Y) / 2;

            // determine the right target scale
            var dist = Math.Sqrt(dx * dx + dy * dy);
            var targetDist = (1.0 - 2 * DESIDERED_LEFT_EYE_X) * targetWidth;
            var targetScale = targetDist / dist;

            // determine the translation in order to center eyes into cropped image
            var tx = (targetWidth * 0.5) - eyesCenterX;
            var ty = (targetHeight * DESIDERED_LEFT_EYE_Y) - eyesCenterY;

            // create affine matrix
            using (var rotationMatrix = new RotationMatrix2D(new PointF(eyesCenterX, eyesCenterY), rotationAngle, targetScale))
            using (var affineMatrix = new Matrix<double>(rotationMatrix.Rows, rotationMatrix.Cols, rotationMatrix.DataPointer))
            {
                // add the translation component to the rotation matrix
                affineMatrix.SetCellValue(0, 2, affineMatrix.GetCellValue(0, 2) + tx);
                affineMatrix.SetCellValue(1, 2, affineMatrix.GetCellValue(1, 2) + ty);

                var resized = originalImage.WarpAffine(affineMatrix.Mat,
                    targetWidth,
                    targetHeight,
                    Inter.Area,
                    Warp.Default,
                    BorderType.Constant,
                    new Bgr()
                );

                //ImageViewer.Show(resized);
                return resized.Convert<Gray, byte>();
            }
        }

        private void DrawEyesRect<TDepth>(Image<Bgr, TDepth> img, Eyes eyes) where TDepth : new()
        {
            img.Draw(eyes.Left, new Bgr(Color.Blue));
            img.Draw(eyes.Right, new Bgr(Color.Green));
            img.DrawPolyline(new Point[] { eyes.Left.GetCenter(), eyes.Right.GetCenter() }, false, new Bgr(Color.Red));
            /*new System.Threading.Thread(() =>
            {
                ImageViewer.Show(img);
            }).Start();*/
        }

        private double RadToDegree(double rad)
        {
            return rad * (180 / Math.PI);
        }
    }
}
