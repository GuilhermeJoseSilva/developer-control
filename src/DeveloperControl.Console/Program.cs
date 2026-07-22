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