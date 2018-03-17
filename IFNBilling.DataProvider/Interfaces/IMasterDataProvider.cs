using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider.Interfaces
{
  public interface IMasterDataProvider
  {
     List<ProjectStatus> GetProjectStatus();
     List<ProjectType> GetProjectType();
     List<ProjectType> GetProjectTypeByVertical(byte ViewType, int iProjectVerticalID);
     List<ProjectVertical> GetProjectVertical();
     List<ServiceType> GetServiceType();
     List<ServiceType> GetServiceTypeByServiceName(string serviceName, string type);


     List<TranslationServiceType> GetTranslationServiceType();
     List<JobType> GetJobType(int projectNumber);
     List<InhouseLocation> GetInhouseLocation();
     List<CodeMaster> GetTypesetLanguage();
     List<BatchStatus> GetBatchStatus();

     List<OverTime> GetOverTime(string TeamName);

     List<CodeMaster> GetInstructions();
     List<CodeMaster> GetCodeMaster(string Types);
     List<Turnaround> GetTurnAround();
     List<ProjectNumbers> GetProjectNumberJobNumber(int ProjectNumber, string JobNumber, string Types);
     List<CodeMaster> GetProjectPhase();

     List<CodeMaster> GetInvoiceMileStone();
     List<InvoiceType> GetInvoiceType();

 List<DocumentType> GetDocumentType(int projectTypeID ,string type);
 List<BSBManual> GetDocumentManual(int DocumentId, string DocumentName, string CreatedBy, string types);

 List<ServiceTypeHospitality> GetHospitalityTypeOfService();
 List<JobCategory> GetJobCategory();
 List<MealsType> GetTypeOfMeals();
 List<NoOfMeals> GetNoOfMeals();
 List<HospitalityServiceRequested> GetServiceRequestedBy();

 List<ConferenceRoom> GetConferenceRooms();


 List<PrintType> GetPrintType(string PrintTypeCategory);
        List<CodeMaster> GetPhotoCopy();
        List<CodeMaster> GetBindingTypes();
        List<CodeMaster> GetLaminationTypes();
        List<CodeMaster> GetVarnishingTypes();

        List<ExpenseType> GetExpenseType();
        List<VM_ClientContact> GetClientContacts();
        List<Role> GetRoleList();


        List<ProjectSite> GetProjectSite();
        List<ProjectSalesRep> GetProjectSalesRep();

        List<VM_UserDetails> GetIFNContacts();

        List<PaperSize> GetPaperSize();

    List<PrintSpecs> GetPrintSpecs();

    List<PublicHolidays> GetPublicHolidays(int projectSiteID);
    List<ReportType> GetReportTypes();

    List<PrintedAt> GetPrintAt();
    List<BookSize> GetBookSize();
  

    List<MerchantName> GetMerchantNames();
    List<PrintFinishing> GetPrintFinishing();
    List<ProductionType> GetProductionType();
    List<NewsPaper> GetNewsPaperList();
    List<TranslationDoneBy> GetTranslationDoneBy();
    List<KeyValueObject> GetJobListForPrintOrder(int projectNumber, string jobNumber, byte viewType);
  }
}
