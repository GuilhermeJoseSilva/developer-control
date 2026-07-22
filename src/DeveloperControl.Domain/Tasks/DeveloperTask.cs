using System;
using System.Collections.Generic;
using System.Text;

namespace DeveloperControl.Domain.Tasks
{
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
        public DeveloperTask(string title, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("O título da tarefa é obrigatório", nameof(title));
            }
            Id = Guid.NewGuid();
            Title = title.Trim();
            CreatedAt = DateTimeOffset.UtcNow;
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            Status = DeveloperTaskStatus.Pending;
        }
        public void Start()
        {
            if(Status is not DeveloperTaskStatus.Pending)
            {
                throw new InvalidOperationException("Somente tarefas pendentes podem ser iniciadas.");
            }

            Status = DeveloperTaskStatus.InProgress;
            StartedAt = DateTimeOffset.UtcNow;
        }
        public void Complete()
        {
            if(Status is not DeveloperTaskStatus.InProgress)
            {
                throw new InvalidOperationException("Somente tarefas em andamento podem ser concluídas");
            }

            Status = DeveloperTaskStatus.Completed;
            CompletedAt = DateTimeOffset.UtcNow;
        }
        public void Cancel()
        {
            if (Status is  DeveloperTaskStatus.Completed)
            {
                throw new InvalidOperationException("Uma tarefa concluída não poded ser cancelada");
            }
            if(Status is DeveloperTaskStatus.Cancelled)
            {
                throw new InvalidOperationException("A tarefa já está cancelada");
            }
            Status = DeveloperTaskStatus.Cancelled;
            CancelledAt = DateTimeOffset.UtcNow;
        }
    }
}
