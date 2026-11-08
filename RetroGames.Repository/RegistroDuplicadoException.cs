namespace RetroGames.Repository;

public sealed class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception excecaoInterna)
        : base("O banco de dados rejeitou um registro duplicado.", excecaoInterna)
    {
    }
}
