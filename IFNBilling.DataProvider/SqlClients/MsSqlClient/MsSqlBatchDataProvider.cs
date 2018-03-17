using IFNBilling.DataProvider.Interfaces;
using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider.MsSqlClient
{
  public class MsSqlBatchDataProvider : BatchDataProvider, IBatchDataProvider
  {
    public int InsertTypesetBatchDetails(TypeSetBatch typesetBatch)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        int BatchID = 0;
        SqlTransaction sqlTrans;
        sqlConnection.Open();
        sqlTrans = sqlConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        int servicetypeid = 0;
        int translationsBy = 0;
        bool isTranslationFailed = false;


        try
        {
            BatchID = CreateTypesetBatchOnDemand(sqlConnection, sqlTrans, typesetBatch);

            //Check whether the Job has translation service
            //TFS -43725,42830,42831,42832- Composition - Copy batch -Modify QE batch to spin over jobs
            if (typesetBatch.QEbatch == "Typesetting")
            {
                UpdateDestinationCopyBatch(sqlConnection, sqlTrans, Convert.ToInt32(BatchID), typesetBatch.JobNumber, "COPYUPDATEDESTINATION");
                //TFS - 43884 Copy Batch - Part II - Tracking for the Source and Destination Batches.
                InsertCopyBatchTracking(sqlConnection, sqlTrans, typesetBatch.IsCopyBatchid, typesetBatch.IsCopySourceJobnumber, BatchID, typesetBatch.JobNumber, typesetBatch.Batch.CreatedBy);
            }

            servicetypeid = GetServiceTypeOnDemand(sqlConnection, sqlTrans, typesetBatch.JobID);
            translationsBy = GetTranslationsByOnDemand(sqlConnection, sqlTrans, typesetBatch.JobID);


            if (typesetBatch.Batch.BatchID == 0 && servicetypeid == 7 && translationsBy == 1)
            {
                TranslationBatch transbatchdetails = new TranslationBatch();
                Batch batch = new Batch();

                if (typesetBatch.QEbatch != null)
                {
                    transbatchdetails.NoOfPages = 0;
                    transbatchdetails.AlterationPages = 0;
                    transbatchdetails.NewChinesePages = 0;
                    transbatchdetails.AlterationChinesePages = 0;
                    transbatchdetails.TypeOfTransServiceCode = 0;
                }

                if (typesetBatch.QEbatch == "Typesetting")
                {
                    transbatchdetails.PublicHoliday = typesetBatch.PublicHoliday;
                    transbatchdetails.Weekend = typesetBatch.Weekend;
                    transbatchdetails.ClientInHouse = typesetBatch.ClientInHouse;
                }
                //else
                //{
                //    transbatchdetails.NoOfPages = typesetBatch.NewPages;
                //    //transbatchdetails.AlterationPages = typesetBatch.AlterationPages; 
                //}         

                if (typesetBatch.TypeSetLanguageCode == 13)
                {
                    transbatchdetails.TypeOfTransServiceCode = (typesetBatch.InstructionRcvdCode == 32) ? 7 : 1;
                    transbatchdetails.NoOfPages = Convert.ToInt32(typesetBatch.NewPages);
                    transbatchdetails.AlterationPages = Convert.ToInt32(typesetBatch.AlterationPages);
                    transbatchdetails.ClonedEnglish = Convert.ToInt32(typesetBatch.ClonedEnglish);
                    transbatchdetails.ClonedChinese = 0;
                }
                else if (typesetBatch.TypeSetLanguageCode == 14)
                {
                    transbatchdetails.TypeOfTransServiceCode = (typesetBatch.InstructionRcvdCode == 32) ? 7 : 2;
                    transbatchdetails.NewChinesePages = Convert.ToInt32(typesetBatch.NewChinesePages);
                    transbatchdetails.AlterationChinesePages = Convert.ToInt32(typesetBatch.AlterationChinesePages);
                    transbatchdetails.ClonedChinese = Convert.ToInt32(typesetBatch.ClonedChinese);
                    transbatchdetails.ClonedEnglish = 0;
                }
                else if (typesetBatch.TypeSetLanguageCode == 15)
                {
                    transbatchdetails.TypeOfTransServiceCode = (typesetBatch.InstructionRcvdCode == 32) ? 7 : 6;
                    transbatchdetails.NoOfPages = Convert.ToInt32(typesetBatch.NewPages);
                    transbatchdetails.AlterationPages = Convert.ToInt32(typesetBatch.AlterationPages);
                    transbatchdetails.NewChinesePages = Convert.ToInt32(typesetBatch.NewChinesePages);
                    transbatchdetails.AlterationChinesePages = Convert.ToInt32(typesetBatch.AlterationChinesePages);
                    transbatchdetails.ClonedEnglish = Convert.ToInt32(typesetBatch.ClonedEnglish);
                    transbatchdetails.ClonedChinese = Convert.ToInt32(typesetBatch.ClonedChinese);
                }
                transbatchdetails.CompletedDate = (typesetBatch.InstructionRcvdCode == 32) ? DateTime.Now : transbatchdetails.CompletedDate;
                transbatchdetails.Deadline = typesetBatch.TranslationDeadline == null ? (Nullable<DateTime>)null : Convert.ToDateTime(typesetBatch.TranslationDeadline);
                transbatchdetails.TimeIn = typesetBatch.TimeIn;
                transbatchdetails.TurnAroundCode = typesetBatch.TranslationTurnAroundCode;
                //transbatchdetails.OverTimeCode = typesetBatch.;

                transbatchdetails.JobNumber = typesetBatch.JobNumber;
                batch.BatchStatus = (typesetBatch.InstructionRcvdCode == 32) ? 5 : typesetBatch.Batch.BatchStatus;
                batch.CreatedBy = typesetBatch.Batch.CreatedBy;
                batch.ModifiedBy = typesetBatch.Batch.ModifiedBy;
                transbatchdetails.Batch = batch;
                //transbatchdetails.OvertimeHours = 0;
                transbatchdetails.ChinesetoTradChinese = 0;
                //transbatchdetails.Comments = "";
                transbatchdetails.Comments = typesetBatch.TranslationComments;
                transbatchdetails.JobId = typesetBatch.JobID;

                int translationBatchId = InsertTranslationBatch(sqlConnection, sqlTrans, transbatchdetails, false);

                if (translationBatchId.Equals(-1))
                    isTranslationFailed = true;

                if (!isTranslationFailed)
                {
                    MsSqlUserRoleProvider AuditTrailLogProvider = new MsSqlUserRoleProvider();
                    AuditTrailLogProvider.SetAuditTrailLog(new AuditTrailLog
                    {
                        LoginID = typesetBatch.Batch.CreatedBy,
                        Module = "TRANSLATION BATCH",
                        Action = "C",
                        Comments = string.Format("BatchID : {0} | Batch Number: {1} | Job Number : {2}", translationBatchId, transbatchdetails.BatchNumber.Equals(0) ? 1 : transbatchdetails.BatchNumber, transbatchdetails.JobNumber),
                        JobId = typesetBatch.JobID,
                        BatchId = Convert.ToInt64(translationBatchId),
                    });


                    string[] project_number = transbatchdetails.JobNumber.Split('-');
                    //TFS -43725,42830,42831,42832- Composition - Copy batch -Modify QE batch to spin over jobs
                    if (typesetBatch.QEbatch == "Typesetting")
                    {
                        UpdateDestinationCopyBatch(sqlConnection, sqlTrans, Convert.ToInt32(translationBatchId), transbatchdetails.JobNumber, "COPYUPDATEDESTINATION");
                        //TFS - 43884 Copy Batch - Part II - Tracking for the Source and Destination Batches.
                        InsertCopyBatchTracking(sqlConnection, sqlTrans, typesetBatch.IsCopyBatchid, typesetBatch.IsCopySourceJobnumber, translationBatchId, typesetBatch.JobNumber, typesetBatch.Batch.CreatedBy);
                    }

                    JobTranslationsEmailOnDemand(sqlConnection, sqlTrans, Convert.ToInt32(project_number[0]), "Job_TranslationsEmail", transbatchdetails.JobNumber, translationBatchId, "0", transbatchdetails.TimeIn, transbatchdetails.Deadline);


                    if (typesetBatch.fileRepositoryList.Count > 0)
                    {
                        foreach (var file in typesetBatch.fileRepositoryList)
                        {
                            FileRepository fileRepository = new FileRepository();
                            fileRepository.FileName = file.FileName;
                            fileRepository.FileType = file.FileType;
                            fileRepository.ProjectID = file.ProjectID;

                            fileRepository.BatchID = Convert.ToInt64(translationBatchId);
                            string jobNumber = file.jobNumber;
                            fileRepository.ModifiedBy = file.ModifiedBy;
                            fileRepository.typesetBatchId = BatchID;
                            fileRepository.FilePath = GetFilePath(fileRepository.typesetBatchId, Convert.ToInt64(fileRepository.ProjectID), jobNumber);
                            bool success = FileDataProvider.Instance.InsertUpdateFileRepository(fileRepository);

                        }
                    }
                }
            }

            if (!isTranslationFailed)
            {
                UpdateProjectStausAfterBatchCreation(sqlConnection, sqlTrans, typesetBatch.JobNumber);

                sqlTrans.Commit();
                sqlConnection.Close();
            }
            else
            {
                sqlTrans.Rollback();
                sqlConnection.Close();
                BatchID = -1;
            }
        }
        catch (Exception exTrans)
        {
            if (sqlTrans.Connection != null)
            {
                BatchID = -1;
                sqlTrans.Rollback();
                sqlConnection.Close();
               // throw exTrans;
            }
            //try
            //{
            //  sqlTrans.Rollback();
            //}
            //catch (Exception exRlbk)
            //{
            //  sqlConnection.Close();
            //  throw exRlbk;
            //}

        }

        return BatchID;
      }

    }

    public int InsertTranslationBatch(SqlConnection sqlConnection, SqlTransaction sqlTrans,TranslationBatch translationBatch, bool IsSideJob)
    {
            int BatchID = 0;
            try
            {
                SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", sqlConnection);
                sqlCommand.Transaction = sqlTrans;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ServiceTypeId", IsSideJob ? 8 : 2);
                sqlCommand.Parameters.AddWithValue("@BatchStatus", translationBatch.Batch.BatchStatus);
                sqlCommand.Parameters.AddWithValue("@JobId", translationBatch.JobId);
                sqlCommand.Parameters.AddWithValue("@JobNumber", translationBatch.JobNumber);
                sqlCommand.Parameters.AddWithValue("@BatchID", translationBatch.Batch.BatchID);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", translationBatch.Batch.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@ModifiedBy", translationBatch.Batch.ModifiedBy);
                sqlCommand.Parameters.AddWithValue("@ModifiedByIP", translationBatch.Batch.ModifiedByIP);
                SqlDataReader oReader = sqlCommand.ExecuteReader();
                if (oReader.HasRows == true)
                {
                    oReader.Read();
                    BatchID = (int)oReader[0];
                }
                oReader.Close();
                if (BatchID != 0)
                {
                    sqlCommand = IsSideJob
                        ? new SqlCommand("[Batch_InsertUpdateSideJobData]", sqlConnection)
                        : new SqlCommand("[Batch_InsertUpdateTranslationData]", sqlConnection);
                    sqlCommand.Transaction = sqlTrans;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SetTranslation_SideJobValues(sqlCommand, translationBatch);
                    sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
                    if (IsSideJob)
                    {
                        sqlCommand.Parameters.AddWithValue("@NoOfWords", translationBatch.NoOfWords);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@IsCopyofTypeset", translationBatch.IsCopyForTypesetSrvc);
                        int servicetypeid = 0;
                        servicetypeid = GetServiceTypeOnDemand(sqlConnection, sqlTrans, translationBatch.JobId);

                        if (translationBatch.IsCopyForTypesetSrvc && servicetypeid == 7)
                        {
                            TypeSetBatch typstbatchdetails = new TypeSetBatch();
                            Batch batch = new Batch();

                            if (translationBatch.QEBatch != "")
                            {
                                typstbatchdetails.NewPages = 0;
                                typstbatchdetails.NewChinesePages = 0;
                                typstbatchdetails.AlterationPages = 0;
                                typstbatchdetails.AlterationChinesePages = 0;
                                // typstbatchdetails.Deadline = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
                                typstbatchdetails.Deadline = Convert.ToDateTime(translationBatch.Deadline);
                                typstbatchdetails.Weekend = translationBatch.Weekend;
                                typstbatchdetails.PublicHoliday = translationBatch.PublicHoliday;
                                typstbatchdetails.ClientInHouse = translationBatch.ClientInHouse;
                            }
                            else
                            {
                                //typstbatchdetails.NewPages = translationBatch.NoOfPages;
                                //typstbatchdetails.AlterationPages = translationBatch.AlterationPages;
                                typstbatchdetails.Deadline = Convert.ToDateTime(translationBatch.Deadline);
                            }
                            //typstbatchdetails.NewPages = translationBatch.NoOfPages; 
                            typstbatchdetails.TimeIn = translationBatch.TimeIn;
                            typstbatchdetails.TurnAroundCode = translationBatch.TurnAroundCode;
                            //  typstbatchdetails.OverTimeCode = translationBatch.OverTimeCode;
                            //typstbatchdetails.OvertimeHours = translationBatch.OvertimeHours;
                            typstbatchdetails.BlacklinesEnglish = typstbatchdetails.BlacklinesChinese = 0;
                            typstbatchdetails.InstructionRcvdCode = translationBatch.TypeOfTransServiceCode == 7 ? 32 : 0;
                            typstbatchdetails.JobNumber = translationBatch.JobNumber;
                            typstbatchdetails.PDFPages = 0;
                            typstbatchdetails.Strikethrough = 0;
                            typstbatchdetails.WaterMark = 0;
                            typstbatchdetails.WordFileConversion = 0;
                            typstbatchdetails.Graphics = 0;
                            typstbatchdetails.GlobalSearch = 0;
                            typstbatchdetails.FreshType = 0;
                            typstbatchdetails.BlackliningCumulativeEnglish = typstbatchdetails.BlackliningCumulativeChinese = 0;
                            typstbatchdetails.BlackliningAgainstVersionEnglish = typstbatchdetails.BlackliningAgainstVersionChinese = 0;
                            typstbatchdetails.DocumentConversion = 0;
                            typstbatchdetails.PrinterError = 0;
                            typstbatchdetails.Comments = translationBatch.Comments != null ? translationBatch.Comments : "";
                            //typstbatchdetails.Comments = translationBatch.Comments;
                            typstbatchdetails.Attachment = "test";
                            batch.BatchStatus = 1;
                            batch.CreatedBy = translationBatch.Batch.CreatedBy;
                            batch.ModifiedBy = translationBatch.Batch.ModifiedBy;
                            typstbatchdetails.Batch = batch;
                            translationBatch.TypeOfTransServiceCode = translationBatch.TypeOfTransServiceCode != 0 ? translationBatch.TypeOfTransServiceCode : 0;
                            typstbatchdetails.TypeSetLanguageCode = 0;
                            if (translationBatch.TypeOfTransServiceCode == 1 || translationBatch.TypeOfTransServiceCode == 3)
                            {
                                // Translation = English to Chinese || English to Chinese Alterations ,   Typeset = Chinese
                                typstbatchdetails.TypeSetLanguageCode = 14;
                                //typstbatchdetails.NewPages = 0;
                                //typstbatchdetails.AlterationPages = 0;
                                typstbatchdetails.NewChinesePages = translationBatch.NoOfPages;
                                typstbatchdetails.AlterationChinesePages = translationBatch.AlterationPages;
                                typstbatchdetails.ClonedChinese = translationBatch.ClonedEnglish;

                            }
                            else if (translationBatch.TypeOfTransServiceCode == 2 ||
                                     translationBatch.TypeOfTransServiceCode == 4)
                            {
                                // Translation = Chinese to English || Chinese to English Alterations,   Typeset = English
                                typstbatchdetails.TypeSetLanguageCode = 13;
                                typstbatchdetails.NewPages = translationBatch.NewChinesePages;
                                typstbatchdetails.AlterationPages = translationBatch.AlterationChinesePages;
                                typstbatchdetails.ClonedEnglish = translationBatch.ClonedChinese;
                                //typstbatchdetails.NewChinesePages = 0;
                                //typstbatchdetails.AlterationChinesePages = 0;
                            }
                            else if (translationBatch.TypeOfTransServiceCode == 5)
                            {
                                // Translation = No Translation,   Typeset = Chinese
                                typstbatchdetails.TypeSetLanguageCode = 14;
                                typstbatchdetails.NewChinesePages = translationBatch.NewChinesePages;
                                typstbatchdetails.AlterationChinesePages = translationBatch.AlterationChinesePages;
                                typstbatchdetails.ClonedChinese = translationBatch.ClonedChinese;
                            }
                            else if (translationBatch.TypeOfTransServiceCode == 6 || translationBatch.TypeOfTransServiceCode == 7)
                            {
                                // Translation = Both || Not To Be Billed,   Typeset = Both 
                                typstbatchdetails.TypeSetLanguageCode = 15;
                                typstbatchdetails.AlterationPages = translationBatch.AlterationPages;
                                typstbatchdetails.NewPages = translationBatch.NoOfPages;
                                typstbatchdetails.NewChinesePages = translationBatch.NewChinesePages;
                                typstbatchdetails.AlterationChinesePages = translationBatch.AlterationChinesePages;
                                typstbatchdetails.ClonedEnglish = translationBatch.ClonedEnglish;
                                typstbatchdetails.ClonedChinese = translationBatch.ClonedChinese;
                            }
                            int typesetBatchId = CreateTypesetBatchOnDemand(sqlConnection, sqlTrans, typstbatchdetails);


                            MsSqlUserRoleProvider AuditTrailLogProvider = new MsSqlUserRoleProvider();
                            AuditTrailLogProvider.SetAuditTrailLog(new AuditTrailLog
                            {
                                LoginID = translationBatch.Batch.CreatedBy,
                                Module = IsSideJob ? "SIDEJOB BATCH" : "TYPESET BATCH",
                                Action = "C",
                                Comments = string.Format("BatchID : {0} | Batch Number: {1} | Job Number : {2}", typesetBatchId, typstbatchdetails.BatchNumber.Equals(0) ? 1 : typstbatchdetails.BatchNumber, typstbatchdetails.JobNumber),
                                JobId = translationBatch.JobId,
                                BatchId = Convert.ToInt64(typesetBatchId)
                            });



                            if (translationBatch.QEBatch == "Translation")
                            {
                                UpdateDestinationCopyBatch(sqlConnection, sqlTrans, Convert.ToInt32(typesetBatchId), typstbatchdetails.JobNumber, "COPYUPDATEDESTINATION");
                                InsertCopyBatchTracking(sqlConnection, sqlTrans, translationBatch.IsCopyBatchid, translationBatch.IsCopySourceJobnumber, typesetBatchId, translationBatch.JobNumber, translationBatch.Batch.CreatedBy);
                            }

                            string[] project_number = typstbatchdetails.JobNumber.Split('-');
                            JobTranslationsEmailOnDemand(sqlConnection, sqlTrans, Convert.ToInt32(project_number[0]),
                                "Job_TranslationsEmail", typstbatchdetails.JobNumber, typesetBatchId, "0",
                                typstbatchdetails.TimeIn, typstbatchdetails.Deadline);
                            if (translationBatch.fileRepositoryList.Count > 0)
                            {
                                foreach (var file in translationBatch.fileRepositoryList)
                                {
                                    FileRepository fileRepository = new FileRepository();
                                    fileRepository.FileName = file.FileName;
                                    fileRepository.FileType = file.FileType;
                                    fileRepository.ProjectID = file.ProjectID;

                                    fileRepository.BatchID = Convert.ToInt64(typesetBatchId);
                                    string jobNumber = file.jobNumber;
                                    fileRepository.ModifiedBy = file.ModifiedBy;
                                    fileRepository.translationBatchId = BatchID;
                                    fileRepository.FilePath = GetFilePath(fileRepository.translationBatchId,
                                        Convert.ToInt64(fileRepository.ProjectID), jobNumber);
                                    bool success = FileDataProvider.Instance.InsertUpdateFileRepository(fileRepository);

                                }
                            }

                        }
                    }
                    ExecuteNonQuery(sqlCommand);
                    if (translationBatch.QEBatch == "Translation")
                    {
                        UpdateDestinationCopyBatch(sqlConnection, sqlTrans, Convert.ToInt32(BatchID), translationBatch.JobNumber, "COPYUPDATEDESTINATION");
                        InsertCopyBatchTracking(sqlConnection, sqlTrans, translationBatch.IsCopyBatchid, translationBatch.IsCopySourceJobnumber, BatchID, translationBatch.JobNumber, translationBatch.Batch.CreatedBy);
                    }
                    UpdateProjectStausAfterBatchCreation(sqlConnection, sqlTrans, translationBatch.JobNumber);
                }

               // sqlTrans.Commit();
               // sqlConnection.Close();
            }
            catch (Exception exTrans)
            {
                BatchID = -1;
                try
                {
                    sqlTrans.Rollback();
                }
                catch (Exception exRlbk)
                {

                  //  sqlConnection.Close();
                    //throw exRlbk;
                }
               // sqlConnection.Close();
                //throw exTrans;
            }

            return BatchID;
        

    }



    /// <summary>
    /// Updates the destination copy batch.
    /// </summary>
    /// <param name="sqlConnection">The SQL connection.</param>
    /// <param name="sqlTrans">The SQL trans.</param>
    /// <param name="BatchId">The batch identifier.</param>
    /// <param name="destinationJobNumber">The destination job number.</param>
    /// <param name="Types">The types.</param>
    /// //TFS -43725,42830,42831,42832- Composition - Copy batch -Modify QE batch to spin over jobs
    private void UpdateDestinationCopyBatch(SqlConnection sqlConnection, SqlTransaction sqlTrans, int BatchId, string destinationJobNumber,string Types)
    {
        try
        {
            SqlCommand sqlcommand = new SqlCommand("[GetCopyDestinationJobNumbers]", sqlConnection);
            sqlcommand.Transaction = sqlTrans;
            sqlcommand.CommandType = CommandType.StoredProcedure;
            sqlcommand.Parameters.AddWithValue("@ProjectNumber", BatchId);
            sqlcommand.Parameters.AddWithValue("@SourceJobNumber", destinationJobNumber);
            sqlcommand.Parameters.AddWithValue("@Type", Types);
            ExecuteNonQuery(sqlcommand);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// Inserts the copy batch tracking.
    /// </summary>
    /// <param name="sqlConnection">The SQL connection.</param>
    /// <param name="sqlTrans">The SQL trans.</param>
    /// <param name="SourceBatchID">The source batch identifier.</param>
    /// <param name="SourceJobNumber">The source job number.</param>
    /// <param name="DestinationBatchID">The destination batch identifier.</param>
    /// <param name="DestinationJobNumber">The destination job number.</param>
    /// <param name="CreatedBy">The created by.</param>
    ///TFS - 43884 Copy Batch - Part II - Tracking for the Source and Destination Batches.
      private void InsertCopyBatchTracking(SqlConnection sqlConnection, SqlTransaction sqlTrans, int SourceBatchID, string SourceJobNumber, int DestinationBatchID, string DestinationJobNumber, string CreatedBy)
    {
        try
        {
            SqlCommand sqlcommand = new SqlCommand("[InsertCopyBatchTracking]", sqlConnection);
            sqlcommand.Transaction = sqlTrans;
            sqlcommand.CommandType = CommandType.StoredProcedure;
            sqlcommand.Parameters.AddWithValue("@SourceBatchID", SourceBatchID);
            sqlcommand.Parameters.AddWithValue("@SourceJobNumber", SourceJobNumber);
            sqlcommand.Parameters.AddWithValue("@DestinationBatchID", DestinationBatchID);
            sqlcommand.Parameters.AddWithValue("@DestinationJobNumber", DestinationJobNumber);
            sqlcommand.Parameters.AddWithValue("@CreatedBy", CreatedBy);
            ExecuteNonQuery(sqlcommand);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public static string GetFilePath(long translationOrTypesetBatchID, long projectId, string jobNumber)
    {
      string sanarea = "";
      sanarea = ConfigurationManager.AppSettings["SanArea"].ToString();
      sanarea = Path.Combine(sanarea, projectId.ToString(), jobNumber, translationOrTypesetBatchID.ToString());
      return sanarea;
    }

    private void UpdateProjectStausAfterBatchCreation(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string jobNumber)
    {
      SqlCommand sqlCommand = new SqlCommand("[Project_UpdateProjectStatusAfterBatchCreation]", sqlConnection);
      sqlCommand.Transaction = sqlTransaction;
      sqlCommand.CommandType = CommandType.StoredProcedure;
      string[] projectNumber = jobNumber.Split('-');
      sqlCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber[0]);
      ExecuteNonQuery(sqlCommand);
    }

    private int GetServiceTypeOnDemand(SqlConnection sqlConnection, SqlTransaction sqlTrans, int JobId)
    {
      int servicetypeid = 0;
      SqlCommand sqlCommand = new SqlCommand("[Job_GetJobDetailsByJobID]", sqlConnection);
      sqlCommand.Transaction = sqlTrans;
      sqlCommand.CommandType = CommandType.StoredProcedure;
      sqlCommand.Parameters.AddWithValue("@JobID", JobId);
      SqlDataReader reader = sqlCommand.ExecuteReader();
      if (reader.HasRows == true)
      {
        reader.Read();
        //Validation DBNull values 
        if (reader["ServiceTypeID"] != DBNull.Value)
            servicetypeid = (int)reader["ServiceTypeID"];
        else
            servicetypeid = 0;
      }

      reader.Close();


      return servicetypeid;
    }

    private int GetTranslationsByOnDemand(SqlConnection sqlConnection, SqlTransaction sqlTrans, int JobId)
    {
        int TranslationsBy = 0;
        SqlCommand sqlCommand = new SqlCommand("[Job_GetJobDetailsByJobID]", sqlConnection);
        sqlCommand.Transaction = sqlTrans;
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobID", JobId);
        SqlDataReader reader = sqlCommand.ExecuteReader();
        if (reader.HasRows == true)
        {
            reader.Read();
            // Validation DBNull values  
            if (reader["TranslationsBy"] != DBNull.Value)
                TranslationsBy = (int)reader["TranslationsBy"];
            else
                TranslationsBy = 0;
        }

        reader.Close();


        return TranslationsBy;
    }

    private int CreateTypesetBatchOnDemand(SqlConnection sqlConnection, SqlTransaction sqlTrans, TypeSetBatch typesetbatchdetails)
    {
      int BatchID = 0;
      SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", sqlConnection);
      sqlCommand.Transaction = sqlTrans;
      sqlCommand.CommandType = CommandType.StoredProcedure;
      sqlCommand.Parameters.AddWithValue("@ServiceTypeId", 1);
      sqlCommand.Parameters.AddWithValue("@BatchStatus", typesetbatchdetails.Batch.BatchStatus);
      sqlCommand.Parameters.AddWithValue("@JobNumber", typesetbatchdetails.JobNumber);
      sqlCommand.Parameters.AddWithValue("@BatchID", typesetbatchdetails.Batch.BatchID);
      sqlCommand.Parameters.AddWithValue("@JobId", typesetbatchdetails.JobID);
      sqlCommand.Parameters.AddWithValue("@CreatedBy", typesetbatchdetails.Batch.CreatedBy);
      sqlCommand.Parameters.AddWithValue("@ModifiedBy", typesetbatchdetails.Batch.ModifiedBy);
      sqlCommand.Parameters.AddWithValue("@ModifiedByIP", typesetbatchdetails.Batch.ModifiedByIP);
      SqlDataReader oReader = sqlCommand.ExecuteReader();



      if (oReader.HasRows == true)
      {
        oReader.Read();
        BatchID = (int)oReader[0];

      }
      oReader.Close();


      if (BatchID != 0)
      {
        sqlCommand = new SqlCommand("[Batch_InsertUpdateTypesetData]", sqlConnection);
        sqlCommand.Transaction = sqlTrans;
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@TypesetLanguageCode", typesetbatchdetails.TypeSetLanguageCode);
        sqlCommand.Parameters.AddWithValue("@InstructionsRcvdCode", typesetbatchdetails.InstructionRcvdCode);
        sqlCommand.Parameters.AddWithValue("@TurnAroundCode", typesetbatchdetails.TurnAroundCode);
        sqlCommand.Parameters.AddWithValue("@Weekend", typesetbatchdetails.Weekend);
        sqlCommand.Parameters.AddWithValue("@TimeIn", typesetbatchdetails.TimeIn);
        sqlCommand.Parameters.AddWithValue("@NewPages", Convert.ToInt32(typesetbatchdetails.NewPages));
        sqlCommand.Parameters.AddWithValue("@NewChinesePages", Convert.ToInt32(typesetbatchdetails.NewChinesePages));
        sqlCommand.Parameters.AddWithValue("@AlterationPages", Convert.ToInt32(typesetbatchdetails.AlterationPages));
        sqlCommand.Parameters.AddWithValue("@AlterationChinesePages", Convert.ToInt32(typesetbatchdetails.AlterationChinesePages));
        sqlCommand.Parameters.AddWithValue("@PdfPages", typesetbatchdetails.PDFPages);
        sqlCommand.Parameters.AddWithValue("@GlobalSearch", typesetbatchdetails.GlobalSearch);
        sqlCommand.Parameters.AddWithValue("@FreshType", typesetbatchdetails.FreshType);
        sqlCommand.Parameters.AddWithValue("@PublicHoliday", typesetbatchdetails.PublicHoliday);
        sqlCommand.Parameters.AddWithValue("@Graphics", typesetbatchdetails.Graphics);
        sqlCommand.Parameters.AddWithValue("@Deadline", typesetbatchdetails.Deadline);
        sqlCommand.Parameters.AddWithValue("@CompletedDate", typesetbatchdetails.CompletedDate);
        sqlCommand.Parameters.AddWithValue("@Comments", typesetbatchdetails.Comments);
        sqlCommand.Parameters.AddWithValue("@StatusChangeReason ", (String.IsNullOrEmpty(typesetbatchdetails.StatusChangeReason) ? (object)DBNull.Value : typesetbatchdetails.StatusChangeReason));
        sqlCommand.Parameters.AddWithValue("@Attachment", "test");
        sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
        sqlCommand.Parameters.AddWithValue("@BatchNumber", typesetbatchdetails.BatchNumber);
        sqlCommand.Parameters.AddWithValue("@JobNumber", typesetbatchdetails.JobNumber);
        sqlCommand.Parameters.AddWithValue("@ClientInHouse", typesetbatchdetails.ClientInHouse);
        sqlCommand.Parameters.AddWithValue("@DocumentConversion", typesetbatchdetails.DocumentConversion);
        sqlCommand.Parameters.AddWithValue("@WordFileConversion", typesetbatchdetails.WordFileConversion);
        sqlCommand.Parameters.AddWithValue("@StrikeThrough", typesetbatchdetails.Strikethrough);
        sqlCommand.Parameters.AddWithValue("@WaterMark", typesetbatchdetails.WaterMark);
        sqlCommand.Parameters.AddWithValue("@BlackliningAgainstVersionEnglish", Convert.ToInt32(typesetbatchdetails.BlackliningAgainstVersionEnglish));
        sqlCommand.Parameters.AddWithValue("@BlackliningAgainstVersionChinese", Convert.ToInt32(typesetbatchdetails.BlackliningAgainstVersionChinese));
        sqlCommand.Parameters.AddWithValue("@BlackliningCumulativeEnglish", Convert.ToInt32(typesetbatchdetails.BlackliningCumulativeEnglish));
        sqlCommand.Parameters.AddWithValue("@BlackliningCumulativeChinese", Convert.ToInt32(typesetbatchdetails.BlackliningCumulativeChinese));
        sqlCommand.Parameters.AddWithValue("@PrinterError", typesetbatchdetails.PrinterError);
        sqlCommand.Parameters.AddWithValue("@JobId", typesetbatchdetails.JobID);
        sqlCommand.Parameters.AddWithValue("@BlacklinesEnglish", Convert.ToInt32(typesetbatchdetails.BlacklinesEnglish));
        sqlCommand.Parameters.AddWithValue("@BlacklinesChinese", Convert.ToInt32(typesetbatchdetails.BlacklinesChinese));
        sqlCommand.Parameters.AddWithValue("@ClonedEnglish", typesetbatchdetails.ClonedEnglish);
        sqlCommand.Parameters.AddWithValue("@ClonedChinese", typesetbatchdetails.ClonedChinese);
        //sqlCommand.Parameters.AddWithValue("@TranslationComments", (String.IsNullOrEmpty(typesetbatchdetails.TranslationComments) ? (object)DBNull.Value : typesetbatchdetails.TranslationComments));
        ExecuteNonQuery(sqlCommand);
     

      }
      return BatchID;

    }

    public int CopyTypeSetAndTranslationData(TypeSetBatch typesetbatchdetails, string TypeOfJob)
    {

        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {

            try
            {   
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("[Batch_CopyTypeSetAndTranslationData]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@BatchID", typesetbatchdetails.BatchID);
                sqlCommand.Parameters.AddWithValue("@TypeOfJob", TypeOfJob);
                int i = ExecuteNonQuery(sqlCommand);
                return i;
            }
            catch (Exception exTrans)
            {
                throw exTrans;
            }
        }
    }

    public TypeSetBatch GetTypeSetBatchDetailsByBatchNumber(int BatchNumber, string JobNumber)
    {

      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        SqlCommand sqlCommand = new SqlCommand("[Batch_GetTypeSetBatchDetailsByBatchNumber]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobNumber", JobNumber);
        sqlCommand.Parameters.AddWithValue("@BatchNumber", BatchNumber);


        sqlConnection.Open();
        return GetTypesetBatchResultsFromReader(ExecuteReader(sqlCommand));
      }
    }

    public int InsertTranslationBatchDetails(TranslationBatch translationBatch, bool IsSideJob)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        int BatchID = 0;
        SqlTransaction sqlTrans;
        sqlConnection.Open();
        sqlTrans = sqlConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        try
        {
          SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", sqlConnection);
          sqlCommand.Transaction = sqlTrans;
          sqlCommand.CommandType = CommandType.StoredProcedure;
          sqlCommand.Parameters.AddWithValue("@ServiceTypeId", IsSideJob ? 8 : 2);
          sqlCommand.Parameters.AddWithValue("@BatchStatus", translationBatch.Batch.BatchStatus);
          sqlCommand.Parameters.AddWithValue("@JobId", translationBatch.JobId);
          sqlCommand.Parameters.AddWithValue("@JobNumber", translationBatch.JobNumber);
          sqlCommand.Parameters.AddWithValue("@BatchID", translationBatch.Batch.BatchID);
          sqlCommand.Parameters.AddWithValue("@CreatedBy", translationBatch.Batch.CreatedBy);
          sqlCommand.Parameters.AddWithValue("@ModifiedBy", translationBatch.Batch.ModifiedBy);
          sqlCommand.Parameters.AddWithValue("@ModifiedByIP", translationBatch.Batch.ModifiedByIP);
          SqlDataReader oReader = sqlCommand.ExecuteReader();
          if (oReader.HasRows == true)
          {
            oReader.Read();
            BatchID = (int)oReader[0];
          }
          oReader.Close();
          if (BatchID != 0)
          {
              sqlCommand = IsSideJob
                  ? new SqlCommand("[Batch_InsertUpdateSideJobData]", sqlConnection)
                  : new SqlCommand("[Batch_InsertUpdateTranslationData]", sqlConnection);
              sqlCommand.Transaction = sqlTrans;
              sqlCommand.CommandType = CommandType.StoredProcedure;
              SetTranslation_SideJobValues(sqlCommand, translationBatch);
              sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
              if (IsSideJob)
              {
                  sqlCommand.Parameters.AddWithValue("@NoOfWords", translationBatch.NoOfWords);
              }
              else
              {
                  sqlCommand.Parameters.AddWithValue("@IsCopyofTypeset", translationBatch.IsCopyForTypesetSrvc);
                  int servicetypeid = 0;
                  servicetypeid = GetServiceTypeOnDemand(sqlConnection, sqlTrans, translationBatch.JobId);

                  if (translationBatch.IsCopyForTypesetSrvc && servicetypeid == 7)
                  {
                      TypeSetBatch typstbatchdetails = new TypeSetBatch();
                      Batch batch = new Batch();

                      if (translationBatch.QEBatch != "")
                      {
                          typstbatchdetails.NewPages = 0;
                          typstbatchdetails.NewChinesePages = 0;
                          typstbatchdetails.AlterationPages = 0;
                          typstbatchdetails.AlterationChinesePages = 0;
                          // typstbatchdetails.Deadline = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
                          typstbatchdetails.Deadline = Convert.ToDateTime(translationBatch.Deadline);
                          typstbatchdetails.Weekend = translationBatch.Weekend;
                          typstbatchdetails.PublicHoliday = translationBatch.PublicHoliday;
                          typstbatchdetails.ClientInHouse = translationBatch.ClientInHouse;
                      }
                      else
                      {
                          //typstbatchdetails.NewPages = translationBatch.NoOfPages;
                          //typstbatchdetails.AlterationPages = translationBatch.AlterationPages;
                          typstbatchdetails.Deadline = Convert.ToDateTime(translationBatch.Deadline);
                      }
                      //typstbatchdetails.NewPages = translationBatch.NoOfPages; 
                      typstbatchdetails.TimeIn = translationBatch.TimeIn;
                      typstbatchdetails.TurnAroundCode = translationBatch.TurnAroundCode;
                      //  typstbatchdetails.OverTimeCode = translationBatch.OverTimeCode;
                      //typstbatchdetails.OvertimeHours = translationBatch.OvertimeHours;
                      typstbatchdetails.BlacklinesEnglish = typstbatchdetails.BlacklinesChinese = 0;
                      typstbatchdetails.InstructionRcvdCode = translationBatch.TypeOfTransServiceCode == 7 ? 32 :  0;
                      typstbatchdetails.JobNumber = translationBatch.JobNumber;
                      typstbatchdetails.PDFPages = 0;
                      typstbatchdetails.Strikethrough = 0;
                      typstbatchdetails.WaterMark = 0;
                      typstbatchdetails.WordFileConversion = 0;
                      typstbatchdetails.Graphics = 0;
                      typstbatchdetails.GlobalSearch = 0;
                      typstbatchdetails.FreshType = 0;
                      typstbatchdetails.BlackliningCumulativeEnglish = typstbatchdetails.BlackliningCumulativeChinese = 0;
                      typstbatchdetails.BlackliningAgainstVersionEnglish = typstbatchdetails.BlackliningAgainstVersionChinese = 0;
                      typstbatchdetails.DocumentConversion = 0;
                      typstbatchdetails.PrinterError = 0;
                      typstbatchdetails.Comments = translationBatch.Comments != null ? translationBatch.Comments : "";
                      //typstbatchdetails.Comments = translationBatch.Comments;
                      typstbatchdetails.Attachment = "test";
                      batch.BatchStatus = 1;
                      batch.CreatedBy = translationBatch.Batch.CreatedBy;
                      batch.ModifiedBy = translationBatch.Batch.ModifiedBy;
                      typstbatchdetails.Batch = batch;
                      translationBatch.TypeOfTransServiceCode = translationBatch.TypeOfTransServiceCode != 0 ? translationBatch.TypeOfTransServiceCode : 0;
                      typstbatchdetails.TypeSetLanguageCode = 0;
                      if (translationBatch.TypeOfTransServiceCode == 1 || translationBatch.TypeOfTransServiceCode == 3)
                      {
                          // Translation = English to Chinese || English to Chinese Alterations ,   Typeset = Chinese
                          typstbatchdetails.TypeSetLanguageCode = 14;
                          //typstbatchdetails.NewPages = 0;
                          //typstbatchdetails.AlterationPages = 0;
                          typstbatchdetails.NewChinesePages = translationBatch.NoOfPages;
                          typstbatchdetails.AlterationChinesePages = translationBatch.AlterationPages;
                          typstbatchdetails.ClonedChinese = translationBatch.ClonedEnglish;
                          
                      }
                      else if (translationBatch.TypeOfTransServiceCode == 2 ||
                               translationBatch.TypeOfTransServiceCode == 4)
                      {
                          // Translation = Chinese to English || Chinese to English Alterations,   Typeset = English
                          typstbatchdetails.TypeSetLanguageCode = 13;
                          typstbatchdetails.NewPages = translationBatch.NewChinesePages;
                          typstbatchdetails.AlterationPages = translationBatch.AlterationChinesePages;
                          typstbatchdetails.ClonedEnglish = translationBatch.ClonedChinese;
                          //typstbatchdetails.NewChinesePages = 0;
                          //typstbatchdetails.AlterationChinesePages = 0;
                      }
                      else if (translationBatch.TypeOfTransServiceCode == 5)
                      {
                          // Translation = No Translation,   Typeset = Chinese
                          typstbatchdetails.TypeSetLanguageCode = 14;
                          typstbatchdetails.NewChinesePages = translationBatch.NewChinesePages;
                          typstbatchdetails.AlterationChinesePages = translationBatch.AlterationChinesePages;
                          typstbatchdetails.ClonedChinese = translationBatch.ClonedChinese;
                      }
                      else if (translationBatch.TypeOfTransServiceCode == 6 || translationBatch.TypeOfTransServiceCode == 7)
                      {
                          // Translation = Both || Not To Be Billed,   Typeset = Both 
                          typstbatchdetails.TypeSetLanguageCode = 15;
                          typstbatchdetails.AlterationPages = translationBatch.AlterationPages;
                          typstbatchdetails.NewPages = translationBatch.NoOfPages;
                          typstbatchdetails.NewChinesePages = translationBatch.NewChinesePages;
                          typstbatchdetails.AlterationChinesePages = translationBatch.AlterationChinesePages;
                          typstbatchdetails.ClonedEnglish = translationBatch.ClonedEnglish;
                          typstbatchdetails.ClonedChinese = translationBatch.ClonedChinese;
                      }
                      int typesetBatchId = CreateTypesetBatchOnDemand(sqlConnection, sqlTrans, typstbatchdetails);


                      MsSqlUserRoleProvider AuditTrailLogProvider = new MsSqlUserRoleProvider();
                      AuditTrailLogProvider.SetAuditTrailLog(new AuditTrailLog
                        {
                            LoginID = translationBatch.Batch.CreatedBy,
                            Module = IsSideJob ? "SIDEJOB BATCH" : "TYPESET BATCH",
                            Action = "C",
                            Comments = string.Format("BatchID : {0} | Batch Number: {1} | Job Number : {2}", typesetBatchId, typstbatchdetails.BatchNumber.Equals(0) ? 1 : typstbatchdetails.BatchNumber, typstbatchdetails.JobNumber),
                            JobId = translationBatch.JobId,
                            BatchId = Convert.ToInt64(typesetBatchId)
                        });
                       


                      if (translationBatch.QEBatch == "Translation")
                      {
                          UpdateDestinationCopyBatch(sqlConnection, sqlTrans, Convert.ToInt32(typesetBatchId), typstbatchdetails.JobNumber, "COPYUPDATEDESTINATION");
                          InsertCopyBatchTracking(sqlConnection, sqlTrans, translationBatch.IsCopyBatchid, translationBatch.IsCopySourceJobnumber, typesetBatchId, translationBatch.JobNumber, translationBatch.Batch.CreatedBy);
                      }

                      string[] project_number = typstbatchdetails.JobNumber.Split('-');
                      JobTranslationsEmailOnDemand(sqlConnection, sqlTrans, Convert.ToInt32(project_number[0]),
                          "Job_TranslationsEmail", typstbatchdetails.JobNumber, typesetBatchId, "0",
                          typstbatchdetails.TimeIn, typstbatchdetails.Deadline);
                      if (translationBatch.fileRepositoryList.Count > 0)
                      {
                          foreach (var file in translationBatch.fileRepositoryList)
                          {
                              FileRepository fileRepository = new FileRepository();
                              fileRepository.FileName = file.FileName;
                              fileRepository.FileType = file.FileType;
                              fileRepository.ProjectID = file.ProjectID;

                              fileRepository.BatchID = Convert.ToInt64(typesetBatchId);
                              string jobNumber = file.jobNumber;
                              fileRepository.ModifiedBy = file.ModifiedBy;
                              fileRepository.translationBatchId = BatchID;
                              fileRepository.FilePath = GetFilePath(fileRepository.translationBatchId,
                                  Convert.ToInt64(fileRepository.ProjectID), jobNumber);
                              bool success = FileDataProvider.Instance.InsertUpdateFileRepository(fileRepository);

                          }
                      }

                  }
              }
              ExecuteNonQuery(sqlCommand);
              if (translationBatch.QEBatch == "Translation")
              {
                  UpdateDestinationCopyBatch(sqlConnection, sqlTrans, Convert.ToInt32(BatchID), translationBatch.JobNumber, "COPYUPDATEDESTINATION");
                  InsertCopyBatchTracking(sqlConnection, sqlTrans, translationBatch.IsCopyBatchid, translationBatch.IsCopySourceJobnumber, BatchID, translationBatch.JobNumber, translationBatch.Batch.CreatedBy);
              }
              UpdateProjectStausAfterBatchCreation(sqlConnection, sqlTrans, translationBatch.JobNumber);
          }

            sqlTrans.Commit();
            sqlConnection.Close();
        }
        catch (Exception exTrans)
        {
          BatchID = -1;
          try
          {
            sqlTrans.Rollback();
          }
          catch (Exception exRlbk)
          {
            sqlConnection.Close();
            //throw exRlbk;
          }
          sqlConnection.Close();
          //throw exTrans;
        }

        return BatchID;
      }

    }
  

    private void SetTranslation_SideJobValues(SqlCommand sqlCommand, TranslationBatch trbtch)
    {
      sqlCommand.Parameters.AddWithValue("@TypeOfTransServiceCode", trbtch.TypeOfTransServiceCode);
      sqlCommand.Parameters.AddWithValue("@TimeIn", trbtch.TimeIn);
      sqlCommand.Parameters.AddWithValue("@TurnAroundCode", trbtch.TurnAroundCode);
      sqlCommand.Parameters.AddWithValue("@Weekend", trbtch.Weekend);
      sqlCommand.Parameters.AddWithValue("@PublicHoliday", trbtch.PublicHoliday);
      sqlCommand.Parameters.AddWithValue("@NoOfPages", Convert.ToInt32(trbtch.NoOfPages));
      sqlCommand.Parameters.AddWithValue("@AlterationPages", Convert.ToInt32(trbtch.AlterationPages));
      //BEGIN - Sprint21-TFS#44457/44475
      sqlCommand.Parameters.AddWithValue("@NewChinesePages", Convert.ToInt32(trbtch.NewChinesePages));
      sqlCommand.Parameters.AddWithValue("@AlterationChinesePages", Convert.ToInt32(trbtch.AlterationChinesePages));
      //END - Sprint21-TFS#44457/44475
      sqlCommand.Parameters.AddWithValue("@ChinesetoTradChinese", trbtch.ChinesetoTradChinese);
      sqlCommand.Parameters.AddWithValue("@Deadline", trbtch.Deadline);
      sqlCommand.Parameters.AddWithValue("@CompletedDate", trbtch.CompletedDate == null ? (object)DBNull.Value : trbtch.CompletedDate);
      sqlCommand.Parameters.AddWithValue("@Comments", trbtch.Comments);
      sqlCommand.Parameters.AddWithValue("@BatchNumber", trbtch.BatchNumber);
      sqlCommand.Parameters.AddWithValue("@JobNumber", trbtch.JobNumber);
      sqlCommand.Parameters.AddWithValue("@ClientInHouse", trbtch.ClientInHouse);
      sqlCommand.Parameters.AddWithValue("@StatusChangeReason", (String.IsNullOrEmpty(trbtch.StatusChangeReason) ? (object)DBNull.Value : trbtch.StatusChangeReason));

      sqlCommand.Parameters.AddWithValue("@ClonedEnglish", Convert.ToInt32(trbtch.ClonedEnglish));
      sqlCommand.Parameters.AddWithValue("@ClonedChinese", Convert.ToInt32(trbtch.ClonedChinese));

    }

    public TranslationBatch GetTranslationBatchDetailsByBatchID(int BatchID, string JobNumber, bool IsSideJob)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        SqlCommand sqlCommand = IsSideJob ? new SqlCommand("[Batch_GetSideJobBatchDetailsByBatchNumber]", sqlConnection) : new SqlCommand("Batch_GetTranslationBatchDetailsByBatchNumber", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobNumber", JobNumber);
        sqlCommand.Parameters.AddWithValue("@BatchId", BatchID);


        sqlConnection.Open();
        return GetTranslationBatchResultsFromReader(ExecuteReader(sqlCommand));
      }
    }
    //Sprint-22#TFS#43603Translation -Bulk Update
    public List<TranslationVM> GetTranslationBulkUpdateBatchDetails(string JobNumber, int FromBatch, int ToBatch, string Types)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            SqlCommand sqlCommand = new SqlCommand("[Batch_GetTranslationBulkUpdateBatchDetailsByJobNumber]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@JobNumber", JobNumber);
            sqlCommand.Parameters.AddWithValue("@FromBatch", FromBatch);
            sqlCommand.Parameters.AddWithValue("@ToBatch", ToBatch);
            sqlCommand.Parameters.AddWithValue("@Type", Types);
            sqlConnection.Open();
            return GetTranslationBatchCollectionFromReader(ExecuteReader(sqlCommand));
        }
    }
    //Sprint-22#TFS#43603Translation -Bulk Update
    public int UpdateTranslationBulkUpdateBatchDetails(BatchBulkUpdate BatchBulkupdate)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {

            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("[Batch_BulkUpdateTranslationDetails]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@BatchID", BatchBulkupdate.BatchID);
                sqlCommand.Parameters.AddWithValue("@TurnaroundNameCode", BatchBulkupdate.TurnaroundNameCode);
                sqlCommand.Parameters.AddWithValue("@TranslationServiceCode", BatchBulkupdate.TranslationServiceCode);
                sqlCommand.Parameters.AddWithValue("@AlterationPages", BatchBulkupdate.AlterationPages);
                sqlCommand.Parameters.AddWithValue("@NoOfPages", BatchBulkupdate.NoOfPages);
                sqlCommand.Parameters.AddWithValue("@AlterationChinesePages", BatchBulkupdate.AlterationChinesePages);
                sqlCommand.Parameters.AddWithValue("@NewChinesePages", BatchBulkupdate.NewChinesePages);
                sqlCommand.Parameters.AddWithValue("@ClonedEnglish", BatchBulkupdate.ClonedEnglish);
                sqlCommand.Parameters.AddWithValue("@ClonedChinese", BatchBulkupdate.ClonedChinese);
                sqlCommand.Parameters.AddWithValue("@ChinesetoTradChinese", BatchBulkupdate.ChinesetoTradChinese);
                sqlCommand.Parameters.AddWithValue("@Deadline", BatchBulkupdate.Deadline);
                sqlCommand.Parameters.AddWithValue("@CompletedDate", BatchBulkupdate.CompletedDate);
                sqlCommand.Parameters.AddWithValue("@ModifiedByIP", BatchBulkupdate.ModifiedByIP);
                sqlCommand.Parameters.AddWithValue("@ModifiedBy", BatchBulkupdate.ModifiedBy);

                int i = ExecuteNonQuery(sqlCommand);
                return i;
            }
            catch (Exception exTrans)
            {
                throw exTrans;
            }
        }
    }

    public List<TypesetVM> GetTypesetBatchDetails(string jobNumber)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {

        SqlCommand sqlCommand = new SqlCommand("[Batch_GetTypesetBatchDetailsByJobNumber]", sqlConnection);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobNumber", jobNumber);

        sqlConnection.Open();
        return GetTypeSetBatchCollectionFromReader(ExecuteReader(sqlCommand));
      }
    }

    public List<TranslationVM> GetTranslationBatchDetails(string jobNumber, bool IsSideJob)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {

        SqlCommand sqlCommand = IsSideJob ? new SqlCommand("[Batch_GetSideJobBatchDetailsByJobNumber]", sqlConnection) : new SqlCommand("[Batch_GetTranslationBatchDetailsByJobNumber]", sqlConnection);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobNumber", jobNumber);

        sqlConnection.Open();
        return GetTranslationBatchCollectionFromReader(ExecuteReader(sqlCommand));

      }
    }

    public int UpdateTypsetBatchFromGrid(TypeSetBatch typesetBatchDetails)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        int batchId = 0;
        SqlCommand sqlCommand = new SqlCommand("[Batch_UpdateTypeSetFromGrid]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@newpages", Convert.ToInt32(typesetBatchDetails.NewPages));
        sqlCommand.Parameters.AddWithValue("@newchinesepages", Convert.ToInt32(typesetBatchDetails.NewChinesePages));
        sqlCommand.Parameters.AddWithValue("@alterationpages", Convert.ToInt32(typesetBatchDetails.AlterationPages));
        sqlCommand.Parameters.AddWithValue("@alterationchinesepages", Convert.ToInt32(typesetBatchDetails.AlterationChinesePages));
        sqlCommand.Parameters.AddWithValue("@batchid", typesetBatchDetails.BatchID);
        SqlParameter sqlparam = new SqlParameter();
        sqlparam.ParameterName = "@returnbatchid";
        sqlparam.DbType = DbType.Int32;
        sqlparam.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlparam);
        sqlConnection.Open();
        ExecuteNonQuery(sqlCommand);
        batchId = Convert.ToInt32(sqlCommand.Parameters["@returnbatchid"].Value);
        return batchId;
      }
    }

    public int InsertUpdatePrintBatchTable(PrintBatch printBatch)
    {
        int BatchId = 0;
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            int BatchID = 0;
            SqlTransaction sqlTrans;
            sqlConnection.Open();
            sqlTrans = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", sqlConnection);
                sqlCommand.Transaction = sqlTrans;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ServiceTypeId", 3);
                sqlCommand.Parameters.AddWithValue("@BatchStatus", printBatch.Batch.BatchStatus);
                sqlCommand.Parameters.AddWithValue("@JobNumber", printBatch.JobNumber);
                sqlCommand.Parameters.AddWithValue("@BatchID", printBatch.BatchID);
                sqlCommand.Parameters.AddWithValue("@JobId", printBatch.JobId);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", printBatch.Batch.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@ModifiedBy", printBatch.Batch.ModifiedBy);
                sqlCommand.Parameters.AddWithValue("@ModifiedByIP", printBatch.Batch.ModifiedByIP);


                SqlDataReader oReader = sqlCommand.ExecuteReader();



                if (oReader.HasRows == true)
                {
                    oReader.Read();
                    BatchID = (int)oReader[0];

                }
                oReader.Close();


                if (BatchID != 0)
                {
                    sqlCommand = new SqlCommand("[Batch_InsertUpdatePrintBatchTable]", sqlConnection);
                    sqlCommand.Transaction = sqlTrans;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@BatchNumber", printBatch.BatchNumber);
                    sqlCommand.Parameters.AddWithValue("@Turnaround", printBatch.Turnaround);
                    sqlCommand.Parameters.AddWithValue("@Overtime", printBatch.OverTime);
                    sqlCommand.Parameters.AddWithValue("@OvertimeHours", printBatch.OvertimeHours);
                    sqlCommand.Parameters.AddWithValue("@PrintStartTime", printBatch.PrintStartTime);
                    sqlCommand.Parameters.AddWithValue("@ExpectedSignofftime", printBatch.ExpectedSignofftime);
                    
                    sqlCommand.Parameters.AddWithValue("@ExpectedDeliveryDate", printBatch.ExpectedDeliveryDate);
                    sqlCommand.Parameters.AddWithValue("@ContactAtIFN", printBatch.ContactAtIFN);
                    sqlCommand.Parameters.AddWithValue("@DeadlineDate", printBatch.DeadLineDate);
                    sqlCommand.Parameters.AddWithValue("@CompletedDate", printBatch.CompletedDate);
                    sqlCommand.Parameters.AddWithValue("@Photocopy", printBatch.IsPhotocopy);

                    sqlCommand.Parameters.AddWithValue("@InHouse", printBatch.IsInHouse);
                    sqlCommand.Parameters.AddWithValue("@NoOfBWPhotocopyPages", printBatch.NoOfBWPhotocopyPages);
                    sqlCommand.Parameters.AddWithValue("@PrintTypeID", printBatch.PrintTypeID);
                    sqlCommand.Parameters.AddWithValue("@TypeOfPrintOther", printBatch.TypeOfPrintOther);

                    sqlCommand.Parameters.AddWithValue("@ClientQuantity", printBatch.ClientQuantity);
                    sqlCommand.Parameters.AddWithValue("@Overs", printBatch.Overs);
                    sqlCommand.Parameters.AddWithValue("@TotalQuantity", printBatch.TotalQuantity);

                    sqlCommand.Parameters.AddWithValue("@NoOfPagesCover", printBatch.NoOfPagesCover);
                    sqlCommand.Parameters.AddWithValue("@NoOfPagesText", printBatch.NoOfPagesText); ;
                    sqlCommand.Parameters.AddWithValue("@FinishedSize", printBatch.FinishedSize);
                    sqlCommand.Parameters.AddWithValue("@OtherPaperSize", printBatch.OtherPaperSize);
                    sqlCommand.Parameters.AddWithValue("@NumberofAddresee", printBatch.NumberofAddresee);
                    sqlCommand.Parameters.AddWithValue("@PaperCover", printBatch.IsPaperCover);
                    sqlCommand.Parameters.AddWithValue("@PaperText", printBatch.IsPaperText);
                    sqlCommand.Parameters.AddWithValue("@PaperOther", printBatch.IsPaperOther);
                    sqlCommand.Parameters.AddWithValue("@CoverStock", printBatch.CoverStock);
                    sqlCommand.Parameters.AddWithValue("@TextStock1", printBatch.TextStock1);
                    sqlCommand.Parameters.AddWithValue("@TextStock2", printBatch.TextStock2);
                    sqlCommand.Parameters.AddWithValue("@OtherStock", printBatch.OtherStock);
                    sqlCommand.Parameters.AddWithValue("@DocumentDescription", printBatch.DocumentDescription);
                    sqlCommand.Parameters.AddWithValue("@CoverDescription", printBatch.CoverDescription);



                    sqlCommand.Parameters.AddWithValue("@ColourCover", printBatch.IsColourCover);
                    sqlCommand.Parameters.AddWithValue("@ColorText", printBatch.IsColorText);
                    sqlCommand.Parameters.AddWithValue("@CoverSpecifications", printBatch.CoverSpecifications);
                    sqlCommand.Parameters.AddWithValue("@TextSpecifications1", printBatch.TextSpecifications1);
                    sqlCommand.Parameters.AddWithValue("@TextSpecifications2", printBatch.TextSpecifications2);
                    sqlCommand.Parameters.AddWithValue("@FourColourSpecs", printBatch.IsFourColourSpecs);
                    sqlCommand.Parameters.AddWithValue("@Specs", printBatch.Specs);
                    sqlCommand.Parameters.AddWithValue("@TypeOfVarnish", printBatch.TypeOfVarnish);
                    sqlCommand.Parameters.AddWithValue("@BindingType", printBatch.BindingType);
                    sqlCommand.Parameters.AddWithValue("@PrintDistribution", printBatch.IsPrintDistribution);
                    sqlCommand.Parameters.AddWithValue("@ShippingandMailing", printBatch.ShippingandMailing);


                    sqlCommand.Parameters.AddWithValue("@Comments", printBatch.Comments);
                    sqlCommand.Parameters.AddWithValue("@StatusChangeReason", (String.IsNullOrEmpty(printBatch.StatusChangeReason) ? (object)DBNull.Value : printBatch.StatusChangeReason));
                    sqlCommand.Parameters.AddWithValue("@BatchId", BatchID);
                    sqlCommand.Parameters.AddWithValue("@JobId", printBatch.JobId);
                    sqlCommand.Parameters.AddWithValue("@JobNumber", printBatch.JobNumber);
                 
                    #region Sprint 8 Changes by Srini on (4/6/2015)                
                    sqlCommand.Parameters.AddWithValue("@PrintedAt", printBatch.PrintedAt);
                    sqlCommand.Parameters.AddWithValue("@BookSize", printBatch.BookSize);
                    sqlCommand.Parameters.AddWithValue("@BookSizeDescription", printBatch.BookSizeDescription);
                    sqlCommand.Parameters.AddWithValue("@SpecsOtherDescription", printBatch.SpecsOtherDescription);
                    sqlCommand.Parameters.AddWithValue("@BindingOtherDescription", printBatch.BindingOtherDescription);
                    sqlCommand.Parameters.AddWithValue("@NumberOfTombstone", printBatch.NumberOfTombstone);
                    sqlCommand.Parameters.AddWithValue("@DocumentTypeID", printBatch.DocumentTypeId.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.DocumentTypeId);
                    #endregion

                    #region "Sprint19-TFS#43909/43446 - PhotoCopy/PrintOrder"
                    sqlCommand.Parameters.AddWithValue("@ActualSignoffTime", printBatch.ActualSignoffTime);
                    sqlCommand.Parameters.AddWithValue("@POVersion", (String.IsNullOrEmpty(printBatch.POVersion) ? (object)DBNull.Value : printBatch.POVersion));
                    sqlCommand.Parameters.AddWithValue("@IsPhotoCopyColor", printBatch.IsPhotocopyColor);
                    sqlCommand.Parameters.AddWithValue("@IsPhotoCopyBAndW", printBatch.IsPhotocopyBAndW);
                    sqlCommand.Parameters.AddWithValue("@Colorpages", printBatch.Colorpages);
                    sqlCommand.Parameters.AddWithValue("@BAndWpages", printBatch.BAndWpages);

                    sqlCommand.Parameters.AddWithValue("@IsCover", printBatch.IsCover);
                    sqlCommand.Parameters.AddWithValue("@IsText", printBatch.IsText);
                    sqlCommand.Parameters.AddWithValue("@IsSelfCover", printBatch.IsSelfCover);
                    sqlCommand.Parameters.AddWithValue("@NewCoverStock", printBatch.NewCoverStock);
                    sqlCommand.Parameters.AddWithValue("@CoverStockOther", (String.IsNullOrEmpty(printBatch.CoverStockOther) ? (object)DBNull.Value : printBatch.CoverStockOther));
                    sqlCommand.Parameters.AddWithValue("@PaperStock1", printBatch.PaperStock1);
                    sqlCommand.Parameters.AddWithValue("@PaperStock2", printBatch.PaperStock2);
                    sqlCommand.Parameters.AddWithValue("@PaperStockOther1", (String.IsNullOrEmpty(printBatch.PaperStockOther1) ? (object)DBNull.Value : printBatch.PaperStockOther1));
                    sqlCommand.Parameters.AddWithValue("@PaperStockOther2", (String.IsNullOrEmpty(printBatch.PaperStockOther2) ? (object)DBNull.Value : printBatch.PaperStockOther2));
                    #endregion

                    #region "Sprint23-TFS#44853"
                    sqlCommand.Parameters.AddWithValue("@VendorNameDesc", (String.IsNullOrEmpty(printBatch.VendorNameDesc) ? (object)DBNull.Value : printBatch.VendorNameDesc));
                    sqlCommand.Parameters.AddWithValue("@Pagination", printBatch.Pagination.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.Pagination);
                    sqlCommand.Parameters.AddWithValue("@PaginationOtherDesc", (String.IsNullOrEmpty(printBatch.PaginationOtherDesc) ? (object)DBNull.Value : printBatch.PaginationOtherDesc));
                    sqlCommand.Parameters.AddWithValue("@Language", printBatch.Language.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.Language);
                    sqlCommand.Parameters.AddWithValue("@OtherLangDesc", (String.IsNullOrEmpty(printBatch.OtherLangDesc) ? (object)DBNull.Value : printBatch.OtherLangDesc));
                    sqlCommand.Parameters.AddWithValue("@ClientQtyEnglish", printBatch.ClientQtyEnglish.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.ClientQtyEnglish);
                    sqlCommand.Parameters.AddWithValue("@BufferEnglish", printBatch.BufferEnglish.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.BufferEnglish);
                    sqlCommand.Parameters.AddWithValue("@TotalQtyEnglish", printBatch.TotalQtyEnglish.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.TotalQtyEnglish);
                    sqlCommand.Parameters.AddWithValue("@ClientQtyChinese", printBatch.ClientQtyChinese.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.ClientQtyChinese);
                    sqlCommand.Parameters.AddWithValue("@BufferChinese", printBatch.BufferChinese.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.BufferChinese);
                    sqlCommand.Parameters.AddWithValue("@TotalQtyChinese", printBatch.TotalQtyChinese.GetValueOrDefault(0).Equals(0) ? (object)DBNull.Value : printBatch.TotalQtyChinese);                    
                    #endregion
                    ExecuteNonQuery(sqlCommand);

                }

                UpdateProjectStausAfterBatchCreation(sqlConnection, sqlTrans, printBatch.JobNumber);

                sqlTrans.Commit();
                sqlConnection.Close();


            }
            catch (Exception exTrans)
            {

                try
                {
                    sqlTrans.Rollback();
                }
                catch (Exception exRlbk)
                {
                    sqlConnection.Close();
                    throw exRlbk;
                }
                sqlConnection.Close();
                throw exTrans;
            }
            return BatchID;
        }
    }

    #region 'Source Update By : VenkateshPrabu-108024 On 2015-Feb-03'
    public List<MediaBatchVM> GetAllMediaBatchesByJob(string Job)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            SqlCommand sqlCommand = new SqlCommand("[Batch_GetAllMediaBatchesByJob]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@jobNumber", Job);
            sqlConnection.Open();
            return GetMediaBatchCollectionFromReader(ExecuteReader(sqlCommand));
        }
    }

    public MediaBatch GetMediaBatchesByBatchNumber(int batchNumber, string jobNumber)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            SqlCommand sqlCommand = new SqlCommand("[Batch_GetAllMediaBatchesByBatchID]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@BatchID", batchNumber);
            sqlCommand.Parameters.AddWithValue("@jobNumber", jobNumber);
            sqlConnection.Open();
            return GetMediaBatchFromReader(ExecuteReader(sqlCommand));
        }
    }

    public int InsertUpdateMediaBatch(MediaBatch mediaBatch)
    {
        bool isInserted = false;

        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            SqlTransaction sqlTrans;
            sqlConnection.Open();
            sqlTrans = sqlConnection.BeginTransaction();
            int BatchID = 0;

            try
            {

                SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", sqlConnection);
                sqlCommand.Transaction = sqlTrans;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ServiceTypeId", mediaBatch.batch.ServiceTypeID);
                sqlCommand.Parameters.AddWithValue("@BatchStatus", mediaBatch.batch.BatchStatus);
                sqlCommand.Parameters.AddWithValue("@JobId", mediaBatch.batch.JobID);
                sqlCommand.Parameters.AddWithValue("@JobNumber", mediaBatch.batch.JobNumber);
                sqlCommand.Parameters.AddWithValue("@BatchID", mediaBatch.batch.BatchID);
                sqlCommand.Parameters.AddWithValue("@CreatedBy", mediaBatch.batch.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@ModifiedBy", mediaBatch.batch.ModifiedBy);
                sqlCommand.Parameters.AddWithValue("@ModifiedByIP", mediaBatch.batch.ModifiedByIP);

                SqlDataReader oReader = sqlCommand.ExecuteReader();

                if (oReader.HasRows == true)
                {
                    oReader.Read();
                    BatchID = (int)oReader[0];

                }
                oReader.Close();

                if (BatchID != 0)
                {


                    sqlCommand = new SqlCommand("[Batch_UpdateMediaBatchDetails]", sqlConnection);
                    sqlCommand.Transaction = sqlTrans;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("@InstructionsReceived", mediaBatch.InstructionsReceived == 0 ? (object)DBNull.Value : mediaBatch.InstructionsReceived);
                    sqlCommand.Parameters.AddWithValue("@TimeIn", mediaBatch.TimeIn);
                    sqlCommand.Parameters.AddWithValue("@Turnaround", mediaBatch.TurnaroundID);                    

                    sqlCommand.Parameters.AddWithValue("@TypesetNewEnglishPages", mediaBatch.TypesetNewEnglishPages);
                    sqlCommand.Parameters.AddWithValue("@TypesetNewChinesePages", mediaBatch.TypesetNewChinesePages);
                    sqlCommand.Parameters.AddWithValue("@TypesetAltEnglishPages", mediaBatch.TypesetAltEnglishPages);
                    sqlCommand.Parameters.AddWithValue("@TypesetAltChinesePages", mediaBatch.TypesetAltChinesePages);
                    sqlCommand.Parameters.AddWithValue("@TypesetFinalProductEnglish", mediaBatch.TypesetFinalProductEnglish);
                    sqlCommand.Parameters.AddWithValue("@TypesetFinalProductChinese", mediaBatch.TypesetFinalProductChinese);

                    sqlCommand.Parameters.AddWithValue("@TranslationNewEnglishPages", mediaBatch.TranslationNewEnglishPages);
                    sqlCommand.Parameters.AddWithValue("@TranslationNewChinesePages", mediaBatch.TranslationNewChinesePages);
                    sqlCommand.Parameters.AddWithValue("@TranslationAltEnglishPages", mediaBatch.TranslationAltEnglishPages);
                    sqlCommand.Parameters.AddWithValue("@TranslationAltChinesePages", mediaBatch.TranslationAltChinesePages);
                    sqlCommand.Parameters.AddWithValue("@TranslationFinalProductEnglish", mediaBatch.TranslationFinalProductEnglish);
                    sqlCommand.Parameters.AddWithValue("@TranslationFinalProductChinese", mediaBatch.TranslationFinalProductChinese);

                    sqlCommand.Parameters.AddWithValue("@AdsInPaper", string.IsNullOrEmpty(mediaBatch.AdsInPaper) ? (object)DBNull.Value : mediaBatch.AdsInPaper);
                    sqlCommand.Parameters.AddWithValue("@AdsSpecification", string.IsNullOrEmpty(mediaBatch.AdsSpecification) ? (object)DBNull.Value : mediaBatch.AdsSpecification);
                    sqlCommand.Parameters.AddWithValue("@TypeSet", mediaBatch.TypesetKey);
                    sqlCommand.Parameters.AddWithValue("@Translation", mediaBatch.Translation);
                    sqlCommand.Parameters.AddWithValue("@ESubmission", mediaBatch.isESubmission);
                    sqlCommand.Parameters.AddWithValue("@TimeOfESubmission", mediaBatch.TimeOfESubmission == null ? (object)DBNull.Value : mediaBatch.TimeOfESubmission);
                    sqlCommand.Parameters.AddWithValue("@DeadLine", mediaBatch.DeadLine);
                    sqlCommand.Parameters.AddWithValue("@CompletedDate", mediaBatch.CompletedDate == null ? (object)DBNull.Value : mediaBatch.CompletedDate);
                    sqlCommand.Parameters.AddWithValue("@Comments", mediaBatch.Comments);
                    sqlCommand.Parameters.AddWithValue("@StatusChangeReason", (String.IsNullOrEmpty(mediaBatch.StatusChangeReason) ? (object)DBNull.Value : mediaBatch.StatusChangeReason));
                    sqlCommand.Parameters.AddWithValue("@InHouse", mediaBatch.InHouse);
                    sqlCommand.Parameters.AddWithValue("@HKEx", mediaBatch.HKEx);
                    sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
                    sqlCommand.Parameters.AddWithValue("@BatchNumber", mediaBatch.BatchNumber);
                    sqlCommand.Parameters.AddWithValue("@JobNumber", mediaBatch.batch.JobNumber);

                    sqlCommand.Parameters.AddWithValue("@TypeClonedEnglish", mediaBatch.TypeClonedEnglish);
                    sqlCommand.Parameters.AddWithValue("@TypeClonedChinese", mediaBatch.TypeClonedChinese);
                    sqlCommand.Parameters.AddWithValue("@TransClonedEnglish", mediaBatch.TransClonedEnglish);
                    sqlCommand.Parameters.AddWithValue("@TransClonedChinese", mediaBatch.TransClonedChinese);


                    ExecuteNonQuery(sqlCommand);

                }
                UpdateProjectStausAfterBatchCreation(sqlConnection, sqlTrans, mediaBatch.batch.JobNumber);
                sqlTrans.Commit();
                sqlConnection.Close();
                isInserted = true;
                return BatchID;

            }
            catch (Exception ex)
            {
                sqlTrans.Rollback();
                sqlConnection.Close();
                throw ex;
            }
        }


    }


    public int UpdateTanslationFromGrid(TranslationBatch translationBatchDetails)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        int batchId = 0;
        SqlCommand sqlCommand = new SqlCommand("[Batch_UpdateTranslationsFromGrid]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@noofpages", translationBatchDetails.NoOfPages);
        sqlCommand.Parameters.AddWithValue("@alterationpages", translationBatchDetails.AlterationPages);
        sqlCommand.Parameters.AddWithValue("@batchid", translationBatchDetails.BatchID);
        SqlParameter sqlparam = new SqlParameter();
        sqlparam.ParameterName = "@returnbatchid";
        sqlparam.DbType = DbType.Int32;
        sqlparam.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlparam);
        sqlConnection.Open();
        ExecuteNonQuery(sqlCommand);
        batchId = Convert.ToInt32(sqlCommand.Parameters["@returnbatchid"].Value);
        return batchId;
      }
    }
    #endregion



    public DataTable ConvertToDataTable<T>(IList<T> data)
    {
      PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
      DataTable table = new DataTable();
      foreach (PropertyDescriptor prop in propertyDescriptorCollection)
      {
        if (!prop.Name.Equals("ConferenceRoomName"))
          table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
      }
      foreach (T item in data)
      {
        DataRow row = table.NewRow();
        foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
        {
          if (!propertyDescriptor.Name.Equals("ConferenceRoomName"))
          row[propertyDescriptor.Name] = propertyDescriptor.GetValue(item) ?? DBNull.Value;

        }
        table.Rows.Add(row);
      }
      return table;

    }


