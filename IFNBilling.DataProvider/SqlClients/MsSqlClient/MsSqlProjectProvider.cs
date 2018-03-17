using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
using IFNBilling.DataProvider.Interfaces;
using System.ComponentModel;

namespace IFNBilling.DataProvider.MsSqlClient
{
    public class MsSqlProjectProvider : ProjectProvider, IProjectDataProvider
    {
        public List<Project> GetAllProjects()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[Project_GetAllProjects]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetProjectCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public int AddOrModifyProject(Project project, string test)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                int project_number = 0;
                SqlTransaction sqlTrans;
                cn.Open();
                sqlTrans = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("[Project_AddOrModifyProject]", cn);
                    cmd.Transaction = sqlTrans;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsTypeSet", project.IsTypeSet);
                    cmd.Parameters.AddWithValue("@ContactId", project.Contact.ContactID);
                    cmd.Parameters.AddWithValue("@NoOfPages", project.InitialEstimatedPages);
                    cmd.Parameters.AddWithValue("@IsTranslations", project.IsTranslations);
                    cmd.Parameters.AddWithValue("@IsHospitality", project.IsHospitality);
                    cmd.Parameters.AddWithValue("@IsMedia", project.IsMedia);
                    cmd.Parameters.AddWithValue("@IsPrint", project.IsPrint);
                    cmd.Parameters.AddWithValue("@IsFiling", project.IsFiling);
                    cmd.Parameters.AddWithValue("@ExpectedProjectDate", project.ExpectedStartDate);
                    cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                    cmd.Parameters.AddWithValue("@ExpectedFilingDate", project.ExpectedFilingDate);
                    cmd.Parameters.AddWithValue("@Comments", project.Comments);
                    cmd.Parameters.AddWithValue("@ProjectStatusID", project.ProjectStatus.ProjectStatusID);
                    cmd.Parameters.AddWithValue("@ProjectTypeID", project.ProjectType.ProjectTypeID);
                    cmd.Parameters.AddWithValue("@ProjectVerticalID", project.ProjectVertical.ProjectVerticalID);
                    // cmd.Parameters.AddWithValue("@DistributionList", project.DistributionList);
                    cmd.Parameters.AddWithValue("@StatusDate", project.StatusDate);
                    cmd.Parameters.AddWithValue("@ProjectPhaseID", project.CodeMaster.CodeMasterID);
                    cmd.Parameters.AddWithValue("@ProjectNumber", project.ProjectNumber);
                    cmd.Parameters.AddWithValue("@CreatedBy", project.CreatedBy);
                    cmd.Parameters.AddWithValue("@ModifiedBy", project.ModifiedBy);
                    cmd.Parameters.AddWithValue("@ModifiedByIP", project.ModifiedByIP);
                    cmd.Parameters.AddWithValue("@ProjectSiteID", project.projectSite.ProjectSiteID);
                    cmd.Parameters.AddWithValue("@ProjectSalesRepID", project.projectSalesRep.ProjectSalesRepID);
                    cmd.Parameters.AddWithValue("@ProjectTypeDescription", project.ProjectType.ProjectTypeDescription);
                    cmd.Parameters.AddWithValue("@ProjectVerticalDescription", project.ProjectVertical.ProjectVerticalDescription);
                    cmd.Parameters.AddWithValue("@ClientCompanyName", project.ClientCompanyName);
                    cmd.Parameters.AddWithValue("@ClientContactName", project.ClientContactName);
                    cmd.Parameters.AddWithValue("@ProjectManager", project.ProjectManager);
                    cmd.Parameters.AddWithValue("@OracleProjectTypeID", project.OracleProjectTypeID);
                    
                    //cmd.Parameters.AddWithValue("@ContactID", project.Contact.ContactID);
                    SqlParameter sqlparam = new SqlParameter();
                    sqlparam.ParameterName = "@project_number";
                    sqlparam.DbType = DbType.Int32;
                    sqlparam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(sqlparam);

                    ExecuteNonQuery(cmd);
                    project_number = Convert.ToInt32(cmd.Parameters["@project_number"].Value);

                    #region AutoJobCreation

                    MsSqlMasterDataProvider oMsSqlMasterDataProvider = new MsSqlMasterDataProvider();
                    //List<JobType> oJobType = oMsSqlMasterDataProvider.GetJobType();

