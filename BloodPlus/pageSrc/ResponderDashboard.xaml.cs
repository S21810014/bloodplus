using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using SocketIOClient;
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
    /// Interaction logic for ResponderDashboard.xaml
    /// </summary>
    public partial class ResponderDashboard : UserControl
    {
        currentDonorPage cdonorPage;
        Action<string> changeParentPanel;
        Action<string, Action<SocketIOResponse>> sendRequestLatestDonor;
        Action<string, Action<string>> sendNotifyDonor;
        Dictionary<string, object> userData;

        public ResponderDashboard(
            Action<string> changeParentPanel,
            List<Action<SocketIOResponse>> currentDonorListener,
            Action<Dictionary<string, object>,Action<SocketIOResponse>> sendEventDone,
            Action<string, Action<SocketIOResponse>> sendRequestLatestDonor,
            Dictionary<string, object> userData,
            List<Action<SocketIOResponse>> latestDonorListener,
            Action<string, Action<string>> sendNotifyDonor)
        {
            InitializeComponent();
            this.changeParentPanel = changeParentPanel;
            this.sendRequestLatestDonor = sendRequestLatestDonor;
            this.sendNotifyDonor = sendNotifyDonor;
            this.userData = userData;

            cdonorPage = new currentDonorPage(changePanel, currentDonorListener, sendEventDone);
            container.Children.Add(cdonorPage);
            cdonorPage.Visibility = Visibility.Hidden;

            latestDonorList.Children.Clear();
            //addToLatestDonorList("CODECODECODECODECODECODECODECODECODE", "WAW");
            //addToLatestDonorList("CODECODECODECODECODECODECODECODECODE", "WAW");
            //addToLatestDonorList("CODECODECODECODECODECODECODECODECODE", "WAW");

            Loaded += (sender, e) =>
            {
                latestDonorListener.Add(response =>
                {
                    var test = response.GetValue().ToList();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        latestDonorList.Children.Clear();
                        test.ForEach(el => addToLatestDonorList(
                            el.ToObject<Dictionary<string, object>>()["nama"].ToString(),
                            el.ToObject<Dictionary<string, object>>()["tipe_darah"].ToString(),
                            el.ToObject<Dictionary<string, object>>()["id"].ToString()
                        ));
                    }));
                });

                sendRequestLatestDonor(userData["id"].ToString(), response =>
                {
                    var test = response.GetValue().ToList();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        latestDonorList.Children.Clear();
                        test.ForEach(el => addToLatestDonorList(
                            el.ToObject<Dictionary<string, object>>()["nama"].ToString(),
                            el.ToObject<Dictionary<string, object>>()["tipe_darah"].ToString(),
                            el.ToObject<Dictionary<string, object>>()["id"].ToString()
                        ));
                    }));
                });
            };
        }

        private void changePanel(string panelName)
        {
            if (panelName == "Dashboard")
            {
                dashboardPage.Visibility = Visibility.Visible;
                cdonorPage.Visibility = Visibility.Hidden;
            }
            else if (panelName == "Current Donor")
            {
                dashboardPage.Visibility = Visibility.Hidden;
                cdonorPage.Visibility = Visibility.Visible;
            }
        }

        private void currentDonorClick(object sender, RoutedEventArgs e)
        {
            changePanel("Current Donor");
        }

        private void addToLatestDonorList(string name, string bloodType, string id)
        {
            Grid itemContainer = new Grid()
            {
                Name = "item" + (latestDonorList.Children.Count + 1).ToString(),
                Height = 75,
                ColumnDefinitions = {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition() {Width = new GridLength(0.75, GridUnitType.Star)}
                }
            };

            List<Label> itemData = new List<Label>
            {
                new Label{
                    Name = "name",
                    Content = name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(8),
                    Opacity = 0.65,
                    FontSize = 14
                },
                new Label{
                    Name = "bloodType",
                    Content = bloodType,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(8),
                    Opacity = 0.65,
                    FontSize = 14
                }
            };

            Label usrId = new Label
            {
                Name = "usrID",
                Content = id,
                Visibility = Visibility.Hidden
            };

            itemContainer.Children.Add(usrId);

            for (int i = 0; i < itemData.Count; i++)
            {
                Grid.SetColumn(itemData[i], i);
                itemContainer.Children.Add(itemData[i]);
            }

            Button contactButton = new Button()
            {
                Content = new Label
                {
                    Content = "Contact",
                    Foreground = new SolidColorBrush(Colors.Black),
                    Opacity = 0.5
                },
                Margin = new Thickness(25, 0, 25, 0),
                Background = new SolidColorBrush(Color.FromRgb(255, 161, 161)),
                BorderBrush = new SolidColorBrush(Colors.Transparent)
            };
            ButtonAssist.SetCornerRadius(contactButton, new CornerRadius(15));
            Grid.SetColumn(contactButton, 2);
            contactButton.Click += (sender, e) =>
            {
                sendNotifyDonor(
                    JsonConvert.SerializeObject(new Dictionary<string, object> {
                        {"bloodType", bloodType },
                        {"responder", userData["nama"] },
                        {"alamat", userData["alamat"] },
                        {"id_responder", userData["id"] },
                        {"id_donor", id }
                    }),
                    response =>
                    {
                        //MessageBox.Show(response);
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mBox invalidMsgBox = new mBox("Pendonor sedang merespon di responder lain", 500, 300);
                            invalidMsgBox.Show();
                        }));
                    }
                );
            };

            itemContainer.Children.Add(contactButton);
            latestDonorList.Children.Add(itemContainer);
        }

        private void openNotifyDonor(object sender, RoutedEventArgs e)
        {
            changeParentPanel("Notify Donors");
        }
    }
}
