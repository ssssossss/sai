using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;
using IFNBilling.DataProvider.Interfaces;
using IFNBilling.Domain.Model;
using System.Collections.Generic;
using System.Globalization;


namespace IFNBilling.DataProvider.MsSqlClient
{

    public class MsSqlReportProvider : IReportDataProvider
    {
        private static SqlParameter AddParameter(IFNParameterTypes oType, String Name, Object Value)
        {
            SqlParameter obj = new SqlParameter();
            obj.SqlDbType = (SqlDbType)oType;
            obj.ParameterName = Name;
            obj.Value = Value;
            return obj;
        }

        public DataSet GetReportDataSet(String SPName, IFNParameter[] Params)
        {
            DataSet oResult = null;
            SqlConnection oCon;
            SqlCommand oCmd;
            SqlDataAdapter oAdop;
            oCon = new SqlConnection(ServiceConfig.ConnectionString);
            oCon.Open();
            oCmd = new SqlCommand();
            oAdop = new SqlDataAdapter(oCmd);
            oResult = new DataSet("Data");
            oCmd.Connection = oCon;

            oCmd.CommandText = SPName;
            oCmd.CommandType = CommandType.StoredProcedure;

            if (Params != null)
            {
                for (Int16 cnt = 0; cnt < Params.Length; cnt++)
                {
                    oCmd.Parameters.Add(MsSqlReportProvider.AddParameter(Params[cnt].ParamType, Params[cnt].Name, Params[cnt].Value));
                }
            }
            oAdop.Fill(oResult, "Data");
            oAdop = null;
            oCmd = null;
            oCon.Close();
            return oResult;
        }

