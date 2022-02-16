namespace Prospecto.Repositorio
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> BDLista(string NomStored, string[] storedParam, string[] cVariables);
        Task<int> BDAddAsync(string NomStored, string[] storedParam, string[] cVariables);
    }
}
