﻿using System.ComponentModel.DataAnnotations.Schema;

namespace EnVietSocialNetWorkAPI.Old
{
    [Table("Employees")]
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public int CompanyId { get; set; }
    }
}
