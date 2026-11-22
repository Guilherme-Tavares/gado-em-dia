using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public class Aplicacao
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe a data.")]
    [DataNaoFutura]
    public DateTime Data { get; set; } = DateTime.Today;

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o animal.")]
    public int BovinoId { get; set; }
    public Bovino? Bovino { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o aplicador.")]
    public int AplicadorId { get; set; }
    public Aplicador? Aplicador { get; set; }

    [StringLength(250, ErrorMessage = "A observação deve ter no máximo 250 caracteres.")]
    public string? Observacao { get; set; }

    public List<AplicacaoProduto> Itens { get; set; } = new();
}
