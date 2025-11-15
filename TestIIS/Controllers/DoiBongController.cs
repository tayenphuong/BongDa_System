using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class DoiBongController : Controller
    {
        //string connectionString = DataModel.connectionString;
        private string connectionString = new DataModel().GetConnectionString();
        // 🟩 Hiển thị danh sách đội bóng
        public ActionResult Index()
        {
            List<DoiBong> list = new List<DoiBong>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT MaDB, TenDB, CLB FROM DoiBong";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DoiBong
                    {
                        MaDB = reader["MaDB"].ToString(),
                        TenDB = reader["TenDB"].ToString(),
                        CLB = reader["CLB"].ToString(),
                       
                    });
                }
                con.Close();
            }
            return View(list);
        }
        public ActionResult ToanCuc()
        {
            List<DoiBong> list = new List<DoiBong>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT MaDB, TenDB, CLB FROM DoiBong
                    UNION
                    SELECT MaDB, TenDB, CLB FROM LINK.BongDa.dbo.DoiBong";
                   
                
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DoiBong
                    {
                        MaDB = reader["MaDB"].ToString(),
                        TenDB = reader["TenDB"].ToString(),
                        CLB = reader["CLB"].ToString(),

                    });
                }

                con.Close();
            }
                ViewBag.ToanCuc = true; // để hiển thị tiêu đề khác
            return View("Index", list); // Dùng lại view Index
        }

        // 🟦 GET: Tạo mới
        public ActionResult Create()
        {
            return View();
        }

        // 🟦 POST: Tạo mới
        [HttpPost]
        public ActionResult Create(DoiBong db)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    // Kiểm tra trùng mã
                    string check = "SELECT COUNT(*) FROM DoiBong WHERE MaDB = @MaDB";
                    SqlCommand checkCmd = new SqlCommand(check, con);
                    checkCmd.Parameters.AddWithValue("@MaDB", db.MaDB);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["msg"] = " Mã đội bóng đã tồn tại!";
                        return RedirectToAction("Index");
                    }

                    string sql = "INSERT INTO DoiBong (MaDB, TenDB, CLB) VALUES (@MaDB, @TenDB, @HLV)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDB", db.MaDB);
                    cmd.Parameters.AddWithValue("@TenDB", db.TenDB);
                    cmd.Parameters.AddWithValue("@HLV", db.CLB);
                  
                    cmd.ExecuteNonQuery();
                }
                TempData["msg"] = " Thêm đội bóng thành công!";
                return RedirectToAction("Index");
            }
            return View(db);
        }

        // 🟨 GET: Sửa
        public ActionResult Edit(string id)
        {
            DoiBong db = new DoiBong();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DoiBong WHERE MaDB = @MaDB";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDB", id);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    db.MaDB = r["MaDB"].ToString();
                    db.TenDB = r["TenDB"].ToString();
                    db.CLB = r["CLB"].ToString();
                   
                }
                con.Close();
            }
            return View(db);
        }

        // 🟨 POST: Sửa
        [HttpPost]
        public ActionResult Edit(DoiBong db)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "UPDATE DoiBong SET TenDB = @TenDB, CLB = @CLB WHERE MaDB = @MaDB";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDB", db.MaDB);
                cmd.Parameters.AddWithValue("@TenDB", db.TenDB);
                cmd.Parameters.AddWithValue("@CLB", db.CLB);
               
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            TempData["msg"] = "✅ Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // 🟥 Xóa
        public ActionResult Delete(string id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM DoiBong WHERE MaDB = @MaDB";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDB", id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            TempData["msg"] = "🗑️ Đã xóa đội bóng!";
            return RedirectToAction("Index");
        }
    }
}
