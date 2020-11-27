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
using System.Windows.Shapes;

namespace BloodPlus
{
    /// <summary>
    /// Interaction logic for devPageTester.xaml
    /// </summary>
    public partial class devPageTester : Window
    {
        public devPageTester()
        {
            InitializeComponent();
        }

        public void setContent(UIElement page)
        {
            contentPage.Children.Clear();
            contentPage.Children.Add(page);
        }
    }
}
