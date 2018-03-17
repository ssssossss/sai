using System;
using System.Data;
using System.Collections.Generic;


using IFNBilling.Domain.Model;
using IFNBilling.Domain.Model.ViewModel;

namespace IFNBilling.DataProvider
{
    public abstract class UserRoleProvider : DataAccess
    {
        static private UserRoleProvider _instance = null;

        static public UserRoleProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (UserRoleProvider)Activator.CreateInstance(Type.GetType("IFNBilling.DataProvider.MsSqlClient.MsSqlUserRoleProvider"));
                return _instance;
            }
        }

        public UserRoleProvider()
        {
            this.ConnectionString = ServiceConfig.ConnectionString;
        }

        #region User

        //Abstract Method - User Detail
        public abstract List<User> GetUserDetails(string username, string Password = null, string Types = null);

        //Virtual Method - User Details in reader
        protected virtual User GetUserDetailsFromReader(IDataReader reader)
        {
            EntityConverter<User> usr = new EntityConverter<User>();
            User user = usr.Convert(reader);
            return user;
        }

        //Virtual Method - User Details in collection
        protected virtual List<User> GetUserDetailsCollectionFromReader(IDataReader reader)
        {
            List<User> userList = new List<User>();
            while (reader.Read())
            {
                userList.Add(GetUserDetailsFromReader(reader));
            }
            return userList;
        }

        #endregion User


      #region get all ifn users

        protected virtual VM_UserDetails GetIfnUserFromReader(IDataReader iDataReader)
        {
          EntityConverter<VM_UserDetails> userDetailEntity = new EntityConverter<VM_UserDetails>();
          VM_UserDetails userDetailVM = userDetailEntity.Convert(iDataReader);
          
          return userDetailVM;

        }

        protected virtual List<VM_UserDetails> GetAllIfnUsersCollectionFromReader(IDataReader iDataReader)
        {
          List<VM_UserDetails> userDetailsList = new List<VM_UserDetails>();
          

          while (iDataReader.Read())
          {
            userDetailsList.Add(GetIfnUserFromReader(iDataReader));
          }
      
          

          return userDetailsList;
        }


      

      #endregion

        #region UserDetailsByUserId
        protected virtual VM_UserDetails GetUserDetailsByUserIdFromReader(IDataReader reader)
        {
            EntityConverter<VM_UserDetails> usr = new EntityConverter<VM_UserDetails>();
            EntityConverter<UserRole> userRolesEntity = new EntityConverter<UserRole>();
            VM_UserDetails user = new VM_UserDetails();
            UserRole userRoles = new UserRole();
            if (reader.Read())
            {
                user = usr.Convert(reader);
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    userRoles = userRolesEntity.Convert(reader);
                    user.Roles.Add(userRoles);
                }
            }
            return user;
        }


        #endregion
    }
}
