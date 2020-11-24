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
using SocketIOClient;
using Newtonsoft.Json;

namespace BloodPlus.pageSrc
{
    /// <summary>
    /// Interaction logic for tiwowApiTest.xaml
    /// </summary>
    public partial class ApiTest : UserControl
    {
        public ApiTest()
        {
            InitializeComponent();
        }

        private async void triggerRegisterAccountClick(object sender, RoutedEventArgs e)
        {
            SocketIO socket = new SocketIO(new Uri("http://localhost:5000"));

            socket.OnConnected += async (object s, EventArgs evt) =>
            {
                SocketIO sock = s as SocketIO;

                await sock.EmitAsync("registerAccount",
                    (SocketIOResponse response) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            registerAccountData.Content = response.ToString();
                        }));
                    },
                    JsonConvert.SerializeObject(new Dictionary<string, object> {
                        {"hallo", "hey" },
                        {"haha", "wkww" },
                        {"bruh", 12345 }
                    })
                );


            };

            try
            {
                await socket.ConnectAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
