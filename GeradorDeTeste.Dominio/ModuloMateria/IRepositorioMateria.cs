namespace GeradorDeTestes2024.Dominio.ModuloMateria
{
    public interface IRepositorioMateria
    {
        void Cadastrar(Materia novaMateria);
        bool Editar(int id, Materia materiaEditada);
        bool Excluir(int id);
        Materia SelecionarPorId(int id);
        List<Materia> SelecionarTodos();
    }
}
