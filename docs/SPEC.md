# Gado em Dia: especificação

Sistema web para registrar o manejo sanitário de um rebanho de gado de corte: quais animais receberam vacinas, vermífugos ou outros medicamentos, quando, em que dose e por quem.

Trabalho final de Linguagem de Programação Visual II. Entrega e apresentação até **01/12/2026**. A divisão do trabalho entre os integrantes está em [DIVISAO.md](DIVISAO.md).

## 1. Problema e usuário

Em muitas propriedades, o controle sanitário do gado fica na memória ou em cadernos. Fica difícil saber:

- o que cada animal já recebeu;
- quando vence o próximo reforço;
- se um animal medicado ainda está no período de carência e não pode ir para o abate.

O usuário é o **produtor**, ou quem cuida do gado para ele. O sistema não exige brinco, registro profissional ou dados que pequenas propriedades não costumam ter: os animais podem ser identificados por nome ou apelido, e quem aplica os produtos geralmente é o próprio dono ou um funcionário.

## 2. Tecnologias

- Blazor Server, .NET 10, desenvolvido no Visual Studio.
- Bootstrap, já incluído no modelo do projeto.
- MySQL com Entity Framework Core.
  - Provedor `MySql.EntityFrameworkCore` 10.0.x, da Oracle. O Pomelo não suporta EF Core 10.
  - Pacote `Microsoft.EntityFrameworkCore.Design` e ferramenta `dotnet-ef` 10.0.x.
- Banco `gado_em_dia`, com a string de conexão `GadoEmDia` no `appsettings.json`.
- O contexto é registrado com `AddDbContextFactory`, e as páginas usam `IDbContextFactory<GadoContext>`.

## 3. Modelagem

### Relações

```
Bovino      1 ──── N  Aplicacao  N ──── 1  Aplicador
                          1
                          │
                          N
                   AplicacaoProduto   (com a dose aplicada)
                          N
                          │
                          1
                       Produto
```

- Um **Bovino** recebe várias **Aplicações**.
- Um **Aplicador** realiza várias **Aplicações**.
- Uma **Aplicação** usa um ou mais **Produtos**, e um Produto aparece em várias Aplicações. Essa relação muitos-para-muitos é resolvida pela tabela intermediária **AplicacaoProduto**, que também guarda a dose aplicada de cada produto.

Para a proposta, as duas entidades principais são o Bovino e a Aplicação: **um Bovino recebe várias Aplicações**.

### Por que a tabela intermediária tem classe própria

Em um mesmo manejo, o animal pode receber, por exemplo, uma vacina e um vermífugo. A relação N:N exige uma tabela de ligação.

O EF Core consegue criar essa tabela sozinho, mas só com as duas chaves. Como precisamos guardar a **dose de cada produto**, a ligação vira a entidade `AplicacaoProduto`. Ela ainda deixa a tabela visível no código, o que facilita a explicação na apresentação.

### Entidades

