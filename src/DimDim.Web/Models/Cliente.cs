using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimDim.Web.Models;

[Table("CLIENTE")]
public class Cliente
{
    [Key]
    [Column("ID_CLIENTE")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome")]
    [StringLength(100)]
    [Column("NOME")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF com 11 dígitos, só números")]
    [Column("CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(120)]
    [Column("EMAIL")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("TELEFONE")]
    public string? Telefone { get; set; }

    [Display(Name = "Cadastro")]
    [Column("DT_CADASTRO")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public List<Conta> Contas { get; set; } = new();
}
