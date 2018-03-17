using IFNBilling.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider
{
  public abstract class JobProvider:DataAccess
  {

    static private JobProvider _instance = null;

    static public JobProvider Instance
    {
      get
      {
        if (_instance == null)
          _instance = (JobProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlJobProvider"));
        return _instance;
      }
    }

    
        public JobProvider()
        {
            this.ConnectionString = ServiceConfig.ConnectionString;
        }

        #region

       


        protected virtual List<JobSearchVM> GetJobSearchCollectionFromReader(IDataReader reader)
        {
          List<JobSearchVM> jobSearchList = new List<JobSearchVM>();
          while (reader.Read())
            jobSearchList.Add(GetJobFromReader(reader));
          return jobSearchList;
        }

        protected virtual JobSearchVM GetJobFromReader(IDataReader reader)
        {
          EntityConverter<JobSearchVM> jobSearchEntity = new EntityConverter<JobSearchVM>();
          JobSearchVM jobSearchVM = jobSearchEntity.Convert(reader);
          return jobSearchVM;
        }

        protected virtual List<JobSearchVM> GetJobresultsCollectionFromReader(IDataReader reader)
        {
            List<JobSearchVM> jobSearchList = new List<JobSearchVM>();
            while (reader.Read())
                jobSearchList.Add(GetJobresultsFromReader(reader));
            return jobSearchList;
        }

        protected virtual JobSearchVM GetJobresultsFromReader(IDataReader reader)
        {
            EntityConverter<JobSearchVM> jobSearchEntity = new EntityConverter<JobSearchVM>();
            JobSearchVM jobSearchVM = jobSearchEntity.Convert(reader);
            return jobSearchVM;
        }

        #endregion

   


  

   


    #region

     

        
    #endregion

    #region Add Job Abstract Method

       
    #endregion Add Job Abstract Method        

       

        protected virtual JobSearchVM GetJobDetailsByJobIDFromReader(IDataReader reader)
    {
        EntityConverter<JobSearchVM> jobSearchEntity = new EntityConverter<JobSearchVM>();
        JobSearchVM jobDetails = new JobSearchVM();
        if (reader.Read())
        {
            jobDetails = jobSearchEntity.Convert(reader);
        }
        return jobDetails;
    }


    #region expensetracking

        protected virtual HospitalityExpense GetHospitalityExpenseFromreader(IDataReader reader)
        {
          EntityConverter<HospitalityExpense> hospitalityExpenseEntity = new EntityConverter<HospitalityExpense>();
          HospitalityExpense hospitalityExpense = hospitalityExpenseEntity.Convert(reader);
          return hospitalityExpense;

        }


        protected virtual List<HospitalityExpense> GetHospitalityExpenseCollectionFromReader(IDataReader reader)
        {
          List<HospitalityExpense> hospitalityExpenseList = new List<HospitalityExpense>();
          while (reader.Read())
          {
            hospitalityExpenseList.Add(GetHospitalityExpenseFromreader(reader));
          }
          return hospitalityExpenseList;
        }
#endregion


        protected virtual JobSearchVM GetJobnumberandJobTypeFromReader(IDataReader reader)
        {
            EntityConverter<JobSearchVM> jobSearchEntity = new EntityConverter<JobSearchVM>();
            JobSearchVM jobDetails = new JobSearchVM();
            if (reader.Read())
            {
                jobDetails = jobSearchEntity.Convert(reader);
            }
            return jobDetails;
        }


        #region Job Over Time Hours

        protected virtual JobOverTimeHours GetJobOverTimeHoursFromreader(IDataReader reader)
        {
            EntityConverter<JobOverTimeHours> jobOverTimeHoursExpenseEntity = new EntityConverter<JobOverTimeHours>();
            JobOverTimeHours JobOverTimeExpense = jobOverTimeHoursExpenseEntity.Convert(reader);
            return JobOverTimeExpense;

        }

        protected virtual EmailDistribution GetEmailDistributionListFromreader(IDataReader reader)
        {
            EntityConverter<EmailDistribution> jobEmailDistributionExpenseEntity = new EntityConverter<EmailDistribution>();
            EmailDistribution emailDistributionExpense = jobEmailDistributionExpenseEntity.Convert(reader);
            return emailDistributionExpense;

        }



        protected virtual List<JobOverTimeHours> GetJobOverTimeHoursCollectionFromReader(IDataReader reader)
        {
            List<JobOverTimeHours> jobOverTimeExpenseList = new List<JobOverTimeHours>();
            while (reader.Read())
            {
                jobOverTimeExpenseList.Add(GetJobOverTimeHoursFromreader(reader));
            }
            return jobOverTimeExpenseList;
        }


        protected virtual List<EmailDistribution> GetEmailDistributionCollectionFromReader(IDataReader reader)
        {
            List<EmailDistribution> emailDistributionExpenseList = new List<EmailDistribution>();
            while (reader.Read())
            {
                emailDistributionExpenseList.Add(GetEmailDistributionListFromreader(reader));
            }
            return emailDistributionExpenseList;
        }

        #endregion

  }
}