                    /* TFS# 41880 - Allow all Services and Project status is in Active / In-Progress */
                    //if ((project.ProjectType.ProjectTypeID != 7) && (project.ProjectStatus.ProjectStatusID == 1 || project.ProjectStatus.ProjectStatusID == 2))
                    if (project.ProjectStatus.ProjectStatusID == 1 || project.ProjectStatus.ProjectStatusID == 2)
                    {

                        #region MediaJob

                        cmd = new SqlCommand("[Job_AddOrRemoveMediaAutoJob]", cn);
                        cmd.Transaction = sqlTrans;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProjectNumber", project_number);
                        cmd.Parameters.AddWithValue("@IsMedia", project.IsMedia);
                        cmd.Parameters.AddWithValue("@CreatedBy", project.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", project.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedByIP", project.ModifiedByIP);

                        ExecuteNonQuery(cmd);

                        #endregion MediaJob

                        #region Hospitality

                        cmd = new SqlCommand("[Job_AddOrRemoveHospitalityAutoJob]", cn);
                        cmd.Transaction = sqlTrans;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProjectNumber", project_number);
                        cmd.Parameters.AddWithValue("@IsHospitality", project.IsHospitality);
                        cmd.Parameters.AddWithValue("@CreatedBy", project.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", project.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedByIP", project.ModifiedByIP);

                        ExecuteNonQuery(cmd);

                        #endregion Hospitality

                        #region Composition

                        cmd = new SqlCommand("[Job_AddOrRemoveCompositionAutoJob]", cn);
                        cmd.Transaction = sqlTrans;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProjectNumber", project_number);
                        cmd.Parameters.AddWithValue("@IsTranslations", project.IsTranslations);
                        cmd.Parameters.AddWithValue("@IsTypeSet", project.IsTypeSet);
                        cmd.Parameters.AddWithValue("@CreatedBy", project.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", project.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedByIP", project.ModifiedByIP);

                        ExecuteNonQuery(cmd);

                        #endregion Composition
                    }

                    /* Validate for JMS Print Only Job and Project status is in Active / In-Progress */
                    if (project.ProjectStatus.ProjectStatusID == 1 || project.ProjectStatus.ProjectStatusID == 2)
                    {
                        #region Print

                        cmd = new SqlCommand("[Job_AddOrRemovePrintAutoJob]", cn);
                        cmd.Transaction = sqlTrans;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProjectNumber", project_number);
                        cmd.Parameters.AddWithValue("@IsPrint", project.IsPrint);
                        cmd.Parameters.AddWithValue("@CreatedBy", project.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", project.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedByIP", project.ModifiedByIP);

                        ExecuteNonQuery(cmd);

                        #endregion Print
                    }

                    #endregion AutoJobCreation

                    sqlTrans.Commit();
                    cn.Close();
                }
                catch (Exception exTrans)
                {
                    try
                    {
                        sqlTrans.Rollback();
                    }
                    catch (Exception exRlbk)
                    {
                        cn.Close();
                        throw exRlbk;
                    }
                    cn.Close();
                    throw exTrans;
                }

