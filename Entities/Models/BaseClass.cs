using EnVietSocialNetWorkAPI.Entities.Interface;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnVietSocialNetWorkAPI.Entities.Models
{
    public class BaseClass : IBaseClass
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
