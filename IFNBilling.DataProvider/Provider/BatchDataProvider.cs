using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider
{
    public abstract class BatchDataProvider : DataAccess
    {
        static private BatchDataProvider _instance = null;

        static public BatchDataProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (BatchDataProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlBatchDataProvider"));
                return _instance;
            }
        }

        public BatchDataProvider()
        {
            this.ConnectionString = ServiceConfig.ConnectionString;
        }

        #region InsertTypeSettingData

       
        protected virtual TypeSetBatch GetTypesetBatchResultsFromReader(IDataReader reader)
        {
            EntityConverter<TypeSetBatch> typesetBatchEntity = new EntityConverter<TypeSetBatch>();
            EntityConverter<Batch> batchEntity = new EntityConverter<Batch>();
            TypeSetBatch typesetBatch = new TypeSetBatch();
            if (reader.Read())
            {
                typesetBatch = typesetBatchEntity.Convert(reader);
                typesetBatch.Batch = batchEntity.Convert(reader);
            }
            return typesetBatch;
        }

        protected virtual TranslationBatch GetTranslationBatchResultsFromReader(IDataReader reader)
        {
            EntityConverter<TranslationBatch> translationBatchEntity = new EntityConverter<TranslationBatch>();
            EntityConverter<Batch> batchEntity = new EntityConverter<Batch>();
            TranslationBatch translationBatch = new TranslationBatch();
            if (reader.Read())
            {
                translationBatch = translationBatchEntity.Convert(reader);
                translationBatch.Batch = batchEntity.Convert(reader);
            }
            return translationBatch;
        }



        #endregion

        #region



        protected virtual List<TypesetVM> GetTypeSetBatchCollectionFromReader(IDataReader reader)
        {
            List<TypesetVM> typeSetList = new List<TypesetVM>();
            while (reader.Read())
                typeSetList.Add(GetTypeSetFromReader(reader));
            return typeSetList;
        }

        protected virtual TypesetVM GetTypeSetFromReader(IDataReader reader)
        {
            EntityConverter<TypesetVM> typesetEntity = new EntityConverter<TypesetVM>();
            TypesetVM typeSetBatch = typesetEntity.Convert(reader);
            return typeSetBatch;
        }

     

        protected virtual List<TranslationVM> GetTranslationBatchCollectionFromReader(IDataReader reader)
        {
            List<TranslationVM> translationBatchList = new List<TranslationVM>();
            while (reader.Read())
                translationBatchList.Add(GetTranslationFromReader(reader));
            return translationBatchList;
        }

        protected virtual TranslationVM GetTranslationFromReader(IDataReader reader)
        {
            EntityConverter<TranslationVM> translationEntity = new EntityConverter<TranslationVM>();
            TranslationVM translationBatch = translationEntity.Convert(reader);
            return translationBatch;
        }

        #endregion

        #region 'Source Update By : VenkateshPrabu-108024 On 2015-Jan-29'

        //Abstract Methods - Batch
      

        //Virtual Methods - Batch
        protected virtual List<MediaBatchVM> GetMediaBatchCollectionFromReader(IDataReader reader)
        {
            List<MediaBatchVM> batchList = new List<MediaBatchVM>();
            while (reader.Read())
                batchList.Add(GetMediaBatchDetailFromReader(reader));
            return batchList;
        }

        protected virtual MediaBatchVM GetMediaBatchDetailFromReader(IDataReader reader)
        {
            EntityConverter<MediaBatchVM> p = new EntityConverter<MediaBatchVM>();
            MediaBatchVM batchDetail = p.Convert(reader);
            return batchDetail;
        }


        protected virtual MediaBatch GetMediaBatchFromReader(IDataReader dataReader)
        {
            EntityConverter<MediaBatch> mediaEntity = new EntityConverter<MediaBatch>();
            EntityConverter<Batch> batchEntity = new EntityConverter<Batch>();
            MediaBatch mediaBatch = new MediaBatch();
            if (dataReader.Read())
                mediaBatch = mediaEntity.Convert(dataReader);
            mediaBatch.batch = batchEntity.Convert(dataReader);
            return mediaBatch;

        }

        #endregion



        protected virtual HospitalityBatch GetHospitalityBatchFromReader(IDataReader reader)
        {
          EntityConverter<HospitalityBatch> hospitalityBatchEntity = new EntityConverter<HospitalityBatch>();
          EntityConverter<Batch> batchEntity = new EntityConverter<Batch>();

          EntityConverter<HospitalityConferenceRoomUsage> conferenceUsage = new EntityConverter<HospitalityConferenceRoomUsage>();
      EntityConverter<HospitalityNoOfMealsUsage> hospitalityNoOfMealsUsage = new EntityConverter<HospitalityNoOfMealsUsage>();

          HospitalityBatch hospitalityBatch = new HospitalityBatch();
          List<HospitalityConferenceRoomUsage> hospitalityConferenceRoomUsagelist = new List<HospitalityConferenceRoomUsage>();
      List<HospitalityNoOfMealsUsage> hospitalityNoOfMealsUsageList = new List<HospitalityNoOfMealsUsage>();

          if (reader.Read())
          {
            hospitalityBatch = hospitalityBatchEntity.Convert(reader);
            hospitalityBatch.Batch = batchEntity.Convert(reader);
       // hospitalityBatch.conferenceRoomDetailsList.Add(conferenceUsage.Convert(reader));
        //hospitalityBatch.hospitalityNoOfMealsUsageList.Add(hospitalityNoOfMealsUsage.Convert(reader));

      }

      if (reader.NextResult())
      {


        while (reader.Read())
        {
          hospitalityConferenceRoomUsagelist.Add(conferenceUsage.Convert(reader));
          
        }
      }

      if (reader.NextResult())
      {
        while (reader.Read())
        {
          hospitalityNoOfMealsUsageList.Add(hospitalityNoOfMealsUsage.Convert(reader));

          }

      }

      hospitalityBatch.hospitalityNoOfMealsUsageList = hospitalityNoOfMealsUsageList;
      hospitalityBatch.conferenceRoomDetailsList = hospitalityConferenceRoomUsagelist;

          return hospitalityBatch;
        }
    
        protected virtual PrintBatch GetPrintBatchFromReader(IDataReader reader)
        {
            EntityConverter<PrintBatch> printBatchEntity = new EntityConverter<PrintBatch>();
            EntityConverter<Batch> batchEntity = new EntityConverter<Batch>();
            PrintBatch printBatch = new PrintBatch();
            if (reader.Read())
            {
                printBatch = printBatchEntity.Convert(reader);
                printBatch.Batch = batchEntity.Convert(reader);
            }
            return printBatch;
        }
    
    
    protected virtual List<HospitalityVM> GetHospitalityVMBatchCollectionFromReader(IDataReader reader)
    {
      List<HospitalityVM> hospitalityVMBatchList = new List<HospitalityVM>();
      while (reader.Read())
        hospitalityVMBatchList.Add(GethospitalityVMBatchFromReader(reader));
      return hospitalityVMBatchList;
    }
       
    
    protected virtual HospitalityVM GethospitalityVMBatchFromReader(IDataReader reader)
    {
      EntityConverter<HospitalityVM> hospitalityVMEntity = new EntityConverter<HospitalityVM>();
      HospitalityVM hospitalityVMList = hospitalityVMEntity.Convert(reader);
      return hospitalityVMList;
    }



    protected virtual List<PrintVM> GetPrintBatchCollectionFromReader(IDataReader reader)
    {
      List<PrintVM> printVMList = new List<PrintVM>();
      while (reader.Read())
        printVMList.Add(GetPrintVMFromReader(reader));
      return printVMList;
    }
        protected virtual PrintVM GetPrintVMFromReader(IDataReader reader)
        {
            EntityConverter<PrintVM> printVMEntity = new EntityConverter<PrintVM>();
            PrintVM printVM = printVMEntity.Convert(reader);
            return printVM;
        }
    
    
    
    
    
    }
}
