using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;
using GeradorDeTestes2024.Dominio.ModuloTeste;

namespace GeradorDeTestes2024.ModuloTeste
{
    internal class ControladorTeste : ControladorBase, IControladorDuplicavel, IControladorVisualizavel, IControladorPDF
    {
        private IRepositorioTeste repositorioTeste;
        private TabelaTesteControl tabelaTeste;
        private IRepositorioDisciplina repositorioDisciplina;
        private IRepositorioQuestao repositorioQuestao;
        private IRepositorioMateria repositorioMateria;

        public ControladorTeste(IRepositorioTeste repositorioTeste, IRepositorioDisciplina repositorioDisciplina,
            IRepositorioQuestao repositorioQuestao, IRepositorioMateria repositorioMateria)
        {
            this.repositorioTeste = repositorioTeste;
            this.repositorioDisciplina = repositorioDisciplina;
            this.repositorioQuestao = repositorioQuestao;
            this.repositorioMateria = repositorioMateria;
            AtualizarRodapeQuantidadeRegistros();
        }

        public override string TipoCadastro => "Testes";

        public override string ToolTipAdicionar => "Cadastrar um novo teste";

        public override string ToolTipEditar => "Editar um teste existente";

        public override string ToolTipExcluir => "Excluir um teste existente";

        public string ToolTipDuplicar => "Duplicar o teste selecionado";

        public string ToolTipVisualizar => "Visualizar os detalhes do teste selecionado";

        public string ToolTipPDF => "Gerar arquivo PDF do teste selecionado";

