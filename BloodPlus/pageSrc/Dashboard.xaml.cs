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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        Action<string> changePanel;

        public Dashboard(Action<string> changePanel, Dictionary<string, object> userData)
        {
            InitializeComponent();

            this.changePanel = changePanel;
            txtName.Content = userData["nama"] as string;
            txtBloodType.Content = userData["tipe_darah"] as string;
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
