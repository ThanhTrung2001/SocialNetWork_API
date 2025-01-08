namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class SurveyQuery
    {
        public Guid Id { get; set; }
        public DateTime Expired_At { get; set; }
        public string Question { get; set; }
        public int Total_Vote { get; set; }
        public string Survey_Type { get; set; }
        public List<SurveyItemQuery> SurveyItems { get; set; } = new List<SurveyItemQuery>();
    }

    public class SurveyItemQuery
    {
        public Guid SurveyItem_Id { get; set; }
        public Guid Survey_Id { get; set; }
        public string Option_Name { get; set; }
        public int Total_Vote { get; set; }
        public List<SurveyVoteQuery> Votes { get; set; } = new List<SurveyVoteQuery>();
    }

    public class SurveyVoteQuery
    {
        public Guid Vote_UserId { get; set; }
        public string Vote_FirstName { get; set; }
        public string Vote_LastName { get; set; }
        public string Vote_Avatar { get; set; }
        //public Guid SurveyItemId { get; set; }
    }


}
