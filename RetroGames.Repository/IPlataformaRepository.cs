using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPlataformaRepository
{
    Task<List<Plataforma>> ListarAsync(CancellationToken cancelamento = default);
    Task<Plataforma?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default);
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null, CancellationToken cancelamento = default);
    Task<bool> PossuiJogosAsync(int id, CancellationToken cancelamento = default);
    Task<int> InserirAsync(Plataforma plataforma, CancellationToken cancelamento = default);
    Task AlterarAsync(Plataforma plataforma, CancellationToken cancelamento = default);
    Task ExcluirAsync(int id, CancellationToken cancelamento = default);
}
