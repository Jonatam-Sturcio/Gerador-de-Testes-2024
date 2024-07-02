using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;

namespace GeradorDeTestes2024.ModuloDisciplina
{
    public partial class TelaDisciplinaForm : Form
    {
        private int id = -1;
        private Disciplina disciplina;
        private List<Disciplina> disciplinas;
        public Disciplina Disciplina
        {
            set
            {
                id = value.Id;
                txtNomeDisciplina.Text = value.Nome;
            }
            get
            {
                return disciplina;
            }
        }

        public TelaDisciplinaForm(List<Disciplina> disciplinas)
        {
            InitializeComponent();
            this.disciplinas = disciplinas;
            this.ConfigurarDialog();
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            string nome = txtNomeDisciplina.Text.Trim();

            if (id != -1)
                disciplina = new Disciplina(id, nome);
            else
                disciplina = new Disciplina(nome);


            List<string> erros = disciplina.Validar();

            if (erros.Count > 0)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape(erros[0]);

                DialogResult = DialogResult.None;
            }

            if (disciplina.ExisteDisciplina(disciplinas))
            {
                TelaPrincipalForm.Instancia.AtualizarRodape($"Já existe uma \"Disciplina\" com o nome de: \"{Disciplina.Nome}\"!");

                DialogResult = DialogResult.None;
            }
        }
    }
}