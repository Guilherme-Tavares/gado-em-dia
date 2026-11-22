using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public class Aplicador
{
    private string? crmv;
    private string? telefone;
    private string? email;

    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "Selecione a função.")]
    public Funcao? Funcao { get; set; }

    [StringLength(10)]
    [RegularExpression(@"^\d{1,6}/[A-Z]{2}$", ErrorMessage = "Use o formato 1234/RO.")]
    public string? Crmv
    {
        get => crmv;
        set => crmv = string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    [Phone(ErrorMessage = "Telefone inválido.")]
    public string? Telefone
    {
        get => telefone;
        set => telefone = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email
    {
        get => email;
        set => email = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public List<Aplicacao> Aplicacoes { get; set; } = new();
}
