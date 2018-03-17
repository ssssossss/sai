using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFNBilling.DataProvider
{
    public class EntityConverter<T> where T : new()
    {
        private T id = default(T);

        public T Convert(IDataReader reader)
        {
            id = new T();
            Type myObjectType = id.GetType();

            for (int iCnt = 0; iCnt < reader.FieldCount; iCnt++)
            {
                string Name = reader.GetName(iCnt);

                System.Reflection.PropertyInfo retprop = myObjectType.GetProperty(Name);
                if (retprop != null)
                {
                    if (!(reader.GetValue(iCnt) == DBNull.Value))
                    {
                        retprop.SetValue(id, reader.GetValue(iCnt), null);
                    }
                }
            }
            return id;
        }
    }
}
