using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV.Face;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.UI;
using Vision.Model;
using System.Collections;
using Emgu.CV.CvEnum;

namespace Vision.Detector
{
    public class FaceLandmarkDetectorLBF : FaceComponentsDetector
    {
        private static Range EYES_POINT_RANGE = new Range(36, 47);
        private static Range EYEBROWS_POINT_RANGE = new Range(17, 26);
        private static Range NOSE_POINT_RANGE = new Range(28, 35);
        private static Range MOUTH_POINT_RANGE = new Range(48, 67);

        private FaceDetector faceDetector;
        private FacemarkLBF facemark;

        public FaceLandmarkDetectorLBF(FaceDetector faceDetector, string landmarkModel)
        {
            this.faceDetector = faceDetector;
            this.facemark = new FacemarkLBF(new FacemarkLBFParams());
            this.facemark.LoadModel(landmarkModel);
        }

        public FaceComponentContainer<Rectangle, PointF[]> Fit(IImage image)
        {
            var faces = new VectorOfRect(faceDetector.DetectBoxFaces(image));
            var landmarks = new VectorOfVectorOfPointF();
            
            if (facemark.Fit(image, faces, landmarks))
            {
                return ExtractFacialComponentRects(image, landmarks.ToArrayOfArray().First());
            }
            return null;
        }

        private FaceComponentContainer<Rectangle, PointF[]> ExtractFacialComponentRects(IImage image, PointF[] landamarksPoints)
        {
            var eyebrows = ExtractRectFromLandmarks(landamarksPoints, EYEBROWS_POINT_RANGE);
            var eyes = ExtractRectFromLandmarks(landamarksPoints, EYES_POINT_RANGE);
            var nose = ExtractRectFromLandmarks(landamarksPoints, NOSE_POINT_RANGE);
            var mouth = ExtractRectFromLandmarks(landamarksPoints, MOUTH_POINT_RANGE);
            var hair = ExtractHairRectangle(image, eyebrows);
            return FaceComponentContainer.Create(hair, eyebrows, eyes, nose, mouth, landamarksPoints);
        }
        
        private Rectangle ExtractRectFromLandmarks(PointF[] landamarksPoints, Range range)
        {
            var points = landamarksPoints.Skip(range.Start)
                .Take(range.End - range.Start + 1)
                .ToArray();
            var rect = PointCollection.BoundingRectangle(points);
            return rect;
        }

        private Rectangle ExtractHairRectangle(IImage image, Rectangle eyebrowsRect)
        {
            var hairHeight = image.Size.Height - (image.Size.Height - eyebrowsRect.Top);
            return new Rectangle(0, 0, image.Size.Width, hairHeight);
        }
    }
}
