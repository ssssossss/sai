using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFNBilling.DataProvider;

namespace IFNBilling.DataProvider
{
    public static class IFNBillingDataProvider
    {        
        public static ProjectProvider Project
        {
            get { return ProjectProvider.Instance; }
        }

        public static MasterDataProvider MasterData
        {
            get { return MasterDataProvider.Instance; }
        }

        public static UserRoleProvider UserRole
        {
            get { return UserRoleProvider.Instance; }
        }

        public static FileDataProvider FileData
        {
            get { return FileDataProvider.Instance; }
        }

        public static JobProvider Job
        {
          get { return JobProvider.Instance; }
        }

        public static BatchDataProvider Batch
        {
            get { return BatchDataProvider.Instance; }
        }

    }
}
