using GeradorDeTestes2024.Dominio.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloTeste;

namespace GeradorDeTestes2024.Dominio.ModuloQuestao
{
    public class Questao : EntidadeBase
    {
        public Materia Materia { get; set; }
        public string Enunciado { get; set; }
        public List<Alternativa> Alternativas { get; set; }
        public List<Teste> Testes { get; set; }
        public Questao()
        {

        }
        public Questao(Materia materia, string enunciado, List<Alternativa> alternativas)
        {
            Materia = materia;
            Enunciado = enunciado;
            Alternativas = alternativas;
            Testes = new List<Teste>();
        }
        public Questao(int id, Materia materia, string enunciado, List<Alternativa> alternativas)
        {
            Id = id;
            Materia = materia;
            Enunciado = enunciado;
            Alternativas = alternativas;
            Testes = new List<Teste>();
        }

        public override void AtualizarRegistro(EntidadeBase novoRegistro)
        {
            Questao q = (Questao)novoRegistro;
            Materia = q.Materia;
            Enunciado = q.Enunciado;
            Alternativas = q.Alternativas;
        }

        public override List<string> Validar()
        {
            List<string> erros = new List<string>();

            if (string.IsNullOrEmpty(Enunciado.Trim()))
                erros.Add("O campo \"Enunciado\" é obrigatório");

            if (Alternativas.Count < 2)
                erros.Add("Deve haver ao menos duas alternativas.");

            if (Alternativas.Count > 4)
                erros.Add("Deve haver menos de cinco alternativas.");

            if (QuantidadeRespostaCorreta() == 0)
                erros.Add("Deve haver ao menos uma resposta correta entre as alternativas.");

            if (QuantidadeRespostaCorreta() > 1)
                erros.Add("Deve haver apenas uma resposta correta entre as alternativas.");

            return erros;
        }
        public Alternativa RetornarRespostaCorreta()
        {
            foreach (Alternativa a in Alternativas)
            {
                if (a.Correta)
                    return a;
            }
            return null;
        }
        public int QuantidadeRespostaCorreta()
        {
            int quantidade = 0;
            foreach (Alternativa a in Alternativas)
            {
                if (a.Correta)
                    quantidade++;
            }
            return quantidade;
        }
        public bool EnunciadoIgual(List<Questao> questoes)
        {
            foreach (Questao q in questoes)
            {
                if (q.Enunciado == Enunciado)
                    return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{Enunciado}";
        }
    }
}
