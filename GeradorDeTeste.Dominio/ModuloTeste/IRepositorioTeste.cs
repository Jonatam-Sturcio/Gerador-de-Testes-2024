namespace GeradorDeTestes2024.Dominio.ModuloTeste
{
    public interface IRepositorioTeste
    {
        void Cadastrar(Teste novoTeste);
        bool Editar(int id, Teste testeEditado);
        bool Excluir(int id);
        Teste SelecionarPorId(int id);
        List<Teste> SelecionarTodos();
    }
}
