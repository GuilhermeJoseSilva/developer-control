using DeveloperControl.Domain.Tasks;

var task = new DeveloperTask("Estudar estrutura de soluções .NET");

Console.WriteLine($"Id: {task.Id}");
Console.WriteLine($"Titulo: {task.Title}");
Console.WriteLine($"Criada em UTC: {task.CreatedAt}");