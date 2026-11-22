using System.ComponentModel.DataAnnotations;

namespace GadoEmDia.Models;

public class DataNaoFuturaAttribute : ValidationAttribute
{
    public DataNaoFuturaAttribute()
        : base("A data não pode ser futura.")
    {
    }

    public override bool IsValid(object? value)
    {
        return value is not DateTime data || data.Date <= DateTime.Today;
    }
}
