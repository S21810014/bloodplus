using Newtonsoft.Json;
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
    /// Interaction logic for ResponderNotifyDonor.xaml
    /// </summary>
    public partial class ResponderNotifyDonor : UserControl
    {
        Action<string, Action<string>> sendNotifyDonor;
        Dictionary<string, object> userData;

        public ResponderNotifyDonor(Action<string, Action<string>> sendNotifyDonor, Dictionary<string, object> userData)
        {
            InitializeComponent();

            this.sendNotifyDonor = sendNotifyDonor;
            this.userData = userData;
        }

        private void btnSendNotificationClick(object sender, RoutedEventArgs e)
        {
            sendNotifyDonor(
                JsonConvert.SerializeObject(new Dictionary<string, object> {
                    {"bloodType", (cboxBloodType.SelectedItem as ComboBoxItem).Content },
                    {"responder", userData["alamat"] },
                    {"latitude", userData["latitude"] },
                    {"longitude", userData["longitude"] }
                }),
                response =>
                {
                    MessageBox.Show(response);
                }
            );
        }
    }
}
