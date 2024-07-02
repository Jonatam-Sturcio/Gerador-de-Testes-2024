using GeradorDeTestes2024.Dominio.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;

namespace GeradorDeTestes2024.Dominio.ModuloTeste
{
    public class Teste : EntidadeBase
    {
        public string Titulo { get; set; }
        public Disciplina Disciplina { get; set; }
        public Materia Materia { get; set; }
        public string Serie { get; set; }
        public List<Questao> Questoes { get; set; }
        public bool Recuperacao { get; set; }
        public Teste()
        {

        }

        public Teste(string titulo, string serie, Disciplina disciplina, List<Questao> questoes)
        {
            Titulo = titulo;
            Serie = serie;
            Disciplina = disciplina;
            Questoes = questoes;
        }
        public Teste(int id, string titulo, string serie, Disciplina disciplina, List<Questao> questoes)
        {
            Id = id;
            Titulo = titulo;
            Serie = serie;
            Disciplina = disciplina;
            Questoes = questoes;
        }
        public int RetornarQuantidadeQuestoes()
        {
            return Questoes.Count;
        }
        public bool TituloTesteIgual(List<Teste> testes)
        {
            foreach (Teste t in testes)
            {
                if (t.Titulo == Titulo)
                    return true;
            }
            return false;
        }
        public override void AtualizarRegistro(EntidadeBase novoRegistro)
        {
            Teste teste = (Teste)novoRegistro;
            Titulo = teste.Titulo;
            Disciplina = teste.Disciplina;
            Materia = teste.Materia;
            Serie = teste.Serie;
            Questoes = teste.Questoes;
            Recuperacao = teste.Recuperacao;
        }

        public override List<string> Validar()
        {
            List<string> erros = new List<string>();

            if (string.IsNullOrEmpty(Titulo.Trim()))
                erros.Add("O campo \"Enunciado\" é obrigatório");

            if (string.IsNullOrEmpty(Serie))
                erros.Add("É necessário selecionar uma \"Série\"");

            if (Materia == null && !Recuperacao)
                erros.Add("É necessário selecionar uma \"Matéria\"");

            return erros;
        }

        public List<Questao> SortearQuestoes(List<Questao> questoesDisponiveis, decimal numeroQuestoes)
        {
            List<Questao> copiaQuestoes = new List<Questao>(questoesDisponiveis);

            List<Questao> questoesSorteadas = new List<Questao>();

            Random rand = new Random();

            while (questoesSorteadas.Count < numeroQuestoes)
            {
                int index = rand.Next(copiaQuestoes.Count);
                questoesSorteadas.Add(copiaQuestoes[index]);
                copiaQuestoes.RemoveAt(index);
            }

            return questoesSorteadas;
        }
    }
}

