using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class PostRepository
    {
        private readonly DatabaseContext _context;

        public PostRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
