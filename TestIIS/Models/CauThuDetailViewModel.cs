using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestIIS.Models
{
    public class CauThuDetailViewModel
    {
        public string MaCT { get; set; }
        public string HoTen { get; set; }
        public string MaDB { get; set; }
        public int SoTranDaThamGia { get; set; }
        public int TongBanThang { get; set; }
        public List<TranDauThamGiaVM> TranThamGia { get; set; }
    }

    public class TranDauThamGiaVM
    {
        public string MaTD { get; set; }
        public int SoTrai { get; set; }
        public string MaDB1 { get; set; }
        public string MaDB2 { get; set; }
        public string TrongTai { get; set; }
        public string SanDau { get; set; }
    }

}