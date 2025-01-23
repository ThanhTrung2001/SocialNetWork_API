using EV.DataAccess.DataConnection;

namespace EV.DataAccess.Repositories
{
    public class SurveyVoteRepository
    {
        private readonly DatabaseContext _context;

        public SurveyVoteRepository(DatabaseContext context)
        {
            _context = context;
        }

    }
}
