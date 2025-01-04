namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewSurvey
    {
        public DateTime ExpiredIn { get; set; }
        public string Question { get; set; }
        public List<NewSurveyItem> SurveyItems { get; set; }
    }

    public class NewSurveyItem()
    {
        public string Content { get; set; }
        public int Vote { get; set; }
        //public List<NewSurveyVote> SurveyVotes { get; set; }
    }

    public class NewSurveyVote
    {
        public Guid OptionId { get; set; }
        public Guid UserId { get; set; }
    }
}