Os nomes no código não têm acento, seguindo o padrão das atividades. As opções de cada enum vêm descritas em [Enums](#enums).

**Bovino**

| Campo | Tipo | Regras |
|---|---|---|
| `Id` | int | Chave primária |
| `Identificacao` | string | **Obrigatório** e único; até 50 caracteres. Texto livre: nome, apelido, número ou brinco ("Mimosa", "Vaca 12", "BR-001") |
| `Sexo` | enum `Sexo` | Obrigatório |
| `Categoria` | enum `Categoria`? | Opcional |
| `Raca` | string? | Opcional; até 50 caracteres (ex.: "Nelore", "Mestiço") |
| `DataNascimento` | DateTime? (só a data) | Opcional; não pode ser futura |
| `PesoKg` | decimal? | Opcional; de 1 a 1500; `decimal(6,2)`. Último peso conhecido, útil para calcular doses por peso vivo |
| `Situacao` | enum `Situacao` | Obrigatório; padrão `Ativo` |
| `Observacoes` | string? | Opcional; até 250 caracteres (pelagem, marcas, características) |
| `Aplicacoes` | `List<Aplicacao>` | Navegação |

**Produto**

| Campo | Tipo | Regras |
|---|---|---|
| `Id` | int | Chave primária |
| `Nome` | string | Obrigatório; 3 a 60 caracteres (ex.: "Ivermectina 1%") |
| `Tipo` | enum `TipoProduto` | Obrigatório |
| `Finalidade` | string? | Opcional; até 80 caracteres (ex.: "Raiva", "Verminose", "Carrapato") |
| `Fabricante` | string? | Opcional; até 60 caracteres |
| `Unidade` | string | Obrigatório; até 10 caracteres; padrão "mL" (ou "g", "dose") |
| `IntervaloReforcoDias` | int | De 0 a 730; 0 = sem reforço programado |
| `CarenciaDias` | int | De 0 a 365; dias, conforme a bula, sem poder abater depois da aplicação |

**Aplicador** (quem aplica: proprietário, funcionário ou veterinário)

| Campo | Tipo | Regras |
|---|---|---|
| `Id` | int | Chave primária |
| `Nome` | string | Obrigatório; 3 a 100 caracteres |
| `Funcao` | enum `Funcao` | Obrigatório |
| `Crmv` | string? | Opcional; quando preenchido, formato `1234/RO` |
| `Telefone` | string? | Opcional; `[Phone]` |
| `Email` | string? | Opcional; `[EmailAddress]` |
| `Aplicacoes` | `List<Aplicacao>` | Navegação |

**Aplicacao** (um animal, em uma data, com um aplicador)

| Campo | Tipo | Regras |
|---|---|---|
| `Id` | int | Chave primária |
| `Data` | DateTime (só a data) | Obrigatório; não pode ser futura nem anterior ao nascimento do bovino, quando a data de nascimento for conhecida |
| `BovinoId` / `Bovino` | int / Bovino | Obrigatório (mensagem "Selecione o animal") |
| `AplicadorId` / `Aplicador` | int / Aplicador | Obrigatório (mensagem "Selecione o aplicador") |
| `Observacao` | string? | Opcional; até 250 caracteres |
| `Itens` | `List<AplicacaoProduto>` | Ao menos um produto |

**AplicacaoProduto**

| Campo | Tipo | Regras |
|---|---|---|
| `AplicacaoId` / `Aplicacao` | int / Aplicacao | Chave composta (`AplicacaoId`, `ProdutoId`) |
| `ProdutoId` / `Produto` | int / Produto | Chave composta |
| `Dose` | decimal? | Opcional; de 0,01 a 1000; `decimal(7,2)`; na unidade do produto |

### Enums

Ficam gravados como inteiros. Nas telas, aparecem com acento por meio de `[Display(Name = "...")]`.

| Enum | Opções |
|---|---|
| `Sexo` | Macho, Fêmea |
| `Categoria` | Bezerro(a), Garrote, Novilha, Vaca, Boi, Touro |
| `Situacao` | Ativo, Vendido, Morto |
| `TipoProduto` | Vacina, Vermífugo, Antibiótico, Anti-inflamatório, Carrapaticida, Outro |
| `Funcao` | Proprietário, Funcionário, Veterinário, Outro |

### Configuração no `GadoContext` (`OnModelCreating`)

- Chave composta: `HasKey(ap => new { ap.AplicacaoId, ap.ProdutoId })`.
- `HasIndex(b => b.Identificacao).IsUnique()`.
- Precisão: `PesoKg` com `HasPrecision(6, 2)` e `Dose` com `HasPrecision(7, 2)`.
- Exclusão:
  - de Aplicacao para AplicacaoProduto: `Cascade`. Apagar uma aplicação apaga os itens dela.
  - de Bovino, Aplicador e Produto para o que depende deles: `Restrict`.
    - Quem tem histórico não pode ser apagado, e a tela explica o motivo.
    - Um animal vendido ou morto muda de **Situação**, em vez de ser excluído. O Excluir fica para cadastros feitos por engano.

### Validações além do obrigatório

São as que contam para o critério de "regra além de campo obrigatório".

- `[StringLength]` nos textos e `[Range]` no peso, na dose, no intervalo de reforço e na carência.
- `[RegularExpression]` no CRMV, que aceita o campo vazio.
- `[Phone]` e `[EmailAddress]` no aplicador.
- Feitas na página antes de salvar, com a mensagem exibida no formulário:
  - identificação repetida;
  - data futura ou anterior ao nascimento;
  - aplicação sem nenhum produto marcado;
  - aplicação sem nenhum animal marcado.

## 4. Telas

São 14 telas. As de cadastro e edição são uma página só, com duas rotas (`/novo` e `/editar/{Id:int}`).

| Área | Rota | Tela |
|---|---|---|
| Início | `/` | Dashboard (ver [Dashboard](#dashboard)) |
| Bovinos | `/bovinos` | Lista, com busca por identificação e filtro por situação (só ativos por padrão) |
| | `/bovinos/novo` e `/bovinos/editar/{Id:int}` | Cadastro e edição |
| | `/bovinos/{Id:int}` | Visualização |
| Produtos | `/produtos` | Lista, mostrando o tipo |
| | `/produtos/novo` e `/produtos/editar/{Id:int}` | Cadastro e edição |
| | `/produtos/{Id:int}` | Visualização |
| Aplicadores | `/aplicadores` | Lista, mostrando a função |
| | `/aplicadores/novo` e `/aplicadores/editar/{Id:int}` | Cadastro e edição |
| | `/aplicadores/{Id:int}` | Visualização |
| Aplicações | `/aplicacoes` | Lista com **identificação do animal, nome do aplicador e nomes dos produtos com a dose**, e não os Ids |
| | `/aplicacoes/nova` | Nova aplicação **em lote** (ver abaixo) |
| | `/aplicacoes/editar/{Id:int}` | Edição da aplicação de um único animal |
| | `/aplicacoes/{Id:int}` | Visualização |

O NavMenu terá Início, Bovinos, Produtos, Aplicadores e Aplicações. As páginas e os itens de exemplo do modelo (Counter, Weather) saem.

### Padrão das listas

Cada linha de todas as listas tem três botões, pelo componente `BotoesAcoes`:

- **Visualizar**: abre a página do registro.
- **Editar**: abre o formulário preenchido.
- **Excluir**: pede confirmação pelo `ConfirmarExclusao` e apaga. Se o registro tiver histórico, mostra o motivo do bloqueio.

As listas também têm um botão "Novo", no topo.

### Telas de visualização

São telas com parâmetro na rota, que mostram os dados cadastrados e o histórico ligado ao registro.

| Tela | Conteúdo |
|---|---|
| Bovino | Dados do animal, **situação sanitária** (reforços e carência, ver [Cálculos sanitários](#cálculos-sanitários)) e histórico de aplicações |
| Produto | Dados do produto e aplicações em que foi usado |
| Aplicador | Dados do aplicador e aplicações que realizou |
| Aplicação | Data, animal, aplicador, produtos com dose e observação, com links para o animal e para o aplicador |

As três primeiras reutilizam o `TabelaAplicacoes` para o histórico. Cada tela tem botões para editar o registro e voltar à lista.

### Aplicação em lote

No dia do manejo, o rebanho inteiro passa pelo curral, e cadastrar um animal por vez seria inviável. Na tela de nova aplicação:

1. Escolhe-se a data e o aplicador.
2. Marcam-se os produtos e, para cada um, informa-se a dose, se quiser.
3. Marcam-se os animais. Só aparecem os ativos, e há a opção "marcar todos".
4. Ao salvar, a página cria **uma Aplicação para cada animal marcado**, todas com os mesmos produtos e doses.

O modelo não muda, e o histórico de cada animal continua individual. Uma dose diferente para um animal específico pode ser corrigida depois, na tela de edição.

### Dashboard

A tela inicial mostra, logo de cara, se o rebanho está em dia.

1. **Cabeçalho**: nome e finalidade do sistema em uma linha, e o botão **"Nova aplicação"** à direita, porque é a ação mais frequente.
2. **Cartões de resumo**, um ao lado do outro (`CartaoResumo`, 4 vezes):
   - bovinos ativos;
   - aplicações no mês;
   - reforços atrasados, em destaque de alerta quando maior que zero;
   - animais em carência.

   Cada cartão leva à tela correspondente.
3. **Próximos reforços** (`TabelaReforcos`): os atrasados e os que vencem nos próximos 30 dias, com animal, produto, data prevista e selo de situação. Os atrasados vêm primeiro.
4. **Últimas aplicações** (`TabelaAplicacoes`): as 5 mais recentes.
5. **Rebanho por categoria**: barras horizontais com a barra de progresso do Bootstrap, mostrando quantidade e percentual de cada categoria dos animais ativos, incluindo "Sem categoria".

Em telas largas, os itens 3 e 4 ficam lado a lado, e o 5 fica abaixo ou na coluna lateral. No celular, tudo vira uma coluna.

### Visual

Moderno, limpo e consistente em todas as telas:

- Fundo claro, cartões brancos com bordas arredondadas e sombra leve (`shadow-sm`, `border-0`, `rounded-3`), e bastante espaço entre os blocos.
- Uma cor de destaque (verde) para botões principais e para o menu. Amarelo para "Vence em breve" e vermelho para "Atrasado".
- Números grandes e rótulos discretos (`text-muted`, `small`) nos cartões de resumo.
- Ícones do Bootstrap Icons, com o CSS incluído no `App.razor`.
- Tabelas com `table-hover` e `align-middle`, sem bordas pesadas.

### CRUD

- A meta é o CRUD completo, incluindo a visualização, nas quatro áreas.
- O mínimo exigido é CRUD completo em **Bovino** e **Aplicação**, e cadastrar e listar em Produto e Aplicador.

## 5. Componentes

Ficam na pasta `Components`. Nenhum deles acessa o banco: recebem dados por parâmetro e avisam a página por EventCallback.

| Componente | Técnica | Onde é reutilizado |
|---|---|---|
| `Cartao` | `ChildContent` + `[Parameter] Titulo` | Moldura dos formulários e das telas de visualização |
| `CartaoResumo` | `[Parameter]` Titulo, Valor, Icone, Link e Destaque | 4 vezes no dashboard |
| `AvisoVazio` | `[Parameter] Mensagem` | Todas as listas e históricos vazios |
| `BotoesAcoes` | `[Parameter]` VisualizarUrl e EditarUrl + `EventCallback OnExcluir` | Cada linha das quatro listas |
| `ConfirmarExclusao` | `ChildContent` (texto do aviso) + `[Parameter] Visivel` + `EventCallback` OnConfirmar e OnCancelar | As quatro listas |
| `TabelaAplicacoes` | `[Parameter] List<Aplicacao>` | `/aplicacoes`, dashboard e visualização de bovino, produto e aplicador |
| `TabelaReforcos` | `[Parameter] List<Reforco>` | Dashboard e visualização de bovino |
| `SeloSituacao` | `[Parameter] Situacao` (Em dia, Vence em breve, Atrasado, Em carência) | `TabelaReforcos`, lista de bovinos e visualização de bovino |

Com isso, as três técnicas aparecem em mais de um componente e todos são reutilizados.

## 6. Cálculos sanitários

Ficam em uma classe estática `SituacaoSanitaria`, na pasta `Services`, com métodos que recebem listas já carregadas pela página e devolvem os resultados. A classe não acessa o banco: a página consulta, a classe calcula e os componentes exibem. A mesma regra serve ao dashboard, à lista e à visualização de bovinos.

Os cálculos consideram só os **bovinos ativos**, e "hoje" é a data atual.

### Reforços

- Para cada animal e cada produto **que ele já recebeu** com `IntervaloReforcoDias > 0`:

  ```
  próximo reforço = data da última aplicação daquele produto + IntervaloReforcoDias
  ```

- Situação:
  - **Atrasado**: o próximo reforço é antes de hoje.
  - **Vence em breve**: de hoje até 30 dias.
  - **Em dia**: depois de 30 dias.
- O sistema não conhece o calendário sanitário da propriedade. Um produto que o animal nunca recebeu não gera reforço.
- O resultado é uma lista de `Reforco` (Bovino, Produto, DataPrevista, Situacao), um record simples, sem tabela no banco.
- O cartão **"Reforços atrasados"** conta os animais com pelo menos um reforço atrasado.

### Carência

- Para cada animal:

  ```
  fim da carência = maior valor, entre todas as aplicações do animal, de (data da aplicação + CarenciaDias do produto)
  ```

- Se o fim da carência for depois de hoje, o animal está **Em carência até** essa data. Caso contrário, está liberado para abate.
- O cartão **"Animais em carência"** conta os animais nessa condição. Na lista de bovinos, eles ganham o selo "Em carência".

### Outros números do dashboard

- **Aplicações no mês**: aplicações com data no mês e ano atuais.
- **Rebanho por categoria**: bovinos ativos agrupados por `Categoria`.

## 7. Notas de implementação

- **Carregar os relacionados:**

  ```csharp
  db.Aplicacoes
      .Include(a => a.Bovino)
      .Include(a => a.Aplicador)
      .Include(a => a.Itens).ThenInclude(i => i.Produto)
  ```

- **Cálculos sanitários:** o dashboard carrega as aplicações dos bovinos ativos com esses `Include` e passa a lista para a `SituacaoSanitaria`. Para o tamanho de um rebanho de apresentação, calcular em memória com LINQ é suficiente.
- **Aplicação em lote:** um `foreach` nos animais marcados. Para cada um, cria-se uma `Aplicacao` com novos `AplicacaoProduto`, sem reaproveitar as mesmas instâncias entre animais, e faz-se um único `SaveChangesAsync()` no final.
- **Editar uma aplicação:** carregar com `Include(a => a.Itens)`, limpar `Itens` e adicionar os itens dos produtos marcados, com as doses, antes do `SaveChangesAsync()`.
- **Selects:** `InputSelect` com `int` começando em 0 e opção "Selecione...". Os campos de Id usam `[Range(1, int.MaxValue)]` para exigir a escolha.
- **Enums:** `InputSelect` funciona direto com enum. Para os campos opcionais (`Categoria?`), usar uma opção vazia.
- **Exclusão bloqueada:** verificar antes com `AnyAsync` e mostrar a mensagem na tela, sugerindo mudar a situação no caso de bovinos.
- **Registro inexistente:** nas telas de visualização e edição, se o Id não existir, mostrar "Registro não encontrado" com um link de volta para a lista.
- **Migrations:** uma migration inicial com todas as tabelas (ex.: `CriarTabelas`). Mudanças posteriores no modelo viram novas migrations.

## 8. Extras, se sobrar tempo

- **Dose por peso:** no produto, um campo opcional de dose por kg. No lote, o sistema sugere a dose de cada animal a partir do `PesoKg`.
- Filtro da lista de aplicações por período, produto ou aplicador.

## 9. Apresentação

### Dados de exemplo

São inseridos pelas telas durante a apresentação. Os nomes são fictícios, e os valores de intervalo e carência são ilustrativos: no uso real, valem os da bula.

- **Produtos:**
  - Antirrábica (Vacina, 2 mL, reforço 365 dias, carência 0);
  - Polivalente clostridial (Vacina, 5 mL, reforço 365 dias, carência 0);
  - Ivermectina 1% (Vermífugo, mL, reforço 120 dias, carência 35 dias);
  - Anti-inflamatório (Anti-inflamatório, mL, sem reforço, carência 5 dias).
- **Aplicadores:**
  - João (Proprietário);
  - Zé do Curral (Funcionário);
  - Dra. Ana Souza (Veterinário, 1234/RO).
- **Bovinos:**
  - Mimosa (Fêmea, Vaca, Nelore, 420 kg);
  - Vaca 12 (Fêmea, Vaca, Mestiço);
  - BR-003 (Macho, Garrote);
  - Pintado (Macho, Boi, 500 kg).
- **Aplicações** (pensadas para o dashboard ter o que mostrar):
  - Um lote com **data de cerca de 5 meses atrás**, todos os animais com Ivermectina. Os reforços de 120 dias já venceram e aparecem como **Atrasado**.
  - Um lote com **data de cerca de 350 dias atrás**, todos os animais com Antirrábica. Os reforços de 365 dias vencem em cerca de 15 dias e aparecem como **Vence em breve**.
  - Uma aplicação **de hoje**, Pintado com Anti-inflamatório. Ele fica **Em carência** por 5 dias.

### Roteiro

1. Dashboard.
2. Cadastros: produto, aplicador e bovino. Mostrar que basta um nome para cadastrar o animal.
3. Aplicação em lote com dois produtos, com a dose da ivermectina calculada por peso.
4. Volta ao dashboard: os números mudaram.
5. Visualização de um bovino: situação sanitária e histórico.
6. Editar e excluir, incluindo a exclusão bloqueada de um bovino com histórico. Em seguida, mudar a situação dele para "Vendido".
7. Reiniciar o app e mostrar que os dados continuam.
8. Mostrar as tabelas no MySQL Workbench, com destaque para a `AplicacaoProduto`.

### Trechos para cada integrante explicar

Cada um explica um trecho do que construiu. A lista de quem explica o quê está em [DIVISAO.md](DIVISAO.md#apresentação).
