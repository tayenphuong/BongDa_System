using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class hoadonController : Controller
    {
        // GET: hoadon
        public JsonResult Index()
        {
            DataModel db = new DataModel();
            String sql = "EXEC DULIEUHOADON;";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
    }
}