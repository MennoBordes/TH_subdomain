namespace TH2.Shared.Modules.People
{
    using Base.Database;
    using Entities;
    using System.Collections.Generic;
    using System.Linq;
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
    }
}
