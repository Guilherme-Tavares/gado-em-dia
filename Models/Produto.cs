using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 60 caracteres.")]
    public string Nome { get; set; } = "";

    public TipoProduto Tipo { get; set; } = TipoProduto.Vacina;

    [StringLength(80, ErrorMessage = "A finalidade deve ter no máximo 80 caracteres.")]
    public string? Finalidade { get; set; }

    [StringLength(60, ErrorMessage = "O fabricante deve ter no máximo 60 caracteres.")]
    public string? Fabricante { get; set; }

    [Required(ErrorMessage = "Informe a unidade da dose.")]
    [StringLength(10, ErrorMessage = "A unidade deve ter no máximo 10 caracteres.")]
    public string Unidade { get; set; } = "mL";

    [Range(0, 730, ErrorMessage = "O intervalo de reforço deve estar entre 0 e 730 dias.")]
    public int IntervaloReforcoDias { get; set; }

    [Range(0, 365, ErrorMessage = "A carência deve estar entre 0 e 365 dias.")]
    public int CarenciaDias { get; set; }

    public List<AplicacaoProduto> Itens { get; set; } = new();
}
