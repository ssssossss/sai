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

namespace IFNBilling.DataProvider
{
    public abstract class FileDataProvider : DataAccess
    {
        static private FileDataProvider _instance = null;

        static public FileDataProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (FileDataProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlFileDataProvider"));
                return _instance;
            }
        }

        public FileDataProvider()
        {
            this.ConnectionString = ServiceConfig.ConnectionString;
        }

        public abstract bool InsertUpdateFileRepository(FileRepository filedetails);

        public abstract List<FileRepository> GetFileDetailsByProjectId(int project_id);

        public abstract void RemoveFilesById(DataTable filerepositoryids);

        public abstract List<FileRepository> GetVendorInvoiceDetails(string filetype, string sourceid);

        protected virtual List<FileRepository> GetFileDetailsResultsCollectionFromReader(IDataReader reader)
        {
            List<FileRepository> fileDetailsList = new List<FileRepository>();
            while (reader.Read())
                fileDetailsList.Add(GetFileDetailsByIdFromReader(reader));
            return fileDetailsList;
        }

        protected virtual FileRepository GetFileDetailsByIdFromReader(IDataReader reader)
        {
            EntityConverter<FileRepository> file = new EntityConverter<FileRepository>();
            FileRepository filedetails = new FileRepository();
            filedetails = file.Convert(reader);
            return filedetails;
        }

        protected virtual List<FileRepository> GetVendorFileDetailsResultsCollectionFromReader(IDataReader reader)
        {
            List<FileRepository> vendorfileDetailsList = new List<FileRepository>();
            while (reader.Read())
                vendorfileDetailsList.Add(GetVendorFileDetailsByIdFromReader(reader));
            return vendorfileDetailsList;
        }

        protected virtual FileRepository GetVendorFileDetailsByIdFromReader(IDataReader reader)
        {
            EntityConverter<FileRepository> file = new EntityConverter<FileRepository>();
            FileRepository vendorfiledetails = new FileRepository();
            vendorfiledetails = file.Convert(reader);
            return vendorfiledetails;
        }


        public abstract List<FileRepository> GetFileDetailsByBatchId(long BatchId);

        protected virtual List<FileRepository> GetBatchFileDetailsResultsCollectionFromReader(IDataReader reader)
        {
          List<FileRepository> batchFileDetailsList = new List<FileRepository>();
          while (reader.Read())
            batchFileDetailsList.Add(GetVendorFileDetailsByIdFromReader(reader));
          return batchFileDetailsList;
        }

        protected virtual FileRepository GetBatchFileDetailsByIdFromReader(IDataReader reader)
        {
          EntityConverter<FileRepository> file = new EntityConverter<FileRepository>();
          FileRepository batchFileDetails = new FileRepository();
          batchFileDetails = file.Convert(reader);
          return batchFileDetails;
        }


    public abstract List<FileRepository> GetExpenseTrackingFilesById(int jobId, int expenseTrackingId);



  

    }
}