                return project_number;
            }
        }

        public Project GetProjectDetailsByProjectId(int project_id)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[Project_GetProjectDetailsById]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@in_projectnumber", project_id);
                cn.Open();
                return GetProjectDetailsByIdFromReader(ExecuteReader(cmd));
            }

        }

        public List<VM_CompanyContactDetails> GetCompanyDetails(string firstname, string lastname, string companyname, string salesrep)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[Project_GetCompanyDetails]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Firstname", string.IsNullOrEmpty(firstname) ? (object)DBNull.Value : firstname);
                cmd.Parameters.AddWithValue("@Lastname", string.IsNullOrEmpty(lastname) ? (object)DBNull.Value : lastname);
                cmd.Parameters.AddWithValue("@Companyname", string.IsNullOrEmpty(companyname) ? (object)DBNull.Value : companyname);
                cmd.Parameters.AddWithValue("@Salesrep", string.IsNullOrEmpty(salesrep) ? (object)DBNull.Value : salesrep);
                cn.Open();
                //return Da
                return GetCompanyDetailsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        //public List<VM_CompanyContactDetails> GetCompanySearchDetails(string firstname, string lastname, string companyname, string salesrep, Int64 parentcontactid)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("[Project_GetCompanySearchDetails]", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Firstname", string.IsNullOrEmpty(firstname) ? (object)DBNull.Value : firstname);
        //        cmd.Parameters.AddWithValue("@Lastname", string.IsNullOrEmpty(lastname) ? (object)DBNull.Value : lastname);
        //        cmd.Parameters.AddWithValue("@Companyname", string.IsNullOrEmpty(companyname) ? (object)DBNull.Value : companyname);
        //        cmd.Parameters.AddWithValue("@Salesrep", string.IsNullOrEmpty(salesrep) ? (object)DBNull.Value : salesrep);
        //        cmd.Parameters.AddWithValue("@ParentContactID", parentcontactid);
        //        cn.Open();
        //        //return Da
        //        return GetCompanyDetailsCollectionFromReader(ExecuteReader(cmd));
        //    }
        //}

        public List<VM_ProjectSearch> GetProjectSearchResults(int? projectNumber, string projectName, DateTime? dateFrom, DateTime? dateTo, DateTime? CompletedDateFrom, DateTime? CompletedDateTo, string company, int? projectStatusId, int? projectTypeId)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                if (projectNumber == null && projectName == null && dateFrom == null && dateTo == null && CompletedDateFrom == null && CompletedDateTo ==null && company == null && projectStatusId == null && projectTypeId == null)
                {
                    return new List<VM_ProjectSearch>();
                }
                SqlCommand cmd = new SqlCommand("[spGetProjectSearchResults]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", (projectNumber == null ? null : projectNumber));
                cmd.Parameters.Add("@ProjectName", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(projectName) ? (object)DBNull.Value : projectName);
                cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(company) ? (object)DBNull.Value : company);
                cmd.Parameters.Add("@ProjectDateFrom", SqlDbType.DateTime).Value = (dateFrom == null ? (object)DBNull.Value : (DateTime)dateFrom);
                cmd.Parameters.Add("@ProjectDateTo", SqlDbType.DateTime).Value = (dateTo == null ? (object)DBNull.Value : (DateTime)dateTo);

                #region "Sprint20-HotFix-TFS#44390 Completed Date to be captured in the Project Search and Grid"
                cmd.Parameters.Add("@ProjectCompletedDateFrom", SqlDbType.DateTime).Value = (CompletedDateFrom == null ? (object)DBNull.Value : (DateTime)CompletedDateFrom);
                cmd.Parameters.Add("@ProjectCompletedDateTo", SqlDbType.DateTime).Value = (CompletedDateTo == null ? (object)DBNull.Value : (DateTime) CompletedDateTo);
                #endregion

                cmd.Parameters.AddWithValue("@ProjectStatusId", (projectStatusId == null ? null : projectStatusId));
                cmd.Parameters.AddWithValue("@ProjectTypeId", (projectTypeId == null ? null : projectTypeId));

                cn.Open();
                return GetProjectSearchResultsCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public List<ProjectInvoiceStatus> GetProjectInvoiceStatusList(int projectNumber)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlcommand = new SqlCommand("[GetProjectInvoiceByProjectNumber]", cn);
                sqlcommand.CommandType = CommandType.StoredProcedure;
                sqlcommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                cn.Open();
                return GetProjectInvoiceStatusCollectionFromReader(ExecuteReader(sqlcommand));


            }


        }

        public string CheckInvoiceNumber(string invoiceNumber)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlcommand = new SqlCommand("[spProjectInvoiceNumberIsExist]", cn);
                sqlcommand.CommandType = CommandType.StoredProcedure;
                sqlcommand.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);


                //SqlParameter sqlparam = new SqlParameter();
                //sqlparam.ParameterName = "@ErrorText";
                //sqlparam.DbType = (DbType)SqlDbType.NVarChar;
                //sqlparam.Direction = ParameterDirection.Output;
                //sqlcommand.Parameters.Add(sqlparam);

                cn.Open();

                int i = (int)ExecuteScalar(sqlcommand);
                //string error = (string)sqlcommand.Parameters["@InvoiceNumber"].Value.ToString();
                string error = string.Empty;
                if (i > 0)
                    error = "Invoice number already exist";
                else
                    error = "";
                return error;


            }

        }
        public bool InsertUpdateProjectInvoice(ProjectInvoiceStatus projectInvoiceStatus)
        {

            bool success = false;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {



                SqlCommand sqlcommand = new SqlCommand("[Project_InsertUpdateProjectInvoiceStatus]", cn);
                sqlcommand.CommandType = CommandType.StoredProcedure;

                sqlcommand.Parameters.AddWithValue("@InvoiceNumber", projectInvoiceStatus.InvoiceNumber);
                sqlcommand.Parameters.AddWithValue("@ProjectNumber", projectInvoiceStatus.ProjectNumber);
                sqlcommand.Parameters.AddWithValue("@InvoiceAsofDate", projectInvoiceStatus.InvoiceAsOfDate);
                sqlcommand.Parameters.AddWithValue("@InvoiceAmount ", projectInvoiceStatus.InvoiceAmount);
                sqlcommand.Parameters.AddWithValue("@InvoiceType", projectInvoiceStatus.InvoiceType1.InvoiceTypeID);
                sqlcommand.Parameters.AddWithValue("@InvoiceMileStone", projectInvoiceStatus.codeMaster.CodeMasterID);
                sqlcommand.Parameters.AddWithValue("@InvoiceComments", projectInvoiceStatus.InvoiceComments);
                sqlcommand.Parameters.AddWithValue("@InvoiceDate", projectInvoiceStatus.InvoiceDate);
                sqlcommand.Parameters.AddWithValue("@ProjectInvoiceID", projectInvoiceStatus.ProjectInvoiceID);

                cn.Open();
                int value = ExecuteNonQuery(sqlcommand);

                success = value == 1 ? true : success;

                return success;





            }


        }

        public List<ProjectVendorTracking> GetProjectVendorTracking(int projectNumber)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[spGetProjectVendorTracking]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@projectNumber", projectNumber);
                cn.Open();
                return GetProjectVendorTrackingCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public int AddOrModifyProjectVendorTracking(ProjectVendorTracking projectVendorTracking)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[spAddOrModifyProjectVendorTracking]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VendorTrackingID", projectVendorTracking.VendorTrackingID);
                cmd.Parameters.AddWithValue("@JobNumber", projectVendorTracking.JobNumber);
                cmd.Parameters.AddWithValue("@ServiceType", projectVendorTracking.ServiceType);
                cmd.Parameters.AddWithValue("@IsPhotocopyColor", projectVendorTracking.IsPhotocopyColor);
                cmd.Parameters.AddWithValue("@IsPhotocopyBAndW", projectVendorTracking.IsPhotocopyBAndW);
                cmd.Parameters.AddWithValue("@Colorpages", projectVendorTracking.Colorpages);
                cmd.Parameters.AddWithValue("@BAndWpages", projectVendorTracking.BAndWpages);
                cmd.Parameters.AddWithValue("@PONumber", projectVendorTracking.PONumber);
                cmd.Parameters.AddWithValue("@POAmount", projectVendorTracking.POAmount);
                cmd.Parameters.AddWithValue("@PODate", projectVendorTracking.PODate);
                cmd.Parameters.AddWithValue("@VendorName", projectVendorTracking.VendorName);
                cmd.Parameters.AddWithValue("@VendorInvoiceNumber", projectVendorTracking.VendorInvoiceNumber);
                cmd.Parameters.AddWithValue("@VendorInvoiceAmount", projectVendorTracking.VendorInvoiceAmount);
                cmd.Parameters.AddWithValue("@VendorInvoiceDate", projectVendorTracking.VendorInvoiceDate);
                cmd.Parameters.AddWithValue("@ProjectNumber", projectVendorTracking.ProjectNumber);
                //cmd.Parameters.AddWithValue("@PaymentTerms", projectVendorTracking.PaymentTerms);
                cmd.Parameters.AddWithValue("@PaymentTerms", string.Empty);
                cmd.Parameters.AddWithValue("@Comments", projectVendorTracking.Comments);
                cmd.Parameters.AddWithValue("@UserName", string.Empty);
                cmd.Parameters.AddWithValue("@ModifiedByIP", string.Empty);
                //cmd.Parameters.AddWithValue("@UserName", projectVendorTracking.ModifiedBy);
                //cmd.Parameters.AddWithValue("@ModifiedByIP", projectVendorTracking.ModifiedByIP);      

                SqlParameter sqlparam = new SqlParameter();
                sqlparam.ParameterName = "@OutVendorTrakcingId";
                sqlparam.DbType = DbType.Int32;
                sqlparam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam);



                cn.Open();
                ExecuteNonQuery(cmd);
                int vendorTrackingId = 0;

                if (projectVendorTracking.VendorTrackingID != 0)
                {
                    vendorTrackingId = projectVendorTracking.VendorTrackingID;
                    return vendorTrackingId;
                }

                vendorTrackingId = Convert.ToInt32(cmd.Parameters["@OutVendorTrakcingId"].Value);

                return vendorTrackingId;

            }
        }

        public List<string> GetJobNumbers(int projectNumber, string type)
        {
            using (SqlConnection sc = new SqlConnection(this.ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("[spGetJobNumberForVendorInvoice]", sc);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                cmd.Parameters.AddWithValue("@Type", type);

                sc.Open();
                return GetJobNumbersFromReader(ExecuteReader(cmd));
            }
        }



        public List<EmailJobNumber> GetEmailBatchNumber(string JobNumber, string type)
        {
            using (SqlConnection sc = new SqlConnection(this.ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("[spGetBatchNumberAndCreateby]", sc);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Jobnumber", JobNumber);
                cmd.Parameters.AddWithValue("@OperationMode", type);

                sc.Open();
                return GetEmailBatchNumberCollectionFromReader(ExecuteReader(cmd));
            }
        }



        //TFS -43725,42830,42831,42832- Composition - Copy batch -Modify QE batch to spin over jobs
        /// <summary>
        /// Gets the copy destination job numbers.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <param name="SourceJobNumber">The source job number.</param>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        public List<JobNumberText> GetCopyDestinationJobNumbers(int projectNumber, string SourceJobNumber, string types)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[GetCopyDestinationJobNumbers]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                sqlCommand.Parameters.AddWithValue("@SourceJobNumber", SourceJobNumber);
                sqlCommand.Parameters.AddWithValue("@Type", types);

                sqlConnection.Open();
                return GetJobNumberTextCollectionFromReader(ExecuteReader(sqlCommand));
                
            }
        }

         

        public List<DistributionList> GetProjectDistributionList(int projectNumber)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_GetProjectDistributionListByProjectNumber]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);

                sqlConnection.Open();
                return GetDistributionListCollectionFromReader(ExecuteReader(sqlCommand));

            }
        }

        public bool DeleteContact(int DistributionListID, int projectNumber,int ContactID)
        {

            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_DeleteProjectDistributionContactByID]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                sqlCommand.Parameters.AddWithValue("@DistributionListID", DistributionListID);
                sqlCommand.Parameters.AddWithValue("@ContactID", ContactID);
                sqlConnection.Open();

                if (ExecuteNonQuery(sqlCommand) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }



            }
        }


        public List<DistributionList> GetMapContactDistributionList(string contactFirstName, string contactLastName, string companyName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_MapContactForDistribution]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@ContactFirstName", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(contactFirstName) ? (object)DBNull.Value : contactFirstName);
                sqlCommand.Parameters.Add("@CompanyName", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(companyName) ? (object)DBNull.Value : companyName);
                sqlCommand.Parameters.Add("@ContactLastName", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(contactLastName) ? (object)DBNull.Value : contactLastName);
                sqlConnection.Open();
                return GetDistributionListCollectionFromReader(ExecuteReader(sqlCommand));
            }
        }

        public bool InsertProjectDistributionList(List<ProjectDistributionListMap> projectDistributions)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_InsertProjectDistributionList]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlConnection.Open();

                SqlParameter projectDistributionsParam = sqlCommand.Parameters.AddWithValue("@projectDistributions", ConvertToDataTable(projectDistributions));
                projectDistributionsParam.SqlDbType = SqlDbType.Structured;
                if (ExecuteNonQuery(sqlCommand) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in propertyDescriptorCollection)
            {
                if (!prop.Name.Equals("ProjectDistributionListMapID"))
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
                {
                    if (!propertyDescriptor.Name.Equals("ProjectDistributionListMapID"))
                        row[propertyDescriptor.Name] = propertyDescriptor.GetValue(item) ?? DBNull.Value;

                }
                table.Rows.Add(row);
            }
            return table;

        }

        public List<DistributionList> GetCompanyNames(string compName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_GetCompanyNames]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CompanyName", compName);

                sqlConnection.Open();
                return GetCompanyCollectionFromReader(ExecuteReader(sqlCommand));
            }
        }

        //public List<Company> GetCompanyIdandNameByCompany(string compName)
        //{
        //    using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        //    {

        //        SqlCommand sqlCommand = new SqlCommand("[Project_GetCompanyIdandNameByCompany]", sqlConnection);

        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        sqlCommand.Parameters.AddWithValue("@CompanyName", compName);

        //        sqlConnection.Open();
        //        return GetCompanyidandnameFromReader(ExecuteReader(sqlCommand));
        //    }
        //}

        public List<VM_AddDistribution> GetAddressDetailsByCompany(string companyName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_GetAddressDetailsByCompany]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CompanyName", companyName);

                sqlConnection.Open();
                return GetAddressDetailsCollectionFromReader(ExecuteReader(sqlCommand));
            }

        }

        public int AddContacttoDistribution(DistributionList contactDetails)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_AddContacttoDistribution]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@DistributionFirstName", contactDetails.DistributionFirstName == null ? (object)DBNull.Value : contactDetails.DistributionFirstName);
                sqlCommand.Parameters.AddWithValue("@DistributionLastName", contactDetails.DistributionLastName == null ? (object)DBNull.Value : contactDetails.DistributionLastName);
                sqlCommand.Parameters.AddWithValue("@DistributionEmailid", contactDetails.DistributionEmailID == null ? (object)DBNull.Value : contactDetails.DistributionEmailID);
                sqlCommand.Parameters.AddWithValue("@DistributionAccount", contactDetails.DistributionAccount == null ? (object)DBNull.Value : contactDetails.DistributionAccount);
                sqlCommand.Parameters.AddWithValue("@DistributionStreet", contactDetails.DistributionStreet == null ? (object)DBNull.Value : contactDetails.DistributionStreet);
                sqlCommand.Parameters.AddWithValue("@DistributionCity", contactDetails.DistributionCity == null ? (object)DBNull.Value : contactDetails.DistributionCity);
                sqlCommand.Parameters.AddWithValue("@DistributionState", contactDetails.DistributionState == null ? (object)DBNull.Value : contactDetails.DistributionState);
                sqlCommand.Parameters.AddWithValue("@DistributionZipCode", contactDetails.DistributionZipCode == null ? (object)DBNull.Value : contactDetails.DistributionZipCode);
                sqlCommand.Parameters.AddWithValue("@DistributionCountry", contactDetails.DistributionCountry == null ? (object)DBNull.Value : contactDetails.DistributionCountry);
                sqlCommand.Parameters.AddWithValue("@DistributionPhone", contactDetails.DistributionPhone == null ? (object)DBNull.Value : contactDetails.DistributionPhone);
                sqlCommand.Parameters.AddWithValue("@DistributionFax", contactDetails.DistributionFax == null ? (object)DBNull.Value : contactDetails.DistributionFax);

                sqlConnection.Open();
                SqlParameter sqlparam = new SqlParameter();
                sqlparam.ParameterName = "@DistributionListId";
                sqlparam.DbType = DbType.Int32;
                sqlparam.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(sqlparam);
                ExecuteNonQuery(sqlCommand);
                int distributionListId = 0;
                distributionListId = Convert.ToInt32(sqlCommand.Parameters["@DistributionListId"].Value);
                return distributionListId;
            }
        }

        public DistributionList GetContactDetailsByDistributionListId(int distributionListId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlCommand = new SqlCommand("[Project_GetContactDetailsByDistributionId]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@DistributionListId", distributionListId);

                sqlConnection.Open();
                return GetContactDetailsByDistributionListIdFromReader(ExecuteReader(sqlCommand));
            }
        }

        public List<LabelSystem> GetLabelMessagesByProjectNumber(int projectNumber, string Category)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[Project_GetLabelMessagesByProjectNumber]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                sqlCommand.Parameters.AddWithValue("@Type", Category);

                sqlConnection.Open();
                return GetLabelSystemCollectionFromReader(ExecuteReader(sqlCommand));

            }

        }

        public bool InsertLabelSystemMessage(LabelSystem labelSystem)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[Project_InsertLabelSystemMessage]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", labelSystem.ProjectNumber);
                sqlCommand.Parameters.AddWithValue("@LabelMessage", labelSystem.LabelSystemMessage);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", labelSystem.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@MessageCategory", labelSystem.MessageCategory);
                sqlCommand.Parameters.AddWithValue("@BatchNumber", labelSystem.BatchNumber);
                sqlCommand.Parameters.AddWithValue("@JobNumber", labelSystem.JobNumber);
                sqlCommand.Parameters.AddWithValue("@MessageTitle", labelSystem.MessageTitle);
 
                sqlConnection.Open();

                if (ExecuteNonQuery(sqlCommand) != 0)
                    return true;
                else
                    return false;
            }

        }

        public bool UpdateLabelSystemMessage(LabelSystem labelSystem)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[Project_UpdateLabelSystemMessage]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@LabelSystemID", labelSystem.LabelSystemID);
                sqlCommand.Parameters.AddWithValue("@LabelSystemMessage", labelSystem.LabelSystemMessage);
                sqlCommand.Parameters.AddWithValue("@ModifiedBy", labelSystem.ModifiedBy);
                sqlCommand.Parameters.AddWithValue("@BatchNumber", labelSystem.BatchNumber);
                sqlCommand.Parameters.AddWithValue("@JobNumber", labelSystem.JobNumber);
                sqlCommand.Parameters.AddWithValue("@MessageTitle", labelSystem.MessageTitle);
                
                sqlConnection.Open();

                if (ExecuteNonQuery(sqlCommand) != 0)
                    return true;
                else
                    return false;
            }
        }

        public bool DeleteLabelSystemMessage(LabelSystem labelSystem)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[Project_DeleteLabelSystemMessage]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@LabelSystemID", labelSystem.LabelSystemID);
                sqlCommand.Parameters.AddWithValue("@ModifiedBy", labelSystem.ModifiedBy);

                sqlConnection.Open();

                if (ExecuteNonQuery(sqlCommand) != 0)
                    return true;
                else
                    return false;
            }
        }

        public string CheckActualFilingDate(int projectNumber)
        {

            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlcommand = new SqlCommand("[Project_checkActualFilingDate]", sqlConnection);
                sqlcommand.CommandType = CommandType.StoredProcedure;
                sqlcommand.Parameters.AddWithValue("@projectNumber", projectNumber);
                sqlConnection.Open();

                int i = (int)ExecuteScalar(sqlcommand);

                string error = string.Empty;
                if (i > 0)
                    error = "Please enter Actual Filing Date in composition/media jobs in order to complete the project.";
                else
                    error = "";
                return error;


            }
        }

        public int EditDistributionContactByDistributionId(DistributionList contactDetails)
        {
            int distributionListId = 0;

            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("[Project_UpdateDistributionListChangeRequest]", sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@DistributionId", contactDetails.DistributionListID);
                sqlCommand.Parameters.AddWithValue("@ContactCompanyName", contactDetails.DistributionAccount);
                sqlCommand.Parameters.AddWithValue("@ContactFirstName", contactDetails.DistributionFirstName);
                sqlCommand.Parameters.AddWithValue("@ContactLastName", contactDetails.DistributionLastName);
                sqlCommand.Parameters.AddWithValue("@ContactAddress", contactDetails.DistributionStreet);
                sqlCommand.Parameters.AddWithValue("@ContactCity", contactDetails.DistributionCity);
                sqlCommand.Parameters.AddWithValue("@ContactState", contactDetails.DistributionState);
                sqlCommand.Parameters.AddWithValue("@ContactZipCode", contactDetails.DistributionZipCode);
                sqlCommand.Parameters.AddWithValue("@ContactCountry", contactDetails.DistributionCountry);
                sqlCommand.Parameters.AddWithValue("@ContactEmail", contactDetails.DistributionEmailID);
                sqlCommand.Parameters.AddWithValue("@Comments", contactDetails.Comments);
                sqlCommand.Parameters.AddWithValue("@AllJobs", contactDetails.AllJobs);
                sqlCommand.Parameters.AddWithValue("@ContactPhone", contactDetails.DistributionPhone);
                sqlCommand.Parameters.AddWithValue("@ContactFax", contactDetails.DistributionFax);

                if (ExecuteNonQuery(sqlCommand) != 0)
                {
                    distributionListId = contactDetails.DistributionListID;
                    return distributionListId;
                }
                else
                {
                    return distributionListId;
                }

            }

        }

        public int IsBatchExists(int projectNumber)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlcommand = new SqlCommand("[Project_IsBatchCreated]", cn);
                sqlcommand.CommandType = CommandType.StoredProcedure;
                sqlcommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                cn.Open();

                int i = (int)ExecuteScalar(sqlcommand);

                return i;


            }

        }

        public bool CheckAccountDetailsByEmail(string emailId)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    var cmd = new SqlCommand("Project_CheckAccountDetailsByEmail", cn);
                    cmd.Parameters.AddWithValue("@Email", emailId);
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

        public int CheckInCompleteBatches(int projectNumber)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand sqlcommand = new SqlCommand("[Project_CheckInCompleteBatchesByProjectNumber]", cn);
                sqlcommand.CommandType = CommandType.StoredProcedure;
                sqlcommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                cn.Open();

                int i = (int)ExecuteScalar(sqlcommand);

                return i;
            }

        }

        public int ProjectSalesRepEmail(int projectNumber, string DeliveryEmail)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    cn.Open();

                    SqlCommand cmd = new SqlCommand("spDeliveryEmail", cn);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProjectNumber", SqlDbType.Int).Value = projectNumber;
                    cmd.Parameters.Add("@DeliveryEmail", SqlDbType.VarChar).Value = DeliveryEmail;
                    cmd.Parameters.Add("@JobNumber", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@Batch_Number", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Time_In", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@DeadLine_Date", SqlDbType.DateTime).Value = DateTime.Now;
                    int i = ExecuteNonQuery(cmd);

                    return i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int ProjectHospitalityEmail(int projectNumber, string DeliveryEmail)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    cn.Open();

                    SqlCommand sqlcommand = new SqlCommand("[spDeliveryEmail]", cn);
                    sqlcommand.CommandType = CommandType.StoredProcedure;
                    sqlcommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                    sqlcommand.Parameters.AddWithValue("@DeliveryEmail", DeliveryEmail);
                    sqlcommand.Parameters.AddWithValue("@JobNumber", "");
                    sqlcommand.Parameters.AddWithValue("@BatchID", "");
                    sqlcommand.Parameters.AddWithValue("@Batch_Number", "");
                    sqlcommand.Parameters.AddWithValue("@Time_In", DateTime.Now);
                    sqlcommand.Parameters.AddWithValue("@DeadLine_Date", DateTime.Now);

                    int i = ExecuteNonQuery(sqlcommand);

                    return i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ProjectCostToDateHistory(ProjectCostToDateHistory ProjectCTD)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    cn.Open();

                    SqlCommand sqlcommand = new SqlCommand("[Sp_ProjectCostToDateHistory]", cn);
                    sqlcommand.CommandType = CommandType.StoredProcedure;
                    sqlcommand.Parameters.AddWithValue("@ProjectNumber",ProjectCTD.ProjectNumber);
                    sqlcommand.Parameters.AddWithValue("@Fromdate", ProjectCTD.Fromdate);
                    sqlcommand.Parameters.AddWithValue("@ToDate", ProjectCTD.ToDate);
                    sqlcommand.Parameters.AddWithValue("@DownloadLinkPath", ProjectCTD.DownloadLinkPath);
                    sqlcommand.Parameters.AddWithValue("@ModifiedBy", ProjectCTD.ModifiedBy);
                    sqlcommand.Parameters.AddWithValue("@ModifiedDate", ProjectCTD.ModifiedDate);
                    sqlcommand.Parameters.AddWithValue("@ModifiedIP", ProjectCTD.ModifiedIP);
                    sqlcommand.Parameters.AddWithValue("@Type", ProjectCTD.Types);
                    int i = ExecuteNonQuery(sqlcommand);
                    return i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        //public int GetParentandChildCompanyRelationship(int ParentcompName, int SubsidarycompName)
        //{
        //    using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        //    {

        //        SqlCommand sqlCommand = new SqlCommand("[Project_GetParentandChildCompanyRelationship]", sqlConnection);

        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        sqlCommand.Parameters.AddWithValue("@ParentcompName", ParentcompName);
        //        sqlCommand.Parameters.AddWithValue("@SubsidarycompName", SubsidarycompName);

        //        sqlConnection.Open();

        //        int i = (int)ExecuteScalar(sqlCommand);
        //        return i;
        //    }
        //}
        //public bool UpdateParentandChildCompanyRelationship(int ParentcompName, int SubsidarycompName)
        //{
        //    bool success = false;
        //    using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        //    {

        //        SqlCommand sqlCommand = new SqlCommand("[Project_UpdateParentandChildCompanyRelationship]", sqlConnection);
        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        sqlCommand.Parameters.AddWithValue("@ParentcompName", ParentcompName);
        //        sqlCommand.Parameters.AddWithValue("@SubsidarycompName", SubsidarycompName);

        //        sqlConnection.Open();

        //        int value = ExecuteNonQuery(sqlCommand);
        //        success = value == -1 ? true : success;
        //        return success;
        //    }
        //}
        //public DataTable GetParentCompanyDetails()
        //{

        //    DataTable dataSet = null;
        //    using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand sqlCommand;
        //        SqlDataAdapter sqlDataAdapter;
        //        sqlConnection.Open();
        //        sqlCommand = new SqlCommand();
        //        sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        //        dataSet = new DataTable("Data");
        //        sqlCommand.Connection = sqlConnection;

        //        sqlCommand.CommandText = "Project_GetParentCompanyNodes";
        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        dataSet.Load(sqlCommand.ExecuteReader());
        //        sqlDataAdapter = null;
        //        sqlCommand = null;
        //        sqlConnection.Close();
        //        return dataSet;
        //    }


        //}
        //public DataTable GetChildCompanyNodesByParent(string Nodevalue)
        //{

        //    DataTable dataSet = null;
        //    using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand sqlCommand;
        //        SqlDataAdapter sqlDataAdapter;
        //        sqlConnection.Open();
        //        sqlCommand = new SqlCommand();
        //        sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        //        dataSet = new DataTable("Data");
        //        sqlCommand.Connection = sqlConnection;

        //        sqlCommand.CommandText = "Project_GetChildCompanyNodesByParents";
        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        sqlCommand.Parameters.AddWithValue("@ParentID", Nodevalue);
        //        dataSet.Load(sqlCommand.ExecuteReader());
        //        sqlDataAdapter = null;
        //        sqlCommand = null;
        //        sqlConnection.Close();
        //        return dataSet;
        //    }
        //}

    }
}

