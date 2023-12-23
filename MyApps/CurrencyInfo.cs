using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApps
{
    internal class CurrencyInfo
    {
        private string _Currency { get; set; }
        private float _BuyingRate { get; set; }
        private float _SellingRate { get; set; }

        public CurrencyInfo(string currency, float buyingRate, float sellingRate) {
            _Currency = currency;
            _BuyingRate = buyingRate;
            _SellingRate = sellingRate;
        }
    }
}
