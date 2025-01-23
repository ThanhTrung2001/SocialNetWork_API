using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class MessageRepository
    {
        private readonly DatabaseContext _context;

        public MessageRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
