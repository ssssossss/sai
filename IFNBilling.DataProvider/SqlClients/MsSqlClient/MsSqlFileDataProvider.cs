using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using IFNBilling.Domain.Model;

namespace IFNBilling.DataProvider.MsSqlClient
{
  public class MsSqlFileDataProvider : FileDataProvider
  {
    public override bool InsertUpdateFileRepository(FileRepository filedetails)
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("InsertFileRepository", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@filename", filedetails.FileName);
        cmd.Parameters.AddWithValue("@filetype", filedetails.FileType);
        cmd.Parameters.AddWithValue("@projectid", filedetails.ProjectID);
        cmd.Parameters.AddWithValue("@userid", filedetails.ModifiedBy);
        cmd.Parameters.AddWithValue("@filepath", filedetails.FilePath);
        cmd.Parameters.AddWithValue("@sourceid", filedetails.SourceID == null ? 0 : filedetails.SourceID);
        cmd.Parameters.AddWithValue("@BatchID", filedetails.BatchID);
        cmd.Parameters.AddWithValue("@JobID", filedetails.JobID);

        SqlParameter sqlparam = new SqlParameter();
        sqlparam.ParameterName = "@filerepositoryid";
        sqlparam.DbType = DbType.Int32;
        sqlparam.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(sqlparam);
        cn.Open();
        ExecuteNonQuery(cmd);

        int fileRepositoryId = Convert.ToInt32(cmd.Parameters["@filerepositoryid"].Value);
        if (fileRepositoryId == 0)
          return false;
        else
          return true;
      }
    }

    public override List<FileRepository> GetFileDetailsByProjectId(int project_id)
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("Project_GetFileDetailsByProjectId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@projectid", project_id);
        cn.Open();
        return GetFileDetailsResultsCollectionFromReader(ExecuteReader(cmd));

      }


    }

    public override void RemoveFilesById(DataTable filerepositoryids)
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("Project_RemoveFilesById", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter newparam = cmd.Parameters.AddWithValue("@filerepositoryid", filerepositoryids);
        newparam.SqlDbType = SqlDbType.Structured;
        cn.Open();
        ExecuteNonQuery(cmd);
        //return GetFileDetailsResultsCollectionFromReader(ExecuteReader(cmd));

      }
    }

    public override List<FileRepository> GetVendorInvoiceDetails(string filetype, string sourceid)
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("Project_GetVendorInvoiceFileDetails", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@filetype", filetype);
        cmd.Parameters.AddWithValue("@sourceid", Convert.ToInt64(sourceid));
        cn.Open();
        ExecuteNonQuery(cmd);
        return GetVendorFileDetailsResultsCollectionFromReader(ExecuteReader(cmd));

      }


    }

    public override List<FileRepository> GetFileDetailsByBatchId(long BatchId)
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("Batch_GetFileDetailsByBatchId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@BatchID", BatchId);
        cn.Open();
        ExecuteNonQuery(cmd);
        return GetVendorFileDetailsResultsCollectionFromReader(ExecuteReader(cmd));
      }
    }


    public override List<FileRepository> GetExpenseTrackingFilesById(int jobId, int expenseTrackingId)
    {


      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("Job_GetExpenseTrackingFileListById", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@JobId", jobId);
        cmd.Parameters.AddWithValue("@ExpenseTrackingId", expenseTrackingId);

        cn.Open();
        ExecuteNonQuery(cmd);
        return GetFileDetailsResultsCollectionFromReader(ExecuteReader(cmd));
      }
    }
  }
}
