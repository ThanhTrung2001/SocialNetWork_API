using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class ReactRepository
    {
        private readonly DatabaseContext _context;

        public ReactRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
