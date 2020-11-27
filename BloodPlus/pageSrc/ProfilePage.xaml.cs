using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BloodPlus.pageSrc
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : UserControl
    {
        Action<object, Action<string>> sendProfileJpeg;

        public ProfilePage(Dictionary<string, object> userData, Action<object, Action<string>> sendProfileJpeg)
        {
            InitializeComponent();

            this.sendProfileJpeg = sendProfileJpeg;

            txtName.Content = userData["nama"] as string;
            txtAddress.Content = userData["alamat"] as string;
            txtBloodType.Content = userData["tipe_darah"] as string;
            txtHeight.Content = userData["tinggi_badan"].ToString();
            txtWeight.Content = userData["berat_badan"].ToString();
            txtPhoneNumber.Content = userData["nomor_telepon"] as string;

            if(userData.ContainsKey("profilePic"))
            {
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(userData["profilePic"] as string));

                BitmapImage bm = new BitmapImage();

                bm.BeginInit();
                bm.StreamSource = ms;
                bm.EndInit();

                imgProfile.Source = bm;
            }
        }

        private void changeProfilePictureClick(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                //System.Windows.Forms.MessageBox.Show(ofd.FileName);


                pictureCropWindow pcw = new pictureCropWindow(ofd.FileName, changeProfileImg);
                pcw.Show();
            }
        }

        private void changeProfileImg(TransformedBitmap bimg)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 80;
            BitmapFrame bf = BitmapFrame.Create(bimg);
            encoder.Frames.Add(bf);

            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);

            //this writes jpeg to console
            //Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            sendProfileJpeg(new {phoneNumber = txtPhoneNumber.Content, bytes = Convert.ToBase64String(ms.ToArray())}, response =>
            {
                MessageBox.Show(response);
                
            });
            //using(FileStream fs = new FileStream("test.jpg", FileMode.Create))
            //{
            //    encoder.Save(fs);
            //}

            //imgProfile.Source = bimg;
        }
    }
}
