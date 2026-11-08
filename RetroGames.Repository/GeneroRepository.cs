using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<List<Genero>> ListarAsync(CancellationToken cancelamento = default)
    {
        const string sql = """
            SELECT co_seq_genero, no_genero
            FROM tb_genero
            ORDER BY no_genero
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);

        var generos = new List<Genero>();
        while (await leitor.ReadAsync(cancelamento))
        {
            generos.Add(new Genero
            {
                Id = leitor.GetInt32("co_seq_genero"),
                Nome = leitor.GetString("no_genero"),
            });
        }

        return generos;
    }
}
