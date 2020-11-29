using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using SocketIOClient;

namespace BloodPlus
{
    /// <summary>
    /// Interaction logic for currentDonorPage.xaml
    /// </summary>
    public partial class currentDonorPage : UserControl
    {
        Action<string> changePanel;
        Action<Dictionary<string, object>, Action<SocketIOResponse>> sendEventDone;

        public currentDonorPage(Action<string> changePanel, List<Action<SocketIOResponse>> currentDonorListener, Action<Dictionary<string, object>, Action<SocketIOResponse>> sendEventDone)
        {
            InitializeComponent();

            this.changePanel = changePanel;
            this.sendEventDone = sendEventDone;
            donorList.Children.Clear();
            //addToList("TESTING", "A");
            //addToList("TESTING", "B");
            //addToList("TESTING", "C");

            currentDonorListener.Add((response) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    addToList(response);
                }));
            });
        }

        private void addToList(SocketIOResponse resp)
        {
            Grid itemContainer = new Grid()
            {
                Name = "item" + (donorList.Children.Count + 1).ToString(),
                Height = 50,
                ColumnDefinitions = {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };

            Card cardBg = new Card()
            {
                Name = "card",
                Margin = new Thickness(4),
                UniformCornerRadius = 4
            };
            Grid.SetColumnSpan(cardBg, 3);
            itemContainer.Children.Add(cardBg);

            

            List<Label> itemData = new List<Label>
            {
                new Label{
                    Name = "nama",
                    Content = resp.GetValue().Value<string>("nama_donor"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new Label{
                    Name = "tipeDarah",
                    Content = resp.GetValue().Value<string>("bloodType"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };

            itemContainer.Children.Add(new Label {
                Name = "id",
                Content = resp.GetValue().Value<string>("nomor_telepon_donor"),
                Visibility = Visibility.Hidden
            });

            for (int i = 0; i < itemData.Count; i++)
            {
                Grid.SetColumn(itemData[i], i);
                itemContainer.Children.Add(itemData[i]);
            }

            Button doneButton = new Button()
            {
                Content = new Label
                {
                    Content = "Done",
                    Foreground = new SolidColorBrush(Colors.Black),
                    Opacity = 0.5
                },
                Width = 100,
                Background = new SolidColorBrush(Colors.LightSkyBlue),
                BorderBrush = new SolidColorBrush(Colors.Transparent)
            };
            ButtonAssist.SetCornerRadius(doneButton, new CornerRadius(5));
            Grid.SetColumn(doneButton, 2);
            doneButton.Click += (sender, e) =>
            {
                //MessageBox.Show(resp.GetValue().Value<string>("nomor_telepon_donor"));
                sendEventDone(JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(resp.ToString())[0], response => Console.WriteLine(response.ToString()));
                donorList.Children.Remove(doneButton.Parent as UIElement);
            };

            itemContainer.Children.Add(doneButton);
            donorList.Children.Add(itemContainer);
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            changePanel("Dashboard");
        }
    }
}
