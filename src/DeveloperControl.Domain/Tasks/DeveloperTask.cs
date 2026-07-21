using System;
using System.Collections.Generic;
using System.Text;

namespace DeveloperControl.Domain.Tasks
{
    public sealed class DeveloperTask
    {
        public Guid Id { get; }
        public string Title { get; }
        public DateTimeOffset CreatedAt { get; }
        public string? Description { get; }
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
        }
    }
}
