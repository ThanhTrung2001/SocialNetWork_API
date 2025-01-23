using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class PageRepository
    {
        private readonly DatabaseContext _context;

        public PageRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
