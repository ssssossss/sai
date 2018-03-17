using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace IFNBilling.DataProvider
{
    public abstract class DataAccess
    {
        private string _connectionString = "";
        protected string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        protected int ExecuteNonQuery(DbCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }



        protected IDataReader ExecuteReader(DbCommand cmd)
        {
            return ExecuteReader(cmd, CommandBehavior.Default);
        }

        protected IDataReader ExecuteReader(DbCommand cmd, CommandBehavior behavior)
        {
            cmd.CommandTimeout = 60;
            return cmd.ExecuteReader(behavior);
        }

        protected object ExecuteScalar(DbCommand cmd)
        {
            return cmd.ExecuteScalar();
        }
    }
}
