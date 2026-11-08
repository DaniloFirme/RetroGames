using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = """
        co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
        qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
        """;

    public JogoRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyList<int>? idsJogos = null,
        bool incluirInativos = false,
        CancellationToken cancelamento = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pagina);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tamanhoPagina);

        if (idsJogos is { Count: 0 })
        {
            return ([], 0);
        }

        var filtro = """
            WHERE (@incluirInativos OR st_ativo = TRUE)
              AND (@busca IS NULL OR no_jogo LIKE @busca ESCAPE '!')
              AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
            """;

        if (idsJogos is not null)
        {
            // Só os nomes dos parâmetros compõem o SQL. Os IDs são vinculados separadamente.
            var parametros = Enumerable.Range(0, idsJogos.Count).Select(indice => $"@idJogo{indice}");
            filtro += $"\n  AND co_seq_jogo IN ({string.Join(", ", parametros)})";
        }

        var padraoBusca = busca is null
            ? null
            : $"%{busca.Replace("!", "!!").Replace("%", "!%").Replace("_", "!_")}%";
        var deslocamento = (pagina - 1L) * tamanhoPagina;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comandoContagem = CriarComando(conexao, $"SELECT COUNT(*) FROM tb_jogo\n{filtro}");
        AdicionarFiltros(comandoContagem, padraoBusca, idPlataforma, idsJogos, incluirInativos);
        var total = Convert.ToInt32(await comandoContagem.ExecuteScalarAsync(cancelamento));

        var sqlPagina = $"""
            SELECT {Colunas}
            FROM tb_jogo
            {filtro}
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @deslocamento
            """;

        await using var comandoPagina = CriarComando(conexao, sqlPagina);
        AdicionarFiltros(comandoPagina, padraoBusca, idPlataforma, idsJogos, incluirInativos);
        AdicionarParametro(comandoPagina, "@limite", tamanhoPagina);
        AdicionarParametro(comandoPagina, "@deslocamento", deslocamento);
        await using var leitor = await comandoPagina.ExecuteReaderAsync(cancelamento);

        var jogos = new List<Jogo>();
        while (await leitor.ReadAsync(cancelamento))
        {
            jogos.Add(MapearJogo(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default)
    {
        var sql = $"""
            SELECT {Colunas}
            FROM tb_jogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);

        return await leitor.ReadAsync(cancelamento) ? MapearJogo(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo, CancellationToken cancelamento = default)
    {
        const string sql = """
            INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
                                 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES (@nome, @descricao, @anoLancamento, @preco, @estoque,
                    @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo)
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarDadosJogo(comando, jogo);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);

        await ExecutarGravacaoAsync(comando, cancelamento);
        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo, CancellationToken cancelamento = default)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco, qt_estoque = @estoque,
                co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarDadosJogo(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando, cancelamento);
    }

    public async Task DesativarAsync(int id, CancellationToken cancelamento = default)
    {
        const string sql = "UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync(cancelamento);
    }

    private static void AdicionarFiltros(
        MySqlCommand comando,
        string? padraoBusca,
        int? idPlataforma,
        IReadOnlyList<int>? idsJogos,
        bool incluirInativos)
    {
        AdicionarParametro(comando, "@busca", padraoBusca);
        AdicionarParametro(comando, "@idPlataforma", idPlataforma);
        AdicionarParametro(comando, "@incluirInativos", incluirInativos);

        if (idsJogos is not null)
        {
            for (var indice = 0; indice < idsJogos.Count; indice++)
            {
                AdicionarParametro(comando, $"@idJogo{indice}", idsJogos[indice]);
            }
        }
    }

    private static void AdicionarDadosJogo(MySqlCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
    }

    private static Jogo MapearJogo(MySqlDataReader leitor)
    {
        var colunaDescricao = leitor.GetOrdinal("ds_jogo");
        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(colunaDescricao) ? null : leitor.GetString(colunaDescricao),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = DateTime.SpecifyKind(leitor.GetDateTime("dt_cadastro"), DateTimeKind.Utc),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo"),
        };
    }
}
