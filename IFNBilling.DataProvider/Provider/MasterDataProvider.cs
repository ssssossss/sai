using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;
using System.Collections;

using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;

namespace IFNBilling.DataProvider
{
  public abstract class MasterDataProvider : DataAccess
  {
    static private MasterDataProvider _instance = null;

    static public MasterDataProvider Instance
    {
      get
      {
        if (_instance == null)
          _instance = (MasterDataProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlMasterDataProvider"));
        return _instance;
      }
    }

    public MasterDataProvider()
    {
      this.ConnectionString = ServiceConfig.ConnectionString;
    }


    #region ProjectStatus

    //Abstract Methods - Project Status
 

    //Virtual Method - Project Status
    protected virtual ProjectStatus GetProjectStatusFromReader(IDataReader reader)
    {
      EntityConverter<ProjectStatus> ps = new EntityConverter<ProjectStatus>();
      ProjectStatus projectStatus = ps.Convert(reader);
      return projectStatus;
    }

    protected virtual List<ProjectStatus> GetProjectStatusCollectionFromReader(IDataReader reader)
    {
      List<ProjectStatus> projectStatusList = new List<ProjectStatus>();
      while (reader.Read())
      {
        projectStatusList.Add(GetProjectStatusFromReader(reader));
      }
      return projectStatusList;
    }
    #endregion ProjectStatus

    #region ProjectType

    //Abstract Methods - Project Type
  

    //Virutal Methods - Project Type
    protected virtual ProjectType GetProjectTypeFromReader(IDataReader reader)
    {
      EntityConverter<ProjectType> pt = new EntityConverter<ProjectType>();
      ProjectType projectType = pt.Convert(reader);
      return projectType;
    }

    protected virtual List<ProjectType> GetProjectTypeCollectionFromReader(IDataReader reader)
    {
      List<ProjectType> projectTypeList = new List<ProjectType>();
      while (reader.Read())
      {
        projectTypeList.Add(GetProjectTypeFromReader(reader));
      }
      return projectTypeList;
    }

    #endregion ProjectType

    #region ProjectVertical

    //Abstract Methods - Project Vertical
   

    //Virutal Methods - Project Vertical
    protected virtual ProjectVertical GetProjectVerticalFromReader(IDataReader reader)
    {
      EntityConverter<ProjectVertical> pv = new EntityConverter<ProjectVertical>();
      ProjectVertical projectVertical = pv.Convert(reader);
      return projectVertical;
    }

    protected virtual List<ProjectVertical> GetProjectVerticalCollectionFromReader(IDataReader reader)
    {
      List<ProjectVertical> projectVerticalList = new List<ProjectVertical>();
      while (reader.Read())
      {
        projectVerticalList.Add(GetProjectVerticalFromReader(reader));
      }
      return projectVerticalList;
    }

    #endregion ProjectVertical

    #region ServiceType

    //Abstract Methods - Servicee Type
   

    //Virutal Methods - Servicee Type
    protected virtual ServiceType GetServiceTypeFromReader(IDataReader reader)
    {
      EntityConverter<ServiceType> st = new EntityConverter<ServiceType>();
      ServiceType serviceType = st.Convert(reader);
      return serviceType;
    }

    protected virtual List<ServiceType> GetServiceTypeCollectionFromReader(IDataReader reader)
    {
      List<ServiceType> serviceTypeList = new List<ServiceType>();
      while (reader.Read())
      {
        serviceTypeList.Add(GetServiceTypeFromReader(reader));
      }
      return serviceTypeList;
    }

    #endregion ServiceType


    #region InvoiceType
  

    protected virtual InvoiceType GetInvoiceTypeFromReader(IDataReader reader)
    {
      EntityConverter<InvoiceType> invoiceTypeEntity = new EntityConverter<InvoiceType>();
      InvoiceType invoiceType = invoiceTypeEntity.Convert(reader);
      return invoiceType;
    }
    protected virtual List<InvoiceType> GetInvoiceTypeCollectionFromReader(IDataReader reader)
    {
      List<InvoiceType> invoiceTypeList = new List<InvoiceType>();
      while (reader.Read())
      {
        invoiceTypeList.Add(GetInvoiceTypeFromReader(reader));

      }
      return invoiceTypeList;
    }
    #endregion


    #region InvoiceMileStone
   

    protected virtual CodeMaster GetInvoiceMileStoneFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> invoiceMileStoneEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterInvoiceMileStone = invoiceMileStoneEntity.Convert(reader);
        return codeMasterInvoiceMileStone;
    }
    protected virtual List<CodeMaster> GetInvoiceMileStoneCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> invoiceMileStoneList = new List<CodeMaster>();
        while (reader.Read())
        {
            invoiceMileStoneList.Add(GetInvoiceMileStoneFromReader(reader));

        }
        return invoiceMileStoneList;
    }
    #endregion

