using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class AttachmentRepository
    {
        private readonly DatabaseContext _context;

        public AttachmentRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
