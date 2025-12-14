using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class DoiBongController : Controller
    {
        private string connectionString = new DataModel().GetConnectionString();

        // -------------------------------
        //  INDEX – HIỂN THỊ DANH SÁCH
        // -------------------------------
        public ActionResult Index()
        {
            string selectedServer = Session["SelectedServer"] as string ?? "SV2";
            ViewBag.SelectedServer = selectedServer;
            ViewBag.IsSV1 = (selectedServer == "SV1");
            ViewBag.IsToanCuc = false;
            try
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
                }
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
                return View(new List<DoiBong>());
            }
        }

        // -------------------------------
        //   HIỂN THỊ TOÀN CỤC
        // -------------------------------
        public ActionResult ToanCuc()
        {
            string selectedServer = Session["SelectedServer"] as string ?? "SV2";
            try
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
                }
                ViewBag.IsToanCuc = true;
                ViewBag.SelectedServer = Session["SelectedServer"];
                ViewBag.IsSV1 = (selectedServer == "SV1");
                return View("Index", list);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View("Index", new List<DoiBong>());
            }
        }

        // -------------------------------
        //   GET: CREATE
        // -------------------------------
        public ActionResult Create()
        {
            return View();
        }

        // -------------------------------
        //   POST: CREATE 
        // -------------------------------
        [HttpPost]
        public ActionResult Create(DoiBong db)
        {
            try
            {
                string selectedServer = Session["SelectedServer"] as string;
                bool isSV1 = (selectedServer == "SV1");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    
                    string checkSql = "";
                    if (isSV1)
                    {
                        checkSql = "SELECT COUNT(*) FROM DoiBong WHERE MaDB = @MaDB";
                    }
                    else
                    {
                        checkSql = @"
                            SELECT COUNT(*) 
                            FROM (
                                SELECT MaDB FROM DoiBong WHERE MaDB = @MaDB
                                UNION
                                SELECT MaDB FROM LINK.BongDa.dbo.DoiBong WHERE MaDB = @MaDB
                            ) AS T";
                    }

                    SqlCommand checkCmd = new SqlCommand(checkSql, con);
                    checkCmd.Parameters.AddWithValue("@MaDB", db.MaDB);

                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        TempData["msg"] = "❌ Mã đội bóng đã tồn tại!";
                        return RedirectToAction("Index");
                    }

                    // INSERT đội bóng
                    string sql = @"INSERT INTO DoiBong (MaDB, TenDB, CLB)
                                   VALUES (@MaDB, @TenDB, @CLB)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDB", db.MaDB);
                    cmd.Parameters.AddWithValue("@TenDB", db.TenDB);
                    cmd.Parameters.AddWithValue("@CLB", db.CLB);

                    cmd.ExecuteNonQuery();
                }

                TempData["msg"] = "✔ Thêm đội bóng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "❌ Lỗi thêm đội bóng: " + ex.Message;
                return View(db);
            }
        }

        // -------------------------------
        //   GET: EDIT
        // -------------------------------
        public ActionResult Edit(string id)
        {
            try
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
                }

                return View(db);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Lỗi tải dữ liệu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // -------------------------------
        //   POST: EDIT
        // -------------------------------
        [HttpPost]
        public ActionResult Edit(DoiBong db)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string sql = @"UPDATE DoiBong 
                                   SET TenDB = @TenDB, CLB = @CLB 
                                   WHERE MaDB = @MaDB";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDB", db.MaDB);
                    cmd.Parameters.AddWithValue("@TenDB", db.TenDB);
                    cmd.Parameters.AddWithValue("@CLB", db.CLB);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["msg"] = "✔ Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "❌ Lỗi cập nhật: " + ex.Message;
                return View(db);
            }
        }

        // -------------------------------
        //   DELETE (KIỂM TRA RÀNG BUỘC)
        // -------------------------------
        public ActionResult Delete(string id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // ❗ Kiểm tra xem đội bóng có nằm trong bảng TranDau không
                    string checkSql = @"
                        SELECT COUNT(*) 
                        FROM TranDau 
                        WHERE MaDB1 = @MaDB OR MaDB2 = @MaDB";

                    SqlCommand checkCmd = new SqlCommand(checkSql, con);
                    checkCmd.Parameters.AddWithValue("@MaDB", id);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        TempData["msg"] = "❌ Không thể xóa! Đội bóng đang tham gia trận đấu.";
                        return RedirectToAction("Index");
                    }

                    // Delete
                    string sql = "DELETE FROM DoiBong WHERE MaDB = @MaDB";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDB", id);

                    cmd.ExecuteNonQuery();
                }

                TempData["msg"] = "🗑 Đã xóa đội bóng!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "❌ Lỗi xóa: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();

            var model = new DoiBongDetailsViewModel();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // LẤY THÔNG TIN ĐỘI BÓNG
                string sqlDB = "SELECT MaDB, TenDB, CLB FROM DoiBong WHERE MaDB = @id";
                using (SqlCommand cmd = new SqlCommand(sqlDB, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        model.MaDB = rd["MaDB"].ToString();
                        model.TenDB = rd["TenDB"].ToString();
                        model.CLB = rd["CLB"].ToString();
                    }
                    rd.Close();
                }

                // 1️⃣ Tổng số cầu thủ của đội
                string sqlCountCT = "SELECT COUNT(*) FROM CauThu WHERE MaDB = @id";
                using (SqlCommand cmd = new SqlCommand(sqlCountCT, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    model.SoCauThu = (int)cmd.ExecuteScalar();
                }

                // 2️⃣ Tổng số trận đã đá
                string sqlSoTran = @"
            SELECT COUNT(*) FROM TranDau 
            WHERE MaDB1 = @id OR MaDB2 = @id";
                using (SqlCommand cmd = new SqlCommand(sqlSoTran, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    model.SoTranDaDa = (int)cmd.ExecuteScalar();
                }

                // 3️⃣ Tổng số bàn thắng (dựa theo cầu thủ)
                string sqlTongBan = @"
                    SELECT SUM(TG.SoTrai)
                    FROM ThamGia TG
                    JOIN CauThu CT ON TG.MaCT = CT.MaCT
                    WHERE CT.MaDB = @id";
                using (SqlCommand cmd = new SqlCommand(sqlTongBan, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    model.TongBanThang = cmd.ExecuteScalar() != DBNull.Value
                        ? Convert.ToInt32(cmd.ExecuteScalar())
                        : 0;
                }

                // 4️⃣ Cầu thủ ghi bàn nhiều nhất (vua phá lưới)
                string sqlVP = @"
                    SELECT TOP 1 CT.HoTen, SUM(TG.SoTrai) AS Ban
                    FROM CauThu CT
                    JOIN ThamGia TG ON CT.MaCT = TG.MaCT
                    WHERE CT.MaDB = @id
                    GROUP BY CT.HoTen
                    ORDER BY Ban DESC";
                using (SqlCommand cmd = new SqlCommand(sqlVP, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        model.VuaPhaLuoi = rd["HoTen"].ToString();
                        model.BanThangCaoNhat = Convert.ToInt32(rd["Ban"]);
                    }
                    else
                    {
                        model.VuaPhaLuoi = "Chưa có dữ liệu";
                        model.BanThangCaoNhat = 0;
                    }
                    rd.Close();
                }
            }

            return View(model);
        }

    }
}
