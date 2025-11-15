using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace TestIIS.Models
{
    public class ChuyenCauThuViewModel
    {
        public string MaCT { get; set; }
        public string MaDBMoi { get; set; }
        public List<SelectListItem> DoiBongMoiList { get; set; }
    }
}