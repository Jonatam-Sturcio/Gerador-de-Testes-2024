using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Infra.Arquivos.Compartilhado;

namespace GeradorDeTestes2024.Infra.Arquivos.ModuloMateria
{
    public class RepositorioMateria : RepositorioBaseEmArquivo<Materia>, IRepositorioMateria
    {
        public RepositorioMateria(ContextoDados contexto) : base(contexto)
        {
            if (contexto.Materias.Any())
                contadorId = contexto.Materias.Max(i => i.Id) + 1;
        }

        protected override List<Materia> ObterRegistros()
        {
            return contexto.Materias;
        }
    }
}