        public DataTable GetInternalCost(CostReport costReport)
        {

            DataTable dataSet = null;
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            sqlConnection = new SqlConnection(ServiceConfig.ConnectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            dataSet = new DataTable("Data");
            sqlCommand.Connection = sqlConnection;

            sqlCommand.CommandText = "Report_GetInternalCosts";
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@ProjectNumber", costReport.ProjectNumber);
            sqlCommand.Parameters.AddWithValue("@StartDate", costReport.StartDate);
            sqlCommand.Parameters.AddWithValue("@EndDate", costReport.EndDate);

            dataSet.Load(sqlCommand.ExecuteReader());
            sqlDataAdapter = null;
            sqlCommand = null;
            sqlConnection.Close();
            return dataSet;
        }


        public DataTable GetCTDReport(int ProjectNumber, DateTime FromDate, DateTime ToDate, string Types)
        {
            DataTable dataSet = null;
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            sqlConnection = new SqlConnection(ServiceConfig.ConnectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            dataSet = new DataTable("Data");
            sqlCommand.Connection = sqlConnection;
            if (Types == "GetCTDReport")
            {
                sqlCommand.CommandText = "Report_CTDSummary";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@in_ProjectNo", ProjectNumber);
                sqlCommand.Parameters.AddWithValue("@in_FromDate", FromDate);
                sqlCommand.Parameters.AddWithValue("@in_ToDate", ToDate);
            }
            else if (Types == "CostToDateHistory" )
            {
                sqlCommand.CommandText = "[Sp_ProjectCostToDateHistory]";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", ProjectNumber);
                sqlCommand.Parameters.AddWithValue("@Type", Types);
            }else if(Types == "DownloadedHistory")
            {
                sqlCommand.CommandText = "[Sp_ProjectCostToDateHistory]";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ProjectNumber", ProjectNumber);
                sqlCommand.Parameters.AddWithValue("@Fromdate", FromDate);
                sqlCommand.Parameters.AddWithValue("@ToDate", ToDate);
                sqlCommand.Parameters.AddWithValue("@Type", Types);
            }

            dataSet.Load(sqlCommand.ExecuteReader());
            sqlDataAdapter = null;
            sqlCommand = null;
            sqlConnection.Close();
            return dataSet;
        }

        public DataTable GetActualFilingDataHistory(int ProjectNumber, int ActualFilingId, int ViewType)
        {
            DataTable dataSet = null;
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            sqlConnection = new SqlConnection(ServiceConfig.ConnectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            dataSet = new DataTable("Data");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "[Job_GetActualFilingDataHistory]";
            if (ViewType == 1)
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@in_ProjectNumber", ProjectNumber);
                sqlCommand.Parameters.AddWithValue("@in_ActualFilingId", ActualFilingId);
                sqlCommand.Parameters.AddWithValue("@in_ViewType", 1);
            }
            else if (ViewType == 2)
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@in_ProjectNumber", ProjectNumber);
                sqlCommand.Parameters.AddWithValue("@in_ActualFilingId", ActualFilingId);
                sqlCommand.Parameters.AddWithValue("@in_ViewType", 2);
            }
            dataSet.Load(sqlCommand.ExecuteReader());
            sqlDataAdapter = null;
            sqlCommand = null;
            sqlConnection.Close();
            return dataSet;
        }

        public DataTable GetCTDReportForIpo(CTDReportForIPO ctdReportForIPO)
        {
            DataTable dataSet = null;
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            sqlConnection = new SqlConnection(ServiceConfig.ConnectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            dataSet = new DataTable("Data");
            sqlCommand.Connection = sqlConnection;

            sqlCommand.CommandText = "Report_GetCTDForIPO";
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@ProjectNumber", ctdReportForIPO.ProjectNumber);
            sqlCommand.Parameters.AddWithValue("@ReportType", ctdReportForIPO.ReportType);
            sqlCommand.Parameters.AddWithValue("@StartDate", ctdReportForIPO.StartDate == null ? (object)DBNull.Value : ctdReportForIPO.StartDate);
            sqlCommand.Parameters.AddWithValue("@EndDate", ctdReportForIPO.EndDate == null ? (object)DBNull.Value : ctdReportForIPO.EndDate);
            //sqlCommand.Parameters.AddWithValue("@EndDate", costReport.EndDate);

            dataSet.Load(sqlCommand.ExecuteReader());
            sqlDataAdapter = null;
            sqlCommand = null;
            sqlConnection.Close();
            return dataSet;
        }



        public DataTable GetQueueReport(string ProductionType, int Status)
        {
            DataTable dataSet = null;
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            sqlConnection = new SqlConnection(ServiceConfig.ConnectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            dataSet = new DataTable("Data");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "[PRC_GETPRODUCTIONQUEUESTATUS_SMRY]";
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@PIN_PRODUCTIONTYPE", ProductionType);
            sqlCommand.Parameters.AddWithValue("@PIN_JOBSTATUS", Status);
            dataSet.Load(sqlCommand.ExecuteReader());
            sqlDataAdapter = null;
            sqlCommand = null;
            sqlConnection.Close();
            return dataSet;
        }

        public DataTable GetQueueReportBatchDetails(int ProjectType, int batchStatus, string dueType, string productionType)
        {
            DataTable dataSet = null;
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            sqlConnection = new SqlConnection(ServiceConfig.ConnectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand();
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            dataSet = new DataTable("Data");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = "[PRC_GETBATCHDETAILS]";
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@PIN_ProjectTypeID", ProjectType);
            sqlCommand.Parameters.AddWithValue("@PIN_BatchStatus", batchStatus);
            sqlCommand.Parameters.AddWithValue("@PIN_DueOn_Type", dueType);
            sqlCommand.Parameters.AddWithValue("@PIN_ProductionType", productionType);
            dataSet.Load(sqlCommand.ExecuteReader());
            sqlDataAdapter = null;
            sqlCommand = null;
            sqlConnection.Close();
            return dataSet;

        }


        public DataTable GetPagesCompletedReport(int ViewType, string ProductionType, string Languages, DateTime FromDate, DateTime ToDate, int ProjectVerticalID, DateTime inDate, string ColumnHeader, string ProjectNumber)
        {
            DataTable dataTable = null;                        
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_CompletedPages]",sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");                    
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_ViewType", ViewType);
                    sqlCommand.Parameters.AddWithValue("@in_ProductionType", ProductionType);
                    sqlCommand.Parameters.AddWithValue("@in_Languages", Languages);
                    sqlCommand.Parameters.AddWithValue("@in_FromDate", FromDate);
                    sqlCommand.Parameters.AddWithValue("@in_ToDate", ToDate);
                    sqlCommand.Parameters.AddWithValue("@in_ProjectVerticalID", ProjectVerticalID);
                    sqlCommand.Parameters.AddWithValue("@in_Date", inDate);
                    sqlCommand.Parameters.AddWithValue("@in_ColHeaderName", ColumnHeader);
                    sqlCommand.Parameters.AddWithValue("@in_ProjectNumber", ProjectNumber);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }

        public DataTable GetReceivedVsCompletedReport(int ViewType, string ProductionType, string Languages, int iDateOrMonthOption, DateTime FromDate, DateTime ToDate, string sYear, string sDateOrMonthValue, string ColumnHeader, string ProjectNumber)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_ReceivedVsCompletedPages]", sqlConnection))
                {
                    sqlConnection.Open();                    
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_ViewType", ViewType);
                    sqlCommand.Parameters.AddWithValue("@in_ProductionType", ProductionType);
                    sqlCommand.Parameters.AddWithValue("@in_Languages", Languages);
                    sqlCommand.Parameters.AddWithValue("@in_DateOption", iDateOrMonthOption);                    
                    sqlCommand.Parameters.AddWithValue("@in_FromDate", FromDate);
                    sqlCommand.Parameters.AddWithValue("@in_ToDate", ToDate);
                    sqlCommand.Parameters.AddWithValue("@in_Year", sYear);
                    sqlCommand.Parameters.AddWithValue("@in_DateOrMonth", sDateOrMonthValue);
                    sqlCommand.Parameters.AddWithValue("@in_ColHeaderName", ColumnHeader);
                    sqlCommand.Parameters.AddWithValue("@in_ProjectNumber", ProjectNumber);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }

        public DataTable GetOnTimeReport(int ViewType, string ProductionType, DateTime FromDate, DateTime ToDate, string sProjectType, DateTime dtDeadLine, string ColumnHeader)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_OnTime]", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_ViewType", ViewType);
                    sqlCommand.Parameters.AddWithValue("@in_ProductionType", ProductionType);
                    sqlCommand.Parameters.AddWithValue("@in_FromDate", FromDate);
                    sqlCommand.Parameters.AddWithValue("@in_ToDate", ToDate);
                    sqlCommand.Parameters.AddWithValue("@in_ProjectType", sProjectType);
                    sqlCommand.Parameters.AddWithValue("@in_Date", dtDeadLine);           
                    sqlCommand.Parameters.AddWithValue("@in_ColHeaderName", ColumnHeader);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }

        public DataTable CompletedProjectReport(DateTime FromDate, DateTime ToDate)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_CompletedProjectSummary]", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_FromDate", FromDate);
                    sqlCommand.Parameters.AddWithValue("@in_ToDate", ToDate);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }

        public DataTable ViewOnTimeReport(string ProductionType, DateTime FromDate, DateTime ToDate)
        {
            DataTable dt = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(@"select * from View_OnTimeReport where ProductionType in (" + ProductionType + @") and 
                    DeadLineDate between cast('" + FromDate.ToString() + @"' as datetime) and cast('" + ToDate.ToString() + @"' as datetime)", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.CommandType = CommandType.Text;
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dt = new DataTable("Data");
                    CultureInfo myCultureInfo = new CultureInfo("en-us");
                    sqlDataAdapter.FillSchema(dt, SchemaType.Source);
                    sqlDataAdapter.Fill(dt);
                    dt.Locale = myCultureInfo;
                    sqlDataAdapter = null;
                }
            }
            return dt;
        }

        public DataTable GetOpenBatchReport(int ProjectVerticalID, string ProductionTypeName)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_ActiveBatches]", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_ProjectVertical", ProjectVerticalID);
                    sqlCommand.Parameters.AddWithValue("@in_ServiceType", ProductionTypeName);                    
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }

        public DataTable GetBulkUpdateHistoryReport(int ProjectNumber, string JobNumber)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_BatchBulkUpdateHistory]", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_ProjectNo", ProjectNumber);
                    sqlCommand.Parameters.AddWithValue("@in_JobNumber", JobNumber);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }


        public DataTable LogReportsHistory(string JobNumbers, string Module, string Types)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[LogReportsHistory]", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@JobNumbers", JobNumbers);
                    sqlCommand.Parameters.AddWithValue("@Module", Module);
                    sqlCommand.Parameters.AddWithValue("@Type", Types);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }

        public DataTable GetOperationsCompletedBatchesReport(DateTime FromDate, DateTime ToDate, int ViewType)
        {
            DataTable dataTable = null;
            SqlDataAdapter sqlDataAdapter;
            using (SqlConnection sqlConnection = new SqlConnection(ServiceConfig.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("[Report_CompletedBatches]", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable("Data");
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@in_FromDate", FromDate);
                    sqlCommand.Parameters.AddWithValue("@in_ToDate", ToDate);
                    sqlCommand.Parameters.AddWithValue("@in_ViewType", ViewType);
                    dataTable.Load(sqlCommand.ExecuteReader());
                    sqlDataAdapter = null;
                }
            }
            return dataTable;
        }        


    }
}

