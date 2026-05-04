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
            _context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseBody = await GetResponseBody(_context);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            data!["message"].ToString().Should().Be($"O email '{email}' já está cadastrado no sistema.");
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

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_QuandoArgumentExceptionLancada_DeveRetornarBadRequest()
        {
            // Arrange
            var mensagem = "Argumento invalido.";
            RequestDelegate next = (innerContext) => throw new ArgumentException(mensagem);
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseBody = await GetResponseBody(_context);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            data!["message"].ToString().Should().Contain(mensagem);
        }

        [Fact]
        public async Task InvokeAsync_QuandoInvalidOperationExceptionLancada_DeveRetornarBadRequest()
        {
            // Arrange
            var mensagem = "Operacao invalida.";
            RequestDelegate next = (innerContext) => throw new InvalidOperationException(mensagem);
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseBody = await GetResponseBody(_context);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            data!["message"].ToString().Should().Be(mensagem);
        }

        [Fact]
        public async Task InvokeAsync_QuandoUnauthorizedAccessExceptionLancada_DeveRetornarUnauthorized()
        {
            // Arrange
            var mensagem = "Acesso nao autorizado.";
            RequestDelegate next = (innerContext) => throw new UnauthorizedAccessException(mensagem);
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);

            var responseBody = await GetResponseBody(_context);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            data!["message"].ToString().Should().Be(mensagem);
        }

        [Fact]
        public async Task InvokeAsync_QuandoNenhumaExcecaoLancada_DeveInvocarNextEManterStatus200()
        {
            // Arrange
            bool nextInvocado = false;
            RequestDelegate next = (innerContext) =>
            {
                nextInvocado = true;
                return Task.CompletedTask;
            };
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            nextInvocado.Should().BeTrue();
            _context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        private static async Task<string> GetResponseBody(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            return await reader.ReadToEndAsync();
        }
    }
}