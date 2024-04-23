using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{
    public class Order : IHasTimestamps
    {
        [Column("id")]
        public int id { get; set; }
        public Address address { get; set; }
        public User user { get; set; }

        [Column("totalPrice", TypeName ="int")]
        public int totalPrice { get; set; }

        [Column("order_number", TypeName ="int")]
        public int orderNumber { get; set; }
        [Column("creation_date", TypeName = "datetime")]
        public DateTime? creationDate { get; set; }

        [Column("updated_date", TypeName = "datetime")]
        public DateTime? updatedDate { get; set; }
    }
}
