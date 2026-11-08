using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    /// <summary>Uma lista nula de IDs não filtra. Uma lista vazia não encontra jogos.</summary>
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyList<int>? idsJogos = null,
        bool incluirInativos = false,
        CancellationToken cancelamento = default);

    Task<Jogo?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default);
    Task<int> InserirAsync(Jogo jogo, CancellationToken cancelamento = default);
    Task AlterarAsync(Jogo jogo, CancellationToken cancelamento = default);
    Task DesativarAsync(int id, CancellationToken cancelamento = default);
}
