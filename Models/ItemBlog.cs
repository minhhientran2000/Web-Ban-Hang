using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [Table("Blog")]
    public class ItemBlog
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Create_at { get; set; }
        public DateTime Update_at { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
    }
}