        public override void Adicionar()
        {
            if (!ValidarExisteRegistrosSuficientes())
            {
                TelaPrincipalForm.Instancia.AtualizarRodape($"Não é possível realizar o cadastro de \"Teste\" " +
                    $"sem possuir uma \"Questão\" cadastradas!");
                return;
            }
            TelaTesteForm telaTeste = new TelaTesteForm(
                repositorioTeste.SelecionarTodos(),
                repositorioDisciplina.SelecionarTodos(),
                repositorioQuestao.SelecionarTodos(),
                repositorioMateria.SelecionarTodos(),
                false);

            DialogResult resultado = telaTeste.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            Teste novoTeste = telaTeste.Teste;

            repositorioTeste.Cadastrar(novoTeste);


            CarregarTestes();

            TelaPrincipalForm.Instancia.AtualizarRodape($"O registro \"{novoTeste.Titulo}\" foi criado com sucesso!");

        }
        public override void Editar()
        {
            Teste testeSelecionado = repositorioTeste.SelecionarPorId(tabelaTeste.ObterRegistroSelecionado());

            TelaTesteForm telaTeste = new TelaTesteForm(
                repositorioTeste.SelecionarTodos(),
                repositorioDisciplina.SelecionarTodos(),
                repositorioQuestao.SelecionarTodos(),
                repositorioMateria.SelecionarTodos(),
                false);

            if (testeSelecionado == null)
            {
                MessageBox.Show(
                    "Não é possível realizar esta ação sem um registro selecionado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            telaTeste.Teste = testeSelecionado;

            DialogResult resultado = telaTeste.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            Teste TesteEditado = telaTeste.Teste;


            repositorioTeste.Editar(testeSelecionado.Id, TesteEditado);
            CarregarTestes();

            TelaPrincipalForm
                .Instancia
                .AtualizarRodape($"O registro \"{TesteEditado.Titulo}\" foi editado com sucesso!");
        }
        public override void Excluir()
        {
            Teste testeSelecionado = repositorioTeste.SelecionarPorId(tabelaTeste.ObterRegistroSelecionado());
            if (testeSelecionado == null)
            {
                MessageBox.Show(
                    "Não é possível realizar esta ação sem um registro selecionado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult resposta = MessageBox.Show($"Você deseja realmente excluir o registro \"{testeSelecionado.Titulo}\"?"
                , "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resposta != DialogResult.Yes)
                return;

            repositorioTeste.Excluir(testeSelecionado.Id);

            CarregarTestes();

            TelaPrincipalForm.Instancia.AtualizarRodape($"O registro \"{testeSelecionado.Titulo}\" foi excluído com sucesso!");
        }
        public void Duplicar()
        {
            Teste testeSelecionado = repositorioTeste.SelecionarPorId(tabelaTeste.ObterRegistroSelecionado());
            if (testeSelecionado == null)
            {
                MessageBox.Show(
                    "Não é possível realizar esta ação sem um registro selecionado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            TelaTesteForm telaTeste = new TelaTesteForm(
                repositorioTeste.SelecionarTodos(),
                repositorioDisciplina.SelecionarTodos(),
                repositorioQuestao.SelecionarTodos(),
                repositorioMateria.SelecionarTodos(),
                true);

            telaTeste.Teste = testeSelecionado;

            DialogResult resultado = telaTeste.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            Teste novoTeste = telaTeste.Teste;

            repositorioTeste.Cadastrar(novoTeste);

            CarregarTestes();

            TelaPrincipalForm.Instancia.AtualizarRodape($"O registro \"{novoTeste.Titulo}\" foi criado com sucesso!");

        }
        public void Visualizar()
        {
            Teste testeSelecionado = repositorioTeste.SelecionarPorId(tabelaTeste.ObterRegistroSelecionado());
            if (testeSelecionado == null)
            {
                MessageBox.Show(
                    "Não é possível realizar esta ação sem um registro selecionado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            TelaVisualizarTesteForm telaVisualisar = new TelaVisualizarTesteForm(
                testeSelecionado,
                repositorioDisciplina.SelecionarTodos(),
                repositorioMateria.SelecionarTodos(),
                repositorioQuestao.SelecionarTodos());

            telaVisualisar.ShowDialog();

        }
        public void GerarPDF()
        {
            Teste testeSelecionado = repositorioTeste.SelecionarPorId(tabelaTeste.ObterRegistroSelecionado());
            if (testeSelecionado == null)
            {
                MessageBox.Show(
                    "Não é possível realizar esta ação sem um registro selecionado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            TelaGerarPdfForm telaGerarPdf = new TelaGerarPdfForm(testeSelecionado);

            DialogResult resultado = telaGerarPdf.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            string caminho = telaGerarPdf.Caminho;
            bool gabarito = telaGerarPdf.Gabarito;

            GerarArquivoPdf gerarArquivoPDF = new GerarArquivoPdf(
                  testeSelecionado,
                repositorioDisciplina.SelecionarTodos(),
                repositorioMateria.SelecionarTodos(),
                repositorioQuestao.SelecionarTodos());

            if (gerarArquivoPDF.GerarPdf(caminho, gabarito))
                TelaPrincipalForm.Instancia.AtualizarRodape($"A prova: \"{testeSelecionado.Titulo}\" foi gerado com sucesso na pasta: {caminho}.");
            else
                TelaPrincipalForm.Instancia.AtualizarRodape("Não foi possível gerar o arquivo PDF!");

        }
        public override UserControl ObterListagem()
        {
            if (tabelaTeste == null)
                tabelaTeste = new TabelaTesteControl();

            List<Teste> questoes = repositorioTeste.SelecionarTodos();

            tabelaTeste.AtualizarRegistros(questoes);

            return tabelaTeste;
        }
        private void CarregarTestes()
        {
            List<Teste> Testes = repositorioTeste.SelecionarTodos();

            tabelaTeste.AtualizarRegistros(Testes);
        }
        private void AtualizarRodapeQuantidadeRegistros()
        {
            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {repositorioTeste.SelecionarTodos().Count} registro(s)...");
        }
        private bool ValidarExisteRegistrosSuficientes()
        {
            if (repositorioQuestao.SelecionarTodos().Any())
                return true;
            return false;
        }
    }
}
