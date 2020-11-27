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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BloodPlus.pageSrc
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : UserControl
    {
        public ProfilePage(Dictionary<string, object> userData)
        {
            InitializeComponent();

            txtName.Content = userData["nama"] as string;
            txtAddress.Content = userData["alamat"] as string;
            txtBloodType.Content = userData["tipe_darah"] as string;
            txtHeight.Content = userData["tinggi_badan"].ToString();
            txtWeight.Content = userData["berat_badan"].ToString();
            txtPhoneNumber.Content = userData["nomor_telepon"] as string;
        }

        private void changeProfilePictureClick(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = ofd.ShowDialog();

            if(result == true)
            {
                //System.Windows.Forms.MessageBox.Show(ofd.FileName);
                

                pictureCropWindow pcw = new pictureCropWindow(ofd.FileName);
                pcw.Show();
            }
        }
    }
}
