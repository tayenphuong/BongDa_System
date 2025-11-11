using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class sanphamController : Controller
    {
        // GET: sanpham
        public JsonResult Index()
        {
            DataModel db = new DataModel();
            String sql = "EXEC DULIEUSANPHAM;";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau2_1(string id)
        {
            if (id == null || id == "")
            {
                return Json("khong co id", JsonRequestBehavior.AllowGet); ;
            }
            DataModel db = new DataModel();
            string sql = "EXEC SP_Cau2_1 " + id;
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau2_2(string id)
        {
            if (id == null || id == "")
            {
                return Json("khong co id", JsonRequestBehavior.AllowGet); ;
            }
            DataModel db = new DataModel();
            string sql = "EXEC SP_Cau2_2 " + id;
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Cau2_3(string id)
        {
            if (id == null || id == "")
            {
                return Json("khong co id", JsonRequestBehavior.AllowGet); ;
            }
            DataModel db = new DataModel();
            string sql = "EXEC SP_Cau2_3 " + id;
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }


    }
}