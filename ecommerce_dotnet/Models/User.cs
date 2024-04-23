using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace quest_web.Models
{
    public enum UserRole
    {
        ROLE_USER = 0,
        ROLE_ADMIN = 1,
    }

     [Table("user")]
     [Index(nameof(Username), IsUnique = true)]
    public class User : IHasTimestamps
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username", TypeName = "varchar(255)")]
        [Required]
        public string Username { get; set; }
        [Column("password", TypeName = "varchar(255)")]
        [Required]
        public string Password { get; set; }
        [Column("role", TypeName = "varchar(255)")]
        public UserRole? Role { get; set; } = UserRole.ROLE_USER;
        [Column("creation_date", TypeName = "datetime")]
        public DateTime? creationDate { get; set; }
        [Column("updated_date", TypeName = "datetime")]
        public DateTime? updatedDate { get; set; }

        public User()
        {

        }
    }
}
