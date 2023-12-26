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
using System.Globalization;

namespace MyApps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CurrencyInfo> currencyList = new List<CurrencyInfo>();

        public MainWindow()
        {
            InitializeComponent();

            //hnb tečajna lista          
        }

        public float GetFloat(JToken value)
        {
            float result;

            // Try parsing in the current culture
            if (!float.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                // Then try in US english
                !float.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then in neutral language
                !float.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = 1;
            }
            return result;
        }
        public void GetConversionRates()
        {
            try
            {
                List<string> currencies = new List<string>();
                var url = "https://api.hnb.hr/tecajn-eur/v3";
                JArray parsedResponse;
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();


                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseJson = streamReader.ReadToEnd();
                    parsedResponse = JArray.Parse(responseJson);
                }
                foreach (var currency in parsedResponse)
                {
                    currencyList.Add(new CurrencyInfo((string)currency["valuta"], GetFloat(currency["kupovni_tecaj"]), (float)currency["prodajni_tecaj"]));
                    currencies.Add(currency["valuta"].ToString());
                }
                currencies.Add("EUR");
                currencies.Sort();
                foreach (string cur in currencies)
                {
                    cbFromCurrency.Items.Add(cur);
                    cbToCurrency.Items.Add(cur);
                }

                //Update label info
                lblUpdateInfo.Content = $"Conversion rates updated {DateTime.Now.ToString("G")}";
            }
            catch (WebException)
            {

                MessageBox.Show("Failed to load currency rate. Using outdated rates. Check your internet connection. ");
            }
        }
        public void OnLoad(object sender, RoutedEventArgs e)
        {
            GetConversionRates();
        }
        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            float amount;
            string toCurrency, fromCurrency;
            try
            {
                amount = float.Parse(txtConvertAmount.Text);
                toCurrency = (string)cbToCurrency.SelectedItem;
                fromCurrency = (string)cbFromCurrency.SelectedItem;

                if (FindConversionRate(fromCurrency, toCurrency) > 0)
                {
                    lblConversion.Content = $"{amount} {fromCurrency} is {Math.Round(FindConversionRate(fromCurrency, toCurrency) * amount, 2)} {toCurrency}";
                }
            }
            catch (FormatException)
            {

                MessageBox.Show("Entered amount is invalid. Please check it and retry.");
            }
        }

        public float FindConversionRate(string fromCurrency, string toCurrency)
        {

            if (fromCurrency != "EUR")
            {
                float fromBuyingRate;
                float toBuyingRate;
                if (fromCurrency == null || toCurrency == null)
                {
                    MessageBox.Show("Please select both From and To currency!");
                    return 0;
                }
                else
                {
                    fromBuyingRate = currencyList.Find(x => x.Currency == fromCurrency).BuyingRate;
                    toBuyingRate = currencyList.Find(x => x.Currency == toCurrency).BuyingRate;

                }

                if (fromBuyingRate > toBuyingRate)
                {
                    return ((1 / fromBuyingRate)) * toBuyingRate;
                }
                else if (fromBuyingRate < toBuyingRate)
                {
                    return ((1 / fromBuyingRate) * toBuyingRate);
                }
                else
                {
                    return 1;
                }
            }
            else //if fromCurrency was EUR just match it with proper conversion rate
            {
                return currencyList.Find(x => x.Currency == toCurrency).BuyingRate;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            currencyList.Clear();
            cbFromCurrency.Items.Clear();
            cbToCurrency.Items.Clear();
            GetConversionRates();
        }
    }
}
