namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateSurveyCommand
    {
        public DateTime ExpiredIn { get; set; }
        public string Question { get; set; }
        public int Total { get; set; }
        public int SurveyTypeId { get; set; }
        public List<CreateSurveyItemCommand> SurveyItems { get; set; }
    }

    public class CreateSurveyItemCommand()
    {
        public string OptionName { get; set; }
        public int ItemTotal { get; set; }
        public List<CreateSurveyItemCommand> SurveyVotes { get; set; }
    }

    public class CreateSurveyVoteCommand
    {
        public Guid UserId { get; set; }
        public Guid SurveyItemId { get; set; }
    }
}
