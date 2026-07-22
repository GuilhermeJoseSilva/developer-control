using DeveloperControl.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeveloperControl.Tests.Tasks
{
    public sealed class DeveloperTaskTests
    {
        [Fact]
        public void Constructor_ShouldCreateTask_WhenTitleIsValid()
        {
            var task = new DeveloperTask("Estudar estrutura de projetos");

            Assert.NotEqual(Guid.Empty, task.Id);
            Assert.Equal("Estudar estrutura de projetos", task.Title);
            Assert.NotEqual(default, task.CreatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenTitleIsInvalid(string invalidTitle)
        {
            var action = () => new DeveloperTask(invalidTitle);

            var exception = Assert.Throws<ArgumentException>(action);

            Assert.Equal("title", exception.ParamName);
        }

        [Fact]
        public void Constructor_ShouldRemoveWhitespace_FromTitle()
        {
            var task = new DeveloperTask(" Estudar C# ");
            Assert.Equal("Estudar C#", task.Title);
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
        [Fact]
        public void Constructor_ShouldCreatePendingTask()
        {
            var task = new DeveloperTask("Estudar encapsulamento");

            Assert.Equal(
                DeveloperTaskStatus.Pending,
                task.Status);

            Assert.Null(task.StartedAt);
            Assert.Null(task.CompletedAt);
            Assert.Null(task.CancelledAt);
        }
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
        [Fact]
        public void Start_ShouldThrow_WhenTaskIsAlreadyInProgress()
        {
            var task = new DeveloperTask("Estudar domínio");
            task.Start();

            var action = () => task.Start();

            Assert.Throws<InvalidOperationException>(action);
        }
        [Fact]
        public void Complete_ShouldChangeStatusToCompleted_WhenTaskIsInProgress()
        {
            var task = new DeveloperTask("Finalizar Dia 2");
            task.Start();

            task.Complete();

            Assert.Equal(
                DeveloperTaskStatus.Completed,
                task.Status);

            Assert.NotNull(task.CompletedAt);
        }
        [Fact]
        public void Complete_ShouldThrow_WhenTaskIsPending()
        {
            var task = new DeveloperTask("Estudar testes");

            var action = () => task.Complete();

            Assert.Throws<InvalidOperationException>(action);
        }
        [Fact]
        public void Cancel_ShouldChangeStatusToCancelled_WhenTaskIsPending()
        {
            var task = new DeveloperTask("Tarefa cancelável");

            task.Cancel();

            Assert.Equal(
                DeveloperTaskStatus.Cancelled,
                task.Status);

            Assert.NotNull(task.CancelledAt);
        }
        [Fact]
        public void Cancel_ShouldChangeStatusToCancelled_WhenTaskIsInProgress()
        {
            var task = new DeveloperTask("Tarefa em execução");
            task.Start();

            task.Cancel();

            Assert.Equal(
                DeveloperTaskStatus.Cancelled,
                task.Status);

            Assert.NotNull(task.CancelledAt);
        }
        [Fact]
        public void Cancel_ShouldThrow_WhenTaskIsCompleted()
        {
            var task = new DeveloperTask("Tarefa concluída");
            task.Start();
            task.Complete();

            var action = () => task.Cancel();

            Assert.Throws<InvalidOperationException>(action);
        }
        [Fact]
        public void Cancel_ShouldThrow_WhenTaskIsAlreadyCancelled()
        {
            var task = new DeveloperTask("Tarefa cancelada");
            task.Cancel();

            var action = () => task.Cancel();

            Assert.Throws<InvalidOperationException>(action);
        }
        [Fact]
        public void Complete_ShouldKeepTaskPending_WhenCompletionIsInvalid()
        {
            var task = new DeveloperTask("Tarefa pendente");

            var action = () => task.Complete();

            Assert.Throws<InvalidOperationException>(action);

            Assert.Equal(
                DeveloperTaskStatus.Pending,
                task.Status);

            Assert.Null(task.CompletedAt);
        }
    }
}
