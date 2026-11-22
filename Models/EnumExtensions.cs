using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GadoEmDia.Models;

public static class EnumExtensions
{
    public static string ObterNome(this Enum valor)
    {
        var campo = valor.GetType().GetField(valor.ToString());
        var display = campo?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? valor.ToString();
    }
}