/// <summary>
 ///  //Sprint 20# TFS Hospitality - Connected Rooms 44239/44240/44241
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="data"></param>
/// <returns></returns>
    public DataTable HospitalityConvertToDataTable<T>(IList<T> data)
    {
        PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in propertyDescriptorCollection.Sort())
        {
            if (!prop.Name.Equals("ConferenceRoomName"))
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
            {
                if (!propertyDescriptor.Name.Equals("ConferenceRoomName"))
                    row[propertyDescriptor.Name] = propertyDescriptor.GetValue(item) ?? DBNull.Value;

            }
            table.Rows.Add(row);
        }
        return table;

    }


    public int InsertHospitalityBatchDetails(HospitalityBatch hospitalityBatch)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        int BatchID = 0;
        SqlTransaction sqlTrans;
        sqlConnection.Open();
        sqlTrans = sqlConnection.BeginTransaction();
        try
        {

          SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", sqlConnection);
          sqlCommand.Transaction = sqlTrans;
          sqlCommand.CommandType = CommandType.StoredProcedure;
          sqlCommand.Parameters.AddWithValue("@ServiceTypeId", 6);
          sqlCommand.Parameters.AddWithValue("@BatchStatus", hospitalityBatch.Batch.BatchStatus);
          sqlCommand.Parameters.AddWithValue("@JobId", hospitalityBatch.JobID);
          sqlCommand.Parameters.AddWithValue("@JobNumber", hospitalityBatch.JobNumber);
          sqlCommand.Parameters.AddWithValue("@BatchID", hospitalityBatch.Batch.BatchID);
          sqlCommand.Parameters.AddWithValue("@CreatedBy", hospitalityBatch.Batch.CreatedBy);
          sqlCommand.Parameters.AddWithValue("@ModifiedBy", hospitalityBatch.Batch.ModifiedBy);
          sqlCommand.Parameters.AddWithValue("@ModifiedByIP", hospitalityBatch.Batch.ModifiedByIP);

          SqlDataReader oReader = sqlCommand.ExecuteReader();

          if (oReader.HasRows == true)
          {
            oReader.Read();
            BatchID = (int)oReader[0];

          }
          oReader.Close();

          if (BatchID != 0)
          {
            sqlCommand = new SqlCommand("[Batch_InsertUpdateHospitalityBatch]", sqlConnection);
            sqlCommand.Transaction = sqlTrans;
            sqlCommand.CommandType = CommandType.StoredProcedure;


            sqlCommand.Parameters.AddWithValue("@RequestedBy", hospitalityBatch.HospitalityServiceId);
            sqlCommand.Parameters.AddWithValue("@JobCategoryID", hospitalityBatch.JobCategoryID);
            sqlCommand.Parameters.AddWithValue("@NoOfAttendeesActual", hospitalityBatch.NoOfAttendeesActual);
            sqlCommand.Parameters.AddWithValue("@DateOfRequest", hospitalityBatch.DateOfRequest);

            // sqlCommand.Parameters.AddWithValue("@HospitalityBatchNumber",); 
            sqlCommand.Parameters.AddWithValue("@IsMealsRequired", hospitalityBatch.IsMealsRequired);

            sqlCommand.Parameters.AddWithValue("@MealsTypeID", hospitalityBatch.MealsTypeID);
            sqlCommand.Parameters.AddWithValue("@IsLaptopsRequired", hospitalityBatch.IsLaptopsRequired);
            sqlCommand.Parameters.AddWithValue("@NoOfLaptops", hospitalityBatch.NoOfLaptops);
            sqlCommand.Parameters.AddWithValue("IsProjector", hospitalityBatch.IsProjector);
            sqlCommand.Parameters.AddWithValue("@NoOfProjector", hospitalityBatch.NoOfProjector);
            sqlCommand.Parameters.AddWithValue("@IsIDDRequired", hospitalityBatch.IsIDDRequired);
            sqlCommand.Parameters.AddWithValue("@IDDCharges", hospitalityBatch.IDDCharges);
            sqlCommand.Parameters.AddWithValue("@IsDialInConference", hospitalityBatch.IsDialInConference);
            sqlCommand.Parameters.AddWithValue("@DialIncode", hospitalityBatch.DialIncode);
            sqlCommand.Parameters.AddWithValue("@IsVideoConferencing", hospitalityBatch.IsVideoConferencing);

            sqlCommand.Parameters.AddWithValue("@Comments", hospitalityBatch.Comments);
            sqlCommand.Parameters.AddWithValue("@StatusChangeReason", (String.IsNullOrEmpty(hospitalityBatch.StatusChangeReason) ? (object)DBNull.Value : hospitalityBatch.StatusChangeReason));
            sqlCommand.Parameters.AddWithValue("@NoOfAttendeesApprox", hospitalityBatch.NoOfAttendeesApprox);

            sqlCommand.Parameters.AddWithValue("@BatchNumber", hospitalityBatch.BatchNumber);
            sqlCommand.Parameters.AddWithValue("@JobNumber", hospitalityBatch.JobNumber);
            sqlCommand.Parameters.AddWithValue("@JobID", hospitalityBatch.JobID);
            sqlCommand.Parameters.AddWithValue("@HospitalityServiceType", hospitalityBatch.HospitalityServiceTypeID);
            sqlCommand.Parameters.AddWithValue("@FoodComments", hospitalityBatch.FoodComments);
            sqlCommand.Parameters.AddWithValue("@EquipmentComments", hospitalityBatch.EquipmentComments);
            sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
            sqlCommand.Parameters.AddWithValue("@HospitalityNoOfMealsDescription", hospitalityBatch.HospitalityNoOfMealsDescription); 

             ExecuteNonQuery(sqlCommand);

            //inserting no of meals

             if (hospitalityBatch.hospitalityNoOfMealsUsageList.Count.Equals(0))
             {
               sqlCommand = new SqlCommand("[DeleteNoOfMeals]", sqlConnection);
               sqlCommand.Transaction = sqlTrans;
               sqlCommand.CommandType = CommandType.StoredProcedure;
               sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
               ExecuteNonQuery(sqlCommand);

             }
             else
             {
             sqlCommand = new SqlCommand("[Batch_InsertHospitalityNoOfMealsUsage]", sqlConnection);
             sqlCommand.Transaction = sqlTrans;
             sqlCommand.CommandType = CommandType.StoredProcedure;
             foreach (var meal in hospitalityBatch.hospitalityNoOfMealsUsageList)
               meal.BatchID = BatchID;
            
             SqlParameter mealsParams = sqlCommand.Parameters.AddWithValue("@NoOfMeals", ConvertToDataTable<HospitalityNoOfMealsUsage>(hospitalityBatch.hospitalityNoOfMealsUsageList));
             mealsParams.SqlDbType = SqlDbType.Structured; 
             sqlCommand.ExecuteNonQuery(); 
             }
             //Sprint 27#TFS-45819,45824 -Conference Room Usage Hours of a cancelled Hospitality batch
             if ((hospitalityBatch.conferenceRoomDetailsList == null) || (hospitalityBatch.conferenceRoomDetailsList.Count.Equals(0)) )
             {
               sqlCommand = new SqlCommand("[DeleteNoOfHospitality]", sqlConnection);
               sqlCommand.Transaction = sqlTrans;
               sqlCommand.CommandType = CommandType.StoredProcedure;
               sqlCommand.Parameters.AddWithValue("@BatchID", BatchID);
               ExecuteNonQuery(sqlCommand);

             }
             else
             {
            //inserting conference room list
             sqlCommand = new SqlCommand("[Batch_InsertUpdateHospitalityConferenceRoomList]", sqlConnection);
             sqlCommand.Transaction = sqlTrans;
             sqlCommand.CommandType = CommandType.StoredProcedure;
             foreach (var room in hospitalityBatch.conferenceRoomDetailsList)
               room.BatchID = BatchID;
             //Sprint 20# TFS Hospitality - Connected Rooms 44239/44240/44241
             SqlParameter conferenceRoomParams = sqlCommand.Parameters.AddWithValue("@ConferenceRoomList", HospitalityConvertToDataTable<HospitalityConferenceRoomUsage>(hospitalityBatch.conferenceRoomDetailsList));
             conferenceRoomParams.SqlDbType = SqlDbType.Structured;
             sqlCommand.Parameters.AddWithValue("@Type", "");
             SqlParameter OutputParam = new SqlParameter("@ConferenceRoomName", SqlDbType.NVarChar, 400)
             {
                Direction = ParameterDirection.Output
            };
             sqlCommand.Parameters.Add(OutputParam);
             ExecuteNonQuery(sqlCommand);
             }
            sqlTrans.Commit();
            sqlConnection.Close();

          }

        }
        catch (Exception e)
        {
          sqlTrans.Rollback();
          throw;

        }
        finally
        {
          sqlConnection.Close();
        }
        return BatchID;
      }
    }
    //Sprint 20# TFS Hospitality - Connected Rooms 44239/44240/44241
    public string ValidateConferenceRoomList(HospitalityBatch hospitalityBatch)
    {
        string ConferenceRoomName;
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateHospitalityConferenceRoomList]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter conferenceRoomParams = sqlCommand.Parameters.AddWithValue("@ConferenceRoomList", HospitalityConvertToDataTable<HospitalityConferenceRoomUsage>(hospitalityBatch.conferenceRoomDetailsList));
            SqlParameter OutputParam = new SqlParameter("@ConferenceRoomName", SqlDbType.NVarChar, 400)
            {
                Direction = ParameterDirection.Output
            };
            sqlCommand.Parameters.Add(OutputParam);
            sqlCommand.Parameters.AddWithValue("@Type", "VALIDATION");
            conferenceRoomParams.SqlDbType = SqlDbType.Structured;
            ExecuteNonQuery(sqlCommand);
            ConferenceRoomName = Convert.ToString(sqlCommand.Parameters["@ConferenceRoomName"].Value);
        }
        return ConferenceRoomName;
    }

    public HospitalityBatch GetHospitalityBatchDetailsByBatchNumber(int batchNumber, string jobNumber)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        SqlCommand sqlCommand = new SqlCommand("[Batch_GetHospitalityBatchDetailsByBatchNumber]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobNumber", jobNumber);
        sqlCommand.Parameters.AddWithValue("@BatchNumber", batchNumber);


        sqlConnection.Open();
        return GetHospitalityBatchFromReader(ExecuteReader(sqlCommand));
      }
    }


    public List<HospitalityVM> GetHospitalityBatchDetailsByJobNumber(string jobNumber)
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        SqlCommand sqlCommand = new SqlCommand("[Batch_GetHospitalityBatchDetailsByJobNumber]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@JobNumber", jobNumber);
      

        sqlConnection.Open();
        return GetHospitalityVMBatchCollectionFromReader(ExecuteReader(sqlCommand));
      }

    }

    public PrintBatch GetPrintBatchDetailsByBatchId(int batchId, int jobId)
    {
        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {
            SqlCommand sqlCommand = new SqlCommand("[Batch_GetPrintBatchDetailsByBatchId]", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@JobId", jobId);
            sqlCommand.Parameters.AddWithValue("@BatchId", batchId);


            sqlConnection.Open();
            return GetPrintBatchFromReader(ExecuteReader(sqlCommand));
        }
    }


    public List<PrintVM> GetPrintBatchDetailsByJobId(int jobId)
    {


        using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
        {

            SqlCommand sqlCommand = new SqlCommand("[Batch_GetPrintBatchDetailsByJobId]", sqlConnection);

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@JOBID", jobId);

            sqlConnection.Open();
            return GetPrintBatchCollectionFromReader(ExecuteReader(sqlCommand));
        }
    }

    public int GetFirstBatchIdOfJobnumber(string jobNumber)
    {
        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                int batchId = 0;
                SqlCommand sqlCommand = new SqlCommand("[Batch_GetFirstBatchIdOfJobnumber]", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@jobNumber", jobNumber);
                SqlParameter sqlparam = new SqlParameter();
                sqlparam.ParameterName = "@returnbatchid";
                sqlparam.DbType = DbType.Int32;
                sqlparam.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(sqlparam);
                sqlConnection.Open();
                ExecuteNonQuery(sqlCommand);
                batchId = Convert.ToInt32(sqlCommand.Parameters["@returnbatchid"].Value);
                return batchId;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool BatchBulkUpdate(List<BatchUpdate> batchUpdateList, int serviceType, string ModifiedBy, string ModifiedByIP)
    {

      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString)) 
      {
          sqlConnection.Open();
        //SqlTransaction sqlTransaction;
        //sqlTransaction = sqlConnection.BeginTransaction();
        
        try
        {

        SqlCommand sqlCommand = new SqlCommand("[Batch_BulkUpdateBatchStatus]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@BatchType", serviceType);
        sqlCommand.Parameters.AddWithValue("@ModifiedBy", ModifiedBy);
        sqlCommand.Parameters.AddWithValue("@ModifiedByIP", ModifiedByIP);
        SqlParameter batchUpdate = sqlCommand.Parameters.AddWithValue("@batchBulkUpdate", ConvertToDataTable<BatchUpdate>(batchUpdateList));
        batchUpdate.SqlDbType = SqlDbType.Structured;
        sqlCommand.ExecuteNonQuery();


          int i = (int)ExecuteNonQuery(sqlCommand);

          if (i > 0)
            return true;
          else
            return false;
        }
        catch (Exception ex)
        {
          throw;
        }
       
      }

      
    }
  
    public int JobTranslationsEmailOnDemand(SqlConnection sqlConnection, SqlTransaction sqlTrans, int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime timeIn, DateTime? deadLineDate)
    {
        try
        {
            

                SqlCommand sqlcommand = new SqlCommand("[spDeliveryEmail]", sqlConnection);
                sqlcommand.Transaction = sqlTrans;
                sqlcommand.CommandType = CommandType.StoredProcedure;
                sqlcommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                sqlcommand.Parameters.AddWithValue("@DeliveryEmail", DeliveryEmail);
                sqlcommand.Parameters.AddWithValue("@JobNumber", jobNumber);
                sqlcommand.Parameters.AddWithValue("@BatchID", batchID);
                sqlcommand.Parameters.AddWithValue("@Batch_Number", batchNumber);
                sqlcommand.Parameters.AddWithValue("@Time_In", timeIn);
                sqlcommand.Parameters.AddWithValue("@DeadLine_Date", deadLineDate);

                int i = ExecuteNonQuery(sqlcommand);

                return i;
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

      public int PrintEmail(int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime? timeIn, DateTime? deadLineDate)
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
                  sqlcommand.Parameters.AddWithValue("@JobNumber", jobNumber);
                  sqlcommand.Parameters.AddWithValue("@BatchID", batchID);
                  sqlcommand.Parameters.AddWithValue("@Batch_Number", batchNumber);
                  sqlcommand.Parameters.AddWithValue("@Time_In", timeIn);
                  sqlcommand.Parameters.AddWithValue("@DeadLine_Date", deadLineDate);

                  int i = ExecuteNonQuery(sqlcommand);

                  return i;
              }
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }
      public int JobTranslationsEmail(int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime timeIn, DateTime? deadLineDate)
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
                sqlcommand.Parameters.AddWithValue("@JobNumber", jobNumber);
                sqlcommand.Parameters.AddWithValue("@BatchID", batchID);
                sqlcommand.Parameters.AddWithValue("@Batch_Number", batchNumber);
                sqlcommand.Parameters.AddWithValue("@Time_In", timeIn);
                sqlcommand.Parameters.AddWithValue("@DeadLine_Date", deadLineDate);

                int i = ExecuteNonQuery(sqlcommand);

                return i;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public int JobMediaEmail(int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime timeIn, DateTime deadLineDate)
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
                sqlcommand.Parameters.AddWithValue("@JobNumber", jobNumber);
                sqlcommand.Parameters.AddWithValue("@BatchID", batchID);
                sqlcommand.Parameters.AddWithValue("@Batch_Number", batchNumber);
                sqlcommand.Parameters.AddWithValue("@Time_In", timeIn);
                sqlcommand.Parameters.AddWithValue("@DeadLine_Date", deadLineDate);

                int i = ExecuteNonQuery(sqlcommand);

                return i;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public List<int> AddAdditionalPrintBatchbyDocType(Job job)
    {
        int BatchID = 0;
        List<int> ReturnBatches = new List<int>();
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlTransaction sqlTrans;
            cn.Open();
            sqlTrans = cn.BeginTransaction();
            try
            {
                foreach (string DocumentTypeJobKey in job.DocumentTypeJobKeyList)
                {
                    BatchID = CreatePrintBatchbyDocType(cn, sqlTrans, job, DocumentTypeJobKey);
                    ReturnBatches.Add(BatchID);
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
            return ReturnBatches;
        }
    }

    public int CreatePrintBatchbyDocType(SqlConnection cn, SqlTransaction sqlTrans, Job job, string DocumentTypeJobKey)
    {
        int BatchID = 0;
        SqlCommand sqlCommand = new SqlCommand("[Batch_InsertUpdateBatchTable]", cn);
        sqlCommand.Transaction = sqlTrans;
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@ServiceTypeId", 3);
        sqlCommand.Parameters.AddWithValue("@BatchStatus", 1);
        sqlCommand.Parameters.AddWithValue("@JobNumber", job.JobNumber);
        sqlCommand.Parameters.AddWithValue("@BatchID", 0);
        sqlCommand.Parameters.AddWithValue("@JobId", job.JobID);
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


          

            // Author:		<FELIX>
            // Create date: 4-JAN-2017 ( Sprint23-TFS#44681/44853 )
            // Description:	This SP will be invoked by 2 ways.
            // Case 1. Add Print Batch(CreatePrintBatch) 
            //		@DocumentTypeId = Value
            //		@DocumentTypeJobKey = NULL
            // Case 2. Add Additional Document Type (CreatePrintBatchbyDocType) - Only for PrintOrder
            //		@DocumentTypeId = 0 
            //		@DocumentTypeJobKey = Added by DocType (DocType|DocTypeId|0) OR Added by JobNumber (Job|DocTypeId|JobId)
            sqlCommand = new SqlCommand("[Job_AutoCreatePrintBatch]", cn);
            sqlCommand.Transaction = sqlTrans;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@DocumentTypeId", 0); //Convert.ToInt32(DocumentTypeJobKey.Split('|')[1])
            sqlCommand.Parameters.AddWithValue("@PrintStartTime", DateTime.Now);
            sqlCommand.Parameters.AddWithValue("@BatchId", BatchID);
            sqlCommand.Parameters.AddWithValue("@JobId", job.JobID);
            sqlCommand.Parameters.AddWithValue("@DocumentTypeJobKey", DocumentTypeJobKey);
            ExecuteNonQuery(sqlCommand);


            MsSqlUserRoleProvider AuditTrailLogProvider = new MsSqlUserRoleProvider();
            AuditTrailLogProvider.SetAuditTrailLog(new AuditTrailLog
            {
                LoginID = job.CreatedBy,
                Module = "PRINT BATCH",
                Action = "C",
                Comments = string.Format("BatchID : {0} | Batch Number: {1} | Job Number : {2}", BatchID, null, job.JobNumber),
                JobId = job.JobID,
                BatchId = Convert.ToInt64(BatchID),
            });
        }
        return BatchID;
    }


  }
}