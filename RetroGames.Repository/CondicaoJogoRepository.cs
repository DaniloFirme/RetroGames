using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<List<CondicaoJogo>> ListarAsync(CancellationToken cancelamento = default)
    {
        const string sql = """
            SELECT co_condicao_jogo, ds_condicao_jogo
            FROM tb_condicao_jogo
            ORDER BY co_condicao_jogo
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);

        var condicoes = new List<CondicaoJogo>();
        while (await leitor.ReadAsync(cancelamento))
        {
            condicoes.Add(new CondicaoJogo
            {
                Id = leitor.GetInt32("co_condicao_jogo"),
                Descricao = leitor.GetString("ds_condicao_jogo"),
            });
        }

        return condicoes;
    }
}
