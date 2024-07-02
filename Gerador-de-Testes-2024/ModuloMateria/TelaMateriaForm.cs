using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;

namespace GeradorDeTestes2024.ModuloMateria
{
    public partial class TelaMateriaForm : Form
    {
        private int id = -1;
        private List<Materia> materias;
        private Materia materia;
        public Materia Materia
        {
            set
            {
                id = value.Id;
                txtNomeMateria.Text = value.Nome;
                radioButtonSerie1.Checked = value.PrimeiraSerieMarcada();
                if (!radioButtonSerie1.Checked)
                    radioButtonSerie2.Checked = true;
                comboBoxMateriaDisciplina.SelectedItem = value.Disciplina;
            }
            get
            {
                return materia;
            }
        }

        public TelaMateriaForm(List<Disciplina> disciplinas, List<Materia> materias)
        {
            InitializeComponent();
            this.ConfigurarDialog();
            CarregarComboBox(disciplinas);
            this.materias = materias;
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            string nome = txtNomeMateria.Text.Trim();
            string serie = "";
            if (radioButtonSerie1.Checked)
                serie = "1ª Série";
            else if (radioButtonSerie2.Checked)
                serie = "2ª Série";

            Disciplina disciplina = (Disciplina)comboBoxMateriaDisciplina.SelectedItem;

            if (id != -1)
                materia = new Materia(id, nome, serie, disciplina);
            else
                materia = new Materia(nome, serie, disciplina);

            List<string> erros = materia.Validar();

            if (erros.Count > 0)
            {
                TelaPrincipalForm.Instancia.AtualizarRodape(erros[0]);

                DialogResult = DialogResult.None;
            }
            if (materia.ExisteMateria(materias))
            {
                TelaPrincipalForm.Instancia.AtualizarRodape($"Já existe uma \"Matéria\" com o nome de: \"{materia.Nome}\"!");

                DialogResult = DialogResult.None;
            }
        }

        private void CarregarComboBox(List<Disciplina> disciplinas)
        {
            foreach (Disciplina d in disciplinas)
                comboBoxMateriaDisciplina.Items.Add(d);

            comboBoxMateriaDisciplina.SelectedIndex = 0;
        }
    }
}
