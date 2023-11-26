using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ImageProcessingTraining
{
    public partial class Form : System.Windows.Forms.Form
    {
        private List<Bitmap> bitmaps = new List<Bitmap>();
        private Random random = new Random();

        public Form() => InitializeComponent();

        private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK) 
            {
                var processingTimeSw = Stopwatch.StartNew();

                menuStrip.Enabled = trackBar.Enabled = false;

                trackBar.Value = trackBar.Minimum;

                pictureBox.Image = null;
                bitmaps.Clear();

                var bitmap = new Bitmap(openFileDialog.FileName);

                pictureBox1.Visible = true;

                await Task.Run(() => StartProcessing(bitmap));

                processingTimeSw.Stop();

                pictureBox1.Visible = false;

                Text = $"Processing took: {processingTimeSw.Elapsed}";

                menuStrip.Enabled = trackBar.Enabled = true;
            }
        }

        private void StartProcessing(Bitmap bitmap)
        {
            var pixels = GetPixelsFromBitmap(bitmap);
            var pixelsInPercent = (bitmap.Width * bitmap.Height) / trackBar.Maximum;
            var currentPixelsSelection = new List<Pixel>(pixels.Count - pixelsInPercent);

            for (int i = 1; i < trackBar.Maximum; i++)
            {
                for (int j = 0; j < pixelsInPercent; j++)
                {
                    var index = random.Next(pixels.Count);
                    currentPixelsSelection.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }

                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                foreach (var pixel in currentPixelsSelection)
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);

                bitmaps.Add(currentBitmap);

                Invoke(new Action(() => Text = $"Processing progress: {i}%"));
                
            }
            bitmaps.Add(bitmap);

            pictureBox.Image = bitmaps[trackBar.Value];
        }

        private List<Pixel> GetPixelsFromBitmap(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point() { X = x, Y = y},
                    });
                }
            }

            return pixels;
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            if (bitmaps.Count == 0)
                return;

            pictureBox.Image = bitmaps[trackBar.Value - 1];

            Text = $"Pixels: {trackBar.Value}%";
        }
    }
}
