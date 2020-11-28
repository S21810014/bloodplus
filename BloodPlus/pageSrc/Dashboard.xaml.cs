using SocketIOClient;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        Action<string> changePanel;
        Action<string, Action<SocketIOResponse>> sendRequestHistoryTable;

        public Dashboard(
            Action<string> changePanel, 
            Dictionary<string, object> userData,
            Action<string, Action<SocketIOResponse>> sendRequestHistoryTable,
            List<Action<SocketIOResponse>> donorHistoryListener)
        {
            InitializeComponent();

            this.changePanel = changePanel;
            this.sendRequestHistoryTable = sendRequestHistoryTable;

            txtName.Content = userData["nama"] as string;
            txtBloodType.Content = userData["tipe_darah"] as string;

            Loaded += (sender, e) =>
            {
                if (userData.ContainsKey("profilePic"))
                {
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(userData["profilePic"] as string));

                    BitmapImage bm = new BitmapImage();

                    bm.BeginInit();
                    bm.StreamSource = ms;
                    bm.EndInit();

                    profileImg.Source = bm;
                }

                donorHistoryListener.Add(response =>
                {
                    var test = response.GetValue().ToList();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tableHistoryTable.clearTable();
                        test.ForEach(el => tableHistoryTable.addToTable(
                            el.ToObject<Dictionary<string, object>>()["tanggal"].ToString(),
                            el.ToObject<Dictionary<string, object>>()["nama"].ToString()
                        ));
                    }));
                });

                sendRequestHistoryTable(userData["id"].ToString(), response =>
                {
                    var test = response.GetValue().ToList();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tableHistoryTable.clearTable();
                        test.ForEach(el => tableHistoryTable.addToTable(
                            el.ToObject<Dictionary<string, object>>()["tanggal"].ToString(),
                            el.ToObject<Dictionary<string, object>>()["nama"].ToString()
                        ));
                    }));
                });
            };
        }

        private void profileMoreInformationClick(object sender, RoutedEventArgs e)
        {
            changePanel("Profile");
        }

        private void historyMoreInformationClick(object sender, RoutedEventArgs e)
        {
            changePanel("History");
        }
    }
}
