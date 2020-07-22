namespace TH2.Shared.Modules.People
{
    using Base.Database;
    using Entities;
    using System.Collections.Generic;
    using System.Linq;

    public class PeopleManager
    {
        private readonly ThDbEntities _context;

        public PeopleManager(ThDbEntities context)
        {
            _context = context;
        }

        public People GetPerson(int id)
        {
            // Check
            if (id < 1)
                return null;

            // Get
            People people = _context.People.Where(x => x.Id == id).FirstOrDefault();

            if (people == null)
                return null;

            return people;
        }

        public List<People> GetPeoples()
        {
            List<People> peoples = _context.People.ToList();

            return peoples;
        }
    }
}
