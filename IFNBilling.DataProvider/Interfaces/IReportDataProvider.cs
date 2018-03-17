using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFNBilling.Domain.Model;

namespace IFNBilling.DataProvider.Interfaces
{

    public interface IReportDataProvider
    {
        DataSet GetReportDataSet(String SPName, IFNParameter[] Params);

        DataTable GetInternalCost(CostReport costReport);
        DataTable GetCTDReportForIpo(CTDReportForIPO ctdReportForIPO);
        DataTable GetCTDReport(int ProjectNumber, DateTime FromDate, DateTime ToDate, string Types);
        DataTable GetActualFilingDataHistory(int ProjectNumber, int ActualFilingId, int ViewType);
        DataTable GetQueueReport(string ProductionType, int Status);
        DataTable GetQueueReportBatchDetails(int ProjectType, int batchStatus, string dueType, string productionType);
        DataTable GetPagesCompletedReport(int ViewType, string ProductionType, string Languages, DateTime FromDate, DateTime ToDate, int ProjectVerticalID, DateTime inDate, string ColumnHeader, string ProjectNumber);
        DataTable GetReceivedVsCompletedReport(int ViewType, string ProductionType, string Languages, int iDateOrMonthOption, DateTime FromDate, DateTime ToDate, string sYear, string sDateOrMonthValue, string ColumnHeader, string ProjectNumber);
        DataTable GetOnTimeReport(int ViewType, string ProductionType, DateTime FromDate, DateTime ToDate, string sProjectType, DateTime dtDeadLine, string ColumnHeader);
        DataTable ViewOnTimeReport(string ProductionType, DateTime FromDate, DateTime ToDate);
        DataTable CompletedProjectReport(DateTime FromDate, DateTime ToDate);
        DataTable GetOpenBatchReport(int ProjectVerticalID, string ProductionTypeName);
        DataTable GetBulkUpdateHistoryReport(int ProjectNumber, string JobNumber);
        DataTable LogReportsHistory(string JobNumbers, string Module, string Types);
        DataTable GetOperationsCompletedBatchesReport(DateTime FromDate, DateTime ToDate, int ViewType);
    }

}
