using GeradorDeTestes2024.Dominio.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;
using GeradorDeTestes2024.Dominio.ModuloTeste;

namespace GeradorDeTestes2024.Dominio.ModuloDisciplina
{
    public class Disciplina : EntidadeBase
    {
        public string Nome { get; set; }
        public List<Materia> Materias { get; set; }
        public List<Teste> Testes { get; set; }

        public Disciplina()
        {

        }
        public Disciplina(string nome)
        {
            Nome = nome;
            Materias = new List<Materia>();
            Testes = new List<Teste>();
        }
        public Disciplina(int id, string nome)
        {
            Id = id;
            Nome = nome;
            Materias = new List<Materia>();
            Testes = new List<Teste>();
        }

        public override void AtualizarRegistro(EntidadeBase novoRegistro)
        {
            Disciplina d = (Disciplina)novoRegistro;

            Nome = d.Nome;
            Materias = d.Materias;
            Testes = d.Testes;
        }

        public override List<string> Validar()
        {
            List<string> erros = new List<string>();

            if (string.IsNullOrEmpty(Nome.Trim()))
                erros.Add("O campo \"nome\" é obrigatório");

            return erros;
        }

        public bool ExisteDisciplina(List<Disciplina> disciplinas)
        {
            foreach (Disciplina d in disciplinas)
                if (d.Nome == Nome)
                    return true;
            return false;
        }

        public override string ToString()
        {
            return $"{Nome}";
        }

        public decimal QuantidadeQuestoes(List<Questao> questoes, string serie)
        {
            int contador = 0;

            foreach (Questao q in questoes)
            {
                if (q.Materia.Disciplina.Id == Id && q.Materia.Serie == serie)
                    contador++;
            }

            return contador;
        }

        public List<Questao> ListaQuestoes(List<Questao> questoes, string serie)
        {
            List<Questao> lista = new List<Questao>();

            foreach (Questao q in questoes)
            {
                if (q.Materia.Disciplina.Id == Id && q.Materia.Serie == serie)
                    lista.Add(q);
            }
            return lista;
        }
    }
}