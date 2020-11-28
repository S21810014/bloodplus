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
    /// Interaction logic for donorNotificationWindow.xaml
    /// </summary>
    public partial class donorNotificationWindow : Window
    {
        Action acceptAction, passAction;

        public donorNotificationWindow(Action acceptAction, Action passAction, string responder, string address, string bloodType)
        {
            InitializeComponent();

            this.acceptAction = acceptAction;
            this.passAction = passAction;

            labelResponder.Content = responder;
            labelAddress.Content = address;
            labelBloodType.Content = bloodType;
        }

        private void volunteerClick(object sender, RoutedEventArgs e)
        {
            acceptAction();
            Close();
        }

        private void passClick(object sender, RoutedEventArgs e)
        {
            passAction();
            Close();
        }

        private void titleDrag(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
