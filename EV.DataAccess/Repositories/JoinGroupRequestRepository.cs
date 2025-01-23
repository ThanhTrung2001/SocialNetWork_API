using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class JoinGroupRequestRepository
    {
        private readonly DatabaseContext _context;

        public JoinGroupRequestRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
