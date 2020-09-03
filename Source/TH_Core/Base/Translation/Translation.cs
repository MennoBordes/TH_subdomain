using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Base.Database;
using TH.Core.Base.Database.Entities.Localization;
using TH.Core.Base.Exceptions;
using TH.Core.Base.Translation.Attributes;
using THTools.ORM;
using THTools.ORM.Conversion;

namespace TH.Core.Base.Translation
{
    /// <summary> The Translation Module. </summary>
    public class Translation
    {
        /// <summary> The Default value for missing translations. </summary>
        public const string NOT_TRANSLATED = "[Not translated]";

        /// <summary> The Default Language (-id) 'Dutch'. </summary>
        public const int DEFAULT_LANGUAGE = 2;

        private readonly Repository repository = new Repository();

        public Repository Repository { get { return this.repository; } }

        #region Language

        /// <summary> Gets all translation variations of specified language. </summary>
        public IEnumerable<LanguageData> GetLanguageData(int languageId)
        {
            XQuery q = new XQuery()
                .From<LanguageData>()
                .Where().Column<LanguageData>(x => x.LanguageSrcId).Equals().Value(languageId)
                .OrderBy<LanguageData>(x => x.LanguageId)
                .End();

            return repository.GetEntities<LanguageData>(q);
        }

        /// <summary> Get the language by code. </summary>
        public Language GetLanguage(string code)
        {
            // Check
            if (string.IsNullOrWhiteSpace(code))
                return null;

            XQuery q = new XQuery()
                .From<Language>()
                .Where()
                    .Column<Language>(x => x.Code).Equals().Value(code)
                .Limit(1)
                .End();

            return repository.GetEntities<Language>(q).FirstOrDefault();
        }

        /// <summary> Gets all available languages (translated). </summary>
        /// <remarks> TODO: get all languages with the not translated. </remarks>
        public IEnumerable<LanguageData> GetTranslatedLanguages(int languageId)
        {
            XQuery q = new XQuery()
                .From<LanguageData>()
                .Where().Column<LanguageData>(x => x.LanguageId).Equals().Value(languageId)
                .OrderBy<LanguageData>(x => x.LanguageSrcId)
                .End();

            return repository.GetEntities<LanguageData>(q);
        }

        /// <summary> Get name of language by source language Id and current user language Id. </summary>
        public string GetLanguageNameById(int srcLangId, int userLangId)
        {
            // Check
            if (srcLangId < 1)
                throw new CoreException("Invalid source language id was specified.");
            if (userLangId < 1)
                throw new CoreException("Invalid user language id was specified.");

            // Create query
            XQuery q = new XQuery()
                .Select().Column<LanguageData>(ld => ld.Label)
                .From<LanguageData>()
                .Where().Column<LanguageData>(ld => ld.LanguageSrcId).Equals().Value(srcLangId)
                .And().Column<LanguageData>(ld => ld.LanguageId).Equals().Value(userLangId).End();

            // Fetch
            QueryResult qr = repository.GetData(q);
            return qr.ConvertToEnumerable<string>().FirstOrDefault();
        }

        /// <summary> Get Ids of all known languages. </summary>
        public IEnumerable<int> GetLanguageIds()
        {
            // Create query
            XQuery q = new XQuery()
                .Select().Column<Language>(ld => ld.Id)
                .From<Language>()
                .End();

            // Fetch
            QueryResult qr = repository.GetData(q);
            return qr.ConvertToEnumerable<int>();
        }

        #endregion

        #region CultureInfo

        /// <summary> Instantiates a culture info object from specified language id. </summary>
        public static CultureInfo GetCultureInfo(int languageId)
        {
            switch (languageId)
            {
                case (int)Language.Ref.English_UK:
                    return CultureInfo.CreateSpecificCulture("en-GB");
                case (int)Language.Ref.Dutch:
                    return CultureInfo.CreateSpecificCulture("nl-NL");
                case (int)Language.Ref.English_US:
                    return CultureInfo.CreateSpecificCulture("en-US");

                default:
                    return CultureInfo.InvariantCulture;
            }
        }

        #endregion

        #region Generic Translation Data Retrieval

