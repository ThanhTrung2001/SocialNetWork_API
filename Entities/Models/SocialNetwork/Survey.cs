namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Survey : BaseClass
    {
        public DateTime ExpiredIn { get; set; }
        public ICollection<SurveyItem> SurveyItems { get; set; }
        public Post Post { get; set; }
        public string Question { get; set; }
        public int Total { get; set; }
        public int SurveyTypeId { get; set; }
    }

    public class SurveyItem : BaseClass
    {
        public Guid SurveyId { get; set; }
        public string Optional { get; set; }
        public int Total { get; set; }
    }

    public class SurveyVote
    {
        public Guid UserId { get; set; }
        public Guid SurveyItemId { get; set; }
    }

}
