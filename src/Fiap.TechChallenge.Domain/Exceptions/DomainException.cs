namespace Fiap.TechChallenge.Domain.Exceptions
{
    /// <summary>
    /// Exceção de domínio base para todas as exceptions de negócio.
    /// </summary>
    public abstract class DomainException(string message) : Exception(message)
    {
    }

    /// <summary>
    /// Exceção lançada quando um email já existe no sistema.
    /// </summary>
    public class EmailJaExisteException(string email) : DomainException($"O email '{email}' já está cadastrado no sistema.")
    {
    }

    /// <summary>
    /// Exceção lançada quando um usuário não é encontrado.
    /// </summary>
    public class UsuarioNaoEncontradoException : DomainException
    {
        public UsuarioNaoEncontradoException(string login) 
            : base($"Usuário com login '{login}' não encontrado.") { }
    }

    /// <summary>
    /// Exceção lançada quando as credenciais são inválidas.
    /// </summary>
    public class CredenciaisInvalidasException : DomainException
    {
        public CredenciaisInvalidasException() 
            : base("Usuário ou senha inválidos.") { }
    }

    /// <summary>
    /// Exceção lançada quando ocorre erro ao gerar token.
    /// </summary>
    public class ErroGerarTokenException : DomainException
    {
        public ErroGerarTokenException() 
            : base("Erro ao gerar token de autenticação.") { }
    }
}
