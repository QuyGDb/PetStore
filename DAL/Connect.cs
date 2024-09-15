using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class Connect
    {
        public static string connString = @"Data Source=PHUCY\SQLEXPRESS;Initial Catalog=QL_PetStore;Persist Security Info=True;Integrated Security=True";
        private static SqlConnection conn = null;

        public Connect()
        {
            conn = new SqlConnection(connString);
            conn.Open();
        }

        public static bool OpenConnect()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                return false;
            }
            else
                return true;
        }

        public static void CloseConnect()
        {
            conn.Close();
        }

        public DataTable QuerySQL(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            return dt;
        }

        public void ExecuteSQL(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public string ConnectString()
        {
            return connString;
        }
    }
}
