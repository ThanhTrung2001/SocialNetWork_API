using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class SharePostRepository
    {
        private readonly DatabaseContext _context;

        public SharePostRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
