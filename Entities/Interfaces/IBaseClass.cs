namespace EnVietSocialNetWorkAPI.Entities.Interface
{
    public interface IBaseClass
    {
        Guid Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        bool IsDeleted { get; set; }
    }
}
