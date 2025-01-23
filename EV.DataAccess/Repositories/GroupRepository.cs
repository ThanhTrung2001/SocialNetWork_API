using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class GroupRepository
    {
        private readonly DatabaseContext _context;

        public GroupRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
