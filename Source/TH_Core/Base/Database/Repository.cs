namespace TH.Core.Base.Database
{
    using THTools.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using THTools.ORM.Conversion;
    using TH.Core.Base.Exceptions;

    public class Repository
    {
        private Provider provider;

        /// <summary> The underlying DB provider. </summary>
        public Provider Provider { get { return this.provider; } }

        /// <summary> Constructs a new (generic) repository. </summary>
        public Repository()
        {
            provider = new Provider(new ConnectionProvider());
        }

        /// <summary> Constructs a new (generic) repository. </summary>
        public Repository(Provider provider)
        {
            this.provider = provider;
        }

        /// <summary> Get raw data. </summary>
        public QueryResult GetData(string query)
        {
            return provider.ExecuteQuery(query, data: true);
        }

        /// <summary> Get raw data. </summary>
        public QueryResult GetData(XQuery query)
        {
            return provider.ExecuteQuery(query.ToString(), data: true);
        }

        /// <summary> Init entity with default values. </summary>
        public T Init<T>() where T : Entity
        {
            return EntityFactory.Create<T>();
        }

        /// <summary> Get entity. </summary>
        public T GetEntity<T>(int id) where T : class
        {
            return provider.GetEntity<T>(id);
        }

        /// <summary> Get entity by XQuery. </summary>
        public T GetEntity<T>(XQuery query) where T : class
        {
            return GetEntities<T>(query).FirstOrDefault();
        }

        /// <summary> Get entities by XQuery. </summary>
        public IEnumerable<T> GetEntities<T>(XQuery query) where T : class
        {
            //QueryResult result = provider.ExecuteQuery(query.ToString(), data: true);
            //return result.Data.Tables[0].ConvertToEnumerable<T>();
            return GetEntities<T>(query.ToString());
        }

        /// <summary> Get entities by SQL query. </summary>
        /// <remarks> Use only if XQuery is not possible. </remarks>
        public IEnumerable<T> GetEntities<T>(string query) where T : class
        {
            QueryResult result = provider.ExecuteQuery(query, data: true);
            return result.Data.Tables[0].ConvertToEnumerable<T>();
        }

        /// <summary> Update entity. </summary>
        public QueryResult Update(Entity entity)
        {
            return provider.Update(entity);
        }

        /// <summary> Update (partial) entity. </summary>
        public QueryResult UpdatePartial(Entity entity, Expression<Func<object>> partial)
        {
            return provider.UpdatePartial(entity, partial);
        }

        /// <summary> Update entities. </summary>
        public QueryResult Update(IEnumerable<Entity> entities)
        {
            return provider.Update(entities);
        }

        /// <summary> Insert entity. </summary>
        public QueryResult Insert(Entity entity, bool primaryKey = false)
        {
            return provider.Insert(entity, primaryKey: primaryKey);
        }

        /// <summary> Insert entities. </summary>
        public QueryResult Insert(IEnumerable<Entity> entities, bool primaryKey = false)
        {
            return provider.Insert(entities, primaryKey: primaryKey);
        }

        /// <summary> Upsert entity. </summary>
        public QueryResult Upsert(Entity entity)
        {
            return provider.Upsert(entity);
        }

        /// <summary> Upsert entities. </summary>
        public QueryResult Upsert(IEnumerable<Entity> entities)
        {
            return provider.Upsert(entities);
        }

        /// <summary> Delete entity. </summary>
        public QueryResult Delete(Entity entity)
        {
            return provider.Delete(entity);
        }

        /// <summary> Delete entities. </summary>
        public QueryResult Delete(IEnumerable<Entity> entities)
        {
            return provider.Delete(entities);
        }

        /// <summary> Manual delete. </summary>
        public QueryResult Delete(XQuery query)
        {
            return this.Delete(query.ToString());
        }

        /// <summary> Manual delete. </summary>
        public QueryResult Delete(string query)
        {
            return provider.ExecuteQuery(query, transaction: true);
        }

        #region Default Exceptions

        /// <summary> Returns a Core Exception with an 'Invalid Reference' message. 
        /// <para> Message: Invalid '{0}' reference specified.</para>
        /// </summary>
        public static CoreException InvalidReference(string name)
        {
            return new CoreException(string.Format("Invalid '{0}' reference specified.", name));
        }

        /// <summary> Returns a Core Exception with an 'Invalid Reference' message. 
        /// <para> Message: Invalid '{0}' reference specified.</para>
        /// </summary>
        public static CoreException InvalidReference(string name, object value)
        {
            return new CoreException(string.Format("Invalid '{0}' reference specified [{1}].", name, value));
        }

        /// <summary> Returns a Core Exception with an 'Invalid Reference' message. 
        /// <para> Message: Invalid '{0}' reference specified.</para>
        /// </summary>
        public static CoreException InvalidReference<T>() where T : class
        {
            return InvalidReference(typeof(T).Name);
        }

        /// <summary> Returns a Core Exception with an 'Invalid Reference' message. 
        /// <para> Message: Invalid '{0}' reference specified.</para>
        /// </summary>
        public static CoreException InvalidReference<T>(object value) where T : class
        {
            return InvalidReference(typeof(T).Name, value);
        }

        #endregion
    }
}
