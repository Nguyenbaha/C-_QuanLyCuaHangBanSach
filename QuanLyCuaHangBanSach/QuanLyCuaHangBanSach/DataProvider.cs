using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangBanSach
{
    public class DataProvider
    {
        private string connectString = @"Data Source=DESKTOP-LAPTOPA;Initial Catalog=db_quan_ly_ban_sach;Integrated Security=True";

        public DataTable execQuery(string query)
        {
            DataTable data = new DataTable();
            using (SqlConnection con = new SqlConnection(connectString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(data);

                con.Close();
            }
            return data;
        }

        public int execNonQuery(string query)
        {
            int data = 0;
            using (SqlConnection con = new SqlConnection(connectString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);

                data = cmd.ExecuteNonQuery();

                con.Close();

            }
            return data;
        }

        public object execScaler(string query)
        {
            object data = 0;
            using (SqlConnection con = new SqlConnection(connectString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);

                data = cmd.ExecuteScalar();

                con.Close();

            }
            return data;
        }
    }
}
