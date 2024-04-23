using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{
    [Table("Product")]
    public class Product : IHasTimestamps
    {
        [Column("id")]
        public int id { get; set; }
        [Column("title", TypeName ="varchar(45)")]
        [Required]
        public string title { get; set; }
        [Column("description", TypeName ="varchar(100)")]
        [Required]
        public string description { get; set; }

        [Column("link", TypeName="varhchar(45)")]
        [Required]
        public string link { get; set; }
        [Column("price")]
        [Required]
        public int price { get; set; }
        [Column("currency", TypeName ="varchar(1)")]
        [Required]
        public string currency  { get; set; }

        [Column("creation_date", TypeName = "datetime")]
        public DateTime? creationDate { get; set; }

        [Column("updated_date", TypeName = "datetime")]
        public DateTime? updatedDate { get; set; }

        public Product()
        {
        }

    }
}
