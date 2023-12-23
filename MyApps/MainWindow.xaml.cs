using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;

namespace MyApps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //hnb tečajna lista
            try
            {
                var url = "https://api.hnb.hr/tecajn-eur/v3";
                JArray parsedResponse;
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                List<CurrencyInfo> currencyList = new List<CurrencyInfo>();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseJson = streamReader.ReadToEnd();
                    parsedResponse = JArray.Parse(responseJson);
                }
                foreach (var currency in parsedResponse)
                {
                    currencyList.Add(new CurrencyInfo((string)currency["valuta"], (float)currency["kupovni_tecaj"], (float)currency["prodajni_tecaj"]));
                }
            }
            catch (WebException)
            {

                MessageBox.Show("Failed to load currency rate. Using outdated rates. Check your internet connection. ");
            }          
        }       
    }
}
