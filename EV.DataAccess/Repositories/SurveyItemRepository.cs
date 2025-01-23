using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class SurveyItemRepository
    {
        private readonly DatabaseContext _context;

        public SurveyItemRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
