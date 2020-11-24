using System;
using System.IO;
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
using CefSharp;
using Newtonsoft.Json;

namespace BloodPlus.pageSrc
{
    /// <summary>
    /// Class yang akan berinteraksi dengan code javascript di Peta
    /// </summary>
    public class JembatanSablaMinanga
    {
        /// <summary>
        /// daftar pendengar dari class ini
        /// </summary>
        List<Action<string>> beberapaTalinga = new List<Action<string>>();

        /// <summary>
        /// fungsi untuk menambahkan pendengar ke fungsi ini
        /// </summary>
        /// <param name="talingaBaru"></param>
        public void tambaTalinga(Action<string> talingaBaru)
        {
            beberapaTalinga.Add(talingaBaru);
        }

        /// <summary>
        /// javascript akan memanggil fungsi ini, setelah itu
        /// fungsi ini akan memberitahu semua pendengar yang terdaftar
        /// </summary>
        /// <param name="dengarTuInformasi"></param>
        public void seKage(string dengarTuInformasi)
        {
            foreach(Action<string> talinga in beberapaTalinga)
            {
                //eh btw tu informasi dalam bentuk JSON, nanti deserialize pake JsonConvert.DeserializeObject<Dictionary<string, object>>()
                talinga(dengarTuInformasi);
            }
        }
    }

    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public JembatanSablaMinanga jembatan = new JembatanSablaMinanga();
        public string jalanSetapak;
        public float latitude;
        public float longitude;

        public MapWindow()
        {
            InitializeComponent();

            Browser.Loaded += Browser_Loaded;
        }

        /// <summary>
        /// fungsi yang akan dijalankan ketika variabel browser sudah dimuat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.JavascriptObjectRepository.ResolveObject += (s, evt) =>
            {
                var repo = evt.ObjectRepository;

                if (evt.ObjectName == "JembatanSablaMinanga")
                {
                    /* bagian code ini dari https://github.com/cefsharp/CefSharp/wiki/General-Usage#binding-an-async-object-in-javascript */
                    BindingOptions bindingOptions = null; //Binding options is an optional param, defaults to null
                    bindingOptions = BindingOptions.DefaultBinder; //Use the default binder to serialize values into complex objects
                    repo.NameConverter = null; //No CamelCase of Javascript Names
                                               //For backwards compatability reasons the default NameConverter doesn't change the case of the objects returned from methods calls.
                                               //https://github.com/cefsharp/CefSharp/issues/2442
                                               //Use the new name converter to bound object method names and property names of returned objects converted to camelCase
                    
                    jembatan.tambaTalinga(warunkMas);
                    repo.Register("JembatanSablaMinanga", jembatan, isAsync: true, options: bindingOptions);
                }
            };
        }

        //ambe tu data peta (reverse geo dgn latitude longitude) dari javascript
        private void warunkMas(string doraPePeta)
        {
            //Dictionary<string, object> tamparPipiKiri = JsonConvert.DeserializeObject<Dictionary<string, object>>(doraPePeta);

            //jalanSetapak = tamparPipiKiri["display"] as string;
            //latitude = (float)tamparPipiKiri["lat"];
            //longitude = (float)tamparPipiKiri["lng"];
        }
    }
}
