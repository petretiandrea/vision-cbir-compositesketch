using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Vision.UI
{
    public partial class ImageGallery : UserControl
    {
        public ImageGallery()
        {
            InitializeComponent();
        }

        public void AddImage(string path)
        {
            AddImage(new Image<Bgr, float>(path));
        }

        public void AddImages<TColor, TDepth>(Image<TColor, TDepth>[] images) where TColor : struct, IColor where TDepth : new()
        {
            foreach (var img in images) AddImage(img);
        }

        public void AddImage<TColor, TDepth>(Image<TColor, TDepth> image) where TColor : struct, IColor where TDepth : new() 
        {
            var singleImageWidth = table.Size.Width / table.ColumnCount;
            var singleImageHeight = table.Size.Height / 2;
            var box = new ImageBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Width = singleImageWidth,
                Height = singleImageHeight,
                SizeMode = PictureBoxSizeMode.Zoom,
                FunctionalMode = ImageBox.FunctionalModeOption.Minimum,
                Image = image
            };
            table.Controls.Add(box);
        }

        public void ClearGallery()
        {
            foreach (Control box in table.Controls)
            {
                table.Controls.Remove(box);
            }
        }
    }
}
