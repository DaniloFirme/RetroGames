using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarTodosAsync(CancellationToken cancelamento = default);
    Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyList<int> idsJogos, CancellationToken cancelamento = default);
    Task InserirAsync(int idJogo, IReadOnlyList<int> idsGeneros, CancellationToken cancelamento = default);
    Task RemoverPorJogoAsync(int idJogo, CancellationToken cancelamento = default);
}
