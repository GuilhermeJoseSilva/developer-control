# DeveloperControl

Projeto de estudos práticos em C# e .NET 10.

O objetivo é evoluir gradualmente uma aplicação simples, estudando fundamentos do .NET, orientação a objetos, modelagem de domínio, testes, arquitetura, APIs, persistência, workers, mensageria, Hangfire, Docker, observabilidade e DevOps.

## Estrutura

```text
DeveloperControl/
├── DeveloperControl.sln
├── README.md
├── docs/
├── src/
│   ├── DeveloperControl.Console/
│   ├── DeveloperControl.Domain/
│   └── DeveloperControl.Application/
└── tests/
    └── DeveloperControl.Tests/
```

## Documentação de estudo

### Semana 1

- [Dia 1 — Solution, projetos, entidade e testes](docs/Semana-1-Dia-1.md)
- [Dia 2 — Encapsulamento, estado e invariantes](docs/Semana-1-Dia-2.md)

## Executar o projeto

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src\DeveloperControl.Console
```

## Tecnologias atuais

- .NET 10
- C#
- xUnit
- Visual Studio 2026
