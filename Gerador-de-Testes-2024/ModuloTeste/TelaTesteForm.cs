using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;
using GeradorDeTestes2024.Dominio.ModuloTeste;

namespace GeradorDeTestes2024.ModuloTeste
{
    public partial class TelaTesteForm : Form
    {
        private bool duplicar;
        private int id = -1;
        private Disciplina disciplina;
        private Teste teste = new Teste();
        private List<Teste> testes;
        private List<Materia> materias;
        private List<Questao> todasAsQuestoes;
        private List<Questao> questoesDisponiveis;
        public Teste Teste
        {
            get { return teste; }
            set
            {
                id = value.Id;
                txtTitulo.Text = value.Titulo;

                if (duplicar)
                    txtTitulo.Text = value.Titulo + " - Cópia";

                cmbDisciplina.SelectedItem = value.Disciplina;

                checkBoxRecuperacao.Checked = value.Recuperacao;

                if (value.Serie.Contains("1"))
                    rdbPrimeiraSerie.Checked = true;
                else
                    rdbSegundaSerie.Checked = true;

                cmbMateria.SelectedItem = value.Materia;

                numQuestoes.Maximum = value.RetornarQuantidadeQuestoes();
                numQuestoes.Value = value.RetornarQuantidadeQuestoes();

                if (value.Recuperacao)
                {
                    cmbMateria.Enabled = false;
                    cmbMateria.SelectedIndex = -1;
                }
                if (!duplicar)
                    CarregarListaQuestoes(value.Questoes, todasAsQuestoes);
            }
        }
        public TelaTesteForm(List<Teste> testes, List<Disciplina> disciplinas, List<Questao> questoes, List<Materia> materias, bool habilitarDuplicacao)
        {
            InitializeComponent();
            ConfigurarTelaDuplicacao(habilitarDuplicacao);
            this.ConfigurarDialog();
            this.testes = testes;
            this.todasAsQuestoes = questoes;
            this.materias = materias;
            CarregarInformacoes(disciplinas);
        }

        private void cmbMateria_SelectedIndexChanged(object sender, EventArgs e)
        {
            listQuestoes.Items.Clear();
            if (cmbMateria.SelectedItem != null)
            {
                numQuestoes.Value = 0;
                Materia materiaSelecionada = (Materia)cmbMateria.SelectedItem;
                numQuestoes.Maximum = materiaSelecionada.QuantidadeQuestoes(todasAsQuestoes);
                questoesDisponiveis = materiaSelecionada.ListaQuestoes(todasAsQuestoes);
                HabilitarSerie(false);
            }
            else if (checkBoxRecuperacao.Checked)
            {
                HabilitarSerie(true);
                numQuestoes.Value = 0;
            }
        }
        private void cmbDisciplina_SelectedIndexChanged(object sender, EventArgs e)
        {
            listQuestoes.Items.Clear();
            SelecionarQuestoesDisciplina();
            if (!checkBoxRecuperacao.Checked)
            {
                disciplina = (Disciplina)cmbDisciplina.SelectedItem;
                cmbMateria.Items.Clear();
                foreach (Materia m in materias)
                {
                    if (m.Disciplina.Id == disciplina.Id)
                        cmbMateria.Items.Add(m);
                }
            }
        }
        private void HabilitarSerie(bool condicao)
        {
            rdbPrimeiraSerie.Enabled = condicao;
            rdbPrimeiraSerie.Checked = false;
            rdbSegundaSerie.Enabled = condicao;
            rdbSegundaSerie.Checked = false;
        }

