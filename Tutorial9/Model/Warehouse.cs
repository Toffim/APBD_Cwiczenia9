using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class Warehouse
{
    public int IdWarehosue { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Adress { get; set; }
}