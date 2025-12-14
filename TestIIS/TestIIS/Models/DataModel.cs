using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIIS.Models;

namespace TestIIS.Models
{
    public class DataModel
    {
        public string connectionString;
        public DataModel()
        {
            connectionString = GetConnectionString();
        }
        public string GetConnectionString()
        {
            string selectedServer = HttpContext.Current.Session["SelectedServer"] as string;

            // Nếu người dùng chưa chọn thì mặc định là SV1
            if (string.IsNullOrEmpty(selectedServer))
                selectedServer = "SV2";

            switch (selectedServer)
            {
                case "SV1":
                    return "Server=LAPTOP-PE87MKG0\\CSDLPT1;Database=BongDa;User Id=login1;Password=login123;Trusted_Connection=False;";
                case "SV2":
                    return "Server=LAPTOP-PE87MKG0\\CSDLPT2;Database=BongDa;User Id=login1;Password=login123;Trusted_Connection=False;";
                case "SV3":
                    return "Server=LAPTOP-PE87MKG0\\CSDLPT3;Database=BongDa;User Id=login1;Password=login123;Trusted_Connection=False;";
                default:
                    throw new Exception("Server không hợp lệ.");
            }
        }
        //public static string connectionString = "Server=LAPTOP-PE87MKG0\\CSDLPT2;Database=BongDa;User Id=login1; Password= login123; Trusted_Connection=false;";
        //public static string connectionString = "Data Source=10.21.1.55,80\\CSDLPT2;Initial Catalog=BongDa;User ID=login1;Password=login123";
        public ArrayList get(String sql)
        {
            ArrayList datalist = new ArrayList();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();

            using ( SqlDataReader r = command.ExecuteReader())
            {
                while (r.Read())
                {
                    ArrayList row = new ArrayList();
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        row.Add(xulydulieu(r.GetValue(i).ToString()));
                    }
                    datalist.Add(row);
                }

            }
            connection.Close();
            return datalist;
        }
        public string xulydulieu(string text)
        {
            String s = text.Replace(",", "&44;");
            s = s.Replace("\"", "&34;");
            s = s.Replace("'", "&39;");
            s = s.Replace("\r", "");
            s = s.Replace("\n", "");
            return s;
        }
    }
}