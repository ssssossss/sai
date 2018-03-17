using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using IFNBilling.DataProvider.Interfaces;

using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
using System.ComponentModel;

namespace IFNBilling.DataProvider.MsSqlClient
{
    public class MsSqlUserRoleProvider : UserRoleProvider, IUserRoleDataProvider
    {
        public override List<User> GetUserDetails(string username, string Password = null, string Types = null)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetUserDetails", cn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = Password;
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Types == null ? string.Empty : Types;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetUserDetailsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public int InsertUpdateUserDetails(VM_UserDetails userDetails)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlTransaction sqlTrans;
                cn.Open();
                sqlTrans = cn.BeginTransaction();
                int UserId = 0;
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    if (userDetails.UserID == 0)
                    {

                        cmd = new SqlCommand("[Admin_InsertUsersTable]", cn);
                        cmd.Transaction = sqlTrans;
                        cmd.Parameters.AddWithValue("@Username", userDetails.Username);
                        cmd.Parameters.AddWithValue("@FirstName", userDetails.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", userDetails.LastName);
                        cmd.Parameters.AddWithValue("@Location", userDetails.Location);
                        cmd.Parameters.AddWithValue("@CreatedBy", userDetails.CreatedBy);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter sqlparam = new SqlParameter();
                        sqlparam.ParameterName = "@UserId";
                        sqlparam.DbType = DbType.Int32;
                        sqlparam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(sqlparam);
                        cmd.ExecuteNonQuery();
                        UserId = Convert.ToInt32(cmd.Parameters["@UserId"].Value);
                    }
                    else
                    {
                        cmd = new SqlCommand("[Admin_UpdateUsersTable]", cn);
                        cmd.Transaction = sqlTrans;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", userDetails.UserID);
                        cmd.Parameters.AddWithValue("@IsActive", userDetails.IsActive);
                        cmd.Parameters.AddWithValue("@ModifiedBy", userDetails.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedByIP", userDetails.ModifiedByIP);
                        if (ExecuteNonQuery(cmd) != 0)
                        {
                            UserId = userDetails.UserID;
                        }
                    }
                   

                    cmd = new SqlCommand("[Admin_InsertUpdateUserRoles]", cn);
                    cmd.Transaction = sqlTrans;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var item in userDetails.Roles) item.UserId = UserId;
                    SqlParameter userRoles = cmd.Parameters.AddWithValue("@UserRoles", ConvertToDataTable<UserRole>(userDetails.Roles));
                    userRoles.TypeName = "dbo.tvp_AdminUserRoles";
                    userRoles.SqlDbType = SqlDbType.Structured;

                    cmd.ExecuteNonQuery(); 
                    


                }
                catch (Exception)
                {
                    try
                    {
                        sqlTrans.Rollback();
                        cn.Close();
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                    throw;
                }
                sqlTrans.Commit();
                cn.Close();
                return UserId;
            }



        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public bool CheckUserDetailsByUserName(string username)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    var cmd = new SqlCommand("Admin_CheckUserDetailsByUserName", cn);
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    var result = Convert.ToBoolean(cmd.ExecuteScalar());
                    return result;
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }

        }

        public VM_UserDetails GetUserDetailsByUserId(int UserId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[Admin_GetUserDetailsByUserId]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@UserId", UserId);
                sqlConnection.Open();
                return GetUserDetailsByUserIdFromReader(ExecuteReader(sqlCommand));
            }
        }


   public List<VM_UserDetails> GetAllIfnUsers()
   {
     using(SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
     {
       SqlCommand sqlCommand = new SqlCommand("Admin_GetAllIfnUsers", sqlConnection);
       sqlCommand.CommandType = CommandType.StoredProcedure;
       sqlConnection.Open();

       return GetAllIfnUsersCollectionFromReader(ExecuteReader(sqlCommand));

        


     }


   }

   public bool SetAuditTrailLog(AuditTrailLog auditTrailLog)
   {
       using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
       {
           using (SqlCommand sqlCommand = new SqlCommand("[BSB_SetAuditTrailLog]", sqlConnection))
           {
               sqlConnection.Open();
               sqlCommand.CommandType = CommandType.StoredProcedure;
               sqlCommand.Parameters.AddWithValue("@in_LoginID", (String.IsNullOrEmpty(auditTrailLog.LoginID) ? (object)DBNull.Value : auditTrailLog.LoginID));
               sqlCommand.Parameters.AddWithValue("@in_SystemIP", (String.IsNullOrEmpty(auditTrailLog.SystemIP) ? (object)DBNull.Value : auditTrailLog.SystemIP));
               sqlCommand.Parameters.AddWithValue("@in_SessionID", (String.IsNullOrEmpty(auditTrailLog.SessionID) ? (object)DBNull.Value : auditTrailLog.SessionID));
               sqlCommand.Parameters.AddWithValue("@in_Module", (String.IsNullOrEmpty(auditTrailLog.Module) ? (object)DBNull.Value : auditTrailLog.Module));
               sqlCommand.Parameters.AddWithValue("@in_Action", (String.IsNullOrEmpty(auditTrailLog.Action) ? (object)DBNull.Value : auditTrailLog.Action));
               sqlCommand.Parameters.AddWithValue("@in_Comments", (String.IsNullOrEmpty(auditTrailLog.Comments) ? (object)DBNull.Value : auditTrailLog.Comments + " | ClientDate : " + DateTime.Now.ToString()));
               sqlCommand.Parameters.AddWithValue("@in_JobId", (auditTrailLog.JobId == 0 ? (object)DBNull.Value : auditTrailLog.JobId));
               sqlCommand.Parameters.AddWithValue("@in_BatchId", (auditTrailLog.BatchId ==0 ? (object)DBNull.Value : auditTrailLog.BatchId));

               sqlCommand.ExecuteNonQuery();
           }
       }
       return true;
   }


    }
}
