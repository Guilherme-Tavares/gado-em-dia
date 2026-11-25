using GadoEmDia.Models;

namespace GadoEmDia.Services;

// Cálculos sanitários (SPEC, seção 6).
// Esta classe NÃO acessa o banco: a página consulta e entrega as aplicações já
// carregadas (com Bovino e Itens.Produto); aqui só se calcula.
// Só valem os bovinos ativos, e "hoje" é recebido por parâmetro.
public static class SituacaoSanitaria
{
    public const int DiasParaVencer = 30;

    // Atrasado: antes de hoje. Vence em breve: de hoje até 30 dias. Em dia: depois disso.
    public static StatusSanitario ClassificarReforco(DateTime dataPrevista, DateTime hoje)
    {
        if (dataPrevista.Date < hoje.Date)
        {
            return StatusSanitario.Atrasado;
        }

        if (dataPrevista.Date <= hoje.Date.AddDays(DiasParaVencer))
        {
            return StatusSanitario.VenceEmBreve;
        }

        return StatusSanitario.EmDia;
    }

    // Para cada animal e cada produto que ele JÁ recebeu (com reforço programado):
    // próximo reforço = data da última aplicação daquele produto + IntervaloReforcoDias.
    public static List<Reforco> CalcularReforcos(IEnumerable<Aplicacao> aplicacoes, DateTime hoje)
    {
        return aplicacoes
            .Where(a => a.Bovino is { Situacao: SituacaoBovino.Ativo })
            .SelectMany(a => a.Itens
                .Where(i => i.Produto is { IntervaloReforcoDias: > 0 })
                .Select(i => (Bovino: a.Bovino!, Produto: i.Produto!, a.Data)))
            .GroupBy(x => (BovinoId: x.Bovino.Id, ProdutoId: x.Produto.Id))
            .Select(grupo => grupo.MaxBy(x => x.Data))
            .Select(x =>
            {
                DateTime prevista = x.Data.Date.AddDays(x.Produto.IntervaloReforcoDias);
                return new Reforco(x.Bovino, x.Produto, prevista, ClassificarReforco(prevista, hoje));
            })
            .OrderBy(r => r.DataPrevista)
            .ToList();
    }

    // Os atrasados e os que vencem nos próximos 30 dias (tabela do dashboard).
    public static List<Reforco> ProximosReforcos(IEnumerable<Reforco> reforcos)
    {
        return reforcos
            .Where(r => r.Situacao != StatusSanitario.EmDia)
            .OrderBy(r => r.DataPrevista)
            .ToList();
    }

    // Cartão "Reforços atrasados": conta animais, não reforços.
    public static int ContarAnimaisComReforcoAtrasado(IEnumerable<Reforco> reforcos)
    {
        return reforcos
            .Where(r => r.Situacao == StatusSanitario.Atrasado)
            .Select(r => r.Bovino.Id)
            .Distinct()
            .Count();
    }

    // Fim da carência = maior valor de (data da aplicação + CarenciaDias do produto).
    // Devolve a data se ainda for futura (animal em carência); senão, nulo (liberado).
    public static DateTime? FimCarencia(IEnumerable<Aplicacao> aplicacoesDoAnimal, DateTime hoje)
    {
        DateTime? fim = aplicacoesDoAnimal
            .SelectMany(a => a.Itens
                .Where(i => i.Produto is not null)
                .Select(i => (DateTime?)a.Data.Date.AddDays(i.Produto!.CarenciaDias)))
            .Max();

        return fim > hoje.Date ? fim : null;
    }

    // Resumo por animal ativo que tem aplicações (chave = Id do bovino).
    // Animal sem aplicações não aparece no dicionário.
    public static Dictionary<int, ResumoSanitario> Resumir(IEnumerable<Aplicacao> aplicacoes, DateTime hoje)
    {
        var ativas = aplicacoes
            .Where(a => a.Bovino is { Situacao: SituacaoBovino.Ativo })
            .ToList();

        var reforcosPorAnimal = CalcularReforcos(ativas, hoje).ToLookup(r => r.Bovino.Id);

        return ativas
            .GroupBy(a => a.BovinoId)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => new ResumoSanitario(
                    PiorSituacao(reforcosPorAnimal[grupo.Key]),
                    FimCarencia(grupo, hoje)));
    }

    // O enum vai de EmDia (0) a Atrasado (2), então o maior valor é o pior.
    private static StatusSanitario? PiorSituacao(IEnumerable<Reforco> reforcos)
    {
        return reforcos.Select(r => (StatusSanitario?)r.Situacao).Max();
    }
}
