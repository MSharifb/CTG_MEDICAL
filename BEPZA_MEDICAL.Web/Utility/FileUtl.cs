using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class FileUtl
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString ();
        }

        private static void OpenConnection(SqlConnection connection)
        {
            connection.ConnectionString = GetConnectionString();
            connection.Open();
        }

        // Get the list of the files in the database
        public static DataTable GetFileList()
        {
            DataTable fileList = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT ID, Name, ContentType, Size FROM Files";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = cmd;
                adapter.Fill(fileList);

                connection.Close();
            }

            return fileList;
        }

        // Save a file into the database
        public static void SaveFile(string name, string contentType,
            int size, byte[] data)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                string commandText = "INSERT INTO Files VALUES(@Name, @ContentType, ";
                commandText = commandText + "@Size, @Data)";
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@ContentType", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@size", SqlDbType.Int);
                cmd.Parameters.Add("@Data", SqlDbType.VarBinary);

                cmd.Parameters["@Name"].Value = name;
                cmd.Parameters["@ContentType"].Value = contentType;
                cmd.Parameters["@size"].Value = size;
                cmd.Parameters["@Data"].Value = data;
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        // Get a file from the database by ID
        public static DataTable GetAFile(int id)
        {
            DataTable file = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT ID, Name, ContentType, Size, Data FROM Files "
                    + "WHERE ID=@ID";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = id;

                adapter.SelectCommand = cmd;
                adapter.Fill(file);

                connection.Close();
            }

            return file;
        }
    }

    public class EmpFileUtl
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
        }

        private static void OpenConnection(SqlConnection connection)
        {
            connection.ConnectionString = GetConnectionString();
            connection.Open();
        }

        // Get the list of the files in the database
        public static DataTable GetFileList()
        {
            DataTable fileList = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT *  FROM PRM_EmpAttachmentFile";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = cmd;
                adapter.Fill(fileList);

                connection.Close();
            }

            return fileList;
        }

        // Save a file into the database
        public static void SaveFile(int EmployeeId, int AttachmentTypeId, string FileName, string Description,string OriginalFileName, string contentType,
            int size, byte[] data, string IUser)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd = new SqlCommand("INSERT INTO PRM_EmpAttachmentFile (EmployeeId,AttachmentTypeId,FileName,Description,OriginalFileName,ContentType,size,Attachment,IUser,IDate) VALUES (@EmployeeId, @AttachmentTypeId, @FileName,@Description,@OriginalFileName,@contentType,@size,@Attachment,@IUser,@IDate)");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValue("@AttachmentTypeId", AttachmentTypeId);
                cmd.Parameters.AddWithValue("@FileName", FileName);
                cmd.Parameters.AddWithValue("@Description", Description == null? "":Description);
                cmd.Parameters.AddWithValue("@OriginalFileName", OriginalFileName);
                cmd.Parameters.AddWithValue("@ContentType", contentType);
                cmd.Parameters.AddWithValue("@size", size);
                cmd.Parameters.AddWithValue("@Attachment", data);
                cmd.Parameters.AddWithValue("@IUser", IUser);
                cmd.Parameters.AddWithValue("@IDate", DateTime.Now);
                
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        // Update database file 
        public static void UpdateFile(int Id, int EmployeeId, int AttachmentTypeId, string FileName, string Description, string OriginalFileName, string contentType,
            int size, byte[] data, string EUser)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd = new SqlCommand("UPDATE PRM_EmpAttachmentFile SET AttachmentTypeId=@AttachmentTypeId, FileName=@FileName, Description=@Description, OriginalFileName=@OriginalFileName, ContentType=@ContentType,size=@size, Attachment=@Attachment, EUser=@EUser, EDate=@EDate WHERE Id=@Id");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
              
                cmd.Parameters.AddWithValue("@AttachmentTypeId", AttachmentTypeId);
                cmd.Parameters.AddWithValue("@FileName", FileName);
                cmd.Parameters.AddWithValue("@Description", Description == null ? "" : Description);
                cmd.Parameters.AddWithValue("@OriginalFileName", OriginalFileName);
                cmd.Parameters.AddWithValue("@ContentType", contentType);
                cmd.Parameters.AddWithValue("@size", size);
                cmd.Parameters.AddWithValue("@Attachment", data);
                cmd.Parameters.AddWithValue("@EUser", EUser);
                cmd.Parameters.AddWithValue("@EDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Id", Id);

                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static void UpdateFile(int Id, int AttachmentTypeId, string FileName, string Description, string EUser)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd = new SqlCommand("UPDATE PRM_EmpAttachmentFile SET AttachmentTypeId=@AttachmentTypeId, FileName=@FileName, Description=@Description, EUser=@EUser, EDate=@EDate WHERE Id=@Id");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@AttachmentTypeId", AttachmentTypeId);
                cmd.Parameters.AddWithValue("@FileName", FileName);
                cmd.Parameters.AddWithValue("@Description", Description == null ? "" : Description);
                cmd.Parameters.AddWithValue("@EUser", EUser);
                cmd.Parameters.AddWithValue("@EDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Id", Id);

                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        // delete file from database
        public static void DeleteFile(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                string commandText = "DELETE FROM  PRM_EmpAttachmentFile WHERE Id=@ID";
              
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = id;

                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        // Get a file from the database by ID
        public static DataTable GetAFile(int id)
        {
            DataTable file = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT * FROM PRM_EmpAttachmentFile "
                    + "WHERE Id=@ID";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = id;

                adapter.SelectCommand = cmd;
                adapter.Fill(file);

                connection.Close();
            }

            return file;
        }
    }

    public class EmpContractFileUtl
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
        }

        private static void OpenConnection(SqlConnection connection)
        {
            connection.ConnectionString = GetConnectionString();
            connection.Open();
        }

        // Get the list of the files in the database
        public static DataTable GetFileList()
        {
            DataTable fileList = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT *  FROM PRM_EmpContactFiles";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = cmd;
                adapter.Fill(fileList);

                connection.Close();
            }

            return fileList;
        }

        // Save a file into the database
        public static void SaveFile(int empContactInfoId, string userFileName, string originalName, string contentType,
            int size, byte[] data)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;


                string commandText = "INSERT INTO PRM_EmpContactFiles (EmpContactInfoId,UserFileName, OriginalName, ContentType,Size, Data ) ";
                commandText = commandText + " VALUES ( @EmpContactInfoId,@UserFileName, @OriginalName, @ContentType,@Size, @Data)";
            

                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@EmpContactInfoId", SqlDbType.Int);
                cmd.Parameters.Add("@UserFileName", SqlDbType.NVarChar, 100);

                cmd.Parameters.Add("@OriginalName", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@ContentType", SqlDbType.VarChar, 100);

                cmd.Parameters.Add("@Size", SqlDbType.BigInt);
                cmd.Parameters.Add("@Data", SqlDbType.VarBinary);


                cmd.Parameters["@EmpContactInfoId"].Value = empContactInfoId;
                cmd.Parameters["@UserFileName"].Value = userFileName;
                cmd.Parameters["@OriginalName"].Value = originalName;

                cmd.Parameters["@ContentType"].Value = contentType;

                cmd.Parameters["@Size"].Value = size;
                cmd.Parameters["@Data"].Value = data;

                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }
        public static void UpdateFile(int Id, string userFileName, string originalName, string contentType,
            int size, byte[] data)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;


                string commandText = "UPDATE PRM_EmpContactFiles SET UserFileName=@UserFileName, OriginalName=@OriginalName, ContentType=@ContentType,Size=@Size, Data=@Data ";
                commandText = commandText + " WHERE Id=@ID";


                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@UserFileName", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@OriginalName", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@ContentType", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@Size", SqlDbType.BigInt);
                cmd.Parameters.Add("@Data", SqlDbType.VarBinary);
                cmd.Parameters.Add("@ID", SqlDbType.Int);

                cmd.Parameters["@UserFileName"].Value = userFileName;
                cmd.Parameters["@OriginalName"].Value = originalName;
                cmd.Parameters["@ContentType"].Value = contentType;
                cmd.Parameters["@Size"].Value = size;
                cmd.Parameters["@Data"].Value = data;
                cmd.Parameters["@ID"].Value = Id;

                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }
        // Save a file into the database
        public static void DeleteFile(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                string commandText = "DELETE FROM PRM_EmpContactFiles WHERE Id=@ID";

                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = id;

                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        // Get a file from the database by ID
        public static DataTable GetAFile(int id)
        {
            DataTable file = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT * FROM PRM_EmpContactFiles "
                    + "WHERE Id=@ID";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = id;

                adapter.SelectCommand = cmd;
                adapter.Fill(file);

                connection.Close();
            }

            return file;
        }

        // Get a file from the database by EmpContactInfoId
        public static DataTable GetAllFilesByEmpContactInfoId(int empContactInfoId) 
        {
            DataTable file = new DataTable();
            using (SqlConnection connection = new SqlConnection())
            {
                OpenConnection(connection);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandTimeout = 0;

                cmd.CommandText = "SELECT * FROM PRM_EmpContactFiles "
                    + "WHERE EmpContactInfoId=@EmpContactInfoId";
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter();

                cmd.Parameters.Add("@EmpContactInfoId", SqlDbType.Int);
                cmd.Parameters["@EmpContactInfoId"].Value = empContactInfoId;

                adapter.SelectCommand = cmd;
                adapter.Fill(file);

                connection.Close();
            }

            return file;
        }
    }
}