using BirdyModel.Attributes;
using BirdyModel.Birdy;
using BirdyModel.Database;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI.Database
{
    public static class Crud<TEntity> where TEntity : class
    {
        private static long Insert(DbConnection pConnection, TEntity entity, out string pException, DbTransaction pTransaction = null)
        {
            pException = "";

            try
            {
                if (pTransaction == null)
                {
                    return pConnection.Insert(entity);
                }

                return pConnection.Insert(entity, pTransaction);
            }
            catch (Exception ex)
            {
                pException = ex.Message;
                return -1;
            }
        }
        private static bool Update(DbConnection pConnection, TEntity entity, out string pException, DbTransaction pTransaction = null)
        {
            pException = "";

            try
            {
                if (pTransaction == null)
                {
                    return pConnection.Update(entity);
                }

                return pConnection.Update(entity, pTransaction);
            }
            catch (Exception ex)
            {
                pException = ex.Message;
                return false;
            }
        }

        public static dynamic Insert(TEntity entity, out string pException, MesnetDbConnection pConnection = null)
        {
            pException = "";
            long loResult = 0;

            if (pConnection == null)
            {
                using var conn = Connection.ConnectionWinvestate();
                loResult = Insert(conn, entity, out pException);
            }
            else
            {
                loResult = Insert(pConnection.MyConnection, entity, out pException, pConnection.MyTransaction);
            }

            if (loResult <= 0)
            {
                return loResult;
            }

            var loCheckAttribute = Attribute.GetCustomAttribute(entity.GetType(), typeof(IgnoreLoggingAttribute));

            if (loCheckAttribute != null)
            {
                return loResult;
            }

            var loTableName = (TableAttribute)Attribute.GetCustomAttribute(entity.GetType(), typeof(TableAttribute));

            //var loType = entity.GetType().BaseType == null ? entity.GetType().Name : entity.GetType().BaseType.Name;
            var loProperties = entity.GetType().GetProperties();
            var property = entity.GetType().GetProperty("id");
            property.SetValue(entity, (int)loResult, null);

            var loUserId = loProperties.FirstOrDefault(x => x.Name == "row_create_user")?.GetValue(entity);

            if (!Guid.TryParse(loUserId?.ToString(), out Guid loUser))
            {
                loUser = Guid.Empty;
            }

            var loPrimaryKeyField = loProperties.Where(x => Attribute.IsDefined(x, typeof(LoggingPrimaryKeyAttribute))).First()?.Name?.ToString();
            var loPrimaryKeyValue = loProperties.Where(x => Attribute.IsDefined(x, typeof(LoggingPrimaryKeyAttribute))).First().GetValue(entity)?.ToString();

            var loChange = new ChangeLog
            {
                operation = "INSERT",
                date_changed = DateTime.Now,
                old_value = "-",
                new_value = "Yeni Kayıt",
                property_name = loTableName.Name,
                class_name = loTableName.Name,
                primary_key = loPrimaryKeyValue,
                primary_key_field = loPrimaryKeyField,
                changed_user = loUser
            };

            if (pConnection == null)
            {

                Task.Run(() => Crud<ChangeLog>.Insert(loChange, out _));
            }
            else
            {
                if (pConnection.Changes == null)
                {
                    pConnection.Changes = new List<ChangeLog>();
                }

                pConnection.Changes.Add(loChange);
            }
            //Task.Run(() => HelperMethods.TrackChangesOfObject(entity, oldEntity));

            return loResult;
        }

        public static bool Update(TEntity entity, out string pException, MesnetDbConnection pConnection = null, TEntity oldEntity = null)
        {
            pException = "";
            bool loResult = false;

            if (pConnection == null)
            {
                using var conn = Connection.ConnectionWinvestate();
                loResult = Update(conn, entity, out pException);
            }
            else
            {
                loResult = Update(pConnection.MyConnection, entity, out pException, pConnection.MyTransaction);
            }


            if (!loResult)
            {
                return loResult;
            }

            var loChanges = HelperMethods.TrackChangesOfObject(entity, oldEntity);

            if (loChanges.Any())
            {
                if (pConnection == null)
                {
                    Task.Run(() => Crud<ChangeLog>.Insert(loChanges, out _));
                }
                else
                {
                    if (pConnection.Changes == null)
                    {
                        pConnection.Changes = new List<ChangeLog>();
                    }

                    pConnection.Changes.AddRange(loChanges);
                }
            }

            return loResult;
        }

        public static bool Delete(TEntity entity, MesnetDbConnection pConnection = null)
        {
            using (var conn = Connection.ConnectionWinvestate())
            {
                try
                {
                    return conn.Delete(entity);
                }
                catch (Exception ex)
                {
                    //   Common._graylogger.Error("Problem in delete operation-->" + ex.ToString());
                    //   Common._graylogger.Error("Stack Trace-->" + ex.StackTrace);
                    return false;
                }
            }
        }

        public static bool Delete(string pQuery)
        {
            using (var conn = Connection.ConnectionWinvestate())
            {
                try
                {
                    return conn.Execute(pQuery) > 0;
                }
                catch (Exception ex)
                {
                    //  Common._graylogger.Error("Problem in update operation-->" + ex.ToString());
                    //  Common._graylogger.Error("Stack Trace-->" + ex.StackTrace);
                    return false;
                }
            }
        }

        public static dynamic Insert(List<TEntity> entityList, out string pException)
        {
            pException = "";
            using (var conn = Connection.ConnectionWinvestate())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var entity in entityList)
                    {
                        try
                        {
                            conn.Insert(entity, transaction);
                        }
                        catch (Exception ex)
                        {
                            pException = ex.Message;
                            transaction.Rollback();
                            // Common._graylogger.Error("Problem in insert operation-->" + ex.ToString());
                            //Common._graylogger.Error("Stack Trace-->" + ex.StackTrace);
                            return false;
                        }
                    }
                    transaction.Commit();
                }
            }

            return true;
        }

        public static dynamic Update(List<TEntity> entityList, out string pException)
        {
            pException = "";
            using (var conn = Connection.ConnectionWinvestate())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var entity in entityList)
                    {
                        try
                        {
                            conn.Update(entity, transaction);
                        }
                        catch (Exception ex)
                        {
                            pException = ex.Message;
                            transaction.Rollback();
                            // Common._graylogger.Error("Problem in insert operation-->" + ex.ToString());
                            //Common._graylogger.Error("Stack Trace-->" + ex.StackTrace);
                            return false;
                        }
                    }
                    transaction.Commit();
                }
            }

            return true;
        }

        public static dynamic Delete(List<TEntity> entityList, out string pException)
        {
            pException = "";
            using (var conn = Connection.ConnectionWinvestate())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var entity in entityList)
                    {
                        try
                        {
                            conn.Delete(entity, transaction);
                        }
                        catch (Exception ex)
                        {
                            pException = ex.Message;
                            transaction.Rollback();
                            // Common._graylogger.Error("Problem in insert operation-->" + ex.ToString());
                            //Common._graylogger.Error("Stack Trace-->" + ex.StackTrace);
                            return false;
                        }
                    }
                    transaction.Commit();
                }
            }

            return true;
        }

        public static dynamic InsertLog(TEntity entity, out string pException)
        {
            using (var conn = Connection.ConnectionLog())
            {
                pException = "";
                try
                {
                    return conn.Insert(entity);
                }
                catch (Exception ex)
                {
                    pException = ex.Message;
                    // Common._graylogger.Error("Problem in insert operation-->" + ex.ToString());
                    //Common._graylogger.Error("Stack Trace-->" + ex.StackTrace);
                    return -1;
                }
            }
        }

        //public static int InsertCustomerWithCallBack(CallbackRecord pCallBack, Customer pCustomer)
        //{
        //   YAPILAN SON TRANSACTION GÜNCELLENMESİ UYGULANMADI!!!
        //    int loResult = 0;
        //    using var connection = Connection.ConnectionWinvestate();
        //    connection.Open();
        //    using var transaction = connection.BeginTransaction();

        //    try
        //    {
        //        Customer loMyCustomer = null;
        //        if (pCustomer.id <= 0)
        //        {
        //            loMyCustomer = pCustomer;
        //            loResult = (int)connection.Insert(loMyCustomer, transaction);
        //            if (loResult <= 0)
        //            {
        //                transaction.Rollback();
        //                return 0;
        //            }
        //            loMyCustomer.id = (int)loResult;
        //        }
        //        else
        //        {
        //            loMyCustomer = pCustomer;
        //        }

        //        if (pCallBack != null)
        //        {



        //            loResult = (int)connection.Insert(pCallBack, transaction);
        //            if (loResult <= 0)
        //            {
        //                transaction.Rollback();
        //                return 0;
        //            }



        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        return 0;
        //    }


        //    transaction.Commit();
        //    return loResult;
        //}
    }
}
