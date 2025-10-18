using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("categories")]
public partial class Category
{
    [Key]
    [Column("category_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("category_name")]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Column("categories_order")]
    public required short Order { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}