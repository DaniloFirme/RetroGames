using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig configuracao;

    protected RepositoryBase(RepositoryConfig configuracao)
    {
        this.configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
    }

    /// <summary>Abre uma conexão. O chamador deve descartá-la com await using.</summary>
    protected async Task<MySqlConnection> AbrirConexaoAsync(CancellationToken cancelamento = default)
    {
        var conexao = new MySqlConnection(configuracao.ConnectionString);

        try
        {
            await conexao.OpenAsync(cancelamento);
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    /// <summary>Cria um comando. O chamador deve descartar o comando e seu eventual leitor com await using.</summary>
    protected static MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected static void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected static async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando,
        CancellationToken cancelamento = default)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync(cancelamento);
        }
        catch (MySqlException excecao) when (excecao.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new RegistroDuplicadoException(excecao);
        }
    }
}
