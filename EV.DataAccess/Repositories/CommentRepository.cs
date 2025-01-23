using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class CommentRepository
    {
        private readonly DatabaseContext _context;

        public CommentRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
