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
    /// Interaction logic for DashboardTable.xaml
    /// </summary>
    public partial class DashboardTable : UserControl
    {
        public DashboardTable()
        {
            InitializeComponent();
            theList.Children.Clear();
            //addToTable("22-11-2020", "RS TEST");
            //addToTable("22-11-2020", "RS TEST");
            //addToTable("22-11-2020", "RS TEST");
        }

        public void clearTable()
        {
            theList.Children.Clear();
        }

        public void addToTable(string date, string responder)
        {
            Grid itemContainer = new Grid()
            {
                Name = "item" + (theList.Children.Count + 1).ToString(),
                Height = 50,
                ColumnDefinitions = {
                    new ColumnDefinition(),
                    new ColumnDefinition() 
                }
            };

            List<Label> itemData = new List<Label>
            {
                new Label{
                    Name = "date",
                    Content = date,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 18
                },
                new Label{
                    Name = "responder",
                    Content = responder,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 18
                }
            };

            for (int i = 0; i < itemData.Count; i++)
            {
                Grid.SetColumn(itemData[i], i);
                itemContainer.Children.Add(itemData[i]);
            }

            theList.Children.Add(itemContainer);
        }
    }
}
