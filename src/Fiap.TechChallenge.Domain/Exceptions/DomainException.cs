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

    /// <summary>
    /// Exceção lançada quando um serviço não é encontrado.
    /// </summary>
    public class ServicoNaoEncontradoException(Guid id) : DomainException($"Serviço com id '{id}' não encontrado.")
    {
    }

    /// <summary>
    /// Exceção lançada quando o nome do serviço já existe.
    /// </summary>
    public class ServicoNomeJaExisteException(string nome) : DomainException($"Já existe um serviço com o nome '{nome}'.")
    {
    }

    /// <summary>
    /// Exceção lançada quando um cliente não é encontrado.
    /// </summary>
    public class ClienteNaoEncontradoException(Guid id) : DomainException($"Cliente com id '{id}' não encontrado.")
    {
    }

    /// <summary>
    /// Exceção lançada quando o CPF/CNPJ já existe.
    /// </summary>
    public class ClienteCpfCnpjJaExisteException(string cpfCnpj) : DomainException($"Já existe um cliente com o CPF/CNPJ '{cpfCnpj}'.")
    {
    }

    /// <summary>
    /// Exceção lançada quando um veículo não é encontrado.
    /// </summary>
    public class VeiculoNaoEncontradoException(Guid id) : DomainException($"Veículo com id '{id}' não encontrado.")
    {
    }

    /// <summary>
    /// Exceção lançada quando a placa do veículo já existe.
    /// </summary>
    public class VeiculoPlacaJaExisteException(string placa) : DomainException($"Já existe um veículo com a placa '{placa}'.")
    {
    }

    /// <summary>
    /// Exceção lançada quando um status de ordem de serviço não é encontrado.
    /// </summary>
    public class StatusOrdemServicoNaoEncontradoException(string descricao) : DomainException($"Status de ordem de serviço '{descricao}' não encontrado.")
    {
    }

    /// <summary>
    /// Exceção lançada quando uma peça ou insumo não é encontrado.
    /// </summary>
    public class PecaInsumoNaoEncontradaException(Guid id) : DomainException($"Peça/Insumo com id '{id}' não encontrado.")
    {
    }

    /// <summary>
    /// Exceção lançada quando uma ordem de serviço não é encontrada.
    /// </summary>
    public class OrdemServicoNaoEncontradaException(Guid id) : DomainException($"Ordem de serviço com id '{id}' não encontrada.")
    {
    }
}
