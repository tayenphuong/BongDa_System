using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Controllers
{
    public class CauThuController : Controller
    {
        //private string connectionString = DataModel.connectionString;

        private string connectionString = new DataModel().GetConnectionString();
        // GET: Hiển thị danh sách
        public bool IsPlayerInCurrentServer(int maCT)
        {
            using (SqlConnection con = new SqlConnection(Session["SelectedServer"] as string))
            {
                con.Open();
                string sql = "SELECT COUNT(1) FROM CauThu WHERE MaCT = @maCT";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@maCT", maCT);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // Trả về true nếu cầu thủ thuộc server hiện tại
                }
            }
        }
        public ActionResult Index(string keyword = "")
        {
            string selectedServer = Session["SelectedServer"] as string ?? "SV2";
            ViewBag.SelectedServer = selectedServer;
            ViewBag.IsSV1 = (selectedServer == "SV1");
            ViewBag.IsToanCuc = false;

            ArrayList data = new ArrayList();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = $"SELECT MaCT, HoTen, MaDB, '{selectedServer}' AS ServerNguon FROM CauThu";
                if (!string.IsNullOrEmpty(keyword))
                    sql += " WHERE MaCT LIKE @kw OR HoTen LIKE @kw OR MaDB LIKE @kw";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (!string.IsNullOrEmpty(keyword))
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ArrayList row = new ArrayList
                {
                    reader["MaCT"].ToString(),
                    reader["HoTen"].ToString(),
                    reader["MaDB"].ToString(),
                    reader["ServerNguon"].ToString() // thêm
                };
                        data.Add(row);
                    }
                }
            }
            return View(data);
        }

        public ActionResult ToanCuc()
        {
            string selectedServer = Session["SelectedServer"] as string ?? "SV2";
            ArrayList data = new ArrayList();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "";

                if (selectedServer == "SV2") 
                {
                    sql = @"
                    SELECT MaCT, HoTen, MaDB, 'SV2' AS ServerNguon FROM CauThu
                    UNION ALL
                    SELECT MaCT, HoTen, MaDB, 'SV3' AS ServerNguon FROM LINK.BongDa.dbo.CauThu";
                }
                else
                {
                    
                    sql = @"
                    SELECT MaCT, HoTen, MaDB, 'SV3' AS ServerNguon FROM CauThu
                    UNION ALL
                    SELECT MaCT, HoTen, MaDB, 'SV2' AS ServerNguon FROM LINK.BongDa.dbo.CauThu";
                }


                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ArrayList row = new ArrayList
            {
                reader["MaCT"].ToString(),
                reader["HoTen"].ToString(),
                reader["MaDB"].ToString(),
                reader["ServerNguon"].ToString()
            };
                    data.Add(row);
                }
            }
            // Flag: đang xem toàn cục
            ViewBag.IsToanCuc = true;
            ViewBag.SelectedServer = Session["SelectedServer"];
            ViewBag.IsSV1 = (selectedServer == "SV1");
            return View("Index", data);
        }


        // GET: Thêm mới
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string maCT, string hoTen, string maDB)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Bước 1: Kiểm tra trùng mã cầu thủ
                string checkSql = "SELECT COUNT(*) FROM CauThu WHERE MaCT = @maCT";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, con))
                {
                    checkCmd.Parameters.AddWithValue("@maCT", maCT);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["msg"] = "❌ Mã cầu thủ đã tồn tại!";
                        return RedirectToAction("Index");
                    }
                }

                // Bước 2: Nếu không trùng thì thêm mới
                string sql = "INSERT INTO CauThu (MaCT, HoTen, MaDB) VALUES (@maCT, @hoTen, @maDB)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@maCT", maCT);
                    cmd.Parameters.AddWithValue("@hoTen", hoTen);
                    cmd.Parameters.AddWithValue("@maDB", maDB);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["msg"] = "✅ Thêm cầu thủ thành công!";
            return RedirectToAction("Index");
        }

        // GET: Sửa
        public ActionResult Edit(string maCT)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM CauThu WHERE MaCT = @maCT";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@maCT", maCT);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ViewBag.MaCT = reader["MaCT"].ToString();
                        ViewBag.HoTen = reader["HoTen"].ToString();
                        ViewBag.MaDB = reader["MaDB"].ToString();
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Edit(string maCT, string hoTen, string maDB)
        {
            
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string checkSql = "SELECT MaDB FROM CauThu WHERE MaCT = @maCT";
                using (SqlCommand cmd = new SqlCommand(checkSql, con))
                {
                    cmd.Parameters.AddWithValue("@maCT", maCT);
                    var serverOfPlayer = cmd.ExecuteScalar() as string;
                    if (string.IsNullOrEmpty(serverOfPlayer))
                    {
                        TempData["msg"] = "⚠️ Không tìm thấy cầu thủ hoặc dữ liệu không hợp lệ!";
                        return RedirectToAction("Index");
                    }
                    
                }


                string sql = "UPDATE CauThu SET HoTen = @hoTen, MaDB = @maDB WHERE MaCT = @maCT";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@hoTen", hoTen);
                    cmd.Parameters.AddWithValue("@maDB", maDB);
                    cmd.Parameters.AddWithValue("@maCT", maCT);
                    cmd.ExecuteNonQuery();
                }
            }
            TempData["msg"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // GET: Xóa
        public ActionResult Delete(string maCT)
        {
          
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string checkSql = "SELECT MaDB FROM CauThu WHERE MaCT = @maCT";
                    using (SqlCommand cmd = new SqlCommand(checkSql, con))
                    {
                        cmd.Parameters.AddWithValue("@maCT", maCT);
                        var serverOfPlayer = cmd.ExecuteScalar() as string;
                        if (string.IsNullOrEmpty(serverOfPlayer))
                        {
                            TempData["msg"] = "⚠️ Không tìm thấy cầu thủ hoặc dữ liệu không hợp lệ!";
                            return RedirectToAction("Index");
                        }
                        
                    }

                    string sql = "DELETE FROM CauThu WHERE MaCT = @maCT";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@maCT", maCT);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["msg"] = "Xóa cầu thủ thành công!";
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("REFERENCE constraint"))
                    TempData["msg"] = "⚠️ Cầu thủ còn tham gia trận đấu, không thể xóa!";
                else
                    TempData["msg"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
       
        public ActionResult Details2(string maCT)
        {
            string selectedServer = Session["SelectedServer"] as string;
            bool isSV1 = (selectedServer == "SV1");
            if (string.IsNullOrEmpty(maCT))
                return HttpNotFound();

            var model = new CauThuDetailViewModel();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // --- Gọi stored procedure sp_ThongTinCauThu ---
                using (SqlCommand cmd = new SqlCommand("sp_ThongTinCauThu", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaCT", maCT);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.MaCT = reader["MaCT"].ToString();
                            model.HoTen = reader["HoTen"].ToString();
                            model.MaDB = reader["MaDB"].ToString();
                            model.SoTranDaThamGia = Convert.ToInt32(reader["SoTranDaThamGia"]);
                            model.TongBanThang = Convert.ToInt32(reader["TongBanThang"]);
                        }
                    }
                }

                // --- Gọi thêm danh sách các trận đấu cầu thủ đã tham gia ---
                string sqlTran = "";

                if (isSV1)
                {
                    // SV1: chỉ lấy dữ liệu local
                    sqlTran = @"
                    SELECT TG.MaTD, TG.SoTrai, TD.MaDB1, TD.MaDB2, TD.TrongTai, TD.SanDau
                    FROM ThamGia TG
                    JOIN TranDau TD ON TG.MaTD = TD.MaTD
                    WHERE TG.MaCT = @maCT";
                            }
                            else
                            {
                                // SV con: lấy cả local + LINK
                                sqlTran = @"
                    SELECT TG.MaTD, TG.SoTrai, TD.MaDB1, TD.MaDB2, TD.TrongTai, TD.SanDau
                    FROM (
                        SELECT * FROM ThamGia
                        UNION ALL
                        SELECT * FROM LINK.BongDa.dbo.ThamGia
                    ) TG
                    JOIN (
                        SELECT * FROM TranDau
                        UNION ALL
                        SELECT * FROM LINK.BongDa.dbo.TranDau
                    ) TD ON TG.MaTD = TD.MaTD
                    WHERE TG.MaCT = @maCT";
                }

                using (SqlCommand cmd = new SqlCommand(sqlTran, con))
                {
                    cmd.Parameters.AddWithValue("@maCT", maCT);
                    SqlDataReader reader = cmd.ExecuteReader();

                    var tranList = new List<TranDauThamGiaVM>();
                    while (reader.Read())
                    {
                        tranList.Add(new TranDauThamGiaVM
                        {
                            MaTD = reader["MaTD"].ToString(),
                            SoTrai = Convert.ToInt32(reader["SoTrai"]),
                            MaDB1 = reader["MaDB1"].ToString(),
                            MaDB2 = reader["MaDB2"].ToString(),
                            TrongTai = reader["TrongTai"].ToString(),
                            SanDau = reader["SanDau"].ToString()
                        });
                    }
                    model.TranThamGia = tranList;
                }
            }

            return View(model);
        }


        [HttpGet]
         public ActionResult ChuyenCauThu(string maCT)
    {
        if (string.IsNullOrEmpty(maCT))
            return HttpNotFound();

        var model = new ChuyenCauThuViewModel
        {
            MaCT = maCT,
            DoiBongMoiList = GetDoiBongTuLink() // Lấy danh sách đội bên mảnh LINK
        };

        return View(model);
    }

    // POST: CauThu/ChuyenCauThu
    [HttpPost]
    public ActionResult ChuyenCauThu(string maCT, string maDBMoi)
    {
        if (string.IsNullOrEmpty(maCT) || string.IsNullOrEmpty(maDBMoi))
        {
            ViewBag.Message = "Thiếu thông tin cầu thủ hoặc đội bóng mới.";
            return View();
        }
        
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                    string checkSql = "SELECT MaDB FROM CauThu WHERE MaCT = @maCT";
                    using (SqlCommand cmd = new SqlCommand(checkSql, con))
                    {
                        cmd.Parameters.AddWithValue("@maCT", maCT);
                        var serverOfPlayer = cmd.ExecuteScalar() as string;
                          if (string.IsNullOrEmpty(serverOfPlayer))
                        {
                            TempData["msg"] = "⚠️ Không tìm thấy cầu thủ hoặc dữ liệu không hợp lệ!";
                            return RedirectToAction("Index");
                        }
                       
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_ChuyenCauThu", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaCT", maCT);
                        cmd.Parameters.AddWithValue("@MaDB_Moi", maDBMoi);
                        cmd.ExecuteNonQuery();
                    }
            }

            TempData["msg"] = $"✅ Đã chuyển cầu thủ {maCT} sang đội {maDBMoi} thành công!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.Message = "❌ Lỗi khi chuyển cầu thủ: " + ex.Message;
            return View();
        }
    }

    // Hàm phụ: Lấy danh sách đội bóng từ mảnh LINK
    private List<SelectListItem> GetDoiBongTuLink()
    {
        var list = new List<SelectListItem>();
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                // ⚠️ Lấy danh sách đội bóng từ LINK (mảnh khác)
                string sql = "SELECT MaDB, TenDB FROM LINK.BongDa.dbo.DoiBong";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = reader["MaDB"].ToString(),
                                Text = reader["TenDB"].ToString()
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            list.Add(new SelectListItem
            {
                Value = "",
                Text = "⚠️ Không thể kết nối tới mảnh LINK: " + ex.Message
            });
        }

        return list;
    }






    }
}
