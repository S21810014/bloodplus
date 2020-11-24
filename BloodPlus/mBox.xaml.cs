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
    /// Interaction logic for mBox.xaml
    /// </summary>
    public partial class mBox : Window
    {
        public mBox(string message, int width, int height)
        {
            InitializeComponent();
            this.message.Text = message;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Fungsi untuk membolehkan window dapat di-drag
        /// </summary>
        /// <param name="sender">sumber elemen UI yang di drag</param>
        /// <param name="e">event arguments</param>
        private void dragClick(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Fungsi untuk menutup window ketika tombol ditekan
        /// </summary>
        /// <param name="sender">sumber elemen UI yang ditekan</param>
        /// <param name="e">event arguments</param>
        private void okClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
