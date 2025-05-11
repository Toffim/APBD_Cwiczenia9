using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class WarehouseRequestDTO
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    [Range(0, Int32.MaxValue)]
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}