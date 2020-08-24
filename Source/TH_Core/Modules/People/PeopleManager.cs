namespace TH.Core.Modules.People
{
    using Base.Database;
    using Entities;
    using System.Collections.Generic;
    using System.Linq;
    using TH.Core.Base.Exceptions;
    using THTools.ORM;

    public class PeopleManager
    {
        private Repository repository;
        public Repository Repository { get { return this.repository; } }
        public PeopleManager()
        {
            this.repository = new Repository();
        }

        public People GetPerson(int id)
        {
            // Check
            if (id < 1)
                return null;

            // Get
            People people = repository.GetEntity<People>(id);

            return people;
        }

        public List<People> GetPeoples()
        {
            XQuery q = new XQuery()
                .From<People>()
                .Where()
                    .Column<People>(x => x.Id).GreaterThan().Value(0);

            List<People> peoples = repository.GetEntities<People>(q).ToList();

            return peoples;
        }

        public int SavePerson(People person)
        {
            if (person == null)
                throw new CoreException("No person specified!");

            // Insert or update
            if (person.Id == 0)
            {
                person.Id = repository.Insert(person).InsertId.Value;
            }
            else
            {
                repository.Update(person);
            }

            return person.Id;
        }

        public void DeletePerson(int id)
        {
            if (id <= 0)
                throw new CoreException("Invalid person specified!");

            XQuery q = new XQuery()
                .Delete()
                .From<People>()
                .Where()
                    .Column<People>(x => x.Id).Equals().Value(id);

            repository.Delete(q);
        }
    }
}
