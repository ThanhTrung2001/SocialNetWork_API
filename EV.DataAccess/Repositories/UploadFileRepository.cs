using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class UploadFileRepository
    {
        private readonly DatabaseContext _context;

        public UploadFileRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
