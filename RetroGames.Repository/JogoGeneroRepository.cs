using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync(CancellationToken cancelamento = default)
    {
        const string sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        return await LerVinculosAsync(comando, cancelamento);
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        IReadOnlyList<int> idsJogos,
        CancellationToken cancelamento = default)
    {
        if (idsJogos.Count == 0)
        {
            return [];
        }

        var parametros = Enumerable.Range(0, idsJogos.Count).Select(indice => $"@idJogo{indice}");
        var sql = $"""
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", parametros)})
            ORDER BY co_jogo, co_genero
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        for (var indice = 0; indice < idsJogos.Count; indice++)
        {
            AdicionarParametro(comando, $"@idJogo{indice}", idsJogos[indice]);
        }

        return await LerVinculosAsync(comando, cancelamento);
    }

    public async Task InserirAsync(
        int idJogo,
        IReadOnlyList<int> idsGeneros,
        CancellationToken cancelamento = default)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        var linhas = Enumerable.Range(0, idsGeneros.Count).Select(indice => $"(@idJogo, @idGenero{indice})");
        var sql = $"""
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES {string.Join(", ", linhas)}
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);
        for (var indice = 0; indice < idsGeneros.Count; indice++)
        {
            AdicionarParametro(comando, $"@idGenero{indice}", idsGeneros[indice]);
        }

        await ExecutarGravacaoAsync(comando, cancelamento);
    }

    public async Task RemoverPorJogoAsync(int idJogo, CancellationToken cancelamento = default)
    {
        const string sql = "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo";

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync(cancelamento);
    }

    private static async Task<List<JogoGenero>> LerVinculosAsync(
        MySqlCommand comando,
        CancellationToken cancelamento)
    {
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync(cancelamento))
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero"),
            });
        }

        return vinculos;
    }
}
