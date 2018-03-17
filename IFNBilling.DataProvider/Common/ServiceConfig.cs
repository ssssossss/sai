using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace IFNBilling.DataProvider
{
    public class ServiceConfig
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["IFNBSBConnectionString"].ToString();
            }
        }

    }
}
