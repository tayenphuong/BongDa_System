using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestIIS.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password, string selectedServer)
        {
            // (Tuỳ bạn: có thể kiểm tra user ở DB, hoặc hardcode)
            if (username == "admin" && password == "123")
            {
                // Lưu server được chọn vào session
                Session["SelectedServer"] = selectedServer;
                Session["User"] = username;

                return RedirectToAction("Index", "CauThu");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}