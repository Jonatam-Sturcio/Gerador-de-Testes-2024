using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;

namespace GeradorDeTestes2024.ModuloQuestao
{
    public partial class TelaQuestaoForm : Form
    {
        private int id = -1;
        private List<Questao> questoes;
        private Alternativa alternativa;
        private Questao questao;
        public Questao Questao
        {
            get { return questao; }
            set
            {
                id = value.Id;
                cmbMateria.SelectedItem = value.Materia;
                txtEnunciado.Text = value.Enunciado;
                CarregarLista(value);
            }
        }

        private void CarregarLista(Questao questao)
        {
            int i = 0;
            foreach (Alternativa a in questao.Alternativas)
            {
                listAlternativas.Items.Add(a);
                if (a.Correta)
                    listAlternativas.SetItemChecked(i, true);
                i++;
            }
        }

        public TelaQuestaoForm(List<Questao> questoes, List<Materia> materias)
        {
            InitializeComponent();
            this.questoes = questoes;
            this.ConfigurarDialog();
            CarregarComboBox(materias);
        }

        private void CarregarComboBox(List<Materia> materias)
        {

            foreach (Materia m in materias)
            {
                cmbMateria.Items.Add(m);
            }
            cmbMateria.SelectedIndex = 0;
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            if (listAlternativas.Items.Count == 10)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape("Não é possível adicionar mais de 10 alternativas.");
                return;
            }
            string descricao = txtResposta.Text.Trim();

            if (string.IsNullOrEmpty(descricao))
            {
                TelaPrincipalForm.Instancia.AtualizarRodape("Não é possível adicionar uma alternativa vazia.");
                return;
            }
            alternativa = new Alternativa(descricao);
            if (ValidarAlternativaIgual(alternativa))
                return;
            listAlternativas.Items.Add(alternativa);
            RefatorarAlternativasDinamicamente();
        }

        private bool ValidarAlternativaIgual(Alternativa alternativa)
        {
            foreach (Alternativa a in listAlternativas.Items)
            {
                if (a.Descricao.Split(" ")[2].Equals(alternativa.Descricao))
                {
                    TelaPrincipalForm.Instancia.AtualizarRodape("Não é possível cadastrar a mesma alternativa");
                    return true;
                }
            }
            return false;
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (listAlternativas.SelectedIndex != -1)
            {
                listAlternativas.Items.RemoveAt(listAlternativas.SelectedIndex);
                RefatorarAlternativasDinamicamente();

            }

            else
                MessageBox.Show(
                    "Não é possível realizar esta ação sem uma alternativa selecionada.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            Materia materia = (Materia)cmbMateria.SelectedItem;
            string enunciado = txtEnunciado.Text.Trim();

            List<Alternativa> alternativas = new List<Alternativa>();

            foreach (Alternativa a in listAlternativas.Items)
            {
                a.LimparRespostaCorreta();
                if (listAlternativas.CheckedItems.Contains(a))
                {
                    a.Correta = true;
                }
                alternativas.Add(a);
            }

            if (id != -1)
                questao = new Questao(id, materia, enunciado, alternativas);
            else
                questao = new Questao(materia, enunciado, alternativas);

            List<string> erros = questao.Validar();
            if (erros.Count > 0)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape(erros[0]);
                DialogResult = DialogResult.None;
            }
            if (questao.EnunciadoIgual(questoes) && id == -1)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape("Não é possível cadastrar uma questão com o mesmo enunciado");
                DialogResult = DialogResult.None;
            }
        }
        private void RefatorarAlternativasDinamicamente()
        {
            int x = 0;
            foreach (Alternativa a in listAlternativas.Items)
            {
                a.RefatorarModeloAlternativa(x);
                x++;
            }
        }
    }
}
