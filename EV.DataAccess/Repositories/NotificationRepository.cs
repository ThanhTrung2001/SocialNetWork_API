using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class NotificationRepository
    {
        private readonly DatabaseContext _context;

        public NotificationRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
