using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class TranDauController : Controller
    {
        private string connectionString = new DataModel().GetConnectionString();

        // -------------------------------
        // 🔐 Chỉ server toàn cục mới được tạo trận
        // -------------------------------
        private bool IsMainServer()
        {
            string sv = System.Web.HttpContext.Current.Session["SelectedServer"] as string;
            return sv == "SV1";    // chỉ MAIN có quyền  
        }

        // -------------------------------
        // 📌 Hiển thị danh sách trận đấu
        // -------------------------------
        public ActionResult Index()
        {
            ArrayList data = new ArrayList();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"SELECT MaTD, MaDB1, MaDB2, TrongTai, SanDau FROM TranDau";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        ArrayList row = new ArrayList
                        {
                            rd["MaTD"].ToString(),
                            rd["MaDB1"].ToString(),
                            rd["MaDB2"].ToString(),
                            rd["TrongTai"].ToString(),
                            rd["SanDau"].ToString()
                        };

                        data.Add(row);
                    }
                }
            }

            // Gửi quyền tạo trận sang View
            ViewBag.IsMainServer = IsMainServer();

            return View(data);
        }

        // -------------------------------
        // 📌 Create (GET)
        // -------------------------------
        public ActionResult Create()
        {
            if (!IsMainServer())
                return new HttpStatusCodeResult(403, "Chỉ server toàn cục mới được tạo trận đấu.");

            ViewBag.ListDB = LoadDoiBong();
            return View();
        }

        // -------------------------------
        // 📌 Create (POST)
        // -------------------------------
        [HttpPost]
        public ActionResult Create(string MaTD, string MaDB1, string MaDB2, string TrongTai, string SanDau)
        {
            if (!IsMainServer())
                return new HttpStatusCodeResult(403, "Bạn không có quyền thực hiện thao tác này.");

            // RÀNG BUỘC 2 ĐỘI PHẢI KHÁC NHAU
            if (MaDB1 == MaDB2)
            {
                ViewBag.Error = "Hai đội bóng phải khác nhau!";
                ViewBag.ListDB = LoadDoiBong();   // gửi list đội bóng
                return View();
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"INSERT INTO TranDau (MaTD, MaDB1, MaDB2, TrongTai, SanDau)
                               VALUES (@MaTD, @MaDB1, @MaDB2, @TrongTai, @SanDau)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTD", MaTD);
                cmd.Parameters.AddWithValue("@MaDB1", MaDB1);
                cmd.Parameters.AddWithValue("@MaDB2", MaDB2);
                cmd.Parameters.AddWithValue("@TrongTai", TrongTai);
                cmd.Parameters.AddWithValue("@SanDau", SanDau);

                try
                {
                    cmd.ExecuteNonQuery();
                    TempData["msg"] = "Tạo trận đấu thành công!";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi: " + ex.Message;
                    ViewBag.ListDB = LoadDoiBong();   // gửi list đội bóng
                    return View();
                }
            }

            return RedirectToAction("Index");
        }

        // -------------------------------
        // 📌 Lấy danh sách đội bóng (ADO.NET)
        // -------------------------------
        private List<DoiBong> LoadDoiBong()
        {
            List<DoiBong> list = new List<DoiBong>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT MaDB, TenDB FROM DoiBong";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    list.Add(new DoiBong
                    {
                        MaDB = rd["MaDB"].ToString(),
                        TenDB = rd["TenDB"].ToString()
                    });
                }
            }

            return list;
        }

        public ActionResult Details(string id)
        {
            if (id == null) return HttpNotFound();

            ArrayList row = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"SELECT MaTD, MaDB1, MaDB2, TrongTai, SanDau 
                       FROM TranDau WHERE MaTD = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    row = new ArrayList
            {
                rd["MaTD"].ToString(),
                rd["MaDB1"].ToString(),
                rd["MaDB2"].ToString(),
                rd["TrongTai"].ToString(),
                rd["SanDau"].ToString()
            };
                }
            }

            if (row == null) return HttpNotFound();

            return View(row);
        }

        // -------------------------------
        // 📌 Edit (GET)
        // -------------------------------
        public ActionResult Edit(string id)
        {
            if (!IsMainServer())
                return new HttpStatusCodeResult(403, "Chỉ server toàn cục mới được sửa trận.");

            if (id == null)
                return HttpNotFound();

            ArrayList row = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"SELECT MaTD, MaDB1, MaDB2, TrongTai, SanDau 
                       FROM TranDau WHERE MaTD = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    row = new ArrayList
            {
                rd["MaTD"].ToString(),
                rd["MaDB1"].ToString(),
                rd["MaDB2"].ToString(),
                rd["TrongTai"].ToString(),
                rd["SanDau"].ToString()
            };
                }
            }

            if (row == null)
                return HttpNotFound();

            ViewBag.ListDB = LoadDoiBong(); // gửi DS đội bóng

            return View(row);
        }

        // -------------------------------
        // 📌 Edit (POST)
        // -------------------------------
        [HttpPost]
        public ActionResult Edit(string MaTD, string MaDB1, string MaDB2, string TrongTai, string SanDau)
        {
            if (!IsMainServer())
                return new HttpStatusCodeResult(403, "Bạn không có quyền sửa trận đấu.");

            // Ràng buộc 2 đội không được trùng nhau
            if (MaDB1 == MaDB2)
            {
                ViewBag.Error = "Hai đội bóng phải khác nhau!";
                ViewBag.ListDB = LoadDoiBong();
                return View(new ArrayList { MaTD, MaDB1, MaDB2, TrongTai, SanDau });
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"UPDATE TranDau
                       SET MaDB1 = @MaDB1,
                           MaDB2 = @MaDB2,
                           TrongTai = @TrongTai,
                           SanDau = @SanDau
                       WHERE MaTD = @MaTD";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@MaTD", MaTD);
                cmd.Parameters.AddWithValue("@MaDB1", MaDB1);
                cmd.Parameters.AddWithValue("@MaDB2", MaDB2);
                cmd.Parameters.AddWithValue("@TrongTai", TrongTai);
                cmd.Parameters.AddWithValue("@SanDau", SanDau);

                try
                {
                    cmd.ExecuteNonQuery();
                    TempData["msg"] = "Cập nhật trận đấu thành công!";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi: " + ex.Message;
                    ViewBag.ListDB = LoadDoiBong();
                    return View(new ArrayList { MaTD, MaDB1, MaDB2, TrongTai, SanDau });
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM TranDau WHERE MaTD = @MaTD";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTD", id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            TempData["msg"] = "🗑️ Đã hủy trận đấu!";
            return RedirectToAction("Index");
        }


    }
}
