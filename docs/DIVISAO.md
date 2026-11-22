# Gado em Dia: divisão do trabalho

O desenvolvimento é feito em três etapas, uma depois da outra, sempre direto na `main`. Cada um só começa quando o anterior avisar no WhatsApp que terminou. Assim ninguém mexe no código ao mesmo tempo e não há conflito no git.

Cada etapa termina com o app compilando e funcionando até onde chegou. O que cada item precisa ter está no [SPEC](SPEC.md). No final, revisamos tudo juntos.

| Etapa | Responsável | Entrega até |
|---|---|---|
| 1. Fundação e Bovinos | Guilherme | Seg 23/11 |
| 2. Produtos, Aplicadores e Aplicações | Welliton | Qui 26/11 |
| 3. Cálculos, Dashboard e acabamento | Jhoyan | Sáb 28/11 |
| Revisão e ensaio com os dados de exemplo | Todos | Dom 29/11 |

O prazo é 01/12 às 00:00. O dia 30/11 fica de folga para imprevistos.

## Etapa 1: Guilherme

- [ ] Modelos e enums (SPEC, seção 3)
- [ ] `GadoContext` com a configuração do `OnModelCreating`
- [ ] Pacotes do EF Core, `AddDbContextFactory`, string de conexão no `appsettings.json` e `dotnet user-secrets init`
- [ ] Migration `CriarTabelas`
- [ ] NavMenu com Início, Bovinos, Produtos, Aplicadores e Aplicações, sem Counter e Weather
- [ ] `_Imports.razor`, Bootstrap Icons e base visual no `app.css`
- [ ] Componentes `Cartao`, `AvisoVazio`, `BotoesAcoes` e `ConfirmarExclusao`
- [ ] Bovinos: lista, cadastro/edição e visualização (ainda sem o histórico)

**Ao terminar:** dá para cadastrar, ver, editar e excluir bovinos.

## Etapa 2: Welliton

- [ ] Produtos: lista, cadastro/edição e visualização
- [ ] Aplicadores: lista, cadastro/edição e visualização
- [ ] Componente `TabelaAplicacoes`
- [ ] Aplicações: lista, nova aplicação em lote, edição e visualização
- [ ] Histórico de aplicações nas visualizações de bovino, produto e aplicador
- [ ] Exclusão bloqueada para bovino, produto e aplicador com histórico

**Ao terminar:** o sistema inteiro funciona, só sem o dashboard e os alertas.

## Etapa 3: Jhoyan

- [ ] `SituacaoSanitaria` e `Reforco` (SPEC, seção 6)
- [ ] Componentes `SeloSituacao`, `TabelaReforcos` e `CartaoResumo`
- [ ] Dashboard completo
- [ ] Situação sanitária na visualização do bovino e selos na lista de bovinos
- [ ] Revisão visual de todas as telas
- [ ] README com o passo a passo para rodar o projeto

**Ao terminar:** o sistema está pronto para o ensaio.

## Regras

1. **Na primeira vez:**
   - Crie o banco no MySQL com `CREATE DATABASE gado_em_dia;`.
   - Guarde a sua senha no user-secrets, na pasta do projeto:

     ```
     dotnet user-secrets set "ConnectionStrings:GadoEmDia" "Server=localhost;Port=3306;Database=gado_em_dia;User=root;Password=SUA_SENHA;"
     ```

   - A senha nunca vai para o `appsettings.json`.
2. **Ao começar:** `git pull` e depois `dotnet ef database update`.
3. **Se mudar um modelo:** crie uma nova migration (`dotnet ef migrations add NomeDaMudanca`) e suba junto com o código.
4. **Antes do push:** o app precisa compilar e rodar.
5. **Ao terminar:** faça o push e avise no WhatsApp: "etapa X na main".

## Apresentação

Cada um explica um trecho do que construiu:

| Integrante | Trecho |
|---|---|
| Guilherme | Banco: modelos, relação N:N com a tabela intermediária, migration e fábrica de contextos; e o `ConfirmarExclusao` como exemplo de `ChildContent`, `[Parameter]` e `EventCallback` |
| Welliton | Aplicação em lote: `EditForm`, validações, `InputSelect` e caixas de marcação |
| Jhoyan | Dashboard e cálculos sanitários: `SituacaoSanitaria`, `TabelaReforcos` e `CartaoResumo` |
