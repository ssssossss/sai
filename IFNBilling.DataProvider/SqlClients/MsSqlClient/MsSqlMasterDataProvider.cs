using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using IFNBilling.Domain.Model;
using IFNBilling.DataProvider.Interfaces;
using IFNBilling.Domain.Model.ViewModel;

namespace IFNBilling.DataProvider.MsSqlClient
{
  public class MsSqlMasterDataProvider : MasterDataProvider, IMasterDataProvider
    {
    public List<ProjectStatus> GetProjectStatus()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetProjectStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetProjectStatusCollectionFromReader(ExecuteReader(cmd));
            }
        }

    public List<ProjectType> GetProjectType()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetProjectType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetProjectTypeCollectionFromReader(ExecuteReader(cmd));
            }
        }

    public List<ProjectType> GetProjectTypeByVertical(byte ViewType,int iProjectVerticalID)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("GetProjectTypeByVertical", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ViewType", ViewType);
            cmd.Parameters.AddWithValue("@ProjectVerticalID", iProjectVerticalID);
            cn.Open();
            return GetProjectTypeCollectionFromReader(ExecuteReader(cmd));
        }
    }

    public List<ProjectVertical> GetProjectVertical()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetProjectVertical", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetProjectVerticalCollectionFromReader(ExecuteReader(cmd));
            }
        }

    public List<ServiceType> GetServiceType()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetServiceType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceName", "serviceName");
                cmd.Parameters.AddWithValue("@Type", "type");
                cn.Open();
                return GetServiceTypeCollectionFromReader(ExecuteReader(cmd));
            }
        }

    public List<ServiceType> GetServiceTypeByServiceName(string serviceName, string type)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("spGetServiceType", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ServiceName", serviceName);
            cmd.Parameters.AddWithValue("@Type", type);
            cn.Open();
            return GetServiceTypeCollectionFromReader(ExecuteReader(cmd));
        }
    }


    public List<InvoiceType> GetInvoiceType()
        {

          using (SqlConnection cn = new SqlConnection(this.ConnectionString))
          {
            SqlCommand cmd = new SqlCommand("spGetInvoiceType", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetInvoiceTypeCollectionFromReader(ExecuteReader(cmd));
          }
          
        }

    public List<CodeMaster> GetInvoiceMileStone()
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCodeMaster", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "INM");
                cn.Open();
                return GetInvoiceMileStoneCollectionFromReader(ExecuteReader(cmd));
            }

        }

    public List<CodeMaster> GetProjectPhase()
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCodeMaster", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "PPH");
                cn.Open();
                return GetProjectPhaseCollectionFromReader(ExecuteReader(cmd));
            }

        }


    public List<JobType> GetJobType(int projectNumber)
        {

          using (SqlConnection cn = new SqlConnection(this.ConnectionString))
          {
            SqlCommand cmd = new SqlCommand("[Job_GetJobTypeByProjectNumber]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectNumber", projectNumber);
            cn.Open();
            return GetJobTypeCollectionFromReader(ExecuteReader(cmd));
          }
 
        }

    public List<InhouseLocation> GetInhouseLocation()
    {

        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Job_GetInhouselocation]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetInhouseCollectionFromReader(ExecuteReader(cmd));
        }

    }


    public List<DocumentType> GetDocumentType(int projectTypeID, string type)
        {
          using (SqlConnection cn = new SqlConnection(this.ConnectionString))
          {
            SqlCommand cmd = new SqlCommand("spGetDocumentType", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectTypeID", projectTypeID);
            cmd.Parameters.AddWithValue("@Type", type);
            cn.Open();
            return GetDocumentTypeCollectionFromReader(ExecuteReader(cmd));
          }
        }

    public List<BSBManual> GetDocumentManual(int DocumentId, string DocumentName, string CreatedBy, string types)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("Sp_BSB_Manual", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DocumentId", DocumentId);
            cmd.Parameters.AddWithValue("@DocumentName", DocumentName);
            cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
            cmd.Parameters.AddWithValue("@Types", types);
            cn.Open();
            return GetDocumentManualCollectionFromReader(ExecuteReader(cmd));
        }
    }


    public List<Turnaround> GetTurnAround()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Batch_GetTurnAround", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetTurnAroundCollectionFromReader(ExecuteReader(cmd));
            }

        }

    public List<ProjectNumbers> GetProjectNumberJobNumber(int ProjectNumber, string JobNumber, string Types)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GetReOpenProjectJobs", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectNumber", ProjectNumber);
                cmd.Parameters.AddWithValue("@JobNumber", JobNumber);
                cmd.Parameters.AddWithValue("@Type", Types);

                cn.Open();
                 return GetProjectNumberJobNumberCollectionFromReader(ExecuteReader(cmd));
            }

        }

      

    public List<CodeMaster> GetInstructions()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCodeMaster", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "INR");
                cn.Open();
                return GetInstructionsCollectionFromReader(ExecuteReader(cmd));
            }

        }
    //Tfs 43760 Blacklines and Batch Comment will be displayed.
    public List<CodeMaster> GetCodeMaster(string Type)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("spGetCodeMaster", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodeType", Type);
            cn.Open();
            return GetInstructionsCollectionFromReader(ExecuteReader(cmd));
        }

    }

    public List<OverTime> GetOverTime(string TeamName)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Batch_GetOverTime", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeamName", TeamName);
                cn.Open();
                return GetOverTimeCollectionFromReader(ExecuteReader(cmd));
            }

        }

    public List<BatchStatus> GetBatchStatus()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Batch_GetBatchStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                return GetBatchStatusCollectionFromReader(ExecuteReader(cmd));
            }

        }

    public List<CodeMaster> GetTypesetLanguage()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("Batch_GetTypesetLanguages", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "TYL");
                cn.Open();
                return GetTypesetLanguagesCollectionFromReader(ExecuteReader(cmd));
            }

        }


    public List<TranslationServiceType> GetTranslationServiceType()
        {

          using (SqlConnection cn = new SqlConnection(this.ConnectionString))
          {
            SqlCommand cmd = new SqlCommand("[spGetTranslationServiceType]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetTranslationServiceTypeCollectionFromReader(ExecuteReader(cmd));
          }

        }

        public List<PrintType> GetPrintType(string PrintTypeCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[Batch_GetPrintTypes]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@in_PrintTypeCategory", PrintTypeCategory);
                cn.Open();
                return GetPrintTypeCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public List<CodeMaster> GetPhotoCopy()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCodeMaster", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "PHC");
                cn.Open();
                return GetPhotCopyCollectionFromReader(ExecuteReader(cmd));
            }

        }

        public List<CodeMaster> GetBindingTypes()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCodeMaster", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "BDG");
                cn.Open();
                return GetBindingTypesCollectionFromReader(ExecuteReader(cmd));
            }

        }

        public List<CodeMaster> GetLaminationTypes()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[Codemaster_GetCodeMasterByCodeType]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "LMN");
                cn.Open();
                return GetLaminationTypesCollectionFromReader(ExecuteReader(cmd));
            }
        }

        public List<CodeMaster> GetVarnishingTypes()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[Codemaster_GetCodeMasterByCodeType]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodeType", "VRG");
                cn.Open();
                return GetVarnishingTypesCollectionFromReader(ExecuteReader(cmd));
            }
        }








    public List<ServiceTypeHospitality> GetHospitalityTypeOfService()
    {

      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetHospitalityServiceType]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetServiceTypeHospitalityCollectionFromReader(ExecuteReader(cmd));
      }

    }
    public List<JobCategory> GetJobCategory()
    {

      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetJobCategory]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetJobCategoryCollectionFromReader(ExecuteReader(cmd));
      }

    }
    public List<MealsType> GetTypeOfMeals()
    {

      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetMealsType]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetMealsTypeCollectionFromReader(ExecuteReader(cmd));
      }

    }
    public List<NoOfMeals> GetNoOfMeals()
    {

      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetNoOfMeals]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetNoOfMealsCollectionFromReader(ExecuteReader(cmd));
      }

    }
    public List<HospitalityServiceRequested> GetServiceRequestedBy()
    {

      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetHospitalityServiceRequestedBy]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetHospitalityServiceRequestedCollectionFromReader(ExecuteReader(cmd));
      }

    }

    public List<ConferenceRoom> GetConferenceRooms()
    {

      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetConferenceRoom]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetConferenceRoomCollectionFromReader(ExecuteReader(cmd));
      }

    }

    public List<ExpenseType> GetExpenseType()
    {
      using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
      {
        SqlCommand sqlCommand = new SqlCommand("[spGetExpenseType]", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlConnection.Open();
        return GetExpenseTypeCollectionFromReader(ExecuteReader(sqlCommand));

      }
    }
    public List<VM_ClientContact> GetClientContacts()
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Admin_GetCompanyDetails]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetClientContactsCollectionFromReader(ExecuteReader(cmd));
        }

    }   

    public List<Role> GetRoleList()
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Admin_GetRoleList]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetRoleListCollectionFromReader(ExecuteReader(cmd));
        }

    }



    public List<ProjectSite> GetProjectSite()
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetProjectSite]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetProjectSiteCollectionFromReader(ExecuteReader(cmd));
      }

    }


    public List<ProjectSalesRep> GetProjectSalesRep()
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[spGetProjectSalesRep]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetProjectSalesRepCollectionFromReader(ExecuteReader(cmd));
      }

    }


    public List<VM_UserDetails> GetIFNContacts()
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[Print_GetContactPersonIFN]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetContactPersonIFNCollectionFromReader(ExecuteReader(cmd));
      }

    }


    public List<PaperSize> GetPaperSize()
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[Batch_GetPaperSize]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetPaperSizeCollectionFromReader(ExecuteReader(cmd));
      }

    }



    public List<PrintSpecs> GetPrintSpecs()
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("[Batch_GetPrintSpecs]", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        return GetPrintSpecsCollectionFromReader(ExecuteReader(cmd));
      }

    }

    public List<PrintedAt> GetPrintAt()
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Batch_GetPrintAt]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetPrintAtCollectionFromReader(ExecuteReader(cmd));
        }

    }

    public List<BookSize> GetBookSize()
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Batch_GetBookSize]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetBookSizeCollectionFromReader(ExecuteReader(cmd));
        }

    }


    public List<PublicHolidays> GetPublicHolidays(int projectSiteID)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("spGetPublicHolidays", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectSiteID", projectSiteID);
            cn.Open();
            return GetPublicHolidaysCollectionFromReader(ExecuteReader(cmd));
        }
    }

    public List<ReportType> GetReportTypes()
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("Report_GetReportTypes", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            return GetReportTypesCollectionFromReader(ExecuteReader(cmd));
        }
    }

    




    public List<MerchantName> GetMerchantNames()
    {
      using (SqlConnection cn = new SqlConnection(this.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand("spGetMerchantNames", cn);
        cmd.CommandType = CommandType.StoredProcedure;
  
        cn.Open();
        return GetMerchantNameCollectionFromReader(ExecuteReader(cmd));
      }
    }

    public List<PrintFinishing> GetPrintFinishing()
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("Batch_GetPrintFinishingTypes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            return GetFinishingCollectionFromReader(ExecuteReader(cmd));
        }

    }

    public List<ProductionType> GetProductionType()
    {

        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
             SqlCommand cmd = new SqlCommand("Master_GetProductionType", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            return GetProductionTypeCollectionFromReader(ExecuteReader(cmd));
        }
    }


    public List<NewsPaper> GetNewsPaperList()
    {

        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Master_GetNewsPapersList]", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            return GetNewsPaperCollectionFromReader(ExecuteReader(cmd));
        }
    }


    public List<TranslationDoneBy> GetTranslationDoneBy()
    {

        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[Master_GetTranslationDoneBy]", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            return GetTranslationByCollectionFromReader(ExecuteReader(cmd));
        }
    }

    public List<KeyValueObject> GetJobListForPrintOrder(int projectNumber, string jobNumber, byte viewType)
    {
        using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("[getJobListForPrintOrder]", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@in_ProjectNumber", projectNumber);
            cmd.Parameters.AddWithValue("@in_JobNumber", jobNumber);
            cmd.Parameters.AddWithValue("@in_ViewType", viewType);
            cn.Open();
            return GetKeyValueObjectCollectionFromReader(ExecuteReader(cmd));
        }
    }

      }
    


    }
