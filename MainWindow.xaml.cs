using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using Microsoft.Win32;
using OpenCvSharp.WpfExtensions;

namespace RENTEST
{
    public partial class MainWindow : System.Windows.Window
    {
        private BitmapImage sourceMap = new();
        public MainWindow()
        { 
            InitializeComponent();
        }
        //Open and load image in sourceMap
        private void OpenFile_Click(object sender, RoutedEventArgs e){
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Images|*.png; *.jpg; *.jpeg, *.png, *.bmp, *.gif",
                Multiselect = false
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                sourceMap = new BitmapImage(new Uri(openFileDialog.FileName));
                currentImage.Source = sourceMap;
                EnableButtons();
            }
        }
        //Calls on every ComboBox change. Apply any of presented filters with several functions
        private void ChooseFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized)
            {
                    switch (chooseFilterButton.SelectedIndex)
                    {
                        case 0:
                            currentImage.Source = sourceMap;
                            break;
                        case 1:
                            SetGrayScaleFiltering();
                            break;
                        case 2:
                            SetMedianBlur();
                            break;
                        case 3:
                            SetLaplase();
                            break;
                    }
            }
        }
       
        private void SetGrayScaleFiltering()
        {
            Mat src = new Mat(sourceMap.UriSource.AbsolutePath, ImreadModes.Grayscale);
            BitmapSource bit = src.ToBitmapSource();
            currentImage.Source = bit;
            src.Dispose();
        }
        private void SetMedianBlur()
        {
            Mat src = new Mat(sourceMap.UriSource.AbsolutePath, ImreadModes.AnyColor);
            Cv2.MedianBlur(src, src, 5);
            BitmapSource bit = src.ToBitmapSource();
            currentImage.Source = bit;
            src.Dispose();
        }
        private void SetLaplase()
        {
            Mat src = new Mat(sourceMap.UriSource.AbsolutePath, ImreadModes.AnyColor);
            Cv2.Laplacian(src, src, MatType.CV_8UC4);
            BitmapSource bit = src.ToBitmapSource();
            currentImage.Source = bit;
            src.Dispose();
        }
        //Enable or Reload buttons states on every new image loaded
        private void EnableButtons()
        {
            saveCurrentImageButton.IsEnabled = true;
            grayScaleButton.IsEnabled = true;
            chooseFilterButton.IsEnabled = true;
            chooseFilterButton.SelectedIndex = 0;
            grayScaleText.Text = string.Empty;
        }
        //Save current image as JPG, BMP and GIF format
        private void saveCurrentImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new()
            {
                Title = "Save picture as ",
                Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp"
            };

            if (currentImage != null)
            {
                if (saveDialog.ShowDialog() == true)
                {
                    var fileExtension = Path.GetExtension(saveDialog.FileName);
                    using System.IO.Stream stm = File.Create(saveDialog.FileName);
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                    {
                        JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                        jpg.Frames.Add(BitmapFrame.Create((BitmapSource)currentImage.Source));
                        jpg.Save(stm);
                    }
                    else if (fileExtension == ".bpm")
                    {
                        BmpBitmapEncoder bmp = new BmpBitmapEncoder();
                        bmp.Frames.Add(BitmapFrame.Create((BitmapSource)currentImage.Source));
                        bmp.Save(stm);
                    }
                    else if (fileExtension == ".gif")
                    {
                        GifBitmapEncoder gif = new GifBitmapEncoder();
                        gif.Frames.Add(BitmapFrame.Create((BitmapSource)currentImage.Source));
                        gif.Save(stm);
                    }
                }
            }
        }
        //Initialize parameters for calculating gray level
        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            Bitmap bitmap = new Bitmap(sourceMap.UriSource.AbsolutePath);
            var redSum = 0;
            var greenSum = 0;
            var blueSum = 0;
            grayScaleText.Text = CalculatePixelsValue(redSum,greenSum,blueSum,bitmap).ToString();
        }
        //Calculate a average value of pixel's color from 0 to 255
        private int CalculatePixelsValue(int red, int green, int blue, Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var px = bitmap.GetPixel(x, y);
                    red += px.R;
                    green += px.G;
                    blue += px.B;
                }
            }
            red /= bitmap.Width * bitmap.Height;
            blue /= bitmap.Height * bitmap.Width;
            green /= bitmap.Height * bitmap.Height;
            int sum = (red + blue + green) / 3;
            return sum;
        }
    }
}