        /// <summary> Get Translated Data. 
        /// <para> Use with an '...Data' entity. </para>
        /// </summary>
        public T GetTranslatedData<T>(int sourceId, int languageId)
        {
            // Check parameters
            if (sourceId < 1)
                throw new CoreException("No actual 'source' was specified.");
            if (languageId < 1)
                throw new CoreException("No actual 'language' was specified.");

            IEnumerable<T> list = this.GetTranslatedData<T>(new int[] { sourceId }, languageId);

            return list.FirstOrDefault();
        }

        /// <summary> Get Translated Data. 
        /// <para> Use with an '...Data' entity. </para>
        /// <param name="sourceIds"> Optional (set null or empty to get all sources). </param>
        /// </summary>
        public IEnumerable<T> GetTranslatedData<T>(int[] sourceIds, int? languageId = null)
        {
            // Check parameters
            if (languageId < 1)
                throw new CoreException("No actual 'language' was specified.");

            // Get properties source and language
            Type tClass = typeof(T);

            PropertyInfo piSource = tClass.GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(TranslationSourceAttribute))).FirstOrDefault();

            PropertyInfo piLanguage = tClass.GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(TranslationLanguageAttribute))).FirstOrDefault();

            // Check if attributes are set
            if (piSource == null)
                throw new CoreException("Entity '" + tClass.Name + "' does not have the '" + typeof(TranslationSourceAttribute).Name + "' attribute set on one of it's properties.");
            if (piLanguage == null)
                throw new CoreException("Entity '" + tClass.Name + "' does not have the '" + typeof(TranslationLanguageAttribute).Name + "' attribute set on one of it's properties.");

            string pSource = piSource.Name;
            string pLanguage = piLanguage.Name;

            bool whereSet = false;

            // Create XQuery
            XQuery q = new XQuery()
                .From<T>();

            if (languageId != null)
            {
                if (!whereSet)
                {
                    q.Where();
                    whereSet = true;
                }

                // Column<> Language
                this.InvokeXQueryColumnMethod(q, tClass, pLanguage);

                q.Equals().Value(languageId);
            }

            if (sourceIds != null && sourceIds.Length > 0)
            {
                if (!whereSet)
                {
                    q.Where();
                    whereSet = true;
                }
                else
                {
                    q.And();
                }

                // Column<> Source
                this.InvokeXQueryColumnMethod(q, tClass, pSource);

                q.In(sourceIds.Select(x => (object)x));

            }

            q.End();

            // Create Repository 'GetEntities' MethodInfo
            MethodInfo getEntitiesMethod = typeof(Repository).GetMethod("GetEntities", new Type[] { typeof(XQuery) }).MakeGenericMethod(tClass);

            // Invoke 'GetEntities'
            object repositoryResult = getEntitiesMethod.Invoke(repository, new object[] { q });

            IEnumerable<T> list = (IEnumerable<T>)repositoryResult;

            return list;
        }

        /// <summary> Get Translated Data. 
        /// <para> Use with an '...Data' entity. </para>
        /// <param name="sourceIds"> Optional (set null or empty to get all sources). </param>
        /// </summary>
        public IEnumerable<object> GetTranslatedData(Type type, int[] sourceIds, int languageId)
        {
            // Check parameters
            if (languageId < 1)
                throw new CoreException("No actual 'language' was specified.");

            // Get properties source and language
            Type tClass = type;

            PropertyInfo piSource = tClass.GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(TranslationSourceAttribute))).FirstOrDefault();

            PropertyInfo piLanguage = tClass.GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(TranslationLanguageAttribute))).FirstOrDefault();

            // Check if attributes are set
            if (piSource == null)
                throw new CoreException("Entity '" + tClass.Name + "' does not have the '" + typeof(TranslationSourceAttribute).Name + "' attribute set on one of it's properties.");
            if (piLanguage == null)
                throw new CoreException("Entity '" + tClass.Name + "' does not have the '" + typeof(TranslationLanguageAttribute).Name + "' attribute set on one of it's properties.");

            string pSource = piSource.Name;
            string pLanguage = piLanguage.Name;

            // Create XQuery
            XQuery q = new XQuery();

            // From<>
            this.InvokeXQueryFromMethod(q, tClass);

            q.Where();

            // Column<> Language
            this.InvokeXQueryColumnMethod(q, tClass, pLanguage);

            q.Equals().Value(languageId);

            if (sourceIds != null && sourceIds.Length > 0)
            {
                q.And();

                // Column<> Source
                this.InvokeXQueryColumnMethod(q, tClass, pSource);

                q.In(sourceIds.Select(x => (object)x));

            }

            q.End();

            // Create Repository 'GetEntities' MethodInfo
            MethodInfo getEntitiesMethod = typeof(Repository).GetMethod("GetEntities", new Type[] { typeof(XQuery) }).MakeGenericMethod(tClass);

            // Invoke 'GetEntities'
            object repositoryResult = getEntitiesMethod.Invoke(repository, new object[] { q });

            IEnumerable<object> list = (IEnumerable<object>)repositoryResult;

            return list;
        }

        /// <summary> Invokes the generic 'From' Method of the XQuery object. </summary>
        private void InvokeXQueryFromMethod(XQuery xQuery, Type targetEntity)
        {
            string fromMethod = "From";

            // Get method reference
            MethodInfo method = xQuery.GetType().GetMethod(fromMethod);

            // Add generic type constraint
            method = method.MakeGenericMethod(new Type[] { targetEntity });

            // Invoke
            method.Invoke(xQuery, null);
        }

        /// <summary> Invokes the generic 'Column' Method of the XQuery object. </summary>
        private void InvokeXQueryColumnMethod(XQuery xQuery, Type targetEntity, string targetProperty)
        {
            string columnMethod = "Column";

            // Create the lambda parameter 'x'
            ParameterExpression parameter = Expression.Parameter(targetEntity, "x");

            // Create a property accessor using the property name -> x.#propertyName#
            MemberExpression property = Expression.Property(parameter, targetEntity, targetProperty);

            // Method Parameter expects an expression with an object, so let's cast it
            UnaryExpression castExpression = Expression.TypeAs(property, typeof(object));

            // Create the lambda statement
            LambdaExpression lambda = Expression.Lambda(castExpression, parameter);

            // Create XQuery MethodInfo
            MethodInfo method = typeof(XQuery).GetMethods().FirstOrDefault(x => x.Name == columnMethod && x.IsGenericMethod);
            MethodInfo generic = method.MakeGenericMethod(targetEntity);

            // Invoke
            generic.Invoke(xQuery, new object[] { lambda, (string)null, null, null });
        }

        #endregion

        #region Save Translated Data.

        /// <summary> Save translation data. </summary>
        public void SaveTranslatedData(Type type, int sourceId, int languageId, Dictionary<string, string> data)
        {
            if (!data.Any())
                return;

            // Check if data exists
            IEnumerable<object> existingData = this.GetTranslatedData(type, new int[] { sourceId }, languageId);
            object dataObject = existingData.FirstOrDefault();
            bool exists = (dataObject != null);

            // Create dataobject, if its a new entry
            if (dataObject == null)
            {
                // First check, if data isn't all empty (there is no use for adding an empty record)
                if (data.Where(x => !String.IsNullOrEmpty(x.Value)).Count() == 0)
                    return;

                // Create object instance: EntityFactory.Init<T>     
                // Get method reference
                MethodInfo method = repository.GetType().GetMethod("Init");

                // Add generic type constraint
                method = method.MakeGenericMethod(new Type[] { type });

                // Invoke
                dataObject = method.Invoke(repository, null);
            }

            // Loop over property info's
            PropertyInfo[] piFields = dataObject.GetType().GetProperties();
            foreach (PropertyInfo piField in piFields)
            {
                // Check source
                if (Attribute.IsDefined(piField, typeof(TranslationSourceAttribute)) && !exists)
                {
                    piField.SetValue(dataObject, sourceId);
                }

                // Check language
                if (Attribute.IsDefined(piField, typeof(TranslationLanguageAttribute)) && !exists)
                {
                    piField.SetValue(dataObject, languageId);
                }

                // Check fields
                if (Attribute.IsDefined(piField, typeof(TranslationField)))
                {
                    string newValue = data.FirstOrDefault(x => x.Key == piField.Name).Value;
                    if (newValue != null)
                        piField.SetValue(dataObject, newValue);
                }
            }

            // Save data
            if (exists)
                repository.Update((Entity)dataObject);
            else
                repository.Insert((Entity)dataObject);

        }

        #endregion
    }
}
