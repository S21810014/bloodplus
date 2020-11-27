using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BloodPlus.pageSrc
{
    /// <summary>
    /// Interaction logic for pictureCropWindow.xaml
    /// </summary>
    public partial class pictureCropWindow : Window
    {
        Int32Rect finalRect;

        Point currPoint;
        Point prevPoint;

        BitmapImage fullBitmap;
        //Window info = new Window() { Width = 250, Height = 150 };
        //Label x = new Label() { Content = "X" }, y = new Label() { Content = "Y" }, delta = new Label { Content = "DELTA" };

        Action<TransformedBitmap> changeProfileImg;

        public pictureCropWindow(string imageFile, Action<TransformedBitmap> changeProfileImg)
        {
            InitializeComponent();

            this.changeProfileImg = changeProfileImg;
            //BitmapImage reference = new BitmapImage(new Uri(imageFile));
            //float aspectRatio = reference.PixelWidth / reference.PixelHeight;

            fullBitmap = new BitmapImage();
            fullBitmap.BeginInit();
            fullBitmap.UriSource = new Uri(imageFile);
            //fullBitmap.DecodePixelWidth = (int)(600 * aspectRatio);
            //fullBitmap.DecodePixelHeight = 600;
            fullBitmap.EndInit();
            finalRect = new Int32Rect((fullBitmap.PixelWidth / 2) - (100 / 2), (fullBitmap.PixelHeight / 2) - (100 / 2), 100, 100);
            img.Source = new CroppedBitmap(fullBitmap, finalRect);

            //info.Tag = new Dictionary<string, object> { { "X", dragRect.X }, { "Y", dragRect.Y } };
            //info.Content = new StackPanel() { Height = 150 };
            //(info.Content as StackPanel).Children.Add(x);
            //(info.Content as StackPanel).Children.Add(y);
            //(info.Content as StackPanel).Children.Add(delta);
            //info.Show();
        }

        private void confirmClick(object sender, RoutedEventArgs e)
        {
            CroppedBitmap cb = new CroppedBitmap(fullBitmap, finalRect);
            changeProfileImg(new TransformedBitmap(cb, new ScaleTransform(150f / cb.PixelWidth, 150f / cb.PixelHeight)));
            this.Close();
        }

        private void imgMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (finalRect.X + finalRect.Width + 10 < fullBitmap.PixelWidth && finalRect.Y + finalRect.Height + 10 < fullBitmap.PixelHeight)
                {
                    finalRect.Width += 10;
                    finalRect.Height += 10;
                } else if (finalRect.X + finalRect.Width + 10 > fullBitmap.PixelWidth && finalRect.Height <= finalRect.Width)
                {
                    finalRect.X -= 10;
                    finalRect.Y -= 10;
                    finalRect.Width += 10;
                    finalRect.Height += 10;
                }
            }
            else
            {
                if (finalRect.Width > 10)
                {
                    finalRect.Width -= 10;
                    finalRect.Height -= 10;
                }
                else
                {
                    finalRect.Width = 10;
                    finalRect.Height = 10;
                }
            }

            if(finalRect.X < 0)
            {
                finalRect.X = 0;
            }

            if (finalRect.Y < 0)
            {
                finalRect.Y = 0;
            }

            if(finalRect.Width > fullBitmap.PixelWidth)
            {
                finalRect.Width = fullBitmap.PixelWidth;
            }

            if (finalRect.Height > fullBitmap.PixelHeight)
            {
                finalRect.Height = fullBitmap.PixelHeight;
            }


            CroppedBitmap cb = new CroppedBitmap(fullBitmap, finalRect);
            img.Source = new TransformedBitmap(cb, new ScaleTransform(100f / cb.PixelWidth, 100f / cb.PixelHeight));
        }

        private void imgMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                currPoint = e.GetPosition(imgFrameContent);
                finalRect.X -= (int)(currPoint - prevPoint).X;
                finalRect.Y -= (int)(currPoint - prevPoint).Y;

                if (finalRect.X < 0) finalRect.X = 0;
                if (finalRect.Y < 0) finalRect.Y = 0;
                if (finalRect.X + finalRect.Width > fullBitmap.PixelWidth) finalRect.X = (int)fullBitmap.PixelWidth - finalRect.Width;
                if (finalRect.Y + finalRect.Height > fullBitmap.PixelHeight) finalRect.Y = (int)fullBitmap.PixelHeight - finalRect.Height;

                //x.Content = $"{currPoint.X - prevPoint.X} - {finalRect.X} - {fullBitmap.PixelWidth}";
                //y.Content = $"{currPoint.Y - prevPoint.Y} - {finalRect.Y} - {fullBitmap.PixelHeight}";

                CroppedBitmap cb = new CroppedBitmap(fullBitmap, finalRect);
                img.Source = new TransformedBitmap(cb, new ScaleTransform(100f / cb.PixelWidth, 100f / cb.PixelHeight));
                prevPoint = currPoint;
            }
            else
            {
                prevPoint = e.GetPosition(imgFrameContent);
                currPoint = e.GetPosition(imgFrameContent);
            }
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
