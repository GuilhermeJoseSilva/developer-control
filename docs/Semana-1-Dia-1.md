# DeveloperControl — Semana 1, Dia 1

## Objetivo

Montar a estrutura inicial de uma solução .NET 10, entender a função de cada projeto, configurar referências entre projetos, criar uma entidade de domínio simples e validar seu comportamento com testes unitários usando xUnit.

---

## Setup utilizado

- .NET 10
- Visual Studio 2026
- Windows para desenvolvimento, build e testes
- Docker instalado apenas no WSL
- Docker não é necessário nesta etapa

O fluxo adotado é:

```text
Visual Studio + .NET 10 → Windows
Código-fonte             → C:\Projects
Docker Engine            → WSL
```

O .NET não precisa estar instalado no WSL neste momento. Quando for necessário criar uma imagem Docker, o build poderá acontecer dentro de uma imagem que já contenha o SDK do .NET.

---

## Estrutura da solução

A solução foi criada com a seguinte estrutura:

```text
DeveloperControl/
├── DeveloperControl.sln
├── src/
│   ├── DeveloperControl.Console/
│   ├── DeveloperControl.Domain/
│   └── DeveloperControl.Application/
└── tests/
    └── DeveloperControl.Tests/
```

### Responsabilidade dos projetos

#### `DeveloperControl.Domain`

Contém as entidades, regras e comportamentos centrais do domínio.

Esse projeto deve permanecer independente de detalhes externos como:

- banco de dados;
- APIs;
- Docker;
- interface de usuário;
- infraestrutura;
- frameworks específicos.

#### `DeveloperControl.Application`

Responsável por organizar os casos de uso da aplicação.

Depende do projeto `Domain`.

#### `DeveloperControl.Console`

Aplicação executável utilizada inicialmente para testar e demonstrar o funcionamento da solução.

#### `DeveloperControl.Tests`

Projeto de testes unitários com xUnit.

---

## Criação da solução e dos projetos

```powershell
cd C:\Projects
mkdir DeveloperControl
cd DeveloperControl

dotnet new sln -n DeveloperControl

dotnet new console -n DeveloperControl.Console -o src\DeveloperControl.Console
dotnet new classlib -n DeveloperControl.Domain -o src\DeveloperControl.Domain
dotnet new classlib -n DeveloperControl.Application -o src\DeveloperControl.Application
dotnet new xunit -n DeveloperControl.Tests -o tests\DeveloperControl.Tests
```

---

## Adição dos projetos à solution

```powershell
dotnet sln add src\DeveloperControl.Console\DeveloperControl.Console.csproj
dotnet sln add src\DeveloperControl.Domain\DeveloperControl.Domain.csproj
dotnet sln add src\DeveloperControl.Application\DeveloperControl.Application.csproj
dotnet sln add tests\DeveloperControl.Tests\DeveloperControl.Tests.csproj
```

Para verificar os projetos adicionados:

```powershell
dotnet sln list
```

---

## Referências entre projetos

A direção principal das dependências é:

```text
Console → Application → Domain
```

As referências foram configuradas com:

```powershell
dotnet add src\DeveloperControl.Application reference src\DeveloperControl.Domain
dotnet add src\DeveloperControl.Console reference src\DeveloperControl.Application
dotnet add src\DeveloperControl.Console reference src\DeveloperControl.Domain
dotnet add tests\DeveloperControl.Tests reference src\DeveloperControl.Domain
dotnet add tests\DeveloperControl.Tests reference src\DeveloperControl.Application
```

A referência direta do Console para o Domain foi adicionada porque o `Program.cs` utiliza diretamente a classe `DeveloperTask`.

---

## Entidade `DeveloperTask`

Arquivo:

```text
src/DeveloperControl.Domain/Tasks/DeveloperTask.cs
```

Implementação:

```csharp
namespace DeveloperControl.Domain.Tasks;

public sealed class DeveloperTask
{
    public Guid Id { get; }

    public string Title { get; }

    public string? Description { get; }

    public DateTimeOffset CreatedAt { get; }

    public DeveloperTask(
        string title,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "O título da tarefa é obrigatório.",
                nameof(title));
        }

        Id = Guid.NewGuid();
        Title = title.Trim();

        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        CreatedAt = DateTimeOffset.UtcNow;
    }
}
```

---

## Regras implementadas

A entidade protege as seguintes regras:

1. O título é obrigatório.
2. O título não pode ser vazio ou composto apenas por espaços.
3. Espaços extras no início e no final do título são removidos.
4. A descrição é opcional.
5. Descrição `null`, vazia ou composta apenas por espaços é convertida para `null`.
6. Espaços extras de uma descrição válida são removidos.
7. O identificador é criado automaticamente.
8. A data de criação é registrada em UTC.

A normalização da descrição acontece nesta expressão:

```csharp
Description = string.IsNullOrWhiteSpace(description)
    ? null
    : description.Trim();
```

---

## Uso no projeto Console

Arquivo:

```text
src/DeveloperControl.Console/Program.cs
```

Exemplo:

