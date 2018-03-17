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
    public class MsSqlContactsProvider : ContactsProvider, IContactsDataProvider
    {
        public override List<Contacts> GetContactDetailsByEmail(string email, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("", cn);
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = email;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsByCompanyName(string companyname, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("", cn);
                cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 50).Value = companyname;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsByContactName(string contactname, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("", cn);
                cmd.Parameters.Add("@ContactName", SqlDbType.NVarChar, 50).Value = contactname;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsByContactId(Int64 contactid, string type)
        {          
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("spGetContactsByType", cn);
                cmd.Parameters.AddWithValue("@ContactId", contactid);
                cmd.Parameters.AddWithValue("@SFDCContactId", "");
                cmd.Parameters.AddWithValue("@AccountName", "");
                cmd.Parameters.AddWithValue("@FirstName", "");
                cmd.Parameters.AddWithValue("@LastName", "");
                cmd.Parameters.AddWithValue("@Email", "");
                cmd.Parameters.AddWithValue("@OperationMode", type);
                cmd.CommandType = CommandType.StoredProcedure;
                
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsBySFDCContactId(string sfdccontactid, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("spGetContactsByType", cn);
                cmd.Parameters.Add("@ContactId", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@SFDCContactId", SqlDbType.NVarChar, 50).Value = sfdccontactid;              
                cmd.Parameters.Add("@AccountName", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@OperationMode", SqlDbType.NVarChar, 50).Value = type;
                cmd.CommandType = CommandType.StoredProcedure;
               
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsByCompanyNameandContactName(string companyname, string contactname, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("", cn);
                cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 50).Value = companyname;
                cmd.Parameters.Add("@ContactName", SqlDbType.NVarChar, 50).Value = contactname;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsByCompanyNameandEmail(string companyname, string email, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("", cn);
                cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 50).Value = companyname;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = email;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public override List<Contacts> GetContactDetailsByCompanyNameContactNameAndEmail(string companyname, string firstname,string lastname, string email, string type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("spGetContactsByType", cn);
                cmd.Parameters.Add("@ContactId", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@SFDCContactID", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@AccountName", SqlDbType.NVarChar, 50).Value = companyname;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = firstname;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = lastname;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = email;
                cmd.Parameters.Add("@OperationMode", SqlDbType.NVarChar, 50).Value = type;
                cmd.CommandType = CommandType.StoredProcedure;
                
                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public int InsertOrUpdateContactDetails(Contacts contactsDetails)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {                
                cn.Open();
              
                int result = 0;

                try
                {
                    SqlCommand cmd = new SqlCommand();                  

                    cmd = new SqlCommand("spContacts_InsertContactDetails", cn);
                   
                    cmd.Parameters.AddWithValue("@ContactID", contactsDetails.ContactID);
                    cmd.Parameters.AddWithValue("@SFDCContactID", contactsDetails.SFDCContactID);
                    cmd.Parameters.AddWithValue("@ContactRecordType", contactsDetails.ContactRecordType);
                    cmd.Parameters.AddWithValue("@ContactAssignedTo", contactsDetails.ContactAssignedTo);
                    cmd.Parameters.AddWithValue("@LastName", contactsDetails.LastName);
                    cmd.Parameters.AddWithValue("@FirstName", contactsDetails.FirstName);                    
                    cmd.Parameters.AddWithValue("@ContactNo", contactsDetails.ContactNo);
                    cmd.Parameters.AddWithValue("@AccountName", contactsDetails.AccountName);
                    cmd.Parameters.AddWithValue("@Email", contactsDetails.Email);
                    cmd.Parameters.AddWithValue("@Title", contactsDetails.Title);
                    cmd.Parameters.AddWithValue("@City", contactsDetails.City);
                    cmd.Parameters.AddWithValue("@Street", contactsDetails.Street);
                    cmd.Parameters.AddWithValue("@Country", contactsDetails.Country);
                    cmd.Parameters.AddWithValue("@Role", contactsDetails.Role);
                    cmd.Parameters.AddWithValue("@IsActive", contactsDetails.IsActiveForNewContacts);
                    cmd.Parameters.AddWithValue("@OperationMode", contactsDetails.Type);
                    cmd.Parameters.AddWithValue("@State", contactsDetails.State == null ? (object)DBNull.Value : contactsDetails.State);
                    cmd.Parameters.AddWithValue("@ZipCode", contactsDetails.ZipCode);
                    //TFS 44041 Created By and Modified by in the Contacts Table
                    cmd.Parameters.AddWithValue("@CreatedBy", contactsDetails.CreatedBy);
                    cmd.Parameters.AddWithValue("@ModifiedByIP", contactsDetails.ModifiedByIP);
                    cmd.Parameters.AddWithValue("@ModifiedBy", contactsDetails.ModifiedBy);  

                    cmd.CommandType = CommandType.StoredProcedure;
                  
                    result = cmd.ExecuteNonQuery();
                  
                }
                catch (Exception)
                {
                    try
                    {                      
                        cn.Close();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    throw;
                }              
                cn.Close();
                return result;
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

        public bool CheckContactDetailsByEmail(string email,string checkemail)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    cn.Open();

                    var cmd = new SqlCommand("spGetContactsByType", cn);                   
                    cmd.Parameters.Add("@ContactId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@SFDCContactID", SqlDbType.NVarChar, 50).Value = "";
                    cmd.Parameters.Add("@AccountName", SqlDbType.NVarChar, 50).Value = "";
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = "";
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = "";
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = email;
                    cmd.Parameters.Add("@OperationMode", SqlDbType.NVarChar, 50).Value = checkemail;
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    var result = Convert.ToBoolean(cmd.ExecuteScalar());
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Contacts> GetAllContacts(string type)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetContactsByType", sqlConnection);               

                cmd.Parameters.Add("@ContactId", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@SFDCContactID", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@AccountName", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = "";
                cmd.Parameters.Add("@OperationMode", SqlDbType.NVarChar, 50).Value = type;

                cmd.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();

                return GetAllContactsCollectionFromReader(ExecuteReader(cmd));
            }
        }
    }
}
