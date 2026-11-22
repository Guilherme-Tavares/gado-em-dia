using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public class AplicacaoProduto
{
    public int AplicacaoId { get; set; }
    public Aplicacao? Aplicacao { get; set; }

    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    [Range(0.01, 1000.0, ErrorMessage = "A dose deve estar entre 0,01 e 1000.")]
    public decimal? Dose { get; set; }
}
