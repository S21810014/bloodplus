using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using Newtonsoft.Json;
using MaterialDesignThemes.Wpf;

namespace BloodPlus.pageSrc
{
    /// <summary>
    /// Interaction logic for tiwowLoginPage.xaml
    /// </summary>

    public partial class LoginPage : UserControl
    {
        Dictionary<string, util.DoubleTimer> timers = new Dictionary<string, util.DoubleTimer>();
        Dictionary<string, object> registerData = new Dictionary<string, object>();
        Action<string> changePanel;
        Action<string, Action<string>> sendLogin;
        Action<string, Action<string>> sendRegister;

        public LoginPage(Action<string> changePanel, Action<string, Action<string>> sendLogin, Action<string, Action<string>> sendRegister)
        {
            InitializeComponent();

            this.changePanel = changePanel;
            this.sendLogin = sendLogin;
            this.sendRegister = sendRegister;

        }

        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            sendLogin(
                JsonConvert.SerializeObject(
                    //cek kalo ini responder yang mo login (ditandai dengan posisi halaman transitioner skrng index 2)
                    Transitioner.SelectedIndex == 2 ?
                    //io ini responder
                    new Dictionary<string, string> {
                        {"responder", "RESPONDER"},
                        {"nomor_telepon", tboxRespPhoneNumber.Text },
                        {"password", tboxRespPassword.Password }
                    }
                    :
                    //bukang ini pendonor
                    new Dictionary<string, string> {
                        {"nomor_telepon", tboxPhoneNumber.Text },
                        {"password", tboxPassword.Password }
                    }
                ),
                (string json) =>
                {
                    Dictionary<string, object> resp = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json)[0];

                    if (resp["resp"] as string == "Valid")
                    {
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    //changePanel("Dashboard");

                                    //cek kalo ini responder
                                    if (Transitioner.SelectedIndex == 2)
                                    {
                                        tboxRespPhoneNumber.Text = "";
                                        tboxRespPassword.Password = "";
                                    }
                                    //bukang responder
                                    else
                                    {
                                        tboxPhoneNumber.Text = "";
                                        tboxPassword.Password = "";
                                    }
                                }
                            )
                        );
                    }
                    else
                    {
                        //MessageBox.Show("[DEVMODE] " + resp["resp"] as string);
                        Dispatcher.BeginInvoke(new Action(() => {
                            mBox invalidMsgBox = new mBox("Password/Username invalid", 300, 200);
                            invalidMsgBox.Show();
                        }));
                        
                    }
                }
            );
            //changePanel(new pageSrc.ApiTest());
        }

        private void mapSearchClick(object sender, RoutedEventArgs e)
        {
            MapWindow mw = new MapWindow();

            //tambah pendengar ke JembatanSablaMinanga dari fungsi ini
            mw.jembatan.tambaTalinga((string balasan) =>
            {
                Dictionary<string, object> tamparPipiKiri = JsonConvert.DeserializeObject<Dictionary<string, object>>(balasan);

                registerData["latitude"] = tamparPipiKiri["lat"];
                registerData["longitude"] = tamparPipiKiri["lng"];
                registerData["alamat"] = tamparPipiKiri["display"];

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if ((sender as Button).Name == "btnMap")
                        tboxRegAlamat.Text = tamparPipiKiri["display"] as string;
                    else
                        tboxRegRespAlamat.Text = tamparPipiKiri["display"] as string;

                    mw.Close();
                }));
            });

            mw.Show();
        }

        private void TransitionerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine((sender as MaterialDesignThemes.Wpf.Transitions.Transitioner).SelectedIndex);
            tboxPassword.Password = "";
            tboxRespPassword.Password = "";
            tboxPhoneNumber.Text = "";
            tboxRespPhoneNumber.Text = "";
        }

        private void registerAccountClicked(object sender, MouseButtonEventArgs e)
        {
            Transitioner.SelectedIndex = 1;
        }

        private void loginClicked(object sender, MouseButtonEventArgs e)
        {
            Transitioner.SelectedIndex = 0;
        }

        private void registerRespAccountClicked(object sender, MouseButtonEventArgs e)
        {
            Transitioner.SelectedIndex = 3;
        }

        private void btnRegisterRespClick(object sender, RoutedEventArgs e)
        {
            List<List<Func<string, bool>>> predicates = new List<List<Func<string, bool>>>()
            {
                //nama
                new List<Func<string, bool>>{string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsDigit)},
                //alamat
                new List<Func<string, bool>>{string.IsNullOrWhiteSpace},
                //nomor telepon
                new List<Func<string, bool>>{string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsLetter)}
            };

            List<TextBox> targets = new List<TextBox>
            {
                tboxRegRespNama,
                tboxRegRespAlamat,
                tboxRegRespNomorTelepon
            };

            for (int i = 0; i < predicates.Count; i++)
            {
                for (int j = 0; j < predicates[i].Count; j++)
                {
                    if (predicates[i][j](targets[i].Text))
                    {
                        mBox invalidMsgBox = new mBox(HintAssist.GetHint(targets[i]).ToString() + " tidak valid", 300, 200);
                        invalidMsgBox.Show();
                        return;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(tboxRegRespPassword.Password))
            {
                mBox invalidMsgBox = new mBox(HintAssist.GetHint(tboxRegRespPassword).ToString() + " tidak valid", 300, 200);
                invalidMsgBox.Show();
                return;
            }

            MessageBox.Show(
                tboxRegRespNama.Text + "\n" +
                tboxRegRespAlamat.Text + "\n" +
                tboxRegRespNomorTelepon.Text + "\n" +
                tboxRegRespPassword.Password + "\n"
            );

            registerData["responder"] = "RESPONDER";
            registerData["nama"] = tboxRegRespNama.Text;
            registerData["alamat"] = tboxRegRespAlamat.Text;
            registerData["nomor_telepon"] = tboxRegRespNomorTelepon.Text;
            registerData["password"] = tboxRegRespPassword.Password;

            sendRegister(
                JsonConvert.SerializeObject(registerData),
                (string json) =>
                {
                    Dictionary<string, object> resp = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json)[0];

                    if (resp["resp"] as string == "Already Exist and Ignored")
                    {
                        mBox invalidMsgBox = new mBox("Akun dengan nomor telepon tersebut sudah terdaftar", 300, 200);
                        invalidMsgBox.Show();
                    }
                    else
                    {
                        mBox invalidMsgBox = new mBox("Pendaftaran Berhasil", 300, 200);
                        invalidMsgBox.Show();

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            registerData.Clear();
                            tboxRegRespNama.Text = "";
                            tboxRegRespAlamat.Text = "";
                            tboxRegRespNomorTelepon.Text = "";
                            tboxRegRespPassword.Password = "";
                        }));
                    }
                }
            );
        }

        private void btnRegisterClick(object sender, RoutedEventArgs e)
        {
            List<List<Func<string, bool>>> predicates = new List<List<Func<string, bool>>>()
            {
                //nama
                new List<Func<string, bool>>{string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsDigit)},
                //alamat
                new List<Func<string, bool>>{string.IsNullOrWhiteSpace},
                //nomor telepon
                new List<Func<string, bool>>{string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsLetter)},
                //tinggi badan
                new List<Func<string, bool>>{ string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsLetter) },
                //berat badan
                new List<Func<string, bool>>{ string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsLetter) },
                //umur
                new List<Func<string, bool>>{ string.IsNullOrWhiteSpace, (theString) => theString.Any(char.IsLetter) }
            };

            List<TextBox> targets = new List<TextBox>
            {
                tboxRegNama,
                tboxRegAlamat,
                tboxRegNomorTelepon,
                tboxRegTinggiBadan,
                tboxRegBeratBadan,
                tboxRegUmur
            };

            for (int i = 0; i < predicates.Count; i++)
            {
                for (int j = 0; j < predicates[i].Count; j++)
                {
                    if (predicates[i][j](targets[i].Text))
                    {
                        mBox invalidMsgBox = new mBox(HintAssist.GetHint(targets[i]).ToString() + " tidak valid", 300, 200);
                        invalidMsgBox.Show();
                        return;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(tboxRegPassword.Password))
            {
                mBox invalidMsgBox = new mBox(HintAssist.GetHint(tboxRegPassword).ToString() + " tidak valid", 300, 200);
                invalidMsgBox.Show();
                return;
            }

            foreach (ComboBox cbox in new List<ComboBox> { tboxRegGolDar, tboxRegJenisKelamin })
            {
                if (cbox.SelectedItem == null)
                {
                    mBox invalidMsgBox = new mBox(HintAssist.GetHint(cbox).ToString() + " tidak valid", 300, 200);
                    invalidMsgBox.Show();
                    return;
                }
            }

            registerData["nama"] = tboxRegNama.Text;
            registerData["umur"] = tboxRegUmur.Text;
            registerData["jenis_kelamin"] = (tboxRegJenisKelamin.SelectedItem as ComboBoxItem).Content.ToString();
            registerData["berat_badan"] = tboxRegBeratBadan.Text;
            registerData["tinggi_badan"] = tboxRegTinggiBadan.Text;
            registerData["tipe_darah"] = (tboxRegGolDar.SelectedItem as ComboBoxItem).Content.ToString();
            registerData["nomor_telepon"] = tboxRegNomorTelepon.Text;
            registerData["password"] = tboxRegPassword.Password;

            sendRegister(
                JsonConvert.SerializeObject(registerData),
                (string json) =>
                {
                    Dictionary<string, object> resp = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json)[0];

                    if (resp["resp"] as string == "Already Exist and Ignored")
                    {
                        mBox invalidMsgBox = new mBox("Akun dengan nomor telepon tersebut sudah terdaftar", 300, 200);
                        invalidMsgBox.Show();
                    }
                    else
                    {
                        mBox invalidMsgBox = new mBox("Pendaftaran Berhasil", 300, 200);
                        invalidMsgBox.Show();

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            registerData.Clear();
                            tboxRegNama.Text = "";
                            tboxRegUmur.Text = "";
                            tboxRegJenisKelamin.SelectedItem = null;
                            tboxRegBeratBadan.Text = "";
                            tboxRegTinggiBadan.Text = "";
                            tboxRegGolDar.SelectedItem = null;
                            tboxRegNomorTelepon.Text = "";
                            tboxRegPassword.Password = "";
                            tboxRegAlamat.Text = "";
                        }));
                    }
                }
            );
            //MessageBox.Show(
            //    tboxRegNama.Text + "\n" +
            //    tboxRegAlamat.Text + "\n" +
            //    tboxRegUmur.Text + "\n" +
            //    (tboxRegGolDar.SelectedItem as ComboBoxItem).Content.ToString() + "\n" +
            //    (tboxRegJenisKelamin.SelectedItem as ComboBoxItem).Content.ToString() + "\n" +
            //    tboxRegNomorTelepon.Text + "\n" +
            //    tboxRegTinggiBadan.Text + "\n" +
            //    tboxRegBeratBadan.Text + "\n" +
            //    tboxRegPassword.Password + "\n"
            //);
        }

        private void goToResponderLoginClick(object sender, MouseButtonEventArgs e)
        {
            Transitioner.SelectedIndex = 2;
        }

        private void goToResponderRegisterClick(object sender, MouseButtonEventArgs e)
        {
            Transitioner.SelectedIndex = 3;
        }
    }
}
