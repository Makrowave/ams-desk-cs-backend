using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Table("bikes")]
public partial class Bike
{
    [Key]
    [Column("bike_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("model_id")]
    public int ModelId { get; set; }

    [Column("place_id")]
    public short PlaceId { get; set; }

    [Column("status_id")]
    public short StatusId { get; set; }

    [Column("insertion_date")]
    public DateOnly InsertionDate { get; set; }

    [Column("sale_date")]
    public DateOnly? SaleDate { get; set; }

    [Column("sale_price")]
    public int? SalePrice { get; set; }

    [Column("internet_sale")]
    public bool InternetSale { get; set; } = false;

    [Column("assembled_by")]
    public short? AssembledBy { get; set; }

    [ForeignKey(nameof(ModelId))]
    [InverseProperty(nameof(Model.Bikes))]
    public virtual Model? Model { get; set; }

    [ForeignKey(nameof(PlaceId))]
    [InverseProperty(nameof(Place.Bikes))]
    public virtual Place? Place { get; set; }

    [ForeignKey(nameof(StatusId))]
    [InverseProperty(nameof(Status.Bikes))]
    public virtual Status? Status { get; set; }

    [ForeignKey(nameof(AssembledBy))]
    [InverseProperty(nameof(Employee.Bikes))]
    public virtual Employee? Employee { get; set; }
}