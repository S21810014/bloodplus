using MaterialDesignThemes.Wpf;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BloodPlus.mobile
{
    /// <summary>
    /// Interaction logic for MainWindowMobile.xaml
    /// </summary>
    public partial class MainWindowMobile : Window
    {
        public class myEasingFunction : IEasingFunction
        {
            public double Ease(double normalizedTime)
            {
                return utils.easeInOutQuart(normalizedTime);
            }
        }

        public MainWindowMobile()
        {
            InitializeComponent();

            sidebar.Tag = new Dictionary<string, object>() { {"shown", true} };
            sidebarOutArea.MouseDown += (sender, e) => hamburgerClick(hamburger, null);
        }

        private void hamburgerClick(object sender, MouseButtonEventArgs e)
        {
            ThicknessAnimation sidebarMarginAnim = new ThicknessAnimation();
            ThicknessAnimation contentMarginAnim = new ThicknessAnimation();
            sidebarMarginAnim.Duration = TimeSpan.FromMilliseconds(400);
            if(sidebar.Margin.Left < 0)
            {
                sidebarMarginAnim.From = new Thickness(-250, 0, 0, 0);
                sidebarMarginAnim.To = new Thickness(0, 0, 0, 0);

                contentMarginAnim.From = new Thickness(0, 0, 0, 0);
                contentMarginAnim.To = new Thickness(250, 0, -250, 0);
            } 
            else
            {
                sidebarMarginAnim.From = new Thickness(0, 0, 0, 0);
                sidebarMarginAnim.To = new Thickness(-250, 0, 0, 0);

                contentMarginAnim.From = new Thickness(250, 0, -250, 0);
                contentMarginAnim.To = new Thickness(0, 0, 0, 0);
            }
            
            sidebarMarginAnim.EasingFunction = new myEasingFunction();
            contentMarginAnim.EasingFunction = new myEasingFunction();

            Storyboard.SetTarget(sidebarMarginAnim, sidebar);
            Storyboard.SetTargetProperty(sidebarMarginAnim, new PropertyPath(Card.MarginProperty));
            Storyboard sidebarMarginAnimStoryboard = new Storyboard() { Children = { sidebarMarginAnim } };

            Storyboard.SetTarget(contentMarginAnim, contentPage);
            Storyboard.SetTargetProperty(contentMarginAnim, new PropertyPath(Grid.MarginProperty));
            Storyboard contentMarginAnimStoryboard = new Storyboard() { Children = { contentMarginAnim } };

            sidebarMarginAnimStoryboard.CurrentStateInvalidated += (s, evt) =>
            {
                (sender as Frame).IsEnabled = false;
                sidebarOutArea.IsEnabled = false;
            };

            sidebarMarginAnimStoryboard.Completed += (s, evt) =>
            {
                (sender as Frame).IsEnabled = true;
                sidebarOutArea.IsEnabled = true;
            };

            contentMarginAnimStoryboard.CurrentStateInvalidated += (s, evt) =>
            {
                contentPage.IsEnabled = false;
            };

            contentMarginAnimStoryboard.Completed += (s, evt) =>
            {
                contentPage.IsEnabled = true;
            };

            if ((bool)((sidebar.Tag as Dictionary<string, object>)["shown"]))
            {
                sidebarOutArea.Visibility = Visibility.Visible;
                DoubleAnimation opacityAnim = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(400),
                    From = 0.0,
                    To = 0.25
                };

                Storyboard.SetTarget(opacityAnim, sidebarOutArea);
                Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Frame.OpacityProperty));
                Storyboard opacityAnimStoryboard = new Storyboard() { Children = { opacityAnim } };
                opacityAnimStoryboard.Begin(this);
            }
            else
            {
                DoubleAnimation opacityAnim = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(400),
                    From = 0.25,
                    To = 0.0
                };

                Storyboard.SetTarget(opacityAnim, sidebarOutArea);
                Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Frame.OpacityProperty));
                Storyboard opacityAnimStoryboard = new Storyboard() { Children = { opacityAnim } };
                opacityAnimStoryboard.Completed += (s, evt) => sidebarOutArea.Visibility = Visibility.Hidden;
                opacityAnimStoryboard.Begin(this);

            }

            (sidebar.Tag as Dictionary<string, object>)["shown"] = !((bool)(sidebar.Tag as Dictionary<string, object>)["shown"]);

            sidebarMarginAnimStoryboard.Begin(this);
            contentMarginAnimStoryboard.Begin(this);
        }

        private void yellHello(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("HELLO");
        }
    }
}
