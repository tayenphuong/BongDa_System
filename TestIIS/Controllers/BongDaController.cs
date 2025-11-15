using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class BongDaController : Controller
    {
        // GET: BongDa
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Cau1()
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau1 @clb = 'CLB1'; ";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Cau2(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau2 @hoten = N'Nguyễn Văn A';";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau3(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau3 @sandau = 'SD1';";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau4(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau4;";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau5(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau5 @hoten = N'Nguyễn Văn A', @trongtai = N'Trọng tài 1';";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau6(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau6 @hoten1 = N'Nguyễn Văn A', @hoten2 = N'Lê Văn D';";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau7(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau7;";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau8(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau8;";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau9(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau9 @madb = 'DB01';";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cau10(string clb)
        {
            DataModel db = new DataModel();
            String sql = $"EXEC sp_Cau10;";
            return Json(db.get(sql), JsonRequestBehavior.AllowGet);
        }
    }
}