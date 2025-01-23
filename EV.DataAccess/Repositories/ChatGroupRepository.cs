using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class ChatGroupRepository
    {
        private readonly DatabaseContext _context;

        public ChatGroupRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
