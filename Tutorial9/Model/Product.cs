using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutorial9.Model;

public class Product
{
    public int IdProduct { get; set; }
    [MaxLength(200)] public string Name { get; set; }
    public float Amount { get; set; }
}