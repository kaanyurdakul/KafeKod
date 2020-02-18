﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
    public class SiparisDetay
    {
        public string UrunAd { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Adet { get; set; }

        public decimal Tutar() => Adet * BirimFiyat;
    }
}
