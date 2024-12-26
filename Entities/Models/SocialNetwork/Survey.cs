namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Survey : BaseClass
    {
        public DateTime ExpiredIn { get; set; }
        public ICollection<SurveyItem> Surveys { get; set; }
        public Post post { get; set; }
    }

    public class SurveyItem : BaseClass
    {
        public string content { get; set; }
        public ICollection<User>? Users { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
