using MaterialDesignThemes.Wpf;
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

        public ResponderDashboard(Action<string> changeParentPanel, List<Action<SocketIOResponse>> currentDonorListener, Action<Dictionary<string, object>, Action<SocketIOResponse>> sendEventDone)
        {
            InitializeComponent();
            this.changeParentPanel = changeParentPanel;

            cdonorPage = new currentDonorPage(changePanel, currentDonorListener, sendEventDone);
            container.Children.Add(cdonorPage);
            cdonorPage.Visibility = Visibility.Hidden;

            latestDonorList.Children.Clear();
            addToLatestDonorList("CODECODECODECODECODECODECODECODECODE", "WAW");
            addToLatestDonorList("CODECODECODECODECODECODECODECODECODE", "WAW");
            addToLatestDonorList("CODECODECODECODECODECODECODECODECODE", "WAW");
        }

        private void changePanel(string panelName)
        {
            if (panelName == "Dashboard")
            {
                dashboardPage.Visibility = Visibility.Visible;
                cdonorPage.Visibility = Visibility.Hidden;
            } else if(panelName == "Current Donor")
            {
                dashboardPage.Visibility = Visibility.Hidden;
                cdonorPage.Visibility = Visibility.Visible;
            }
        }

        private void currentDonorClick(object sender, RoutedEventArgs e)
        {
            changePanel("Current Donor");
        }

        private void addToLatestDonorList(string name, string bloodType)
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
                Margin = new Thickness(25,0,25,0),
                Background = new SolidColorBrush(Color.FromRgb(255, 161, 161)),
                BorderBrush = new SolidColorBrush(Colors.Transparent)
            };
            ButtonAssist.SetCornerRadius(contactButton, new CornerRadius(15));
            Grid.SetColumn(contactButton, 2);
            contactButton.Click += (sender, e) =>
            {
                //add contact donor code here
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
