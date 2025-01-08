namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateSurveyCommand
    {
        public DateTime Expired_At { get; set; }
        public string Question { get; set; }
        public string Survey_Type { get; set; }
        public List<CreateSurveyItemCommand> SurveyItems { get; set; }
    }

    public class CreateSurveyItemCommand()
    {
        public string Option_Name { get; set; }
        public List<CreateSurveyItemCommand>? SurveyVotes { get; set; }
    }

    public class CreateSurveyVoteCommand
    {
        public Guid User_Id { get; set; }
        public Guid SurveyItem_Id { get; set; }
    }
}
