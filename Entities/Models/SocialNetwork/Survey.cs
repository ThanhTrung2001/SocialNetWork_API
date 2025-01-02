namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Survey : BaseClass
    {
        public DateTime ExpiredIn { get; set; }
        public ICollection<SurveyItem> Surveys { get; set; }
        public Post Post { get; set; }
        public string Question { get; set; }
    }

    public class SurveyItem : BaseClass
    {
        public string Content { get; set; }
        public int Votes { get; set; }
        public ICollection<User>? Users { get; set; }
        public virtual Survey Survey { get; set; }
    }

    public class SurveyVote
    {
        public int Id { get; set; }
        public Guid OptionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
