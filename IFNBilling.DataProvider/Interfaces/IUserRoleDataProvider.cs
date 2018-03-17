using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;
namespace IFNBilling.DataProvider.Interfaces
{
    public interface IUserRoleDataProvider
    {
        List<User> GetUserDetails(string username, string Password = null, string Types = null);
        int InsertUpdateUserDetails(VM_UserDetails userDetails);
        bool CheckUserDetailsByUserName(string username);
        VM_UserDetails GetUserDetailsByUserId(int UserId);
        List<VM_UserDetails> GetAllIfnUsers();
        bool SetAuditTrailLog(AuditTrailLog auditTrailLog);
    }
}
