using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;

namespace IFNBilling.DataProvider.Interfaces
{
    public interface IContactsDataProvider
    {
        List<Contacts> GetContactDetailsByEmail(string email, string type);

        List<Contacts> GetContactDetailsByCompanyName(string companyname, string type);

        List<Contacts> GetContactDetailsByContactName(string contactname, string type);

        List<Contacts> GetContactDetailsByContactId(Int64 contactid, string type);

        List<Contacts> GetContactDetailsBySFDCContactId(string sfdccontactid, string type);

        List<Contacts> GetContactDetailsByCompanyNameandContactName(string companyname, string contactname, string type);

        List<Contacts> GetContactDetailsByCompanyNameandEmail(string companyname, string email, string type);

        List<Contacts> GetContactDetailsByCompanyNameContactNameAndEmail(string companyname, string firstname, string lastname, string email, string type);

        int InsertOrUpdateContactDetails(Contacts contactDetails);

        bool CheckContactDetailsByEmail(string email, string checkemail);

        List<Contacts> GetAllContacts(string type);
    }
}
