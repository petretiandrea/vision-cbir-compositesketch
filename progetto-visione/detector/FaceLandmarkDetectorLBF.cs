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
        private static Range NOSE_POINT_RANGE = new Range(29, 35);
        private static Range MOUTH_POINT_RANGE = new Range(48, 67);

        private FaceDetector faceDetector;
        private FacemarkLBF facemark;

        public FaceLandmarkDetectorLBF(FaceDetector faceDetector, string landmarkModel)
        {
            this.faceDetector = faceDetector;
            this.facemark = new FacemarkLBF(new FacemarkLBFParams());
            this.facemark.LoadModel(landmarkModel);
        }

        private readonly object syncLock = new object();
        public PointF[][] Fit(IImage image)
        {
            lock (syncLock)
            {
                var faces = new VectorOfRect(faceDetector.DetectBoxFaces(image));
                var facesLandmarks = new VectorOfVectorOfPointF();
                if (!facemark.Fit(image, faces, facesLandmarks)) throw new ArgumentException("No landamarks point detected for input image");

                var face = faces.ToArray().First();
                var landmarks = facesLandmarks.ToArrayOfArray().First();
                var componentLandmarkPoints = new PointF[6][];

                // extract landarmarks points for each component
                componentLandmarkPoints[0] = GetComponentPoints(landmarks, EYEBROWS_POINT_RANGE);
                componentLandmarkPoints[1] = GetComponentPoints(landmarks, EYES_POINT_RANGE);
                componentLandmarkPoints[2] = GetComponentPoints(landmarks, NOSE_POINT_RANGE);
                componentLandmarkPoints[3] = GetComponentPoints(landmarks, MOUTH_POINT_RANGE);
                componentLandmarkPoints[4] = landmarks;
                // face bounding box
                componentLandmarkPoints[5] = new PointF[] {
                    new PointF(face.Left, face.Top),
                    new PointF(face.Right, face.Bottom)
                };
                return componentLandmarkPoints;
            }
        }

        private PointF[] GetComponentPoints(PointF[] landmarks, Range range)
        {
            return landmarks.Skip(range.Start)
                .Take(range.End - range.Start + 1)
                .ToArray();
        }
    }
}
