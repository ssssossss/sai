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
    public abstract class ProjectProvider : DataAccess
    {
        static private ProjectProvider _instance = null;

        static public ProjectProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (ProjectProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlProjectProvider"));
                return _instance;
            }
        }

        public ProjectProvider()
        {
            this.ConnectionString = ServiceConfig.ConnectionString;
        }

        //Abstract Methods - Project
     

        //Virtual Methods - Project
        protected virtual List<Project> GetProjectCollectionFromReader(IDataReader reader)
        {
            List<Project> projectList = new List<Project>();
            while (reader.Read())
                projectList.Add(GetProjectDetailFromReader(reader));
            return projectList;
        }

        protected virtual Project GetProjectDetailFromReader(IDataReader reader)
        {
            EntityConverter<Project> p = new EntityConverter<Project>();
            Project projectDetail = p.Convert(reader);
            return projectDetail;
        }

        protected virtual List<VM_CompanyContactDetails> GetCompanyDetailsCollectionFromReader(IDataReader reader)
        {
            List<VM_CompanyContactDetails> companyContactList = new List<VM_CompanyContactDetails>();
            while (reader.Read())
                companyContactList.Add(GetCompanyDetailsFromReader(reader));
            return companyContactList;
        }

        protected virtual VM_CompanyContactDetails GetCompanyDetailsFromReader(IDataReader reader)
        {
            EntityConverter<VM_CompanyContactDetails> c = new EntityConverter<VM_CompanyContactDetails>();
            VM_CompanyContactDetails companyDetail = c.Convert(reader);
            return companyDetail;
        }
        protected virtual List<VM_ProjectSearch> GetProjectSearchResultsCollectionFromReader(IDataReader reader)
        {
          List<VM_ProjectSearch> projectList = new List<VM_ProjectSearch>();
          while (reader.Read())
            projectList.Add(GetProjectFromReader(reader));
          return projectList;
        }

        protected virtual VM_ProjectSearch GetProjectFromReader(IDataReader reader)
        {
          EntityConverter<VM_ProjectSearch> projectEntity = new EntityConverter<VM_ProjectSearch>();
          VM_ProjectSearch projectSearch = projectEntity.Convert(reader);
          return projectSearch;
        }

        protected virtual Project GetProjectDetailsByIdFromReader(IDataReader reader)
        {
            EntityConverter<Project> p = new EntityConverter<Project>();
            EntityConverter<ProjectVertical> pv = new EntityConverter<ProjectVertical>();
            EntityConverter<ProjectStatus> ps = new EntityConverter<ProjectStatus>();
            EntityConverter<Contact> co = new EntityConverter<Contact>();
            EntityConverter<CodeMaster> cm = new EntityConverter<CodeMaster>();
            EntityConverter<ProjectType> pt = new EntityConverter<ProjectType>();
            EntityConverter<Company> com = new EntityConverter<Company>();
            EntityConverter<ProjectSite> projectSiteEntity = new EntityConverter<ProjectSite>();
            EntityConverter<ProjectSalesRep> projectSalesRepEntity = new EntityConverter<ProjectSalesRep>();
            Project project_details = new Project();
            if (reader.Read())
            {
                project_details = p.Convert(reader);
                project_details.Contact = co.Convert(reader);
                project_details.ProjectVertical = pv.Convert(reader);
                project_details.Contact.Company = com.Convert(reader);
                project_details.ProjectStatus = ps.Convert(reader);
                project_details.CodeMaster = cm.Convert(reader);
                project_details.ProjectType = pt.Convert(reader);
                project_details.projectSite = projectSiteEntity.Convert(reader);
                project_details.projectSalesRep = projectSalesRepEntity.Convert(reader);
                

            }

            return project_details;
        }

          #region
  


    protected virtual List<ProjectInvoiceStatus> GetProjectInvoiceStatusCollectionFromReader(IDataReader reader)
    {
      List<ProjectInvoiceStatus> projectInvoiceStatusList = new List<ProjectInvoiceStatus>();
      while (reader.Read())
        projectInvoiceStatusList.Add(GetProjectInvoiceStatusFromReader(reader));
      return projectInvoiceStatusList;
    }

    protected virtual ProjectInvoiceStatus GetProjectInvoiceStatusFromReader(IDataReader reader)
    {
      EntityConverter<ProjectInvoiceStatus> projectInvoiceStatusEntity = new EntityConverter<ProjectInvoiceStatus>();
      EntityConverter<InvoiceType> invoiceTypeEntity = new EntityConverter<InvoiceType>();
      EntityConverter<CodeMaster> codemasterEntity = new EntityConverter<CodeMaster>();

      ProjectInvoiceStatus projectInvoiceStatus = projectInvoiceStatusEntity.Convert(reader);
      InvoiceType invoiceType = new InvoiceType();
      CodeMaster codemaster = new CodeMaster();
      codemaster = codemasterEntity.Convert(reader);
      invoiceType = invoiceTypeEntity.Convert(reader);
      projectInvoiceStatus.InvoiceType1 = invoiceType;
      projectInvoiceStatus.codeMaster = codemaster;

      return projectInvoiceStatus;
    }


          #endregion








 protected virtual List<ProjectVendorTracking> GetProjectVendorTrackingCollectionFromReader(IDataReader reader)
 {
     List<ProjectVendorTracking> projectVendorTrackingList = new List<ProjectVendorTracking>();
     while(reader.Read())    
     {
         projectVendorTrackingList.Add(GetProjectVendorTrackingFromReader(reader));     
     }
     return projectVendorTrackingList;
 }


 protected virtual ProjectVendorTracking GetProjectVendorTrackingFromReader(IDataReader reader)
 {
     EntityConverter<ProjectVendorTracking> pvt = new EntityConverter<ProjectVendorTracking>();
     ProjectVendorTracking projectVendorTracking = pvt.Convert(reader);
     return projectVendorTracking;
 }

 protected virtual List<string> GetJobNumbersFromReader(IDataReader reader)
 {
   List<string> jobNumberList = new List<string>();
   while (reader.Read())
   {
     jobNumberList.Add(reader.GetString(0));
   }
   return jobNumberList;
 }

 protected virtual List<EmailJobNumber> GetEmailBatchNumberCollectionFromReader(IDataReader reader)
 {
     List<EmailJobNumber> jobNumberList = new List<EmailJobNumber>();
     while (reader.Read())
     {
         jobNumberList.Add(GetEmailBatchNumberFromReader(reader));
     }
     return jobNumberList;
 }

 public EmailJobNumber GetEmailBatchNumberFromReader(IDataReader dataReader)
 {
     EntityConverter<EmailJobNumber> distroEntity = new EntityConverter<EmailJobNumber>();
     EmailJobNumber emailBatchNumber = distroEntity.Convert(dataReader);
     return emailBatchNumber;
 }
      




 public DistributionList GetDistributionListFromReader(IDataReader dataReader)
 {
   EntityConverter<DistributionList> distroEntity = new EntityConverter<DistributionList>();
   DistributionList distributionList = distroEntity.Convert(dataReader);
   return distributionList;
 }

 public List<DistributionList> GetDistributionListCollectionFromReader(IDataReader iDataReader)
 {
   List<DistributionList> projectDistributionList = new List<DistributionList>();
   while (iDataReader.Read())
   {
     projectDistributionList.Add(GetDistributionListFromReader(iDataReader));
   }
   return projectDistributionList;
 }

 public JobNumberText GetJobNumberTextFromReader(IDataReader dataReader)
 {
     EntityConverter<JobNumberText> distroEntity = new EntityConverter<JobNumberText>();
     JobNumberText JobNumberTextList = distroEntity.Convert(dataReader);
     return JobNumberTextList;
 }

 public List<JobNumberText> GetJobNumberTextCollectionFromReader(IDataReader iDataReader)
 {
     List<JobNumberText> projectJobNumberTextList = new List<JobNumberText>();
   while (iDataReader.Read())
   {
       projectJobNumberTextList.Add(GetJobNumberTextFromReader(iDataReader));
   }
   return projectJobNumberTextList;
 }


        

 public DistributionList GetCompanyListFromReader(IDataReader dataReader)
 {
     EntityConverter<DistributionList> distroEntity = new EntityConverter<DistributionList>();
     DistributionList companyList = distroEntity.Convert(dataReader);
     return companyList;
 }

 public List<DistributionList> GetCompanyCollectionFromReader(IDataReader iDataReader)
 {
     List<DistributionList> CompanyList = new List<DistributionList>();
     while (iDataReader.Read())
     {
         CompanyList.Add(GetCompanyListFromReader(iDataReader));
     }
     return CompanyList;
 }

 public List<VM_AddDistribution> GetAddressDetailsCollectionFromReader(IDataReader iDataReader)
 {
     List<VM_AddDistribution> addressLists = new List<VM_AddDistribution>();
     while (iDataReader.Read())
     {
         addressLists.Add(GetAddressDetailsFromReader(iDataReader));
     }
     return addressLists;
 }

 public VM_AddDistribution GetAddressDetailsFromReader(IDataReader dataReader)
 {
     EntityConverter<VM_AddDistribution> vmdistroEntity = new EntityConverter<VM_AddDistribution>();
     VM_AddDistribution addressList = vmdistroEntity.Convert(dataReader);
     return addressList;

 }

 public DistributionList  GetContactDetailsByDistributionListIdFromReader(IDataReader dataReader)
 {
     EntityConverter<DistributionList> vmdistroEntity = new EntityConverter<DistributionList>();
     DistributionList contactDetails = new DistributionList();
     if (dataReader.Read())
     {
         contactDetails = vmdistroEntity.Convert(dataReader);
     }
     return contactDetails;

 }

 public LabelSystem GetLabelSystemByProjectNumberFromReader(IDataReader reader)
 {
   EntityConverter<LabelSystem> labelSystemEntity = new EntityConverter<LabelSystem>();
   LabelSystem labelSystem = labelSystemEntity.Convert(reader);
   return labelSystem;

 }


      public List<LabelSystem> GetLabelSystemCollectionFromReader(IDataReader reader)
      {

        List<LabelSystem> labelSystemList = new List<LabelSystem>();

        while (reader.Read())
        {
          labelSystemList.Add(GetLabelSystemByProjectNumberFromReader(reader));
        }
        return labelSystemList;
      }

      
    }


}
