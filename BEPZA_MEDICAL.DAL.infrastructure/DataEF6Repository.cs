using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.Objects;
using System.Linq.Expressions;
using System.Data.Entity;
//using System.Data;
//using System.Data.Objects.DataClasses;
using System.Collections;
using LinqKit;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects.DataClasses;

namespace BEPZA_MEDICAL.DAL.Infrastructure
{

    public class  DataEF6Repository<T> : IEF6Repository<T> where T : class
    {

        /// <summary>
        /// The context object for the database
        /// </summary>
        private ObjectContext _context;

        /// <summary>
        /// The IObjectSet that represents the current entity.
        /// </summary>
        private IObjectSet<T> _objectSet;

        /// <summary>
        /// Initializes a new instance of the DataRepository class
        /// </summary>
        //public DataRepository()
        //    : this(new PRM_MfsIwmEntities())
        //{
        //}

        //private DbContext _dbContext;

        //private IDbSet<T> _dbSet;

        public DataEF6Repository(DbContext context)
        {


             _context = ((IObjectContextAdapter)context).ObjectContext; 
            try
            {
                //_dbSet = context.Set<T>();
                 _objectSet = _context.CreateObjectSet<T>();
            }
            catch (Exception)
            {


            }

            //  _context.Refresh(System.Data.Objects.RefreshMode.ClientWins, _objectSet);
        }

        public DataEF6Repository()
        {
        }

    
        /// <summary>
        /// Gets all records as an IQueryable
        /// </summary>
        /// <returns>An IQueryable object containing the results of the query</returns>
        public IQueryable<T> Fetch()
        {
            return _objectSet;
        }

        /// <summary>
        /// Gets all records as an IEnumberable
        /// </summary>
        /// <returns>An IEnumberable object containing the results of the query</returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _objectSet.AsEnumerable();
        }

