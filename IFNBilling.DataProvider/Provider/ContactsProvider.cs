using System;
using System.Data;
using System.Collections.Generic;


using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;

namespace IFNBilling.DataProvider
{
    public abstract class ContactsProvider : DataAccess
    {
        static private ContactsProvider _instance = null;

        static public ContactsProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (ContactsProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlContactsProvider"));
                return _instance;
            }
        }

        public ContactsProvider()
        {
            this.ConnectionString = ServiceConfig.ConnectionString;
        }

        #region Contact

        //Abstract Method - Contact Detail
        public abstract List<Contacts> GetContactDetailsByEmail(string email, string type);
        public abstract List<Contacts> GetContactDetailsByCompanyName(string companyname, string type);
        public abstract List<Contacts> GetContactDetailsByContactName(string contactname, string type);
        public abstract List<Contacts> GetContactDetailsByContactId(Int64 contactid, string type);
        public abstract List<Contacts> GetContactDetailsBySFDCContactId(string sfdccontactid, string type);
        public abstract List<Contacts> GetContactDetailsByCompanyNameandContactName(string companyname, string contactname, string type);
        public abstract List<Contacts> GetContactDetailsByCompanyNameandEmail(string companyname, string email, string type);
        public abstract List<Contacts> GetContactDetailsByCompanyNameContactNameAndEmail(string companyname, string firstname,string lastname, string email, string type);

        //Virtual Method - Contact Details in reader
        protected virtual Contacts GetContactsDetailsFromReader(IDataReader reader)
        {
            EntityConverter<Contacts> con = new EntityConverter<Contacts>();
            Contacts contact = con.Convert(reader);
            return contact;
        }

        //Virtual Method - Contact Details in collection
        protected virtual List<Contacts> GetContactsDetailsCollectionFromReader(IDataReader reader)
        {
            List<Contacts> contactsList = new List<Contacts>();
            while (reader.Read())
            {
                contactsList.Add(GetContactsDetailsFromReader(reader));
            }
            return contactsList;
        }

        #endregion Contact


        #region Get all contacts

        protected virtual Contacts GetContactsFromReader(IDataReader iDataReader)
        {
            EntityConverter<Contacts> contactDetailEntity = new EntityConverter<Contacts>();
            Contacts contactDetailObjects = contactDetailEntity.Convert(iDataReader);

            return contactDetailObjects;

        }

        protected virtual List<Contacts> GetAllContactsCollectionFromReader(IDataReader iDataReader)
        {
            List<Contacts> contactDetailsList = new List<Contacts>();


            while (iDataReader.Read())
            {
                contactDetailsList.Add(GetContactsFromReader(iDataReader));
            }



            return contactDetailsList;
        }




        #endregion

        #region ContactDetailsByUserId
        protected virtual Contacts GetContactDetailsByEmailFromReader(IDataReader reader)
        {
            EntityConverter<Contacts> cont = new EntityConverter<Contacts>();

            Contacts contact = new Contacts();

            if (reader.Read())
            {
                contact = cont.Convert(reader);
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {

                    contact.Add_New_Contacts_List.Add(contact);
                }
            }
            return contact;
        }


        #endregion
    }
}
