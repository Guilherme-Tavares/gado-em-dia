using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public enum Sexo
{
    Macho,
    [Display(Name = "Fêmea")] Femea
}

public enum Categoria
{
    [Display(Name = "Bezerro(a)")] Bezerro,
    Garrote,
    Novilha,
    Vaca,
    Boi,
    Touro
}

public enum SituacaoBovino
{
    Ativo,
    Vendido,
    Morto
}

public enum TipoProduto
{
    Vacina,
    [Display(Name = "Vermífugo")] Vermifugo,
    [Display(Name = "Antibiótico")] Antibiotico,
    [Display(Name = "Anti-inflamatório")] AntiInflamatorio,
    Carrapaticida,
    Outro
}

public enum Funcao
{
    [Display(Name = "Proprietário")] Proprietario,
    [Display(Name = "Funcionário")] Funcionario,
    [Display(Name = "Veterinário")] Veterinario,
    Outro
}

// A ordem importa: nos reforços, quanto maior o valor, pior a situação.
public enum StatusSanitario
{
    [Display(Name = "Em dia")] EmDia,
    [Display(Name = "Vence em breve")] VenceEmBreve,
    Atrasado,
    [Display(Name = "Em carência")] EmCarencia
}
