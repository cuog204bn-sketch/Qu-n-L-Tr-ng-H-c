using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class DatabaseHelper
{
    private static string connectionString = ConfigurationManager.ConnectionStrings["QLTruongHocConnectionString"].ConnectionString;

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }

    public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi ExecuteQuery: {ex.Message}");
            return null; // Trả về null nếu có lỗi
        }
        return dt; // Trả về DataTable rỗng nếu không có dữ liệu
    }

    public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
    {
        int result = 0;
        using (SqlConnection conn = GetConnection())
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            result = cmd.ExecuteNonQuery();
        }
        return result;
    }
    public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
    {
        object result = null;
        using (SqlConnection conn = GetConnection())
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            if (parameters != null)
            {
            
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            result = cmd.ExecuteScalar();
        }
        return result;
    }
}