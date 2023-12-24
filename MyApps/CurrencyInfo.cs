using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApps
{
    internal class CurrencyInfo
    {
        public string Currency { get; set; }
        public float BuyingRate { get; set; }
        public float SellingRate { get; set; }

        public CurrencyInfo(string currency, float buyingRate, float sellingRate) {
            Currency = currency;
            BuyingRate = buyingRate;
            SellingRate = sellingRate;
        }
    }
}
