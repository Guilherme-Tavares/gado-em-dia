# Explicação da Etapa 2 — Produtos, Aplicadores e Aplicações

Este documento explica os arquivos implementados para a Etapa 2 e serve como roteiro para apresentar o código.

## Visão geral

A etapa trabalha com três cadastros relacionados:

- **Produto**: vacina, vermífugo ou medicamento que será usado.
- **Aplicador**: pessoa que realizou o manejo.
- **Aplicação**: registro de que um ou mais produtos foram aplicados a um bovino, em uma data e por um aplicador.

Uma aplicação pode ter vários produtos. A entidade `AplicacaoProduto`, já criada na Etapa 1, é a ligação entre aplicação e produto e também guarda a dose de cada produto.

## `Components/TabelaAplicacoes.razor`

É um componente reutilizável para mostrar históricos e a lista principal de aplicações.

- O parâmetro `[Parameter, EditorRequired] List<Aplicacao> Aplicacoes` recebe os dados já carregados pela página. O componente não acessa o banco, o que permite reutilizá-lo em bovino, produto, aplicador e na página de aplicações.
- `OrderByDescending(a => a.Data)` mostra os registros mais recentes primeiro.
- `aplicacao.Bovino?.Identificacao`, `Aplicador?.Nome` e `Itens.Select(...)` usam as propriedades de navegação carregadas com `Include` nas páginas.
- Quando a lista está vazia, `AvisoVazio` informa o usuário de forma padronizada.

## Produtos

Os arquivos estão em `Components/Pages/Produtos`.

### `ListaProdutos.razor`

- Busca todos os produtos com `DbFactory.CreateDbContextAsync()` e mostra uma tabela.
- `BotoesAcoes` fornece os botões reutilizáveis de visualizar, editar e excluir.
- Antes de excluir, `AnyAsync(a => a.Itens.Any(i => i.ProdutoId == produto.Id))` procura aplicações que usam o produto. Se existir histórico, a exclusão é bloqueada para preservar a integridade do registro sanitário.
- `ConfirmarExclusao` pede confirmação antes de remover um item sem histórico.

### `FormProduto.razor`

- Possui duas rotas: `/produtos/novo` cria um produto e `/produtos/editar/{Id:int}` edita um existente.
- `EditForm`, `DataAnnotationsValidator` e `ValidationMessage` usam as anotações existentes no modelo `Produto` para validar nome, unidade e intervalos.
- `AsNoTracking()` é usado ao ler para edição porque o objeto será enviado novamente ao contexto apenas no momento de salvar.
- `Limpar` remove espaços e transforma textos opcionais vazios em `null`, evitando salvar dados sem conteúdo.

### `DetalhesProduto.razor`

- Exibe os dados do produto e carrega o seu histórico.
- A consulta usa `Include(a => a.Bovino)`, `Include(a => a.Aplicador)` e `Include(a => a.Itens).ThenInclude(i => i.Produto)` para que a tabela tenha todos os dados necessários.
- O filtro `Where(a => a.Itens.Any(i => i.ProdutoId == Id))` limita o histórico às aplicações que contêm aquele produto.

## Aplicadores

Os arquivos estão em `Components/Pages/Aplicadores` e usam a mesma organização de Produtos.

### `ListaAplicadores.razor`

- Lista nome, função e telefone.
- Ao excluir, `AnyAsync(x => x.AplicadorId == a.Id)` bloqueia a remoção de um aplicador já relacionado a aplicações.

### `FormAplicador.razor`

- Usa `InputSelect` para o enum `Funcao`; a opção inicial vazia, junto com `[Required]`, obriga a escolha.
- Os campos CRMV, telefone e e-mail são validados pelas Data Annotations definidas no modelo.
- Os setters do modelo normalizam CRMV, telefone e e-mail antes de persistir os dados.

### `DetalhesAplicador.razor`

- Mostra dados de contato e reutiliza `TabelaAplicacoes` para mostrar as aplicações realizadas pela pessoa.
- A consulta filtra por `AplicadorId` e carrega todos os relacionamentos mostrados na tabela.

## Aplicações

Os arquivos estão em `Components/Pages/Aplicacoes`.

### `ListaAplicacoes.razor`

- Carrega aplicações com todos os relacionamentos necessários e delega a apresentação para `TabelaAplicacoes`.
- `AsNoTracking()` melhora a leitura porque a lista não será alterada nesse contexto.

### `FormAplicacao.razor`

É a parte principal da etapa, responsável por cadastrar em lote.

- A rota `/aplicacoes/nova` cria aplicações. A rota `/aplicacoes/editar/{Id:int}` altera uma aplicação existente.
- Os selects usam identificadores inteiros. O valor `0` representa “Selecione...”; o `[Range(1, int.MaxValue)]` do modelo impede que o formulário seja salvo sem aplicador.
- Os bovinos e produtos são marcados em caixas de seleção. `HashSet<int>` guarda os IDs escolhidos sem repetição; `Dictionary<int, decimal?> doses` associa a dose ao produto selecionado.
- Antes de salvar, o formulário exige pelo menos um animal e um produto. Também compara a data com o nascimento conhecido de cada bovino para impedir uma aplicação anterior ao nascimento.
- Na criação em lote, o `foreach` cria uma nova `Aplicacao` e novos `AplicacaoProduto` para cada bovino. As instâncias não são reaproveitadas, assim o EF Core grava relacionamentos independentes. Há apenas um `SaveChangesAsync()` ao final.
- Na edição, a aplicação existente é carregada com `Include(a => a.Itens)`. `existente.Itens.Clear()` remove os produtos antigos do relacionamento; depois são adicionados os itens que estão marcados atualmente. Isso evita duplicação de produtos.

### `DetalhesAplicacao.razor`

- Carrega uma aplicação específica com os dados do bovino, aplicador e itens.
- Mostra cada produto com a dose e a unidade correspondente, facilitando conferir o manejo registrado.

## Alteração em `DetalhesBovino.razor`

A visualização do bovino recebeu um cartão “Histórico de aplicações”. A página consulta somente as aplicações daquele bovino, carrega seus relacionados com `Include` e passa a lista para `TabelaAplicacoes`. Isso cumpre o histórico exigido sem copiar o HTML de uma tabela em cada tela.

## Recursos importantes usados

- **`IDbContextFactory<GadoContext>`**: cria um contexto curto para cada operação assíncrona da página Blazor, evitando compartilhar contexto entre interações.
- **`Include` / `ThenInclude`**: carregam as entidades relacionadas que serão exibidas na tela.
- **`AsNoTracking`**: usado em leituras; reduz acompanhamento do EF Core quando não será feita alteração naquele contexto.
- **`EditForm` e Data Annotations**: conectam formulário e regras do modelo, exibindo erros antes da gravação.
- **`[Parameter]`**: permite que componentes recebam dados da página pai, como a lista em `TabelaAplicacoes`.
- **`EventCallback`**: já é fornecido em `BotoesAcoes` e `ConfirmarExclusao`; permite que o componente avise a página quando o usuário deseja excluir ou confirma a exclusão.

## Como demonstrar

1. Cadastre um produto e um aplicador.
2. Em Aplicações, marque dois bovinos e um ou mais produtos, informe as doses e salve.
3. Abra um bovino, o produto e o aplicador para mostrar que cada tela exibe o mesmo histórico filtrado.
4. Tente excluir o produto, aplicador ou bovino utilizado: o sistema deve bloquear a exclusão e preservar o histórico.
