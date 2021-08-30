using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiTest.Data.Entities
{
    public class Address
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public virtual User User { get; set; }
    }
}
