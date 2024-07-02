using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloTeste;

namespace GeradorDeTestes2024.ModuloTeste
{
    public partial class TabelaTesteControl : UserControl
    {
        public TabelaTesteControl()
        {
            InitializeComponent();
            grid.Columns.AddRange(ObterColunas());

            grid.ConfigurarGridSomenteLeitura();
            grid.ConfigurarGridZebrado();
        }
        public void AtualizarRegistros(List<Teste> Questoes)
        {
            string recuperacao = "Recuperação";
            grid.Rows.Clear();

            foreach (Teste Teste in Questoes)
            {
                grid.Rows.Add(Teste.Id, Teste.Titulo, Teste.Disciplina,
                    Teste.Recuperacao ? recuperacao : Teste.Materia,
                    Teste.RetornarQuantidadeQuestoes());
            }
        }

        public int ObterRegistroSelecionado()
        {
            return grid.SelecionarId();
        }
        private DataGridViewColumn[] ObterColunas()
        {
            return new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Titulo", HeaderText = "Título" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Disciplina", HeaderText = "Disciplina" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Materia", HeaderText = "Matéria" },
                new DataGridViewTextBoxColumn { DataPropertyName = "QtdQuestoes", HeaderText = "Quantidade de Questões" }
            };
        }
    }
}
