using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;

namespace IFNBilling.DataProvider.Interfaces
{
    public interface IBatchDataProvider
    {
        #region 'Source Update By : VenkateshPrabu-108024 On 2015-Jan-29'

      List<MediaBatchVM> GetAllMediaBatchesByJob(string Job);

      MediaBatch GetMediaBatchesByBatchNumber(int batchNumber, string jobNumber);

      int InsertUpdateMediaBatch(MediaBatch mediaBatch);

 

        #endregion



      int InsertTypesetBatchDetails(TypeSetBatch typesetbatchdetails);
          TypeSetBatch GetTypeSetBatchDetailsByBatchNumber(int BatchNumber, string JobNumber);

          int InsertTranslationBatchDetails(TranslationBatch trbtch, bool IsSideJob);
          TranslationBatch GetTranslationBatchDetailsByBatchID(int BatchID, string JobNumber, bool IsSideJob);

          int UpdateTypsetBatchFromGrid(TypeSetBatch typesetBatchDetails);
          int UpdateTanslationFromGrid(TranslationBatch translationBatchDetails);

          List<TypesetVM> GetTypesetBatchDetails(string jobNumber);
          List<TranslationVM> GetTranslationBatchDetails(string jobNumber, bool IsSideJob);
          //Sprint-22#TFS#43603Translation -Bulk Update
          List<TranslationVM> GetTranslationBulkUpdateBatchDetails(string JobNumber, int FromBatch, int ToBatch, string Types);
          
          int UpdateTranslationBulkUpdateBatchDetails(BatchBulkUpdate Bulkupdate);

          int InsertUpdatePrintBatchTable(PrintBatch printBatchDetails);


          int InsertHospitalityBatchDetails(HospitalityBatch hospitalityBatch);
        
           //Sprint 20# TFS Hospitality - Connected Rooms 44239/44240/44241
          string ValidateConferenceRoomList(HospitalityBatch hospitalityBatch);

          HospitalityBatch GetHospitalityBatchDetailsByBatchNumber(int batchNumber, string jobNumber);

      List<HospitalityVM> GetHospitalityBatchDetailsByJobNumber(string jobNumber);

          PrintBatch GetPrintBatchDetailsByBatchId(int batchId, int jobId);

          List<PrintVM> GetPrintBatchDetailsByJobId(int jobId);

          int GetFirstBatchIdOfJobnumber(string jobNumber);

          bool BatchBulkUpdate(List<BatchUpdate> batchUpdateList, int serviceType, string ModifiedBy, string ModifiedByIP);
          int PrintEmail(int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime? timeIn, DateTime? deadLineDate);
          int JobTranslationsEmail(int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime timeIn, DateTime? deadLineDate);
          int JobMediaEmail(int projectNumber, string DeliveryEmail, string jobNumber, int batchID, string batchNumber, DateTime timeIn, DateTime deadLineDate);

          int CopyTypeSetAndTranslationData(TypeSetBatch typesetbatchdetails, string TypeOfJob);
          List<int> AddAdditionalPrintBatchbyDocType(Job job);
         
    }
}
