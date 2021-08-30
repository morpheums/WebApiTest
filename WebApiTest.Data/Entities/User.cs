using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiTest.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [StringLength(16)]
        public string Username { get; set; }
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public virtual Address Address { get; set; }
    }
}
