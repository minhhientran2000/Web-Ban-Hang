using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Project.Models
{
    [Table("Orders")]
    public class ItemOrders
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime Create { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
    }
}
