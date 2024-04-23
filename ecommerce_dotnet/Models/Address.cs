using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{

    [Table("Address")]
    public class Address : IHasTimestamps
    {
        [Column("id")]
        public int id { get; set; }
        
        [Column("street", TypeName ="varchar(100)")]
        [Required]
        public string road { get; set; } 
        
        [Column("postal_code", TypeName = "varchar(30)")]
        [Required]
        public string postalCode { get; set; }
        
        [Column("city", TypeName = "varchar(50)")]
        [Required]
        public string city { get; set; }

        [Column("country", TypeName = "varchar(50)")]
        [Required]
        public string country { get; set; }

        public User User { get; set; }
        
        [Column("creation_date", TypeName = "datetime")]
        public DateTime? creationDate { get; set; }
        
        [Column("updated_date", TypeName = "datetime")]
        public DateTime? updatedDate { get; set; }

        public Address()
        {         
        }
    }
}