        private void checkBoxRecuperacao_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRecuperacao.Checked)
            {
                HabilitarSerie(true);
                cmbMateria.SelectedIndex = -1;
                cmbMateria.Enabled = false;
                SelecionarQuestoesDisciplina();
            }
            else
            {
                HabilitarSerie(false);
                cmbMateria.Enabled = true;
                numQuestoes.Value = 0;
                numQuestoes.Maximum = 0;
            }
        }

        private void btnSortear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RetornarSerieString()) && cmbMateria.SelectedItem == null)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape("É necessário selecionar uma \"Série\" para sortear questões");
                return;
            }

            List<Questao> questoesSorteadas = new List<Questao>();

            questoesSorteadas = teste.SortearQuestoes(questoesDisponiveis, numQuestoes.Value);

            listQuestoes.Items.Clear();
            foreach (Questao q in questoesSorteadas)
            {
                listQuestoes.Items.Add(q);
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            string titulo = txtTitulo.Text.Trim();

            Disciplina disciplina = (Disciplina)cmbDisciplina.SelectedItem;

            List<Questao> questoesSelecionadas = new List<Questao>();

            foreach (Questao q in listQuestoes.Items)
            {
                questoesSelecionadas.Add(q);
            }
            string serie = RetornarSerieString();

            if (id != -1 && !duplicar)
                teste = new Teste(id, titulo, serie, disciplina, questoesSelecionadas);
            else
                teste = new Teste(titulo, serie, disciplina, questoesSelecionadas);

            if (checkBoxRecuperacao.Checked)
                teste.Recuperacao = true;

            if (!checkBoxRecuperacao.Checked && cmbMateria.SelectedItem != null)
            {
                teste.Materia = (Materia)cmbMateria.SelectedItem;
                teste.Serie = teste.Materia.Serie;
            }

            List<string> erros = teste.Validar();

            if (erros.Count > 0)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape(erros[0]);
                DialogResult = DialogResult.None;
            }

            if (teste.TituloTesteIgual(testes))
            {
                TelaPrincipalForm.Instancia.AtualizarRodape($"Já existe um \"Teste\" com o título de: \"{teste.Titulo}\"");
                DialogResult = DialogResult.None;
            }

        }
        private void CarregarInformacoes(List<Disciplina> disciplinas)
        {
            numQuestoes.Maximum = 0;
            foreach (Disciplina disciplina in disciplinas)
            {
                cmbDisciplina.Items.Add(disciplina);
            }
            cmbDisciplina.SelectedIndex = 0;
        }

        private void CarregarListaQuestoes(List<Questao> questoesDoTeste, List<Questao> questoes)
        {
            listQuestoes.Items.Clear();
            foreach (Questao quest in questoesDoTeste)
            {
                listQuestoes.Items.Add(questoes.Find(q => q.Id == quest.Id));
            }
        }

        private void ConfigurarTelaDuplicacao(bool habilitarDuplicacao)
        {
            if (habilitarDuplicacao)
            {
                duplicar = habilitarDuplicacao;
                this.Text = "Duplicação de Teste";
            }
        }
        private string RetornarSerieString()
        {
            if (rdbSegundaSerie.Checked)
                return "2ª Série";

            else if (rdbPrimeiraSerie.Checked)
                return "1ª Série";

            else
                return "";
        }

        private void SelecionarQuestoesDisciplina()
        {
            numQuestoes.Value = 0;
            Disciplina disciplinaSelecionada = (Disciplina)cmbDisciplina.SelectedItem;
            if ((rdbPrimeiraSerie.Checked || rdbSegundaSerie.Checked) && checkBoxRecuperacao.Checked)
            {
                numQuestoes.Maximum = disciplinaSelecionada.QuantidadeQuestoes(todasAsQuestoes, RetornarSerieString());
                questoesDisponiveis = disciplinaSelecionada.ListaQuestoes(todasAsQuestoes, RetornarSerieString());
            }

        }

        private void rdbPrimeiraSerie_CheckedChanged(object sender, EventArgs e)
        {
            SelecionarQuestoesDisciplina();
        }

        private void rdbSegundaSerie_CheckedChanged(object sender, EventArgs e)
        {
            SelecionarQuestoesDisciplina();
        }
    }
}

