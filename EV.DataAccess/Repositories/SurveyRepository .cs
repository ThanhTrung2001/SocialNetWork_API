using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class SurveyRepository
    {
        private readonly DatabaseContext _context;

        public SurveyRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
