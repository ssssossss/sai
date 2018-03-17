using IFNBilling.DataProvider.Interfaces;
using IFNBilling.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider.MsSqlClient
{
  public class MsSqlJobProvider : JobProvider, IJobDataProvider
  {


    public  List<JobSearchVM> GetJobSearchResults(string jobNumber, string projectName, string company, int projectTypeId, int jobTypeId, int documentTypeId)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {


        if (jobNumber == null && projectName == null && company == null && jobTypeId == 0 && projectTypeId == 0 && documentTypeId == 0)
        {
          return new List<JobSearchVM>();
        }

        SqlCommand sqlCommand = new SqlCommand("[spGetJobSearchResults]", sqlConnection);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add("@JobNumber", SqlDbType.VarChar).Value=(String.IsNullOrEmpty(jobNumber)?(object)DBNull.Value:jobNumber);
        sqlCommand.Parameters.Add("@ProjectName", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(projectName) ? (object)DBNull.Value : projectName);
        sqlCommand.Parameters.Add("@Company", SqlDbType.NVarChar).Value = (String.IsNullOrEmpty(company) ? (object)DBNull.Value : company);
        sqlCommand.Parameters.AddWithValue("@JobTypeId", jobTypeId);
        sqlCommand.Parameters.AddWithValue("@ProjectTypeId", projectTypeId);
        sqlCommand.Parameters.AddWithValue("@DocumentTypeId", documentTypeId);


        sqlConnection.Open();
        return GetJobSearchCollectionFromReader(ExecuteReader(sqlCommand));
      }

    }

    

    public  List<JobSearchVM> GetProjectResultsByProjectId(int projectid,string Types=null)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {


            if (projectid == 0)
            {
                return new List<JobSearchVM>();
            }

            SqlCommand sqlCommand = new SqlCommand("[Job_GetJobSearchResultsByProjectId]", sqlConnection);

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@IN_ProjectId", projectid);
            sqlCommand.Parameters.AddWithValue("@Types", Types);
           sqlConnection.Open();
            return GetJobresultsCollectionFromReader(ExecuteReader(sqlCommand));
        }

    }

    #region AddOrModifyCompositionJob

    public  string AddOrModifyCompositionJob(Job job)
     {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            string job_number = string.Empty;
            SqlTransaction sqlTrans;
            cn.Open(); 
            sqlTrans = cn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("[Job_AddOrModifyCompositionJob]", cn);
                cmd.Transaction = sqlTrans;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", job.ProjectNumber);
                cmd.Parameters.AddWithValue("@JobNumber", !string.IsNullOrEmpty(job.JobNumber) ? job.JobNumber : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@JobTypeID", job.JobTypeID);
                cmd.Parameters.AddWithValue("@DocumentTypeID", job.DocumentTypeID.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : job.DocumentTypeID);
                cmd.Parameters.AddWithValue("@FileLocation", job.FileLocation == null ? null : job.FileLocation);
                cmd.Parameters.AddWithValue("@OthersDescription", job.OthersDescription);
                cmd.Parameters.AddWithValue("@CreatedBy", job.CreatedBy );
                cmd.Parameters.AddWithValue("@ModifiedBy",  job.ModifiedBy );
                cmd.Parameters.AddWithValue("@ModifiedByIP",  job.ModifiedByIP );
                cmd.Parameters.AddWithValue("@JobID", job.JobID);
                cmd.Parameters.AddWithValue("@IsCoverDesign", job.IsCoverDesign = job.IsCoverDesign == null ?  null : job.IsCoverDesign);
                cmd.Parameters.AddWithValue("@ActualFilingDate", job.ActualFilingDate);
                cmd.Parameters.AddWithValue("@IsCancelled", job.IsCancelled);
                cmd.Parameters.AddWithValue("@JobCancelReason", job.JobCancelReason);
                cmd.Parameters.AddWithValue("@Milestone", job.CodeMasterID);
                cmd.Parameters.AddWithValue("@AdditionalInformation", job.AdditionalInformation == null ? null : job.AdditionalInformation);
                cmd.Parameters.AddWithValue("@InhouseLocation", job.InhouseLocationID);
                cmd.Parameters.AddWithValue("@IsFilingToday", job.IsFilingToday);
                cmd.Parameters.AddWithValue("@IsBulkPrintToday", job.IsBulkPrintToday);
                cmd.Parameters.AddWithValue("@TranslationsBy", job.TranslationsBy == null ? (object)DBNull.Value : job.TranslationsBy);
                cmd.Parameters.AddWithValue("@VendorName", job.VendorName == null ? DBNull.Value.ToString() : job.VendorName);
                cmd.Parameters.AddWithValue("@DocumentTypeFlag", job.DocumentTypeFlag);
                cmd.Parameters.AddWithValue("@PrintType", job.PrintType == 0 ? (object)DBNull.Value : job.PrintType);
                cmd.Parameters.AddWithValue("@PrintDocRefID", job.PrintDocRefID == 0 ? (object)DBNull.Value : job.PrintDocRefID);
                cmd.Parameters.AddWithValue("@PrintDocOtherText", !string.IsNullOrEmpty(job.PrintDocOtherText) ? job.PrintDocOtherText : (object)DBNull.Value);

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@outReturnJobId";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 25;
                sqlParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParam);

                if(ExecuteNonQuery(cmd) != 0)
                {
                    job_number = Convert.ToString(cmd.Parameters["@outReturnJobId"].Value);
                }

                // Sprint19-TFS#43909/43446 - PhotoCopy/PrintOrder
                //if (job.JobTypeID == 3)
                //{
                //    foreach ( int DocumentTypeId in job.DocumentTypeIdList)
                //    CreatePrintBatch( cn,sqlTrans,Convert.ToInt32(job_number),job,DocumentTypeId);
                //}

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

            return job_number;
        }
    }



    #endregion AddOrModifyCompositionJob

    private void UpdateProjectStausAfterBatchCreation(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string jobNumber)
    {
        SqlCommand sqlCommand = new SqlCommand("[Project_UpdateProjectStatusAfterBatchCreation]", sqlConnection);
        sqlCommand.Transaction = sqlTransaction;
        sqlCommand.CommandType = CommandType.StoredProcedure;
        string[] projectNumber = jobNumber.Split('-');
        sqlCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber[0]);
        ExecuteNonQuery(sqlCommand);
    }


    #region AddOrModifyPrintJob

    public  string AddOrModifyPrintJob(Job job)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            string job_number = string.Empty;
            SqlTransaction sqlTrans;
            cn.Open();
            sqlTrans = cn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("[Job_AddOrModifyPrintJob]", cn);
                cmd.Transaction = sqlTrans;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", job.ProjectNumber);
                cmd.Parameters.AddWithValue("@JobNumber", job.JobNumber);
                cmd.Parameters.AddWithValue("@JobTypeID", job.JobTypeID);
                cmd.Parameters.AddWithValue("@DocumentTypeID", job.DocumentTypeID);
                cmd.Parameters.AddWithValue("@FileLocation", job.FileLocation);
                cmd.Parameters.AddWithValue("@OthersDescription", job.OthersDescription);
                cmd.Parameters.AddWithValue("@CreatedBy", !string.IsNullOrEmpty(job.CreatedBy) ? job.CreatedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedBy", !string.IsNullOrEmpty(job.ModifiedBy) ? job.ModifiedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedByIP", !string.IsNullOrEmpty(job.ModifiedByIP) ? job.ModifiedByIP : "system ip");

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ReturnJobNumber";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParam);



                ExecuteNonQuery(cmd);
                job_number = Convert.ToString(cmd.Parameters["@outReturnJobId"].Value);

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

            return job_number;
        }
    }

    #endregion AddOrModifyPrintJob

    #region AddOrModifyMediaJob

    public  string AddOrModifyMediaJob(Job job)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            string job_number = string.Empty;
            SqlTransaction sqlTrans;
            cn.Open();
            sqlTrans = cn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("[Job_AddOrModifyMediaJob]", cn);
                cmd.Transaction = sqlTrans;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", job.ProjectNumber);
                cmd.Parameters.AddWithValue("@JobNumber", job.JobNumber);
                cmd.Parameters.AddWithValue("@Jobid", job.JobID);
                cmd.Parameters.AddWithValue("@JobTypeID", job.JobTypeID);
                cmd.Parameters.AddWithValue("@DocumentTypeID", job.DocumentTypeID);
                cmd.Parameters.AddWithValue("@FileLocation", job.FileLocation);
                cmd.Parameters.AddWithValue("@OthersDescription", job.OthersDescription);
                cmd.Parameters.AddWithValue("@CreatedBy", !string.IsNullOrEmpty(job.CreatedBy) ? job.CreatedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedBy", !string.IsNullOrEmpty(job.ModifiedBy) ? job.ModifiedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedByIP", !string.IsNullOrEmpty(job.ModifiedByIP) ? job.ModifiedByIP : "system ip");

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@outReturnJobId";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 25;
                sqlParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParam);

                if (ExecuteNonQuery(cmd) != 0)
                {
                    job_number = Convert.ToString(cmd.Parameters["@outReturnJobIdr"].Value);
                }

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

            return job_number;
        }
    }

    #endregion AddOrModifyMediaJob

    #region AddOrModifyHospitalityJob

    public  string AddOrModifyHospitalityJob(Job job)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            string job_number = string.Empty;
            SqlTransaction sqlTrans;
            cn.Open();
            sqlTrans = cn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("[Job_AddOrModifyHospitalityJob]", cn);
                cmd.Transaction = sqlTrans;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", job.ProjectNumber);
                cmd.Parameters.AddWithValue("@JobNumber", job.JobNumber);
                cmd.Parameters.AddWithValue("@JobTypeID", job.JobTypeID);
                cmd.Parameters.AddWithValue("@DocumentTypeID", job.DocumentTypeID);
                cmd.Parameters.AddWithValue("@FileLocation", job.FileLocation);
                cmd.Parameters.AddWithValue("@OthersDescription", job.OthersDescription);
                cmd.Parameters.AddWithValue("@CreatedBy", !string.IsNullOrEmpty(job.CreatedBy) ? job.CreatedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedBy", !string.IsNullOrEmpty(job.ModifiedBy) ? job.ModifiedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedByIP", !string.IsNullOrEmpty(job.ModifiedByIP) ? job.ModifiedByIP : "system ip");

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ReturnJobNumber";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParam);

                ExecuteNonQuery(cmd);
                job_number = Convert.ToString(cmd.Parameters["@ReturnJobNumber"].Value);

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

            return job_number;
        }
    }

    #endregion AddOrModifyHospitalityJob

    #region AddOrModifySideJob

    public  string AddOrModifySideJob(Job job)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            string job_number = string.Empty;
            SqlTransaction sqlTrans;
            cn.Open();
            sqlTrans = cn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("[Job_AddOrModifySideJob]", cn);
                cmd.Transaction = sqlTrans;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", job.ProjectNumber);
                cmd.Parameters.AddWithValue("@JobNumber", job.JobNumber);
                cmd.Parameters.AddWithValue("@JobTypeID", job.JobTypeID);
                cmd.Parameters.AddWithValue("@DocumentTypeID", job.DocumentTypeID);
                cmd.Parameters.AddWithValue("@FileLocation", job.FileLocation);
                cmd.Parameters.AddWithValue("@OthersDescription", job.OthersDescription);
                cmd.Parameters.AddWithValue("@CreatedBy", !string.IsNullOrEmpty(job.CreatedBy) ? job.CreatedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedBy", !string.IsNullOrEmpty(job.ModifiedBy) ? job.ModifiedBy : "system");
                cmd.Parameters.AddWithValue("@ModifiedByIP", !string.IsNullOrEmpty(job.ModifiedByIP) ? job.ModifiedByIP : "system ip");

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ReturnJobNumber";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParam);

                ExecuteNonQuery(cmd);
                job_number = Convert.ToString(cmd.Parameters["@ReturnJobNumber"].Value);

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

            return job_number;
        }
    }

    #endregion AddOrModifySideJob


    public void CreatePrintBatch(SqlConnection cn, SqlTransaction sqlTrans,int jobNumber,Job job,int DocumentTypeId)
    {
        int BatchID = 0;
                    SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", cn);
                    sqlCommand.Transaction = sqlTrans;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ServiceTypeId", 3);
                    sqlCommand.Parameters.AddWithValue("@BatchStatus", 1);
                    sqlCommand.Parameters.AddWithValue("@JobNumber", job.JobNumber);
                    sqlCommand.Parameters.AddWithValue("@BatchID", 0);
                    sqlCommand.Parameters.AddWithValue("@JobId", jobNumber);
                    sqlCommand.Parameters.AddWithValue("@CreatedBy", job.CreatedBy);
                    sqlCommand.Parameters.AddWithValue("@ModifiedBy", job.ModifiedBy);
                    sqlCommand.Parameters.AddWithValue("@ModifiedByIP", job.ModifiedByIP);

 
                    SqlDataReader oReader = sqlCommand.ExecuteReader();



                    if (oReader.HasRows == true)
                    {
                        oReader.Read();
                        BatchID = (int)oReader[0];

                    }
                    oReader.Close();


                    if (BatchID != 0)
                    {
                        sqlCommand = new SqlCommand("[Job_AutoCreatePrintBatch]", cn);
                        sqlCommand.Transaction = sqlTrans;
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@DocumentTypeId", DocumentTypeId);
                        sqlCommand.Parameters.AddWithValue("@PrintStartTime", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@BatchId", BatchID);
                        sqlCommand.Parameters.AddWithValue("@JobId", jobNumber);
                        ExecuteNonQuery(sqlCommand);

                    }

                   // UpdateProjectStausAfterBatchCreation(cn, sqlTrans, jobNumber);
    }

    public  JobSearchVM GetJobDetailsByJobID(int jobId)
    {
        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                if (jobId == 0)
                {
                    return new JobSearchVM();
                }

                SqlCommand sqlCommand = new SqlCommand("[Job_GetJobDetailsByJobID]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@JobID", jobId);
                sqlConnection.Open();
                return GetJobDetailsByJobIDFromReader(ExecuteReader(sqlCommand));
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public List<JobSearchVM> GetActiveJobs()
    {
        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[Job_GetACtiveJobs]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();
                return GetJobSearchCollectionFromReader(ExecuteReader(sqlCommand));
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }





    public int InsertUpdateExpenseTracking(HospitalityExpense hospitalityExpense)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        sqlConnection.Open();
        SqlCommand sqlCommand = new SqlCommand("[Job_InsertUpdateExpenseTracking]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@Date", hospitalityExpense.ExpenseDate);
        sqlCommand.Parameters.AddWithValue("@MerchantName", hospitalityExpense.MerchantName);
        sqlCommand.Parameters.AddWithValue("@ExpenseTypeID", hospitalityExpense.ExpenseTypeID);
        sqlCommand.Parameters.AddWithValue("@IsClientBillable", hospitalityExpense.IsClientBillable);
        sqlCommand.Parameters.AddWithValue("@IsServiceCharges", hospitalityExpense.IsServiceCharges);
        sqlCommand.Parameters.AddWithValue("@Comments", hospitalityExpense.Comments);
        sqlCommand.Parameters.AddWithValue("@BillPercentage", hospitalityExpense.BillPercentage);
        sqlCommand.Parameters.AddWithValue("@BillAmount", hospitalityExpense.BillAmount);
        sqlCommand.Parameters.AddWithValue("@AmountChargeable", hospitalityExpense.AmountChargeable);
        sqlCommand.Parameters.AddWithValue("@ServiceChargePercent", hospitalityExpense.ServiceChargePercent);
        sqlCommand.Parameters.AddWithValue("@ServiceChargeAmount", hospitalityExpense.ServiceChargeAmount);
        sqlCommand.Parameters.AddWithValue("@TotalExpenseAmount", hospitalityExpense.TotalExpenseAmount);
        sqlCommand.Parameters.AddWithValue("@HospitalityExpenseID", hospitalityExpense.HospitalityExpenseID);
        sqlCommand.Parameters.AddWithValue("@JobID", hospitalityExpense.JobID);
        sqlCommand.Parameters.AddWithValue("@ExpenseTypeDescription", hospitalityExpense.ExpenseTypeDescription); 


        SqlParameter sqlParameter = new SqlParameter();
        sqlParameter.ParameterName = "@OutHospitalityExpenseId";
        sqlParameter.SqlDbType = SqlDbType.Int;
        sqlParameter.Size = 15;
        sqlParameter.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlParameter);
        int hospitalityExpenseId = 0;
        ExecuteNonQuery(sqlCommand) ;
        if (hospitalityExpense.HospitalityExpenseID!= 0)
        {

          hospitalityExpenseId = hospitalityExpense.HospitalityExpenseID;
          return hospitalityExpenseId;
        }

        hospitalityExpenseId = Convert.ToInt32(sqlCommand.Parameters["@OutHospitalityExpenseId"].Value);
        return hospitalityExpenseId;
      }
    }

    public List<HospitalityExpense> GetExpenseTrackingByJobId(int jobId)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        sqlConnection.Open();
        SqlCommand sqlCommand = new SqlCommand("[Job_GetExpenseTrackingByJobId]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobID", jobId);
        return GetHospitalityExpenseCollectionFromReader(ExecuteReader(sqlCommand));
      }
      
    }

    public JobSearchVM GetJobnumberandJobType(string JobNumber)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("[Master_GetJobidandJobType]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@In_JobID", JobNumber);
            return GetJobnumberandJobTypeFromReader(ExecuteReader(sqlCommand));
        }
    }


    public int AddOrModifyJobOverTimeHours(JobOverTimeHours job_overtime_hours, string type)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("spJobOverTime_InsertUpdateDetails", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@OverTimeID", job_overtime_hours.OverTimeID);
            sqlCommand.Parameters.AddWithValue("@ProjectID", job_overtime_hours.ProjectID);
            sqlCommand.Parameters.AddWithValue("@ProjectStatusID", job_overtime_hours.ProjectStatusID);
            sqlCommand.Parameters.AddWithValue("@ProjectTypeID", job_overtime_hours.ProjectTypeID);
            sqlCommand.Parameters.AddWithValue("@JobNumber", job_overtime_hours.JobNumber);
            sqlCommand.Parameters.AddWithValue("@JobTypeID", job_overtime_hours.JobTypeID);
            sqlCommand.Parameters.AddWithValue("@Date", job_overtime_hours.Date);
            sqlCommand.Parameters.AddWithValue("@OverTime", job_overtime_hours.OverTimeName);
            sqlCommand.Parameters.AddWithValue("@OverTimeHours", job_overtime_hours.OverTimeHours);
            sqlCommand.Parameters.AddWithValue("@OverTimeDoneBy", job_overtime_hours.OverTimeDoneBy);
            sqlCommand.Parameters.AddWithValue("@Active", job_overtime_hours.Active);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", job_overtime_hours.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@CreatedDate", job_overtime_hours.CreatedDate);
            sqlCommand.Parameters.AddWithValue("@OperationMode", type);
            sqlCommand.Parameters.AddWithValue("@Comments", job_overtime_hours.Comments);
            sqlCommand.Parameters.AddWithValue("@DocumentTypeID", job_overtime_hours.DocumentTypeID);
            //Sprint 21#TFS 44278,44472 UI - OverTime ChangesUI - OverTime Changes validation
            sqlCommand.Parameters.AddWithValue("@EndDate", job_overtime_hours.EndDate);
            sqlCommand.Parameters.AddWithValue("@LateBooking", job_overtime_hours.LateBooking);
            sqlCommand.Parameters.AddWithValue("@Cancellation", job_overtime_hours.Cancellation);
            sqlCommand.Parameters.AddWithValue("@OTShift", job_overtime_hours.OTShift);
            
            int job_overtime_hrs = 0;
            job_overtime_hrs = ExecuteNonQuery(sqlCommand);

            return job_overtime_hrs;
        }
    }

    public List<EmailDistribution> GetEmailDistributionList(EmailDistribution emailDistribution, string type)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("spGetEmailDistribution", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@EmailDistributionID", emailDistribution.EmailDistributionID);
            sqlCommand.Parameters.AddWithValue("@Projectnumber", emailDistribution.Projectnumber);
            sqlCommand.Parameters.AddWithValue("@OperationMode", type);
            return GetEmailDistributionCollectionFromReader(ExecuteReader(sqlCommand));
        }
    }




    public int InsertAndUpdateEmailDistribution(EmailDistribution emailDistribution, string type)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("[spInsertUpdateEmailDistribution]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@EmailDistributionID", emailDistribution.EmailDistributionID);
            sqlCommand.Parameters.AddWithValue("@Jobnumber", emailDistribution.Jobnumber);
            sqlCommand.Parameters.AddWithValue("@Projectnumber", emailDistribution.Projectnumber);
            sqlCommand.Parameters.AddWithValue("@DistributionNumber", emailDistribution.DistributionNumber);
            sqlCommand.Parameters.AddWithValue("@FromEmail", emailDistribution.FromEmail);
            sqlCommand.Parameters.AddWithValue("@ToEmail", emailDistribution.ToEmail);
            sqlCommand.Parameters.AddWithValue("@CC", emailDistribution.CC);
            sqlCommand.Parameters.AddWithValue("@BCC", emailDistribution.BCC);
            sqlCommand.Parameters.AddWithValue("@Subject", emailDistribution.Subject);
            sqlCommand.Parameters.AddWithValue("@Createby", emailDistribution.Createby);
            sqlCommand.Parameters.AddWithValue("@CreatedDateTime", emailDistribution.CreatedDateTime);
            sqlCommand.Parameters.AddWithValue("@EmailMessageContent", emailDistribution.EmailMessageContent);
            sqlCommand.Parameters.AddWithValue("@SendersLogin", emailDistribution.SendersLogin);
            sqlCommand.Parameters.AddWithValue("@SendersDateTime", emailDistribution.SendersDateTime);
            sqlCommand.Parameters.AddWithValue("@Send", emailDistribution.Send);
            sqlCommand.Parameters.AddWithValue("@OperationMode", type);
            SqlParameter IDParameter = new SqlParameter("@EmailDistribution_ID", SqlDbType.Int);
            IDParameter.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(IDParameter);
           
                int email_Distribution = 0;
                 ExecuteNonQuery(sqlCommand);
                 return email_Distribution = (int)IDParameter.Value;
        }
    }



    public List<JobOverTimeHours> GetJobOverTimeHours(int project_id, int project_type_id, int project_status_id, string job_number, int job_type_id, string type)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("spGetJobOverTimeHoursByType", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@ProjectID", project_id);
            sqlCommand.Parameters.AddWithValue("@ProjectTypeID", project_type_id);
            sqlCommand.Parameters.AddWithValue("@ProjectStatusID", project_status_id);
            sqlCommand.Parameters.AddWithValue("@JobNumber", job_number);
            sqlCommand.Parameters.AddWithValue("@JobTypeID", job_type_id);
            sqlCommand.Parameters.AddWithValue("@OperationMode", type);

            return GetJobOverTimeHoursCollectionFromReader(ExecuteReader(sqlCommand));
        }
    }



     public bool deleteHospitalityExpenseTracking(int hospitalityExpenseId)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("Job_DeleteHospitalityExpenseTracking", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@hospitalityExpenseID", hospitalityExpenseId);


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

     /// <summary>
     /// Updates the job status by job identifier.
     /// </summary>
     /// <param name="JobId">The job identifier.</param>
     /// <returns></returns>
     public bool UpdateJobStatusByJobID(int JobId, string Type = null)
     {
         using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
         {
             sqlConnection.Open();
             SqlCommand sqlCommand = new SqlCommand("UpdateJobStatusByJobID", sqlConnection);
             sqlCommand.CommandType = CommandType.StoredProcedure;
             sqlCommand.Parameters.AddWithValue("@JobId", JobId);
             sqlCommand.Parameters.AddWithValue("@Type", Type != null ? Type : "1");
             ExecuteNonQuery(sqlCommand);
            return true;
         }
     }



     public bool insertMerchantName(string merchantName)
     {
       using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
       {
         sqlConnection.Open();
         SqlCommand sqlCommand = new SqlCommand("job_InsertMerchantName", sqlConnection);
         sqlCommand.CommandType = CommandType.StoredProcedure;
         sqlCommand.Parameters.AddWithValue("@MerchantName", merchantName);


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

  }
}
