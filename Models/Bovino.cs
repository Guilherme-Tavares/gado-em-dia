using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public class Bovino
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe a identificação do animal.")]
    [StringLength(50, ErrorMessage = "A identificação deve ter no máximo 50 caracteres.")]
    public string Identificacao { get; set; } = "";

    [Required(ErrorMessage = "Selecione o sexo.")]
    public Sexo? Sexo { get; set; }

    public Categoria? Categoria { get; set; }

    [StringLength(50, ErrorMessage = "A raça deve ter no máximo 50 caracteres.")]
    public string? Raca { get; set; }

    [DataNaoFutura(ErrorMessage = "A data de nascimento não pode ser futura.")]
    public DateTime? DataNascimento { get; set; }

    [Range(1.0, 1500.0, ErrorMessage = "O peso deve estar entre 1 e 1500 kg.")]
    public decimal? PesoKg { get; set; }

    public SituacaoBovino Situacao { get; set; } = SituacaoBovino.Ativo;

    [StringLength(250, ErrorMessage = "As observações devem ter no máximo 250 caracteres.")]
    public string? Observacoes { get; set; }

    public List<Aplicacao> Aplicacoes { get; set; } = new();
}
