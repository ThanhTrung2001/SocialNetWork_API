using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnVietSocialNetWorkAPI.Old
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Address")]
        public string Address { get; set; }
        [Column("Country")]
        public string Country { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