    #region ProjectPhase
 
    protected virtual CodeMaster GetProjectPhaseFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> projectPhaseEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterProjectPhase = projectPhaseEntity.Convert(reader);
        return codeMasterProjectPhase;
    }
    protected virtual List<CodeMaster> GetProjectPhaseCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> projectPhaseList = new List<CodeMaster>();
        while (reader.Read())
        {
            projectPhaseList.Add(GetProjectPhaseFromReader(reader));

        }
        return projectPhaseList;
    }
    #endregion

    #region Turnaroundtime

   

    protected virtual Turnaround GetTurnAroundFromReader(IDataReader reader)
    {
        EntityConverter<Turnaround> turnAroundEntity = new EntityConverter<Turnaround>();
        Turnaround codeMasterTurnAround = turnAroundEntity.Convert(reader);
        return codeMasterTurnAround;
    }
    protected virtual List<Turnaround> GetTurnAroundCollectionFromReader(IDataReader reader)
    {
        List<Turnaround> turnAroundList = new List<Turnaround>();
        while (reader.Read())
        {
            turnAroundList.Add(GetTurnAroundFromReader(reader));

        }
        return turnAroundList;
    }

    protected virtual List<ProjectNumbers> GetProjectNumberJobNumberCollectionFromReader(IDataReader reader)
    {
        List<ProjectNumbers> ProjectNumberJobNumberList = new List<ProjectNumbers>();
        while (reader.Read())
        {
            ProjectNumberJobNumberList.Add(GetProjectNumberJobNumberFromReader(reader));

        }
        return ProjectNumberJobNumberList;
    }
    protected virtual ProjectNumbers GetProjectNumberJobNumberFromReader(IDataReader reader)
    {
        EntityConverter<ProjectNumbers> ProjectNumberJobNumberEntity = new EntityConverter<ProjectNumbers>();
        ProjectNumbers codeMasterTurnAround = ProjectNumberJobNumberEntity.Convert(reader);
        return codeMasterTurnAround;
    }

    #endregion 

    #region LaminationType
    protected virtual CodeMaster GetLaminationTypesFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> laminationTypeEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterLaminationType = laminationTypeEntity.Convert(reader);
        return codeMasterLaminationType;
    }
    protected virtual List<CodeMaster> GetLaminationTypesCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> laminationTypeList = new List<CodeMaster>();
        while (reader.Read())
        {
            laminationTypeList.Add(GetLaminationTypesFromReader(reader));

        }
        return laminationTypeList;
    }

    #endregion

    #region VarnishingType

    protected virtual CodeMaster GetVarnishingTypesFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> varnishingTypeEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterVarnishingType = varnishingTypeEntity.Convert(reader);
        return codeMasterVarnishingType;
    }
    protected virtual List<CodeMaster> GetVarnishingTypesCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> varnishingTypeList = new List<CodeMaster>();
        while (reader.Read())
        {
            varnishingTypeList.Add(GetVarnishingTypesFromReader(reader));

        }
        return varnishingTypeList;
    }


    #endregion

    #region InstructionsReceived



    protected virtual CodeMaster GetInstructionsFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> instructionsEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterInstructions = instructionsEntity.Convert(reader);
        return codeMasterInstructions;
    }
    protected virtual List<CodeMaster> GetInstructionsCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> turnAroundList = new List<CodeMaster>();
        while (reader.Read())
        {
            turnAroundList.Add(GetInstructionsFromReader(reader));

        }
        return turnAroundList;
    }


    #endregion 

    #region OvertimeType



    protected virtual OverTime GetOverTimeFromReader(IDataReader reader)
    {
        EntityConverter<OverTime> overTimeEntity = new EntityConverter<OverTime>();
        OverTime codeMasterOverTime = overTimeEntity.Convert(reader);
        return codeMasterOverTime;
    }
    protected virtual List<OverTime> GetOverTimeCollectionFromReader(IDataReader reader)
    {
        List<OverTime> overTimeList = new List<OverTime>();
        while (reader.Read())
        {
            overTimeList.Add(GetOverTimeFromReader(reader));

        }
        return overTimeList;
    }


    #endregion 
      
    #region BatchStatus

   

    protected virtual BatchStatus GetBatchStatusFromReader(IDataReader reader)
    {
        EntityConverter<BatchStatus> batchStatusEntity = new EntityConverter<BatchStatus>();
        BatchStatus codeMasterBatchStatus = batchStatusEntity.Convert(reader);
        return codeMasterBatchStatus;
    }
    protected virtual List<BatchStatus> GetBatchStatusCollectionFromReader(IDataReader reader)
    {
        List<BatchStatus> batchStatusList = new List<BatchStatus>();
        while (reader.Read())
        {
            batchStatusList.Add(GetBatchStatusFromReader(reader));

        }
        return batchStatusList;
    }


    #endregion 

    #region TypesetLanguages

   

    protected virtual CodeMaster GetTypesetLanguagesFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> typesetLanguageEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterTypesetLanguage = typesetLanguageEntity.Convert(reader);
        return codeMasterTypesetLanguage;
    }
    protected virtual List<CodeMaster> GetTypesetLanguagesCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> typesetLanguageList = new List<CodeMaster>();
        while (reader.Read())
        {
            typesetLanguageList.Add(GetTypesetLanguagesFromReader(reader));

        }
        return typesetLanguageList;
    }


    #endregion 

    #region PrintType
    protected virtual PrintType GetPrintTypeFromReader(IDataReader reader)
    {
        EntityConverter<PrintType> printTypeEntity = new EntityConverter<PrintType>();
        PrintType printType = printTypeEntity.Convert(reader);
        return printType;
    }
    protected virtual List<PrintType> GetPrintTypeCollectionFromReader(IDataReader reader)
    {
        List<PrintType> printTypeList = new List<PrintType>();
        while (reader.Read())
        {
            printTypeList.Add(GetPrintTypeFromReader(reader));

        }
        return printTypeList;
    }

    #endregion

    #region PhotCopy

    protected virtual CodeMaster GetPhotoCopyFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> photCopyEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterPhotoCopy = photCopyEntity.Convert(reader);
        return codeMasterPhotoCopy;
    }
    protected virtual List<CodeMaster> GetPhotCopyCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> photoCopyList = new List<CodeMaster>();
        while (reader.Read())
        {
            photoCopyList.Add(GetPhotoCopyFromReader(reader));

        }
        return photoCopyList;
    }

    #endregion

    #region BindingTypes

    protected virtual CodeMaster GetBindingTypesFromReader(IDataReader reader)
    {
        EntityConverter<CodeMaster> bindingTypesEntity = new EntityConverter<CodeMaster>();
        CodeMaster codeMasterBindingTypes = bindingTypesEntity.Convert(reader);
        return codeMasterBindingTypes;
    }
    protected virtual List<CodeMaster> GetBindingTypesCollectionFromReader(IDataReader reader)
    {
        List<CodeMaster> bindingTypesList = new List<CodeMaster>();
        while (reader.Read())
        {
            bindingTypesList.Add(GetBindingTypesFromReader(reader));

        }
        return bindingTypesList;
    }
     

    #endregion


    #region JobType and DocumentType






    protected virtual JobType GetJobTypeFromReader(IDataReader reader)
    {
      EntityConverter<JobType> jobTypeEntity = new EntityConverter<JobType>();
      JobType jobType = jobTypeEntity.Convert(reader);
      return jobType;
    }
    protected virtual List<JobType> GetJobTypeCollectionFromReader(IDataReader reader)
    {
      List<JobType> jobTypeList = new List<JobType>();
      while (reader.Read())
      {
        jobTypeList.Add(GetJobTypeFromReader(reader));

      }
      return jobTypeList;
    }
    protected virtual List<InhouseLocation> GetInhouseCollectionFromReader(IDataReader reader)
    {
        List<InhouseLocation> InhouseLocationList = new List<InhouseLocation>();
        while (reader.Read())
        {
            InhouseLocationList.Add(GetInhouseFromReader(reader));

        }
        return InhouseLocationList;
    }
    protected virtual InhouseLocation GetInhouseFromReader(IDataReader reader)
    {
        EntityConverter<InhouseLocation> InhouseEntity = new EntityConverter<InhouseLocation>();
        InhouseLocation Inhouse = InhouseEntity.Convert(reader);
        return Inhouse;
    }
    protected virtual DocumentType GetDocumentTypeFromReader(IDataReader reader)
    {
      EntityConverter<DocumentType> documentTypeEntity = new EntityConverter<DocumentType>();
      DocumentType documentType = documentTypeEntity.Convert(reader);
      return documentType;
    }
    protected virtual List<DocumentType> GetDocumentTypeCollectionFromReader(IDataReader reader)
    {
      List<DocumentType> documentTypeList = new List<DocumentType>();
      while (reader.Read())
      {
        documentTypeList.Add(GetDocumentTypeFromReader(reader));

      }
      return documentTypeList;
    }

    protected virtual BSBManual GetDocumentManualFromReader(IDataReader reader)
    {
        EntityConverter<BSBManual> documentManualEntity = new EntityConverter<BSBManual>();
        BSBManual documentManual = documentManualEntity.Convert(reader);
        return documentManual;
    }
    protected virtual List<BSBManual> GetDocumentManualCollectionFromReader(IDataReader reader)
    {
        List<BSBManual> documentTypeList = new List<BSBManual>();
        while (reader.Read())
        {
            documentTypeList.Add(GetDocumentManualFromReader(reader));
        }
        return documentTypeList;
    }





    #endregion


    #region RoleList
    protected virtual Role GetRolesFromReader(IDataReader reader)
    {
        EntityConverter<Role> roleEntity = new EntityConverter<Role>();
        Role roles = roleEntity.Convert(reader);
        return roles;
    }
    protected virtual List<Role> GetRoleListCollectionFromReader(IDataReader reader)
    {
        List<Role> roleList = new List<Role>();
        while (reader.Read())
        {
            roleList.Add(GetRolesFromReader(reader));

        }
        return roleList;
    }

    #endregion


    #region translationserviceType


    protected virtual TranslationServiceType GetTranslationServiceTypeFromReader(IDataReader reader)
    {
      EntityConverter<TranslationServiceType> translationServiceTypeEntity = new EntityConverter<TranslationServiceType>();
      TranslationServiceType translationServiceType = translationServiceTypeEntity.Convert(reader);
      return translationServiceType;
    }
    protected virtual List<TranslationServiceType> GetTranslationServiceTypeCollectionFromReader(IDataReader reader)
    {
      List<TranslationServiceType> translationServiceTypeList = new List<TranslationServiceType>();
      while (reader.Read())
      {
        translationServiceTypeList.Add(GetTranslationServiceTypeFromReader(reader));

      }
      return translationServiceTypeList;
    }


    #endregion

    #region ClientContacts
    protected virtual VM_ClientContact GetClientContactsFromReader(IDataReader reader)
    {
        EntityConverter<VM_ClientContact> clientContactEntity = new EntityConverter<VM_ClientContact>();
        VM_ClientContact clientcontact = clientContactEntity.Convert(reader);
        return clientcontact;
    }
    protected virtual List<VM_ClientContact> GetClientContactsCollectionFromReader(IDataReader reader)
    {
        List<VM_ClientContact> ClientContactsList = new List<VM_ClientContact>();
        while (reader.Read())
        {
            ClientContactsList.Add(GetClientContactsFromReader(reader));

        }
        return ClientContactsList;
    }

    #endregion



    protected virtual HospitalityServiceRequested GetHospitalityServiceRequestedFromReader(IDataReader reader)
    {
      EntityConverter<HospitalityServiceRequested> hospitalityServiceRequestedEntity = new EntityConverter<HospitalityServiceRequested>();
      HospitalityServiceRequested hospitalityServiceRequested = hospitalityServiceRequestedEntity.Convert(reader);
      return hospitalityServiceRequested;
    }
    protected virtual List<HospitalityServiceRequested> GetHospitalityServiceRequestedCollectionFromReader(IDataReader reader)
    {
      List<HospitalityServiceRequested> hospitalityServiceRequestedList = new List<HospitalityServiceRequested>();
      while (reader.Read())
      {
        hospitalityServiceRequestedList.Add(GetHospitalityServiceRequestedFromReader(reader));

      }
      return hospitalityServiceRequestedList;
    }


    protected virtual ServiceTypeHospitality GetServiceTypeHospitalityFromReader(IDataReader reader)
    {
      EntityConverter<ServiceTypeHospitality> serviceTypeHospitalityEntity = new EntityConverter<ServiceTypeHospitality>();
      ServiceTypeHospitality serviceTypeHospitality = serviceTypeHospitalityEntity.Convert(reader);
      return serviceTypeHospitality;
    }
    protected virtual List<ServiceTypeHospitality> GetServiceTypeHospitalityCollectionFromReader(IDataReader reader)
    {
      List<ServiceTypeHospitality> serviceTypeHospitalityList = new List<ServiceTypeHospitality>();
      while (reader.Read())
      {
        serviceTypeHospitalityList.Add(GetServiceTypeHospitalityFromReader(reader));

      }
      return serviceTypeHospitalityList;
    }



    protected virtual MealsType GetMealsTypeFromReader(IDataReader reader)
    {
      EntityConverter<MealsType> mealsTypeEntity = new EntityConverter<MealsType>();
      MealsType mealsType = mealsTypeEntity.Convert(reader);
      return mealsType;
    }
    protected virtual List<MealsType> GetMealsTypeCollectionFromReader(IDataReader reader)
    {
      List<MealsType> mealsTypeList = new List<MealsType>();
      while (reader.Read())
      {
        mealsTypeList.Add(GetMealsTypeFromReader(reader));

      }
      return mealsTypeList;
    }


    protected virtual NoOfMeals GetNoOfMealsFromReader(IDataReader reader)
    {
      EntityConverter<NoOfMeals> noOfMealsEntity = new EntityConverter<NoOfMeals>();
      NoOfMeals mealsType = noOfMealsEntity.Convert(reader);
      return mealsType;
    }
    protected virtual List<NoOfMeals> GetNoOfMealsCollectionFromReader(IDataReader reader)
    {
      List<NoOfMeals> noOfMealsList = new List<NoOfMeals>();
      while (reader.Read())
      {
        noOfMealsList.Add(GetNoOfMealsFromReader(reader));

      }
      return noOfMealsList;
    }



    protected virtual JobCategory GetJobCategoryFromReader(IDataReader reader)
    {
      EntityConverter<JobCategory> jobCategoryEntity = new EntityConverter<JobCategory>();
      JobCategory jobCategory = jobCategoryEntity.Convert(reader);
      return jobCategory;
    }
    protected virtual List<JobCategory> GetJobCategoryCollectionFromReader(IDataReader reader)
    {
      List<JobCategory> jobCategoryList = new List<JobCategory>();
      while (reader.Read())
      {
        jobCategoryList.Add(GetJobCategoryFromReader(reader));

      }
      return jobCategoryList;
    }




    protected virtual ConferenceRoom GetConferenceRoomFromReader(IDataReader reader)
    {
      EntityConverter<ConferenceRoom> conferenceRoomEntity = new EntityConverter<ConferenceRoom>();
      ConferenceRoom conferenceRoom = conferenceRoomEntity.Convert(reader);
      return conferenceRoom;
    }
    protected virtual List<ConferenceRoom> GetConferenceRoomCollectionFromReader(IDataReader reader)
    {
      List<ConferenceRoom> conferenceRoomList = new List<ConferenceRoom>();
      while (reader.Read())
      {
        conferenceRoomList.Add(GetConferenceRoomFromReader(reader));

      }
      return conferenceRoomList;
    }

    protected virtual List<ReportType> GetReportTypesCollectionFromReader(IDataReader reader)
    {
        List<ReportType> reportTypeList = new List<ReportType>();
        while (reader.Read())
        {
            reportTypeList.Add(GetReportTypeFromReader(reader));
        }
        return reportTypeList;
    }

    protected virtual ReportType GetReportTypeFromReader(IDataReader reader)
    {
        EntityConverter<ReportType> reportTypeEntity = new EntityConverter<ReportType>();
        ReportType reportType = reportTypeEntity.Convert(reader);
        return reportType;
    }

    

    #region expensetype

    protected virtual ExpenseType GetExpenseTypeFromReader(IDataReader reader)
    {
      EntityConverter<ExpenseType> expenseTypeEntity = new EntityConverter<ExpenseType>();
      ExpenseType expenseType = expenseTypeEntity.Convert(reader);
      return expenseType;
    }
    protected virtual List<ExpenseType> GetExpenseTypeCollectionFromReader(IDataReader reader)
    {
      List<ExpenseType> expenseTypeList = new List<ExpenseType>();
      while (reader.Read())
      {
        expenseTypeList.Add(GetExpenseTypeFromReader(reader));

      }
      return expenseTypeList;
    }
    #endregion



    #region projectsite,projectsalesRep

    protected virtual ProjectSite GetProjectSiteFromReader(IDataReader reader)
    {
      EntityConverter<ProjectSite> ProjectSiteEntity = new EntityConverter<ProjectSite>();
      ProjectSite projectSite = ProjectSiteEntity.Convert(reader);
      return projectSite;
    }
    protected virtual List<ProjectSite> GetProjectSiteCollectionFromReader(IDataReader reader)
    {
      List<ProjectSite> projectSiteList = new List<ProjectSite>();
      while (reader.Read())
      {
        projectSiteList.Add(GetProjectSiteFromReader(reader));

      }
      return projectSiteList;
    }


    protected virtual ProjectSalesRep GetProjectSalesRepFromReader(IDataReader reader)
    {
      EntityConverter<ProjectSalesRep> projectSalesRepEntity = new EntityConverter<ProjectSalesRep>();
      ProjectSalesRep projectSalesRep = projectSalesRepEntity.Convert(reader);
      return projectSalesRep;
    }
    protected virtual List<ProjectSalesRep> GetProjectSalesRepCollectionFromReader(IDataReader reader)
    {
      List<ProjectSalesRep> projectSalesRepList = new List<ProjectSalesRep>();
      while (reader.Read())
      {
        projectSalesRepList.Add(GetProjectSalesRepFromReader(reader));

      }
      return projectSalesRepList;
    }
       #endregion

    #region ifn contacts for print/papersize/printspecs
    protected virtual VM_UserDetails GetContactPersonIFNFromReader(IDataReader reader)
    {
      EntityConverter<VM_UserDetails> userDetailEntity = new EntityConverter<VM_UserDetails>();
      VM_UserDetails userDetail = userDetailEntity.Convert(reader);
      return userDetail;
    }
    protected virtual List<VM_UserDetails> GetContactPersonIFNCollectionFromReader(IDataReader reader)
    {
      List<VM_UserDetails> userDetailList = new List<VM_UserDetails>();
      while (reader.Read())
      {
        userDetailList.Add(GetContactPersonIFNFromReader(reader));

      }
      return userDetailList;
    }


    protected virtual PaperSize GetPaperSizeFromReader(IDataReader reader)
    {
      EntityConverter<PaperSize> PaperSizeEntity = new EntityConverter<PaperSize>();
      PaperSize paperSize = PaperSizeEntity.Convert(reader);
      return paperSize;
    }
    protected virtual List<PaperSize> GetPaperSizeCollectionFromReader(IDataReader reader)
    {
      List<PaperSize> paperSizeList = new List<PaperSize>();
      while (reader.Read())
      {
        paperSizeList.Add(GetPaperSizeFromReader(reader));

      }
      return paperSizeList;
    }



    protected virtual PrintSpecs GetPrintSpecsFromReader(IDataReader reader)
    {
      EntityConverter<PrintSpecs> PrintSpecsEntity = new EntityConverter<PrintSpecs>();
      PrintSpecs printSpecs = PrintSpecsEntity.Convert(reader);
      return printSpecs;
    }
    protected virtual List<PrintSpecs> GetPrintSpecsCollectionFromReader(IDataReader reader)
    {
      List<PrintSpecs> printSpecsList = new List<PrintSpecs>();
      while (reader.Read())
      {
        printSpecsList.Add(GetPrintSpecsFromReader(reader));

      }
      return printSpecsList;
    }

    protected virtual PublicHolidays GetPublicHolidaysFromReader(IDataReader reader)
    {
        EntityConverter<PublicHolidays> PublicHolidaysEntity = new EntityConverter<PublicHolidays>();
        PublicHolidays publicHolidays = PublicHolidaysEntity.Convert(reader);
        return publicHolidays;
    }
    protected virtual List<PublicHolidays> GetPublicHolidaysCollectionFromReader(IDataReader reader)
    {
        List<PublicHolidays> publicHolidaysList = new List<PublicHolidays>();
        while (reader.Read())
        {
            publicHolidaysList.Add(GetPublicHolidaysFromReader(reader));

        }
        return publicHolidaysList;
    }

    protected virtual List<PrintedAt> GetPrintAtCollectionFromReader(IDataReader reader)
    {
        List<PrintedAt> printedAt = new List<PrintedAt>();
        while (reader.Read())
        {
            printedAt.Add(GetPrintedAtFromReader(reader));

        }
        return printedAt;
    }
    protected virtual PrintedAt GetPrintedAtFromReader(IDataReader reader)
    {
        EntityConverter<PrintedAt> PrintedAtEntity = new EntityConverter<PrintedAt>();
        PrintedAt printedAt = PrintedAtEntity.Convert(reader);
        return printedAt;
    }

    protected virtual List<BookSize> GetBookSizeCollectionFromReader(IDataReader reader)
    {
        List<BookSize> BookSize = new List<BookSize>();
        while (reader.Read())
        {
            BookSize.Add(GetBookSizeFromReader(reader));

        }
        return BookSize;
    }
    protected virtual BookSize GetBookSizeFromReader(IDataReader reader)
    {
        EntityConverter<BookSize> PrintedAtEntity = new EntityConverter<BookSize>();
        BookSize BookSize = PrintedAtEntity.Convert(reader);
        return BookSize;
    }
    #endregion

    #region merchantNames

    protected virtual MerchantName GetMerchantNameFromReader(IDataReader reader)
    {
      EntityConverter<MerchantName> merchantNameEntity = new EntityConverter<MerchantName>();
      MerchantName merchantName = merchantNameEntity.Convert(reader);
      return merchantName;
    }
    protected virtual List<MerchantName> GetMerchantNameCollectionFromReader(IDataReader reader)
    {
      List<MerchantName> merchantNameList = new List<MerchantName>();
      while (reader.Read())
      {
        merchantNameList.Add(GetMerchantNameFromReader(reader));

      }
      return merchantNameList;
    }
    #endregion

      #region PrintFinishing
    protected virtual PrintFinishing GetFinishingTypeFromReader(IDataReader reader)
    {
        EntityConverter<PrintFinishing> printFinishingEntity = new EntityConverter<PrintFinishing>();
        PrintFinishing printFinishing = printFinishingEntity.Convert(reader);
        return printFinishing;
    }
    protected virtual List<PrintFinishing> GetFinishingCollectionFromReader(IDataReader reader)
    {
        List<PrintFinishing> printFinishingList = new List<PrintFinishing>();
        while (reader.Read())
        {
            printFinishingList.Add(GetFinishingTypeFromReader(reader));

        }
        return printFinishingList;
    }
      #endregion


      #region ProductionType

    protected virtual List<ProductionType> GetProductionTypeCollectionFromReader(IDataReader reader)
    {
        List<ProductionType> productionTypeList = new List<ProductionType>();
        while (reader.Read())
        {
            productionTypeList.Add(GetProdcutionTypeFromReader(reader));
        }
        return productionTypeList;
    }

    protected virtual ProductionType GetProdcutionTypeFromReader(IDataReader reader)
    {
        EntityConverter<ProductionType> productionTypeEntity = new EntityConverter<ProductionType>();
        ProductionType productionType = productionTypeEntity.Convert(reader);
        return productionType;
    }



      #endregion

    #region NewsPaperList

    protected virtual List<NewsPaper> GetNewsPaperCollectionFromReader(IDataReader reader)
    {
        List<NewsPaper> newsPaperList = new List<NewsPaper>();
        while (reader.Read())
        {
            newsPaperList.Add(GetNewspaperFromReader(reader));
        }
        return newsPaperList;
    }

    protected virtual NewsPaper GetNewspaperFromReader(IDataReader reader)
    {
        EntityConverter<NewsPaper> newsPaperEntity = new EntityConverter<NewsPaper>();
        NewsPaper newsPaper = newsPaperEntity.Convert(reader);
        return newsPaper;
    }



    #endregion

    #region TranslationsBy

    protected virtual List<TranslationDoneBy> GetTranslationByCollectionFromReader(IDataReader reader)
    {
        List<TranslationDoneBy> translatiionsByList = new List<TranslationDoneBy>();
        while (reader.Read())
        {
            translatiionsByList.Add(GetTranslationByFromReader(reader));
        }
        return translatiionsByList;
    }

    protected virtual TranslationDoneBy GetTranslationByFromReader(IDataReader reader)
    {
        EntityConverter<TranslationDoneBy> translationsByEntity = new EntityConverter<TranslationDoneBy>();
        TranslationDoneBy translationBy = translationsByEntity.Convert(reader);
        return translationBy;
    }



    #endregion

    protected virtual KeyValueObject GetKeyValueObjectFromReader(IDataReader reader)
    {
        EntityConverter<KeyValueObject> KeyValueEntity = new EntityConverter<KeyValueObject>();
        KeyValueObject keyValueObject = KeyValueEntity.Convert(reader);
        return keyValueObject;
    }
      
    protected virtual List<KeyValueObject> GetKeyValueObjectCollectionFromReader(IDataReader reader)
    {
        List<KeyValueObject> KeyValueEntityList = new List<KeyValueObject>();
        while (reader.Read())
        {
            KeyValueEntityList.Add(GetKeyValueObjectFromReader(reader));
        }
        return KeyValueEntityList;
    }

  }
   
}
