namespace Fiap.TechChallenge.Domain.Interfaces
{
    /// <summary>
    /// Abstração para gerenciar transações de forma transparente.
    /// Permite que múltiplas operações sejam agrupadas em uma única transação.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Inicia um novo escopo de transação.
        /// Se algo falhar dentro do escopo, faz rollback automaticamente.
        /// Se Complete() for chamado, faz commit ao sair do using.
        /// </summary>
        ITransactionScope BeginScope();
    }

    /// <summary>
    /// Representa um escopo de transação que agrupa múltiplas operações.
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// Marca o escopo como completo e autoriza o commit.
        /// </summary>
        void Complete();
    }
}
