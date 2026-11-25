namespace GadoEmDia.Models;

// Um reforço previsto: qual animal precisa de qual produto e quando.
// É só um resultado de cálculo (record), não vira tabela no banco.
public record Reforco(Bovino Bovino, Produto Produto, DateTime DataPrevista, StatusSanitario Situacao);

// Resumo sanitário de um animal, usado nos selos da lista de bovinos.
// PiorReforco é nulo quando o animal não tem nenhum reforço programado.
// FimCarencia é nulo quando o animal está liberado para abate.
public record ResumoSanitario(StatusSanitario? PiorReforco, DateTime? FimCarencia)
{
    public bool EmCarencia => FimCarencia is not null;
}
