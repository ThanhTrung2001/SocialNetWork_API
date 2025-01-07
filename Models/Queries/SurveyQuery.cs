namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class SurveyQuery
    {
        public Guid Id { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string Question { get; set; }
        public int Total { get; set; }
        public int SurveyTypeId { get; set; }
        public List<SurveyItemQuery> SurveyItems { get; set; } = new List<SurveyItemQuery>();
    }

    public class SurveyItemQuery
    {
        public Guid SurveyId { get; set; }
        public string OptionName { get; set; }
        public int Total { get; set; }
        public List<SurveyVoteQuery> Votes { get; set; } = new List<SurveyVoteQuery>();
    }

    public class SurveyVoteQuery
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        //public Guid SurveyItemId { get; set; }
    }


}