```csharp
using DeveloperControl.Domain.Tasks;

var task = new DeveloperTask(
    "Estudar estrutura de soluções .NET",
    "Revisar solution, projetos e referências");

Console.WriteLine($"Id: {task.Id}");
Console.WriteLine($"Título: {task.Title}");
Console.WriteLine($"Descrição: {task.Description}");
Console.WriteLine($"Criada em UTC: {task.CreatedAt}");
```

Execução:

```powershell
dotnet run --project src\DeveloperControl.Console
```

---

## Testes unitários

Arquivo:

```text
tests/DeveloperControl.Tests/Tasks/DeveloperTaskTests.cs
```

Implementação:

```csharp
using DeveloperControl.Domain.Tasks;

namespace DeveloperControl.Tests.Tasks;

public sealed class DeveloperTaskTests
{
    [Fact]
    public void Constructor_ShouldCreateTask_WhenTitleIsValid()
    {
        var task = new DeveloperTask(
            "Estudar estrutura de projetos");

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(
            "Estudar estrutura de projetos",
            task.Title);
        Assert.NotEqual(default, task.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenTitleIsInvalid(
        string invalidTitle)
    {
        var action = () => new DeveloperTask(invalidTitle);

        var exception =
            Assert.Throws<ArgumentException>(action);

        Assert.Equal(
            "title",
            exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldRemoveWhitespace_FromTitle()
    {
        var task = new DeveloperTask(
            "  Estudar C#  ");

        Assert.Equal(
            "Estudar C#",
            task.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldSetDescriptionToNull_WhenDescriptionIsEmpty(
        string? description)
    {
        var task = new DeveloperTask(
            "Estudar C#",
            description);

        Assert.Null(task.Description);
    }

    [Fact]
    public void Constructor_ShouldTrimDescription_WhenDescriptionIsProvided()
    {
        var task = new DeveloperTask(
            "Estudar C#",
            "  Revisar classes e objetos  ");

        Assert.Equal(
            "Revisar classes e objetos",
            task.Description);
    }

    [Fact]
    public void Constructor_ShouldKeepDescription_WhenDescriptionIsValid()
    {
        var task = new DeveloperTask(
            "Estudar testes",
            "Criar testes com xUnit");

        Assert.Equal(
            "Criar testes com xUnit",
            task.Description);
    }
}
```

Execução dos testes:

```powershell
dotnet test
```

---

## Comandos utilizados

```powershell
dotnet new
dotnet restore
dotnet build
dotnet run
dotnet test
dotnet clean
dotnet sln list
dotnet add reference
```

Fluxo de validação:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src\DeveloperControl.Console
```

---

## Arquivos gerados pelo build

Após a compilação, o .NET cria principalmente as pastas:

```text
bin/
obj/
```

### `bin`

Contém os artefatos finais da compilação, como:

```text
DeveloperControl.Domain.dll
DeveloperControl.Domain.pdb
DeveloperControl.Domain.deps.json
```

### `obj`

Contém arquivos intermediários utilizados pelo MSBuild.

Essas pastas não devem ser versionadas no Git.

Para criar um `.gitignore` adequado:

```powershell
dotnet new gitignore
```

---

## Conceitos estudados

### Solution

O arquivo `.sln` agrupa e organiza os projetos, mas não representa uma aplicação compilável por si só.

### Projeto

Cada arquivo `.csproj` representa uma unidade que pode ser restaurada, compilada e, dependendo do tipo, executada.

### `TargetFramework`

Exemplo:

```xml
<TargetFramework>net10.0</TargetFramework>
```

Define a versão do .NET utilizada como alvo da compilação.

### `ImplicitUsings`

```xml
<ImplicitUsings>enable</ImplicitUsings>
```

Adiciona automaticamente namespaces comuns.

### Nullable Reference Types

```xml
<Nullable>enable</Nullable>
```

Permite que o compilador diferencie referências que aceitam ou não `null`.

Exemplo:

```csharp
string name = "Guilherme";
string? optionalDescription = null;
```

### Encapsulamento de regras

A entidade é responsável por garantir que seu estado seja válido desde a criação.

Em vez de permitir dados inválidos e corrigi-los posteriormente, o construtor aplica validações e normalizações imediatamente.

---

## Critério de conclusão do Dia 1

O Dia 1 está concluído quando os comandos abaixo executarem sem erros:

```powershell
dotnet build
dotnet test
dotnet run --project src\DeveloperControl.Console
```

Também é necessário conseguir explicar:

- a diferença entre solution e projeto;
- o papel do arquivo `.csproj`;
- a direção das referências;
- por que o Domain deve ser independente;
- por que o título é validado dentro da entidade;
- por que descrição vazia é normalizada para `null`;
- o que os testes unitários estão protegendo.

---

## Próximo passo

No Dia 2, a entidade poderá evoluir para possuir estado e comportamento, incluindo:

- status da tarefa;
- início da tarefa;
- conclusão da tarefa;
- validação de transições inválidas;
- novos testes unitários para as regras de estado.