using System.Text.Json;
using Fiap.TechChallenge.Api.Middlewares;
using Fiap.TechChallenge.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fiap.TechChallenge.Tests.Api.Middlewares
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private readonly DefaultHttpContext _context;

        public ExceptionHandlingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _context = new DefaultHttpContext();
            // Precisamos de um MemoryStream para ler o que o middleware escreve na resposta
            _context.Response.Body = new MemoryStream();
        }

        [Fact]
        public async Task InvokeAsync_QuandoDomainExceptionLancada_DeveRetornarBadRequest()
        {
            // Arrange
            var email = "teste@email.com";
            RequestDelegate next = (innerContext) => throw new EmailJaExisteException(email);
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            var responseBody = await GetResponseBody(_context);

            // Desserializa para garantir que o conteúdo é logicamente igual
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            data["message"].ToString().Should().Be($"O email '{email}' já está cadastrado no sistema.");
        }

        [Fact]
        public async Task InvokeAsync_QuandoExceptionGenericaLancada_DeveRetornarInternalServerError()
        {
            // Arrange
            RequestDelegate next = (innerContext) => throw new Exception("Erro fatal");
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            var responseBody = await GetResponseBody(_context);
            responseBody.Should().Contain("An unexpected internal error occurred.");

            // Verifica se o log de erro foi chamado
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        private static async Task<string> GetResponseBody(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            return await reader.ReadToEndAsync();
        }
    }
}