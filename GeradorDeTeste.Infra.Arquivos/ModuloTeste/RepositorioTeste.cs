using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloQuestao;
using GeradorDeTestes2024.Dominio.ModuloTeste;
using GeradorDeTestes2024.Infra.Arquivos.Compartilhado;

namespace GeradorDeTestes2024.Infra.Arquivos.ModuloTeste
{
    public class RepositorioTeste : RepositorioBaseEmArquivo<Teste>, IRepositorioTeste
    {
        public RepositorioTeste(ContextoDados contexto) : base(contexto)
        {
            if (contexto.Testes.Any())
                contadorId = contexto.Questoes.Max(i => i.Id) + 1;
        }

        public override bool Excluir(int id)
        {
            Teste teste = SelecionarPorId(id);

            List<Questao> questoes = new List<Questao>();

            foreach (Questao q in contexto.Questoes)
            {
                if (q.Testes.Find(t => t.Id == teste.Id) != null)
                    questoes.Add(q);
            }
            List<Teste> testes = new List<Teste>();
            foreach (Questao q in questoes)
            {
                foreach (Teste t in q.Testes)
                    if (t.Id != teste.Id)
                        testes.Add(t);
                q.Testes.Clear();
                q.Testes = testes;
            }


            testes = new List<Teste>();
            Disciplina disciplina = contexto.Disciplinas.Find(d => d.Id == teste.Disciplina.Id);
            foreach (Teste t in disciplina.Testes)
            {
                if (t.Id != teste.Id)
                    testes.Add(t);
            }
            disciplina.Testes.Clear();
            disciplina.Testes = testes;

            return base.Excluir(id);
        }
        protected override List<Teste> ObterRegistros()
        {
            return contexto.Testes;
        }
    }
}
