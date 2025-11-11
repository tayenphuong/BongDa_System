using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class chitietController : Controller
    {
        // GET: chitiet
        public JsonResult Index(int ?id)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC DULIEUCHITIETHOADON {id};";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
    }
}