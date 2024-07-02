using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Infra.Arquivos.Compartilhado;

namespace GeradorDeTestes2024.Infra.Arquivos.ModuloDisciplina
{
    public class RepositorioDisciplina : RepositorioBaseEmArquivo<Disciplina>, IRepositorioDisciplina
    {
        public RepositorioDisciplina(ContextoDados contexto) : base(contexto)
        {
            if (contexto.Disciplinas.Any())
                contadorId = contexto.Disciplinas.Max(i => i.Id) + 1;
        }

        protected override List<Disciplina> ObterRegistros()
        {
            return contexto.Disciplinas;
        }
    }
}
