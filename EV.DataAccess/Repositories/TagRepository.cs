using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class TagRepository
    {
        private readonly DatabaseContext _context;

        public TagRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
