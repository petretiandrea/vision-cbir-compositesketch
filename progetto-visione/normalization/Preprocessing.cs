using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Detector;

namespace Vision.Normalization
{
    public static class Preprocessing
    {
        static string EYES_MODEL = @"resources\haarcascade_eye_tree_eyeglasses.xml";

        static int PREPROCESS_TARGET_WIDTH = 200;
        static int PREPROCESS_TARGET_HEIGHT = 250;

        private static FaceNormalizer faceNormalizer = new FaceNormalizer(new HaarEyesDetector(EYES_MODEL));

        public static Image<Gray, byte> PreprocessImage(Image<Bgr, byte> image)
        {
            var faceNorm = faceNormalizer.Normalize(image, PREPROCESS_TARGET_WIDTH, PREPROCESS_TARGET_HEIGHT);
            return faceNorm;
        }

        public static Image<Gray, byte> PreprocessImage(string pathImage)
        {
            return PreprocessImage(new Image<Bgr, byte>(pathImage));
        }
    }
}
