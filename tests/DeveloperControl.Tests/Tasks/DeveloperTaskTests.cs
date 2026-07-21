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
    }
}
