using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using SocketIOClient;

namespace BloodPlus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Dictionary 'timer' untuk menyimpan semua Timer yang digunakan untuk animasi UI
        /// </summary>
        Dictionary<string, util.DoubleTimer> timers = new Dictionary<string, util.DoubleTimer>();

        /// <summary>
        /// Dictionary 'panel' untuk menyimpan semua panel yang akan ditampilkan di aplikasi
        /// </summary>
        Dictionary<string, UserControl> panels = new Dictionary<string, UserControl>();

        /// <summary>
        /// Dictionary 'upperNavs' untuk menyimpan link link navigasi di header
        /// </summary>
        Dictionary<string, TextBlock> upperNavs = new Dictionary<string, TextBlock>();

        /// <summary>
        /// socket sebagai komunikator dengan server
        /// </summary>
        SocketIO socket = new SocketIO(new Uri("http://192.168.43.191:5000"));

        /// <summary>
        /// Dictionary 'userData' untuk menyimpan data-data tentang user setelah user tsb login
        /// </summary>
        Dictionary<string, object> userData { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //helper window to debug one page
            //devPageTester dpt = new devPageTester();
            //dpt.setContent(new pageSrc.ProfilePage(new Dictionary<string, object> {
            //    {"nama", "A" },
            //    {"alamat", "A" },
            //    {"tipe_darah", "A" },
            //    {"tinggi_badan", "A" },
            //    {"berat_badan", "A" },
            //    {"nomor_telepon", "A" },
            //}));
            //dpt.Show();
            //this.Hide();

            //meregister beberapa event handler dari server yang bisa diterima aplikasi
            socket.OnConnected += (object s, EventArgs evt) =>
            {
                SocketIO sock = s as SocketIO;

                sock.On("donorNotify", response =>
                {
                    //pattern Dispatcher.BeginInvoke() digunakan untuk menjalankan fungsi seolah-olah fungsi tsb bersumber dari main thread
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mBox msgbox = new mBox(response.ToString(), 500, 500);
                        msgbox.Show();
                    }));
                });

                MessageBox.Show("Connected to server");
            };

            try
            {
                socket.ConnectAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //init userdata object
            userData = new Dictionary<string, object>();

            panels.Add("Login", new pageSrc.LoginPage(changePanel, sendLogin, sendRegister));
            panels.Add("History", new pageSrc.HistoryPage());
            changePanel("Login");
        }

        #region socket io methods
        /// <summary>
        /// Fungsi untuk mengirim informasi login ke server
        /// </summary>
        /// <param name="json">informasi login pengguna dalam bentuk json</param>
        /// <param name="callback">fungsi yang akan dipanggil ketika server mengakui informasi login tersebut salah atau benar</param>
        private async void sendLogin(string json, Action<string> callback)
        {
            await socket.EmitAsync("login", async (SocketIOResponse response) =>
            {
                //response berupa json dalam bentuk string, maka dari itu kita ubah menjadi Dictionary<string, object>
                Dictionary<string, object> resp = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.ToString())[0];

                //cek kalo response mempunyai key 'resp' bernilai 'valid
                if (resp["resp"] as string == "Valid")
                {

                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        txtUpperRightName.Text = $"Hi, {resp["nama"] as string}";
                        userData = resp;

                        #region setting upper nav items
                        //check if it's responder whos logging in
                        if (resp.ContainsKey("responder"))
                        {
                            upperNavs.Clear();
                            upperNavGrid.Children.Clear();

                            upperNavs["Dashboard"] = new TextBlock()
                            {
                                Text = "Dashboard",
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 15),
                                FontWeight = FontWeights.Bold,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Opacity = 0.65,
                            };
                            Grid.SetColumn(upperNavs["Dashboard"], 1);

                            upperNavs["Notify Donors"] = new TextBlock()
                            {
                                Text = "Notify Donors",
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 15),
                                FontWeight = FontWeights.Normal,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Opacity = 0.65,
                            };
                            Grid.SetColumn(upperNavs["Notify Donors"], 2);

                            foreach (TextBlock navs in upperNavs.Values)
                            {
                                upperNavGrid.Children.Add(navs);
                                navs.MouseDown += upperNavClick;
                            }

                            panels["Dashboard"] = new pageSrc.ResponderDashboard(changePanel);
                            panels["Notify Donors"] = new pageSrc.ResponderNotifyDonor(sendNotifDonor, userData);
                        }
                        else
                        {
                            upperNavs.Clear();
                            upperNavGrid.Children.Clear();

                            upperNavs["Dashboard"] = new TextBlock()
                            {
                                Text = "Dashboard",
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 15),
                                FontWeight = FontWeights.Bold,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Opacity = 0.65,
                            };
                            Grid.SetColumn(upperNavs["Dashboard"], 1);

                            upperNavs["Profile"] = new TextBlock()
                            {
                                Text = "Profile",
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 15),
                                FontWeight = FontWeights.Normal,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Opacity = 0.65,
                            };
                            Grid.SetColumn(upperNavs["Profile"], 2);

                            upperNavs["History"] = new TextBlock()
                            {
                                Text = "History",
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 15),
                                FontWeight = FontWeights.Normal,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Opacity = 0.65,
                            };
                            Grid.SetColumn(upperNavs["History"], 3);

                            foreach (TextBlock navs in upperNavs.Values)
                            {
                                upperNavGrid.Children.Add(navs);
                                navs.MouseDown += upperNavClick;
                            }
                        }
                        #endregion
                    }));

                    if (!resp.ContainsKey("responder"))
                    {
                        await socket.EmitAsync("requestProfileJpeg", rsp =>
                        {
                            Console.WriteLine("during request jpg");
                            if (rsp.GetValue().Value<string>("retval") == "error")
                            {
                                Console.WriteLine("error getting profile pic");
                            }
                            else
                            {
                                userData["profilePic"] = rsp.GetValue().Value<string>("bytes");
                            }

                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                panels["Profile"] = new pageSrc.ProfilePage(resp, sendProfileJpeg);
                                panels["Dashboard"] = new pageSrc.Dashboard(changePanel, resp);
                                changePanel("Dashboard");
                            }));
                        }, new { phoneNumber = userData["nomor_telepon"] });
                    }
                }

                //send response the one who calls this
                callback(response.ToString());
            }, json);
        }


        /// <summary>
        /// Fungsi untuk mengirim data pendaftaran user ke layanan ini
        /// </summary>
        /// <param name="json">data user dalam bentuk json</param>
        /// <param name="callback">fungsi yang akan dipanggil ketika server mengakui data itu valid dan sudah dimasukkan
        /// ke server</param>
        private async void sendRegister(string json, Action<string> callback)
        {
            await socket.EmitAsync("registerAccount", response =>
            {
                callback(response.ToString());
            }, json);
        }

        /// <summary>
        /// Fungsi untuk me-'logout' pengguna dari sistem
        /// </summary>
        private async void sendLogout()
        {
            //just checking if userData actually contains anything
            if (userData.ContainsKey("id"))
            {
                userData.Remove("profilePic");

                await socket.EmitAsync("logout", response =>
                {
                    Console.WriteLine(response);
                    userData.Clear();

                }, JsonConvert.SerializeObject(userData));
            }
            else
            {
                throw new Exception("USER DATA NOT EXIST");
            }
        }


        /// <summary>
        /// Fungsi untuk mengirim notifikasi ke para pendonor
        /// </summary>
        /// <param name="json">informasi emergency/notifikasi yang akan dikirim ke para donor dalam bentuk json</param>
        /// <param name="callback">fungsi yang akan dipanggil ketika server berhasil mengirim notifikasi</param>
        private async void sendNotifDonor(string json, Action<string> callback)
        {
            await socket.EmitAsync("responderSendEvent", response => callback(response.ToString()), json);
        }

        private async void sendProfileJpeg(object imgData, Action<string> callback)
        {
            await socket.EmitAsync("getProfileJpeg", response => callback(response.ToString()), imgData);
        }
        #endregion

        /// <summary>
        /// Fungsi untuk berpindah antara panel-panel yang tersedia di aplikasi
        /// </summary>
        /// <param name="panelName">nama panel</param>
        public void changePanel(string panelName)
        {
            contentCard.Content = panels[panelName];

            //khusus untuk login, jika panel sekarang adalah panel login, jalankan animasi utk menyembunyikan header bar
            if (panelName == "Login")
            {
                timers.Add("headerTimer", new util.DoubleTimer() { timer = new System.Timers.Timer(20), value = 0, add = 0.025 });

                if (timers.ContainsKey("headerTimer"))
                {
                    timers["headerTimer"].timer.Elapsed += async (object sender, ElapsedEventArgs evt) =>
                    {
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mainWindowContainer.RowDefinitions[0].Height = new GridLength(137 - (utils.easeInOutQuart(timers["headerTimer"].value) * 137));
                        }));

                        timers["headerTimer"].value += timers["headerTimer"].add;

                        if (timers["headerTimer"].value > 1)
                        {
                            (sender as Timer).Enabled = false;
                            timers.Remove("headerTimer");
                        }
                    };

                    timers["headerTimer"].timer.Start();
                }
            }
            //jika bukan panel login dan header bar sedang dalam kondisi tersembunyi, jalankan animasi untuk memunculkan header bar
            else if (!(panelName == "Login") && mainWindowContainer.RowDefinitions[0].Height.Value < 25.0)
            {
                timers.Add("headerTimer", new util.DoubleTimer() { timer = new System.Timers.Timer(20), value = 0.25, add = 0.025 });

                if (timers.ContainsKey("headerTimer"))
                {
                    timers["headerTimer"].timer.Elapsed += async (object sender, ElapsedEventArgs evt) =>
                    {
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (timers.ContainsKey("headerTimer"))
                                mainWindowContainer.RowDefinitions[0].Height = new GridLength(utils.easeInOutQuart(timers["headerTimer"].value) * 137);
                        }));

                        timers["headerTimer"].value += timers["headerTimer"].add;

                        if (timers["headerTimer"].value > 1)
                        {
                            (sender as Timer).Enabled = false;
                            timers.Remove("headerTimer");
                        }
                    };

                    timers["headerTimer"].timer.Start();
                }
            }
        }

        /// <summary>
        /// Fungsi yang akan dipanggil ketika user menekan 'sign out' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSignOutClicked(object sender, MouseButtonEventArgs e)
        {
            sendLogout();
            changePanel("Login");
        }

        /// <summary>
        /// Fungsi yang akan dipanggil ketika user menklik salah satu link dari header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upperNavClick(object sender, MouseButtonEventArgs e)
        {
            TextBlock selected = sender as TextBlock;

            changePanel(selected.Text);

            //atur styling font untuk mempertebal link yang sedang aktif
            foreach (KeyValuePair<string, TextBlock> nav in upperNavs)
            {
                if (nav.Value.Text != selected.Text)
                    nav.Value.FontWeight = FontWeights.Normal;
                else
                    nav.Value.FontWeight = FontWeights.Bold;
            }
        }
    }


}
