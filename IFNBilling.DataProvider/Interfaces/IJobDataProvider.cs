using IFNBilling.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider.Interfaces
{
  public interface IJobDataProvider
  {


     List<JobSearchVM> GetJobSearchResults(string jobNumber, string projectName, string company, int projectTypeId, int jobTypeId, int documentTypeId);
     List<JobSearchVM> GetProjectResultsByProjectId(int projectid, string Types = null);

     JobSearchVM GetJobDetailsByJobID(int jobId);

      string AddOrModifyCompositionJob(Job job);
      string AddOrModifyPrintJob(Job job);
      string AddOrModifyMediaJob(Job job);
      string AddOrModifyHospitalityJob(Job job);
      string AddOrModifySideJob(Job job);

      int InsertUpdateExpenseTracking(HospitalityExpense hospitalityExpense);
      List<HospitalityExpense> GetExpenseTrackingByJobId(int jobId);
      JobSearchVM GetJobnumberandJobType(string JobNumber);
      List<JobSearchVM> GetActiveJobs();

      int AddOrModifyJobOverTimeHours(JobOverTimeHours job_overtime_hours, string type);
      List<EmailDistribution> GetEmailDistributionList(EmailDistribution EmailDistributioninfo, string type);
      int InsertAndUpdateEmailDistribution(EmailDistribution EmailDistributioninfo, string type);
      List<JobOverTimeHours> GetJobOverTimeHours(int project_id, int project_type_id, int project_status_id, string job_number, int job_type_id, string type);
      bool deleteHospitalityExpenseTracking(int hospitalityExpenseId);

       bool insertMerchantName(string merchantName);
       bool UpdateJobStatusByJobID(int JobId, string Type = null);
  }
}
