# DeveloperControl — Semana 1, Dia 2

## Objetivo

Evoluir a entidade `DeveloperTask` para que ela não apenas armazene dados, mas também controle seu próprio estado e valide as transições permitidas.

O foco do dia foi:

- encapsulamento;
- estado interno;
- invariantes;
- transições de estado;
- testes unitários de comportamento;
- investigação de falhas encontradas pelos testes.

---

## Resumo do Dia 1

No Dia 1 foi criada a estrutura inicial da solução:

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

Também foi criada a entidade `DeveloperTask`, contendo identificador, título obrigatório, descrição opcional, data de criação, normalização de strings e testes unitários com xUnit.

> Uma entidade deve nascer válida e proteger seu próprio estado.

---

## Enum de status

Arquivo:

```text
src/DeveloperControl.Domain/Tasks/DeveloperTaskStatus.cs
```

```csharp
namespace DeveloperControl.Domain.Tasks;

public enum DeveloperTaskStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}
```

O enum limita os estados possíveis da tarefa e evita o uso de strings arbitrárias.

---

## Fluxo de estados

Fluxo principal:

```text
Pending → InProgress → Completed
```

Também são permitidas:

```text
Pending → Cancelled
InProgress → Cancelled
```

Transições inválidas:

```text
Pending → Completed
Completed → InProgress
Completed → Cancelled
Cancelled → Cancelled
```

---

## Entidade atualizada

Arquivo:

```text
src/DeveloperControl.Domain/Tasks/DeveloperTask.cs
```

```csharp
namespace DeveloperControl.Domain.Tasks;

public sealed class DeveloperTask
{
    public Guid Id { get; }
    public string Title { get; }
    public string? Description { get; }
    public DeveloperTaskStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }

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
        CreatedAt = DateTimeOffset.UtcNow;

        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        Status = DeveloperTaskStatus.Pending;
    }

    public void Start()
    {
        if (Status is not DeveloperTaskStatus.Pending)
        {
            throw new InvalidOperationException(
                "Somente tarefas pendentes podem ser iniciadas.");
        }

        Status = DeveloperTaskStatus.InProgress;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        if (Status is not DeveloperTaskStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Somente tarefas em andamento podem ser concluídas.");
        }

        Status = DeveloperTaskStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status is DeveloperTaskStatus.Completed)
        {
            throw new InvalidOperationException(
                "Uma tarefa concluída não pode ser cancelada.");
        }

        if (Status is DeveloperTaskStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "A tarefa já está cancelada.");
        }

        Status = DeveloperTaskStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
    }
}
```

---

## Encapsulamento

A propriedade `Status` possui `private set`:

```csharp
public DeveloperTaskStatus Status { get; private set; }
```

Outras partes da aplicação podem consultar o status, mas não alterá-lo diretamente.

A mudança de estado acontece por comportamentos da entidade:

```csharp
task.Start();
task.Complete();
task.Cancel();
```

---

## Invariantes implementadas

1. Toda tarefa nasce com status `Pending`.
2. Apenas tarefas pendentes podem ser iniciadas.
3. Apenas tarefas em andamento podem ser concluídas.
4. Uma tarefa concluída não pode ser cancelada.
5. Uma tarefa cancelada não pode ser cancelada novamente.
6. `StartedAt` só recebe valor ao iniciar.
7. `CompletedAt` só recebe valor ao concluir.
8. `CancelledAt` só recebe valor ao cancelar.

---

## Programa Console

```csharp
using DeveloperControl.Domain.Tasks;

var task = new DeveloperTask(
    "Estudar encapsulamento",
    "Semana 1 — Dia 2");

PrintTask(task);

task.Start();
PrintTask(task);

task.Complete();
PrintTask(task);

static void PrintTask(DeveloperTask task)
{
    Console.WriteLine("------------------------------");
    Console.WriteLine($"Id: {task.Id}");
    Console.WriteLine($"Título: {task.Title}");
    Console.WriteLine($"Descrição: {task.Description}");
    Console.WriteLine($"Status: {task.Status}");
    Console.WriteLine($"Criada em: {task.CreatedAt}");
    Console.WriteLine($"Iniciada em: {task.StartedAt}");
    Console.WriteLine($"Concluída em: {task.CompletedAt}");
    Console.WriteLine($"Cancelada em: {task.CancelledAt}");
}
```

Fluxo exibido:

```text
Pending
InProgress
Completed
```

---

## Testes adicionados

Foram criados testes para validar:

- estado inicial `Pending`;
- início de tarefa pendente;
- bloqueio de início duplicado;
- conclusão de tarefa em andamento;
- bloqueio de conclusão sem início;
- cancelamento de tarefa pendente;
- cancelamento de tarefa em andamento;
- bloqueio de cancelamento de tarefa concluída;
- bloqueio de cancelamento duplicado;
- preservação do estado após operação inválida.

Exemplo:

```csharp
[Fact]
public void Start_ShouldChangeStatusToInProgress_WhenTaskIsPending()
{
    var task = new DeveloperTask("Estudar invariantes");

    task.Start();

    Assert.Equal(
        DeveloperTaskStatus.InProgress,
        task.Status);

    Assert.NotNull(task.StartedAt);
}
```

---

## Falhas encontradas pelos testes

Ao executar `dotnet test`, sete testes falharam. Os testes revelaram duas condições lógicas invertidas.

### Erro no `Start`

Incorreto:

```csharp
if (Status == DeveloperTaskStatus.Pending)
```

Isso bloqueava justamente o estado válido.

Correto:

```csharp
if (Status is not DeveloperTaskStatus.Pending)
```

### Erro no `Cancel`

Incorreto:

```csharp
if (Status is not DeveloperTaskStatus.Completed)
```

Isso bloqueava qualquer estado que não fosse `Completed`.

Correto:

```csharp
if (Status is DeveloperTaskStatus.Completed)
```

---

## Aprendizado principal

O código compilava, mas o comportamento estava incorreto.

> Compilar não significa estar correto.

Os testes unitários localizaram as condições invertidas e protegeram as regras de domínio.

---

## Comandos de validação

```powershell
dotnet build
dotnet test
dotnet run --project src\DeveloperControl.Console
```

---

## Próximo passo

No Dia 3 serão estudados:

- interfaces;
- abstrações;
- inversão de dependência;
- serviços na camada Application;
- injeção de dependência manual.
