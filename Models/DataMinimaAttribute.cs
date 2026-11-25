using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

// Barra datas absurdas (ex.: 01/01/0001), que o MySQL não consegue gravar.
public class DataMinimaAttribute : ValidationAttribute
{
    private readonly int ano;

    public DataMinimaAttribute(int ano)
        : base($"Informe uma data a partir de {ano}.")
    {
        this.ano = ano;
    }

    public override bool IsValid(object? value)
    {
        return value is not DateTime data || data.Year >= ano;
    }
}
