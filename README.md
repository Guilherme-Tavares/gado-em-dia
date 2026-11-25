# Gado em Dia

Sistema web para registrar o manejo sanitário de um rebanho de gado de corte: quais animais receberam vacinas, vermífugos ou outros medicamentos, quando, em que dose e por quem. O painel inicial mostra se o rebanho está em dia (reforços atrasados, animais em carência).

Trabalho final de Linguagem de Programação Visual II. A especificação completa está em [docs/SPEC.md](docs/SPEC.md).

## Tecnologias

- Blazor Server, .NET 10
- Bootstrap e Bootstrap Icons
- MySQL com Entity Framework Core (`MySql.EntityFrameworkCore` 10.0.x)

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- MySQL Server em execução (porta 3306)
- Ferramenta do EF Core, instalada uma vez: `dotnet tool install --global dotnet-ef`

## Como rodar

Todos os comandos são executados na pasta do projeto (onde está o `GadoEmDia.csproj`).

1. **Crie o banco** no MySQL:

   ```sql
   CREATE DATABASE gado_em_dia;
   ```

2. **Guarde a string de conexão** no user-secrets (a senha nunca vai para o `appsettings.json`). Troque `SUA_SENHA` pela senha do seu MySQL:

   ```
   dotnet user-secrets set "ConnectionStrings:GadoEmDia" "Server=localhost;Port=3306;Database=gado_em_dia;User=root;Password=SUA_SENHA;"
   ```

3. **Crie as tabelas** aplicando as migrations:

   ```
   dotnet ef database update
   ```

4. **Rode o sistema**:

   ```
   dotnet run --launch-profile http
   ```

5. Abra **http://localhost:5177** no navegador.

No Visual Studio, basta abrir `GadoEmDia.slnx` e executar (F5) depois dos passos 1 a 3.

## Quando alguém muda um modelo

```
dotnet ef migrations add NomeDaMudanca
dotnet ef database update
```

A migration gerada sobe junto com o código.

## Estrutura

| Pasta | Conteúdo |
|---|---|
| `Models` | Entidades, enums e validações (Data Annotations) |
| `Data` | `GadoContext` (EF Core) |
| `Services` | `SituacaoSanitaria`: cálculos de reforço e carência |
| `Components` | Componentes reutilizáveis (`Cartao`, `CartaoResumo`, `SeloSituacao`, `TabelaReforcos`, ...) |
| `Components/Pages` | Telas: Início (dashboard), Bovinos, Produtos, Aplicadores e Aplicações |
| `Migrations` | Histórico do banco |
| `docs` | Especificação e divisão do trabalho |

## Roteiro de teste com os dados de exemplo

Os dados de exemplo (SPEC, seção 9) são cadastrados pelas telas:

1. Cadastre os produtos, os aplicadores e os bovinos.
2. Em **Aplicações > Nova aplicação**, registre os lotes: um com data de uns 5 meses atrás (Ivermectina), outro de uns 350 dias atrás (Antirrábica) e uma aplicação de hoje (Anti-inflamatório no Pintado).
3. Volte ao **Início**: os reforços aparecem como Atrasado e Vence em breve, e o Pintado aparece em carência.
