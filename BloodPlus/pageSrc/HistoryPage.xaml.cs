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
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : UserControl
    {
        List<Action<SocketIOResponse>> donorHistoryListener;
        Action<string, Action<SocketIOResponse>> sendRequestHistoryTable;

        public HistoryPage(List<Action<SocketIOResponse>> donorHistoryListener, Action<string, Action<SocketIOResponse>> sendRequestHistoryTable, Dictionary<string, object> userData)
        {
            InitializeComponent();

            this.donorHistoryListener = donorHistoryListener;
            this.sendRequestHistoryTable = sendRequestHistoryTable;
            clearList();

            donorHistoryListener.Add(response =>
            {
                var test = response.GetValue().ToList();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    clearList();
                    test.ForEach(el => addToList(
                        el.ToObject<Dictionary<string, object>>()["nama"].ToString(),
                        el.ToObject<Dictionary<string, object>>()["alamat"].ToString(),
                        el.ToObject<Dictionary<string, object>>()["tanggal"].ToString()
                    ));
                }));
            });

            sendRequestHistoryTable(userData["id"].ToString(), response =>
            {
                var test = response.GetValue().ToList();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    clearList();
                    test.ForEach(el => addToList(
                        el.ToObject<Dictionary<string, object>>()["nama"].ToString(),
                        el.ToObject<Dictionary<string, object>>()["alamat"].ToString(),
                        el.ToObject<Dictionary<string, object>>()["tanggal"].ToString()
                    ));
                }));
            });
        }

        public void clearList()
        {
            historyList.Children.Clear();
        }

        public void addToList(string responder, string address, string date)
        {
            Card cardBg = new Card()
            {
                Name = "card",
                Margin = new Thickness(8,8,24,8),
                UniformCornerRadius = 9,
                Height = 65,
            };
            Grid.SetColumnSpan(cardBg, 3);

            Grid itemContainer = new Grid()
            {
                Name = "item" + (historyList.Children.Count + 1).ToString(),
                Margin = new Thickness(-8, 0, -24, 0),
                ColumnDefinitions = {
                    new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) },
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };
            cardBg.Content = itemContainer;

            List<Label> itemData = new List<Label>
            {
                new Label{
                    Name = "responder",
                    Content = responder,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(16,8,8,8),
                    FontSize = 20
                },
                new Label{
                    Name = "address",
                    Content = address,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(16,8,8,8),
                    FontSize = 20
                },
                new Label{
                    Name = "date",
                    Content = date,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(16,8,8,8),
                    FontSize = 20
                }
            };

            for (int i = 0; i < itemData.Count; i++)
            {
                Grid.SetColumn(itemData[i], i);
                itemContainer.Children.Add(itemData[i]);
            }
            historyList.Children.Add(cardBg);
        }
    }
}
