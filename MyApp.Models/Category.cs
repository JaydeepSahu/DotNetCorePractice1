﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Display Order")]
        public string DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; }=DateTime.Now;



    }
}
