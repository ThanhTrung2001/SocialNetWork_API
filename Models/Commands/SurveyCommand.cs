namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateSurveyCommand
    {
        public DateTime ExpiredAt { get; set; }
        public string Question { get; set; }
        public int SurveyTypeId { get; set; }
        public List<CreateSurveyItemCommand> SurveyItems { get; set; }
    }

    public class CreateSurveyItemCommand()
    {
        public string OptionName { get; set; }
        public List<CreateSurveyItemCommand>? SurveyVotes { get; set; }
    }

    public class CreateSurveyVoteCommand
    {
        public Guid UserId { get; set; }
        public Guid SurveyItemId { get; set; }
    }
}
