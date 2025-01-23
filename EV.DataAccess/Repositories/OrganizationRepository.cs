using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class OrganizationRepository
    {
        private readonly DatabaseContext _context;

        public OrganizationRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
