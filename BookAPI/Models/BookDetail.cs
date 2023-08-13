using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Models
{
    public class BookDetail
    {
        [Key]
        public int BookId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string BookName { get; set; } = "";
        [Column(TypeName = "nvarchar(100)")]
        public string BookAuthor { get; set; } = "";
        [Column(TypeName = "nvarchar(100)")]
        public string BookGenre { get; set; } = "";
    }
}
