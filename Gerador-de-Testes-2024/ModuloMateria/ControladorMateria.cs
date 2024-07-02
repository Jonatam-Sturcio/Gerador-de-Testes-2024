using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;

namespace GeradorDeTestes2024.ModuloMateria
{
    internal class ControladorMateria : ControladorBase
    {
        private TabelaMateriaControl tabelaMateria;
        private IRepositorioMateria repositorioMateria;
        private IRepositorioDisciplina repositorioDisciplina;
        private IRepositorioQuestao repositorioQuestao;

        public ControladorMateria(IRepositorioMateria repositorioMateria, IRepositorioDisciplina repositorioDisciplina, IRepositorioQuestao repositorioQuestao)
        {
            this.repositorioMateria = repositorioMateria;
            this.repositorioDisciplina = repositorioDisciplina;
            this.repositorioQuestao = repositorioQuestao;
            AtualizarRodapeQuantidadeRegistros();
        }

        public override string TipoCadastro { get { return "Matéria"; } }

        public override string ToolTipAdicionar { get { return "Cadastrar uma nova matéria"; } }

        public override string ToolTipEditar { get { return "Editar uma matéria existente"; } }

        public override string ToolTipExcluir { get { return "Excluir uma matéria existente"; } }

        public override void Adicionar()
        {
            if (!ValidarDisciplinasExistentes())
            {
                TelaPrincipalForm.Instancia.AtualizarRodape($"Não é possível adicionar uma \"Matéria\" sem ter uma \"Disciplina\"!");
                return;
            }

            TelaMateriaForm telaMateria = new TelaMateriaForm(repositorioDisciplina.SelecionarTodos(), repositorioMateria.SelecionarTodos());

            DialogResult resultado = telaMateria.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            Materia novaMateria = telaMateria.Materia;

            repositorioMateria.Cadastrar(novaMateria);

            CarregarMaterias();

            TelaPrincipalForm.Instancia.AtualizarRodape($"O registro \"{novaMateria.Nome}\" foi criado com sucesso!");
        }


        public override void Editar()
        {
            if (!ValidarDisciplinasExistentes())
            {
                TelaPrincipalForm.Instancia.AtualizarRodape($"Não é possível editar uma \"Matéria\" sem ter uma disciplina!");
                return;
            }

            TelaMateriaForm telaMateria = new TelaMateriaForm(repositorioDisciplina.SelecionarTodos(), repositorioMateria.SelecionarTodos());

            int idSelecionado = tabelaMateria.ObterRegistroSelecionado();

            Materia materiaSelecionada = repositorioMateria.SelecionarPorId(idSelecionado);

            if (materiaSelecionada == null)
            {
                MessageBox.Show(
                    "Não é possível realizar esta ação sem um registro selecionado",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            telaMateria.Materia = materiaSelecionada;

            DialogResult resultado = telaMateria.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            Materia materiaEditada = telaMateria.Materia;

            repositorioMateria.Editar(materiaSelecionada.Id, materiaEditada);

            CarregarMaterias();

            TelaPrincipalForm.Instancia.AtualizarRodape($"O registro \"{materiaSelecionada.Nome}\" foi editado com sucesso!");
        }

        public override void Excluir()
        {
            int idSelecionado = tabelaMateria.ObterRegistroSelecionado();

            Materia materiaSelecionada = repositorioMateria.SelecionarPorId(idSelecionado);

            if (materiaSelecionada == null)
            {
                MessageBox.Show(
                     "Não é possível realizar esta ação sem um registro selecionado.",
                     "Aviso",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning
                     );
                return;
            }

            if (PossuiDependencias(materiaSelecionada))
                return;

            DialogResult resposta = MessageBox.Show(
                $"Você deseja realmente excluir o registro \"{materiaSelecionada.Nome}\" ",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
                );

            if (resposta != DialogResult.Yes)
                return;

            repositorioMateria.Excluir(materiaSelecionada.Id);

            CarregarMaterias();

            TelaPrincipalForm.Instancia.AtualizarRodape($"O registro \"{materiaSelecionada.Nome}\" foi excluido com sucesso!");
        }

        public override UserControl ObterListagem()
        {
            if (tabelaMateria == null)
                tabelaMateria = new TabelaMateriaControl();

            List<Materia> materias = repositorioMateria.SelecionarTodos();

            tabelaMateria.AtualizarRegistros(materias);

            return tabelaMateria;
        }
        private void CarregarMaterias()
        {
            List<Materia> materias = repositorioMateria.SelecionarTodos();

            tabelaMateria.AtualizarRegistros(materias);
        }
        private void AtualizarRodapeQuantidadeRegistros()
        {
            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {repositorioMateria.SelecionarTodos().Count} registro(s)...");
        }

        private bool ValidarDisciplinasExistentes()
        {
            return repositorioDisciplina.SelecionarTodos().Any();
        }

        private bool PossuiDependencias(Materia materia)
        {
            foreach (Questao q in repositorioQuestao.SelecionarTodos())
            {
                if (q.Materia.Id == materia.Id)
                {
                    TelaPrincipalForm.Instancia.AtualizarRodape($"Não é possível excluir a matéria: {materia.Nome}, pois possui questões associadas!");
                    return true;
                }
            }
            return false;
        }
    }
}
