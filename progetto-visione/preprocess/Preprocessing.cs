using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Detector;

namespace Vision.Preprocess
{
    public static class Preprocessing
    {
        static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        static string EYES_MODEL = @"resources\haarcascade_eye_tree_eyeglasses.xml";
        static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";

        static int PREPROCESS_TARGET_WIDTH = 300;
        static int PREPROCESS_TARGET_HEIGHT = 350;

        private static PreprocessFaceNormalizer faceNormalizer = new PreprocessFaceNormalizer(new HaarEyesDetector(EYES_MODEL));

        public static Image<Gray, TDepth> PreprocessImage<TDepth>(Image<Bgr, TDepth> image) where TDepth : new()
        {
            return faceNormalizer.Normalize(image, PREPROCESS_TARGET_WIDTH, PREPROCESS_TARGET_HEIGHT);
        }

    }
}
