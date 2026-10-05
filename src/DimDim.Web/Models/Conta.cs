using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimDim.Web.Models;

[Table("CONTA")]
public class Conta
{
    [Key]
    [Column("ID_CONTA")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe a agência")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Agência com 4 dígitos")]
    [Display(Name = "Agência")]
    [Column("AGENCIA")]
    public string Agencia { get; set; } = "0001";

    [Required(ErrorMessage = "Informe o número da conta")]
    [StringLength(12)]
    [Display(Name = "Número")]
    [Column("NUMERO")]
    public string Numero { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Tipo")]
    [Column("TIPO")]
    public string Tipo { get; set; } = "CORRENTE";

    [Range(0, 999999999, ErrorMessage = "Saldo não pode ser negativo")]
    [Column("SALDO", TypeName = "decimal(15,2)")]
    public decimal Saldo { get; set; }

    [Display(Name = "Abertura")]
    [Column("DT_ABERTURA")]
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Escolha o cliente")]
    [Display(Name = "Cliente")]
    [Column("ID_CLIENTE")]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    public static readonly string[] Tipos = { "CORRENTE", "POUPANCA", "SALARIO" };
}
