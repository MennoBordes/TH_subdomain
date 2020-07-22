namespace TH.Shared.Modules.People
{
    using Base.Database;
    using Entities;
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
            if (id < 1) return null;

            // Get
            People people = (People)_context.People.Select(x => x.Id == id);

            if (people == null) return null;

            return people;
        }
    }
}
