using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestIIS.Models
{
    public class DoiBongDetailsViewModel
    {
        // Thông tin đội bóng
        public string MaDB { get; set; }
        public string TenDB { get; set; }
        public string CLB { get; set; }

        // Dashboard
        public int SoCauThu { get; set; }
        public int SoTranDaDa { get; set; }
        public int TongBanThang { get; set; }

        public string VuaPhaLuoi { get; set; }
        public int BanThangCaoNhat { get; set; }
    }
}