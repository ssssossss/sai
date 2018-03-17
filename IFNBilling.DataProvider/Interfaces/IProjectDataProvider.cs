using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
using System.Data;
namespace IFNBilling.DataProvider.Interfaces
{
    public interface IProjectDataProvider
    {
        List<Project> GetAllProjects();
        int AddOrModifyProject(Project project, string test);
        Project GetProjectDetailsByProjectId(int project_id);
        List<VM_CompanyContactDetails> GetCompanyDetails(string firstname, string lastname, string companyname, string salesrep);
        //List<VM_CompanyContactDetails> GetCompanySearchDetails(string firstname, string lastname, string companyname, string salesrep, Int64 parentcontactid);
        List<VM_ProjectSearch> GetProjectSearchResults(int? projectNumber, string projectName, DateTime? dateFrom, DateTime? dateTo, DateTime? CompletedDateFrom, DateTime? CompletedDateTo, string company, int? projectStatusId, int? projectTypeId);
        List<ProjectInvoiceStatus> GetProjectInvoiceStatusList(int projectNumber);
        string CheckInvoiceNumber(string invoiceNumber);
        bool InsertUpdateProjectInvoice(ProjectInvoiceStatus projectInvoiceStatus);
        List<ProjectVendorTracking> GetProjectVendorTracking(int projectNumber);
        int AddOrModifyProjectVendorTracking(ProjectVendorTracking projectVendorTracking);
        List<string> GetJobNumbers(int projectNumber, string type);
        List<EmailJobNumber> GetEmailBatchNumber(string JobNumber, string type);
        List<JobNumberText> GetCopyDestinationJobNumbers(int projectNumber, string SourceJobNumber, string types);
        List<DistributionList> GetProjectDistributionList(int projectNumber);

        bool DeleteContact(int DistributionListID, int projectNumber,int ContactID);
        List<DistributionList> GetMapContactDistributionList(string contacFirstName,string contactLastName, string companyName);
        bool InsertProjectDistributionList(List<ProjectDistributionListMap> projectDistributions);
       
        List<DistributionList> GetCompanyNames(string compName);
        //List<Company> GetCompanyIdandNameByCompany(string compName);
        List<VM_AddDistribution> GetAddressDetailsByCompany(string companyName);
        int AddContacttoDistribution(DistributionList contactDetails);
        DistributionList GetContactDetailsByDistributionListId(int distributionListId);
        List<LabelSystem> GetLabelMessagesByProjectNumber(int projectNumber, string Category);
        bool InsertLabelSystemMessage(LabelSystem labelSystem);
        bool UpdateLabelSystemMessage(LabelSystem labelSystem);
        bool DeleteLabelSystemMessage(LabelSystem labelSystem);
        int EditDistributionContactByDistributionId(DistributionList contactDetails);
        string CheckActualFilingDate(int projectNumber);
        int IsBatchExists(int projectNumber);
        int CheckInCompleteBatches(int projectNumber);
        bool CheckAccountDetailsByEmail(string emailId);
        int ProjectSalesRepEmail(int projectNumber,string DeliveryEmail);
        int ProjectHospitalityEmail(int projectNumber, string DeliveryEmail);
        int ProjectCostToDateHistory(ProjectCostToDateHistory ProjectCTD);
        //int GetParentandChildCompanyRelationship(int ParentcompName, int SubsidarycompName);
        //bool UpdateParentandChildCompanyRelationship(int ParentcompName, int SubsidarycompName);
        //DataTable GetParentCompanyDetails();
        //DataTable GetChildCompanyNodesByParent(string Nodevalue);
    }
}