        /// <summary>
        /// Gets all records as an IEnumberable depening on filter, order and include property
        /// </summary>
        /// <returns>An IEnumberable object containing the results of the query</returns>
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
                                           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                           string includeProperties = "")
        {
            IQueryable<T> query = _objectSet;
            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        /// <summary>
        /// Finds a record with the property of Id from the table
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>Single object</returns>
        public virtual T GetByID(object id)
        {

            // Define the entity key values.
            IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                new KeyValuePair<string, object>[] { 
                new KeyValuePair<string, object>("Id", id) };

            string qualifiedEntitySetName = _context.DefaultContainerName + "." + typeof(T).Name;
            EntityKey key = new EntityKey(qualifiedEntitySetName, entityKeyValues);

            try
            {
                return (T)_context.GetObjectByKey(key);

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Finds a record with the property of Id from the table for 64 Bit operation
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>Single object</returns>
        public virtual T GetByID64Bit(object id)
        {

            // Define the entity key values.
            IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                new KeyValuePair<string, object>[] { 
                new KeyValuePair<string, object>("Id", id) };

            string qualifiedEntitySetName = _context.DefaultContainerName + "." + typeof(T).Name;
            EntityKey key = new EntityKey(qualifiedEntitySetName, entityKeyValues);

            try
            {
                return (T)_context.GetObjectByKey(key);

            }
            catch
            {
                return null;
            }
        }

        public virtual T GetByID(object id, string KeyName)
        {

            // Define the entity key values.
            IEnumerable<KeyValuePair<string, object>> entityKeyValues =
                new KeyValuePair<string, object>[] { 
                new KeyValuePair<string, object>(KeyName, id) };

            string qualifiedEntitySetName = _context.DefaultContainerName + "." + typeof(T).Name;
            EntityKey key = new EntityKey(qualifiedEntitySetName, entityKeyValues);
            try
            {
                return (T)_context.GetObjectByKey(key);

            }
            catch (ObjectNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds record Count with the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>Total co of Recors</returns>
        public int GetCount1(string filterExpression)
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                if (filterExpression.Contains("like"))
                {
                    string[] srcString = filterExpression.Split(' ');
                    IQueryable<T> list = null;
                    list = _objectSet.Contains(srcString[0], srcString[2]);

                    string whereExpression = GetWhereExpression(filterExpression);
                    if (!whereExpression.Contains(("like")))
                    {
                        return list.Where(whereExpression).Count();
                    }
                    else
                    {
                        return list.Count();
                    }
                }
                else
                {
                    return _objectSet.Where(filterExpression).Count();
                }
            }
            else
                return _objectSet.Count();
        }
        public int GetCount(string filterExpression)
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                IQueryable<T> list = _objectSet;
                string[] strQueries = filterExpression.Replace(" AND ", "|").Split('|');

                foreach (string strQry in strQueries)
                {

                    if (strQry.ToLower().Contains("like"))
                    {
                        string[] srcString = strQry.Split(' ');

                        list = list.Contains(srcString[0], srcString[2]);
                    }
                    else
                    {
                        list = list.Where(strQry);
                    }
                }
                return list.Count();
            }
            else
                return _objectSet.Count();
        }

        /// <summary>
        /// Gets all records as an IQueryable depening on filter
        /// </summary>
        /// <returns>An IQueryable object containing the results of the query</returns>
        public IQueryable<T> GetPaged1(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount)
        {
            IQueryable<T> list = null;

            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                if (filterExpression.Contains("like"))
                {
                    string[] srcString = filterExpression.Split(' ');

                    list = _objectSet.Contains(srcString[0], srcString[2]);

                    string whereExpression = GetWhereExpression(filterExpression);
                    if (!whereExpression.Contains(("like")))
                    {
                        return list.Where(whereExpression).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
                    }
                    else
                    {
                        return list.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
                    }
                }
                else
                {
                    return _objectSet.Where(filterExpression).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
                }
            }
            else
            {
                list = _objectSet.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize);
            }

            return list;
        }
        public IQueryable<T> GetPaged(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount)
        {
            IQueryable<T> list = _objectSet;

            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                string[] strQueries = filterExpression.Replace(" AND ", "|").Split('|');

                foreach (string strQry in strQueries)
                {
                    if (strQry.ToLower().Contains("like"))
                    {
                        string[] srcString = strQry.Split(' ');

                        list = list.Contains(srcString[0], srcString[2]);

                    }
                    else
                    {
                        list = list.Where(strQry);
                    }
                }
            }

            // var aa = list.Where(x => ((string)x.GetType().GetProperty(sortExpression).GetValue(0, null)) == "a");

            try
            {
                return list.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize);
            }
            catch
            {
                return list;
            }

        }

        /// <summary>
        /// Gets all records as an IQueryable depening on filter
        /// </summary>
        /// <returns>An IQueryable object containing the results of the query</returns>
        public IQueryable<T> GetPaged1(string filterExpression, string sortExpression, string sortDirection)
        {
            IQueryable<T> list = null;

            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                if (filterExpression.Contains("like"))
                {
                    string[] srcString = filterExpression.Split(' ');

                    list = _objectSet.Contains(srcString[0], srcString[2]);

                    string whereExpression = GetWhereExpression(filterExpression);
                    if (!whereExpression.Contains(("like")))
                    {
                        return list.Where(whereExpression).OrderBy(sortExpression + " " + sortDirection);
                    }
                    else
                    {
                        return list.OrderBy(sortExpression + " " + sortDirection);
                    }
                }
                else
                {
                    return _objectSet.Where(filterExpression).OrderBy(sortExpression + " " + sortDirection);
                }
            }
            else
            {
                list = _objectSet.OrderBy(sortExpression + " " + sortDirection);
            }

            return list;
        }
        public IQueryable<T> GetPaged(string filterExpression, string sortExpression, string sortDirection)
        {
            IQueryable<T> list = _objectSet;

            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                string[] strQueries = filterExpression.Replace(" AND ", "|").Split('|');

                foreach (string strQry in strQueries)
                {
                    if (strQry.ToLower().Contains("like"))
                    {
                        string[] srcString = strQry.Split(' ');

                        list = list.Contains(srcString[0], srcString[2]);
                    }
                    else
                    {
                        list = list.Where(strQry);
                    }
                }
            }
            try
            {
                return list.OrderBy(sortExpression + " " + sortDirection);
            }
            catch
            {
                return list;
            }
        }

        #region DynamicSort
        private static MethodInfo GetCompareToMethod<T>(T genericInstance, string sortExpression)
        {
            Type genericType = genericInstance.GetType();
            object sortExpressionValue = genericType.GetProperty(sortExpression).GetValue(genericInstance, null);
            Type sortExpressionType = sortExpressionValue.GetType();
            MethodInfo compareToMethodOfSortExpressionType = sortExpressionType.GetMethod("CompareTo", new Type[] { sortExpressionType });

            return compareToMethodOfSortExpressionType;
        }


        public List<T> DynamicSort<T>(List<T> genericList, string sortExpression, string sortDirection)
        {
            int sortReverser = sortDirection.ToLower().StartsWith("asc") ? 1 : -1;

            Comparison<T> comparisonDelegate =
            new Comparison<T>(delegate(T x, T y)
            {
                //Just to get the compare method info to compare the values.
                MethodInfo compareToMethod = GetCompareToMethod<T>(x, sortExpression);

                //Getting current object value.
                object xSortExpressionValue = x.GetType().GetProperty(sortExpression).GetValue(x, null);

                //Getting the previous value.
                object ySortExpressionValue = y.GetType().GetProperty(sortExpression).GetValue(y, null);

                //Comparing the current and next object value of collection.
                object result = compareToMethod.Invoke(xSortExpressionValue, new object[] { ySortExpressionValue });

                // result tells whether the compared object is equal,greater,lesser.
                return sortReverser * Convert.ToInt16(result);
            });

            //here we using the comparison delegate to sort the object by its property
            genericList.Sort(comparisonDelegate);

            return genericList;
        }

        #endregion

        /// <summary>
        /// Finds a record with the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A collection containing the results of the query</returns>
        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _objectSet.Where<T>(predicate);
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T Single(Func<T, bool> predicate)
        {
            return _objectSet.Single<T>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T First(Func<T, bool> predicate)
        {
            return _objectSet.First<T>(predicate);
        }

        /// <summary>
        /// Adds the specified entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is null</exception>
        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _objectSet.AddObject(entity);
        }

        /// <summary>
        /// Attaches the specified entity
        /// </summary>
        /// <param name="entity">Entity to attach</param>
        public void Attach(T entity)
        {
            _objectSet.Attach(entity);
        }
        public void Detach(T entity)
        {
            _objectSet.Detach(entity);
        }

        public virtual IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        {
            //to be implemented
            return null;
        }

        public virtual IEnumerable<T> GetWithRawSql(string query)
        {
            //IQueryable<T> list = null;

            var list = _context.ExecuteStoreQuery<T>(query);

            return list.ToList();
        }

        /// <summary>
        /// Deletes the specified entitiy
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is null</exception>
        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _objectSet.DeleteObject(entity);
        }

        /// <summary>
        ///  Delete entity  for 32 Bit ID
        /// </summary>
        /// <param name="Id"></param>
        public void Delete(int Id)
        {
            T entity = this.GetByID(Id);
            if (entity != null)
            {
                try
                {

                    _context.DeleteObject(entity);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Delete entity  for 64 Bit ID
        /// </summary>
        /// <param name="Id"></param>
        public void Delete_64Bit(Int64 Id)
        {
            T entity = this.GetByID(Id);
            if (entity != null)
            {
                try
                {

                    _context.DeleteObject(entity);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Deletes records matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        public void Delete(Func<T, bool> predicate)
        {
            IEnumerable<T> records = from x in _objectSet.Where<T>(predicate) select x;

            foreach (T record in records)
            {
                _objectSet.DeleteObject(record);
            }
        }
        public void Delete(int Id, List<Type> allTypes)
        {

            T entity = this.GetByID(Id);
            if (entity != null)
            {
                try
                {
                    if (allTypes != null)
                    {
                        foreach (Type Class in allTypes)
                        {
                            /*Get All Children from db */

                            System.Reflection.PropertyInfo orginalChilds = typeof(T).GetProperty(Class.Name);
                            Type listType = typeof(EntityCollection<>).MakeGenericType(new Type[] { Class });
                            object oldDbChildList = Activator.CreateInstance(listType);
                            oldDbChildList = orginalChilds.GetValue(entity, null);
                            /* *** */
                            List<object> oldChildList = new List<object>();

                            foreach (var obj in (IEnumerable)oldDbChildList)
                            {
                                System.Reflection.PropertyInfo pId = Class.GetProperty("Id");
                                //int value = Convert.ToInt32(pId.GetValue(obj, null));
                                oldChildList.Add(obj);
                            }

                            foreach (var old in oldChildList)
                            {
                                _context.DeleteObject(old);
                            }
                        }
                    }

                    _context.DeleteObject(entity);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                throw new ArgumentNullException("entity");
            }
        }


        public void Delete_64Bit(Int64 Id, List<Type> allTypes)
        {

            T entity = this.GetByID(Id);
            if (entity != null)
            {
                try
                {
                    if (allTypes != null)
                    {
                        foreach (Type Class in allTypes)
                        {
                            /*Get All Children from db */

                            System.Reflection.PropertyInfo orginalChilds = typeof(T).GetProperty(Class.Name);
                            Type listType = typeof(EntityCollection<>).MakeGenericType(new Type[] { Class });
                            object oldDbChildList = Activator.CreateInstance(listType);
                            oldDbChildList = orginalChilds.GetValue(entity, null);
                            /* *** */
                            List<object> oldChildList = new List<object>();

                            foreach (var obj in (IEnumerable)oldDbChildList)
                            {
                                System.Reflection.PropertyInfo pId = Class.GetProperty("Id");
                                //int value = Convert.ToInt32(pId.GetValue(obj, null));
                                oldChildList.Add(obj);
                            }

                            foreach (var old in oldChildList)
                            {
                                _context.DeleteObject(old);
                            }
                        }
                    }

                    _context.DeleteObject(entity);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                throw new ArgumentNullException("entity");
            }
        }

        public void Delete(int Id, string KeyName, List<Type> allTypes)
        {

            T entity = this.GetByID(Id, KeyName);
            if (entity != null)
            {
                try
                {
                    if (allTypes != null)
                    {
                        foreach (Type Class in allTypes)
                        {
                            /*Get All Children fr7om db */

                            System.Reflection.PropertyInfo orginalChilds = typeof(T).GetProperty(Class.Name);
                            Type listType = typeof(EntityCollection<>).MakeGenericType(new Type[] { Class });
                            object oldDbChildList = Activator.CreateInstance(listType);
                            oldDbChildList = orginalChilds.GetValue(entity, null);
                            /* *** */
                            List<object> oldChildList = new List<object>();

                            foreach (var obj in (IEnumerable)oldDbChildList)
                            {
                                System.Reflection.PropertyInfo pId = Class.GetProperty("Id");
                                //int value = Convert.ToInt32(pId.GetValue(obj, null));
                                oldChildList.Add(obj);
                            }

                            foreach (var old in oldChildList)
                            {
                                _context.DeleteObject(old);
                            }
                        }
                    }

                    _context.DeleteObject(entity);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                throw new ArgumentNullException("entity");
            }
        }

        public void Delete(int Id, string KeyName, List<Type> allTypes, string childKey)
        {

            T entity = this.GetByID(Id, KeyName);
            if (entity != null)
            {
                try
                {
                    if (allTypes != null)
                    {
                        foreach (Type Class in allTypes)
                        {
                            /*Get All Children fr7om db */

                            System.Reflection.PropertyInfo orginalChilds = typeof(T).GetProperty(Class.Name);
                            Type listType = typeof(EntityCollection<>).MakeGenericType(new Type[] { Class });
                            object oldDbChildList = Activator.CreateInstance(listType);
                            oldDbChildList = orginalChilds.GetValue(entity, null);
                            /* *** */
                            List<object> oldChildList = new List<object>();

                            foreach (var obj in (IEnumerable)oldDbChildList)
                            {
                                System.Reflection.PropertyInfo pId = Class.GetProperty(childKey);
                                //int value = Convert.ToInt32(pId.GetValue(obj, null));
                                oldChildList.Add(obj);
                            }

                            foreach (var old in oldChildList)
                            {
                                _context.DeleteObject(old);
                            }
                        }
                    }

                    _context.DeleteObject(entity);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                throw new ArgumentNullException("entity");
            }
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        public virtual void Update(T entityToUpdate)
        {
            EntityKey key = GenerateKey(entityToUpdate);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            Type parentType = entityToUpdate.GetType();
            this.SetInsertAuditInfo(entityToUpdate, originalEntity, parentType);
            _context.ApplyCurrentValues(qualifiedEntitySetName, entityToUpdate);


            //Retrive Audit info by reflection IUser/IDate
            //EntityKey key = GenerateKey(entityToUpdate);
            //var originalEntity = (T)_context.GetObjectByKey(key);
            //string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            //Type parentType = entityToUpdate.GetType();
            //this.SetInsertAuditInfo(entityToUpdate, originalEntity, parentType);
            ////////////////////////////////////////
            // _objectSet.Attach(entityToUpdate);
            //_context.ObjectStateManager.ChangeObjectState(entityToUpdate, EntityState.Modified);

        }

        /// <summary>
        /// Update the specified entity for 64 Bit
        /// </summary>
        /// <param name="entity">Entity to update</param>
        public virtual void Update_64Bit(T entityToUpdate)
        {
            EntityKey key = GenerateKey_64Bit(entityToUpdate);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            Type parentType = entityToUpdate.GetType();
            this.SetInsertAuditInfo(entityToUpdate, originalEntity, parentType);
            _context.ApplyCurrentValues(qualifiedEntitySetName, entityToUpdate);


            //Retrive Audit info by reflection IUser/IDate
            //EntityKey key = GenerateKey(entityToUpdate);
            //var originalEntity = (T)_context.GetObjectByKey(key);
            //string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            //Type parentType = entityToUpdate.GetType();
            //this.SetInsertAuditInfo(entityToUpdate, originalEntity, parentType);
            ////////////////////////////////////////
            // _objectSet.Attach(entityToUpdate);
            //_context.ObjectStateManager.ChangeObjectState(entityToUpdate, EntityState.Modified);

        }

        public virtual void Update(T entityToUpdate, string KeyName)
        {
            EntityKey key = GenerateKey(entityToUpdate, KeyName);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            Type parentType = entityToUpdate.GetType();
            this.SetInsertAuditInfo(entityToUpdate, originalEntity, parentType);
            _context.ApplyCurrentValues(qualifiedEntitySetName, entityToUpdate);
        }

        public virtual void Update(T updatedEntity, Dictionary<Type, ArrayList> NavigationList)
        {
            EntityKey key = GenerateKey(updatedEntity);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));

            Type parentType = updatedEntity.GetType();

            this.SetInsertAuditInfo(updatedEntity, originalEntity, parentType);

            _context.ApplyCurrentValues(qualifiedEntitySetName, updatedEntity);

            if (NavigationList != null)
            {
                foreach (KeyValuePair<Type, ArrayList> navigator in NavigationList)
                {
                    Type Class = navigator.Key;
                    ArrayList newChilds = navigator.Value;
                    UpdateChild(originalEntity, parentType, newChilds, Class);
                }
            }
        }

        public virtual void Update_64Bit(T updatedEntity, Dictionary<Type, ArrayList> NavigationList)
        {
            EntityKey key = GenerateKey_64Bit(updatedEntity);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));

            Type parentType = updatedEntity.GetType();

            this.SetInsertAuditInfo(updatedEntity, originalEntity, parentType);

            _context.ApplyCurrentValues(qualifiedEntitySetName, updatedEntity);

            if (NavigationList != null)
            {
                foreach (KeyValuePair<Type, ArrayList> navigator in NavigationList)
                {
                    Type Class = navigator.Key;
                    ArrayList newChilds = navigator.Value;
                    UpdateChild(originalEntity, parentType, newChilds, Class);
                }
            }
        }

        public virtual void Update(T updatedEntity, string KeyName, Dictionary<Type, ArrayList> NavigationList)
        {
            EntityKey key = GenerateKey(updatedEntity, KeyName);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));

            Type parentType = updatedEntity.GetType();

            this.SetInsertAuditInfo(updatedEntity, originalEntity, parentType);

            _context.ApplyCurrentValues(qualifiedEntitySetName, updatedEntity);

            if (NavigationList != null)
            {
                foreach (KeyValuePair<Type, ArrayList> navigator in NavigationList)
                {
                    Type Class = navigator.Key;
                    ArrayList newChilds = navigator.Value;
                    UpdateChild(originalEntity, parentType, newChilds, Class);
                }
            }
        }

        public virtual void Update(T updatedEntity, string KeyName, Dictionary<Type, ArrayList> NavigationList, string childKey)
        {
            EntityKey key = GenerateKey(updatedEntity, KeyName);
            var originalEntity = (T)_context.GetObjectByKey(key);
            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));

            Type parentType = updatedEntity.GetType();

            this.SetInsertAuditInfo(updatedEntity, originalEntity, parentType);

            _context.ApplyCurrentValues(qualifiedEntitySetName, updatedEntity);

            if (NavigationList != null)
            {
                foreach (KeyValuePair<Type, ArrayList> navigator in NavigationList)
                {
                    Type Class = navigator.Key;
                    ArrayList newChilds = navigator.Value;
                    UpdateChild(originalEntity, parentType, newChilds, Class, childKey);
                }
            }
        }

        private void UpdateChild(T originalEntity, Type parentType, ArrayList newChilds, Type Class, string childKey)
        {
            /*Get All Children from db */

            System.Reflection.PropertyInfo orginalChilds = parentType.GetProperty(Class.Name);
            Type listType = typeof(EntityCollection<>).MakeGenericType(new Type[] { Class });
            object oldDbChildList = Activator.CreateInstance(listType);
            oldDbChildList = orginalChilds.GetValue(originalEntity, null);
            /* *** */

            List<Int32> oldIds = new List<int>();
            List<Int32> newIds = new List<int>();
            List<object> oldChildList = new List<object>();

            foreach (var obj in (IEnumerable)oldDbChildList)
            {
                System.Reflection.PropertyInfo Id = Class.GetProperty(childKey);
                int value = Convert.ToInt32(Id.GetValue(obj, null));
                oldIds.Add(value);
                oldChildList.Add(obj);
            }

            foreach (var obj in newChilds.ToArray(Class))
            {
                System.Reflection.PropertyInfo Id = Class.GetProperty(childKey);
                int value = Convert.ToInt32(Id.GetValue(obj, null));
                newIds.Add(value);
                string qualifiedEntitySetName = this.GetEntitySetName(Class);
                if (oldIds.Contains(value))
                {
                    object childToUpdate = GetUpdatedChildWithAuditInfo(oldChildList, value, obj, childKey);
                    _context.ApplyCurrentValues(qualifiedEntitySetName, childToUpdate);
                }
                else
                {
                    _context.AddObject(qualifiedEntitySetName, obj);
                }
            }

            foreach (var old in oldChildList)
            {
                // Are there child items in the DB which are NOT in the
                // new child item collection anymore?
                System.Reflection.PropertyInfo Id = Class.GetProperty(childKey);
                int value = Convert.ToInt32(Id.GetValue(old, null));
                if (!newIds.Contains(value))
                {
                    // Yes -> It's a deleted child item -> Delete
                    _context.DeleteObject(old);
                }
            }
        }

        private void UpdateChild(T originalEntity, Type parentType, ArrayList newChilds, Type Class)
        {
            /*Get All Children from db */

            System.Reflection.PropertyInfo orginalChilds = parentType.GetProperty(Class.Name);
            Type listType = typeof(EntityCollection<>).MakeGenericType(new Type[] { Class });
            object oldDbChildList = Activator.CreateInstance(listType);
            oldDbChildList = orginalChilds.GetValue(originalEntity, null);
            /* *** */

            List<Int64> oldIds = new List<Int64>();
            List<Int64> newIds = new List<Int64>();
            List<object> oldChildList = new List<object>();

            foreach (var obj in (IEnumerable)oldDbChildList)
            {
                System.Reflection.PropertyInfo Id = Class.GetProperty("Id");
                int value = Convert.ToInt32(Id.GetValue(obj, null));
                oldIds.Add(value);
                oldChildList.Add(obj);
            }

            foreach (var obj in newChilds.ToArray(Class))
            {
                System.Reflection.PropertyInfo Id = Class.GetProperty("Id");
                Int64 value = Convert.ToInt64(Id.GetValue(obj, null));
                newIds.Add(value);
                string qualifiedEntitySetName = this.GetEntitySetName(Class);
                if (oldIds.Contains(value))
                {
                    object childToUpdate = GetUpdatedChildWithAuditInfo(oldChildList, value, obj);
                    _context.ApplyCurrentValues(qualifiedEntitySetName, childToUpdate);
                }
                else
                {
                    _context.AddObject(qualifiedEntitySetName, obj);
                }
            }

            foreach (var old in oldChildList)
            {
                // Are there child items in the DB which are NOT in the
                // new child item collection anymore?
                System.Reflection.PropertyInfo Id = Class.GetProperty("Id");
                int value = Convert.ToInt32(Id.GetValue(old, null));
                if (!newIds.Contains(value))
                {
                    // Yes -> It's a deleted child item -> Delete
                    _context.DeleteObject(old);
                }
            }
        }

        private object GetUpdatedChildWithAuditInfo(List<object> oldChildList, Int64 newid, object updatedChild)
        {
            try
            {
                object item = null;

                foreach (var c in oldChildList)
                {
                    System.Reflection.PropertyInfo[] oldItemProperties = c.GetType().GetProperties();
                    System.Reflection.PropertyInfo propertyID = oldItemProperties.Where(d => d.Name == "Id").FirstOrDefault();

                    int oldID = Convert.ToInt32(propertyID.GetValue(c, null));

                    if (oldID == newid)
                    {
                        System.Reflection.PropertyInfo IUserProperty = oldItemProperties.Where(d => d.Name == "IUser").FirstOrDefault();
                        string oldIUser = Convert.ToString(IUserProperty.GetValue(c, null));

                        System.Reflection.PropertyInfo IDateProperty = oldItemProperties.Where(d => d.Name == "IDate").FirstOrDefault();
                        DateTime oldIDate = Convert.ToDateTime(IDateProperty.GetValue(c, null));

                        IUserProperty.SetValue(updatedChild, oldIUser, null);
                        IDateProperty.SetValue(updatedChild, oldIDate, null);

                        return updatedChild;
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                return updatedChild;
            }

        }

        private object GetUpdatedChildWithAuditInfo(List<object> oldChildList, int newid, object updatedChild, string childKey)
        {
            try
            {
                object item = null;

                foreach (var c in oldChildList)
                {
                    System.Reflection.PropertyInfo[] oldItemProperties = c.GetType().GetProperties();
                    System.Reflection.PropertyInfo propertyID = oldItemProperties.Where(d => d.Name == childKey).FirstOrDefault();

                    int oldID = Convert.ToInt32(propertyID.GetValue(c, null));

                    if (oldID == newid)
                    {
                        System.Reflection.PropertyInfo IUserProperty = oldItemProperties.Where(d => d.Name == "IUser").FirstOrDefault();
                        string oldIUser = Convert.ToString(IUserProperty.GetValue(c, null));

                        System.Reflection.PropertyInfo IDateProperty = oldItemProperties.Where(d => d.Name == "IDate").FirstOrDefault();
                        DateTime oldIDate = Convert.ToDateTime(IDateProperty.GetValue(c, null));

                        IUserProperty.SetValue(updatedChild, oldIUser, null);
                        IDateProperty.SetValue(updatedChild, oldIDate, null);

                        return updatedChild;
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void SetInsertAuditInfo(T updatedEntity, T originalEntity, Type type)
        {
            try
            {
                System.Reflection.PropertyInfo pIUser = type.GetProperty("IUser");
                System.Reflection.PropertyInfo pIDate = type.GetProperty("IDate");

                string IUser = string.Empty;
                DateTime IDate = DateTime.Now;

                if (pIUser != null)
                {
                    string user = Convert.ToString(pIUser.GetValue(originalEntity, null));
                    pIUser.SetValue(updatedEntity, user, null);
                }

                if (pIDate != null)
                {
                    IDate = Convert.ToDateTime(pIDate.GetValue(originalEntity, null));
                    pIDate.SetValue(updatedEntity, IDate, null);
                }

            }
            catch (Exception ex)
            {
                //if property does not exist, just suppress it
            }
        }

        public EntityKey GenerateKey(T t)
        {
            //Int32 value = 0;
            Type type = t.GetType();
            System.Reflection.PropertyInfo ID = type.GetProperty("Id");
            var value = Convert.ToInt32(ID.GetValue(t, null));


            IEnumerable<KeyValuePair<string, object>> entityKeyValues = new KeyValuePair<string, object>[] { 
                new KeyValuePair<string, object>("Id", value) };

            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            EntityKey key = new EntityKey(qualifiedEntitySetName, entityKeyValues);

            return key;
        }

        public EntityKey GenerateKey_64Bit(T t)
        {
            //Int32 value = 0;
            Type type = t.GetType();
            System.Reflection.PropertyInfo ID = type.GetProperty("Id");
            var value = Convert.ToInt64(ID.GetValue(t, null));


            IEnumerable<KeyValuePair<string, object>> entityKeyValues = new KeyValuePair<string, object>[] { 
                new KeyValuePair<string, object>("Id", value) };

            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            EntityKey key = new EntityKey(qualifiedEntitySetName, entityKeyValues);

            return key;
        }

        public EntityKey GenerateKey(T t, string KeyName)
        {
            //Int32 value = 0;
            object value = 0;
            Type type = t.GetType();
            System.Reflection.PropertyInfo ID = type.GetProperty(KeyName);
            //value = Convert.ToInt32(ID.GetValue(t, null));
            value = ID.GetValue(t, null);

            IEnumerable<KeyValuePair<string, object>> entityKeyValues = new KeyValuePair<string, object>[] { 
                new KeyValuePair<string, object>(KeyName, value) };

            string qualifiedEntitySetName = this.GetEntitySetName(typeof(T));
            EntityKey key = new EntityKey(qualifiedEntitySetName, entityKeyValues);

            return key;
        }

        private string GetEntitySetName(Type t)
        {
            string qualifiedEntitySetName = _context.DefaultContainerName + "." + t.Name;
            return qualifiedEntitySetName;
        }

        /// <summary>
        /// Saves all context changes
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Saves all context changes with the specified SaveOptions
        /// </summary>
        /// <param name="options">Options for saving the context</param>
        public void SaveChanges(SaveOptions options)
        {
            _context.SaveChanges(options);
        }

        /// <summary>
        /// Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing">A boolean value indicating whether or not to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

        private string GetWhereExpression(string filterExpression)
        {
            string[] srcString = filterExpression.Split(' ');
            List<string> al = srcString.ToList<string>();
            if (al.Count > 3)
                al.RemoveRange(0, 4);
            string s = string.Join(" ", al.ToArray()); ;
            return s;
        }
    }
}
