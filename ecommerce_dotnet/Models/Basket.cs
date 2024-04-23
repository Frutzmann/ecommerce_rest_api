using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{
    public class Basket: IHasTimestamps
    {
        [Column("id")]
        public int id { get; set; }
        public User User { get; set; }

        public Product Product { get; set; }

        [Column("quantity", TypeName= "int")]
        public int quantity { get; set; }

        [Column("order_number", TypeName = "int")]
        public int orderNumber { get; set; }

        [Column("creation_date", TypeName = "datetime")]
        public DateTime? creationDate { get; set; }

        [Column("updated_date", TypeName = "datetime")]
        public DateTime? updatedDate { get; set; }

        public Basket()
        {
        }

    }
}
