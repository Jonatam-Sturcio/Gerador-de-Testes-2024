using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;
using GeradorDeTestes2024.Dominio.ModuloTeste;
using GeradorDeTestes2024.Infra.Arquivos.Compartilhado;
using GeradorDeTestes2024.Infra.Arquivos.ModuloTeste;
using GeradorDeTestes2024.Infra.Sql.ModuloDisciplina;
using GeradorDeTestes2024.ModuloDisciplina;
using GeradorDeTestes2024.ModuloMateria;
using GeradorDeTestes2024.ModuloQuestao;
using GeradorDeTestes2024.ModuloTeste;

namespace GeradorDeTestes2024
{
    public partial class TelaPrincipalForm : Form
    {
        ControladorBase controlador;
        ContextoDados contexto;
        IRepositorioDisciplina repositorioDisciplina;
        IRepositorioMateria repositorioMateria;
        IRepositorioQuestao repositorioQuestao;
        IRepositorioTeste repositorioTeste;

        public static TelaPrincipalForm Instancia { get; private set; }

        public TelaPrincipalForm()
        {
            InitializeComponent();
            contexto = new(carregarDados: true);

            lblTipoCadastro.Text = string.Empty;
            Instancia = this;

            repositorioDisciplina = new RepositorioDisciplinaEmSql();
            repositorioMateria = new RepositorioMateriaEmSql();
            repositorioQuestao = new RepositorioQuestaoEmSql();
            repositorioTeste = new RepositorioTeste(contexto);
        }

        public void AtualizarRodape(string texto)
        {
            statusLabelPrincipal.Text = texto;
        }

        private void DisciplinaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlador = new ControladorDisciplina(repositorioDisciplina, repositorioMateria, repositorioTeste);

            lblTipoCadastro.Text = "Cadastro de " + controlador.TipoCadastro;
            ConfigurarTelaPrincipal(controlador);
        }

        private void materiaToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            controlador = new ControladorMateria(repositorioMateria, repositorioDisciplina, repositorioQuestao);

            lblTipoCadastro.Text = "Cadastro de " + controlador.TipoCadastro;
            ConfigurarTelaPrincipal(controlador);
        }

        private void questoesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlador = new ControladorQuestao(repositorioQuestao, repositorioMateria, repositorioTeste);

            lblTipoCadastro.Text = "Cadastro de " + controlador.TipoCadastro;
            ConfigurarTelaPrincipal(controlador);
        }
        private void testesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlador = new ControladorTeste(repositorioTeste, repositorioDisciplina, repositorioQuestao, repositorioMateria);

            lblTipoCadastro.Text = "Cadastro de " + controlador.TipoCadastro;
            ConfigurarTelaPrincipal(controlador);
        }
        private void ConfigurarTelaPrincipal(ControladorBase controladorSelecionado)
        {
            lblTipoCadastro.Text = "Cadastro de " + controladorSelecionado.TipoCadastro;

            ConfigurarToolBox(controladorSelecionado);
            ConfigurarListagem(controladorSelecionado);
        }

        private void ConfigurarToolBox(ControladorBase controladorSelecionado)
        {
            btnAdicionar.Enabled = controladorSelecionado is ControladorBase;
            btnEditar.Enabled = controladorSelecionado is ControladorBase;
            btnExcluir.Enabled = controladorSelecionado is ControladorBase;

            btnDuplicar.Enabled = controladorSelecionado is IControladorDuplicavel;
            btnVisualizar.Enabled = controladorSelecionado is IControladorVisualizavel;
            btnPDF.Enabled = controladorSelecionado is IControladorPDF;

            ConfigurarToolTips(controladorSelecionado);
        }

        private void ConfigurarToolTips(ControladorBase controladorSelecionado)
        {
            btnAdicionar.ToolTipText = controladorSelecionado.ToolTipAdicionar;
            btnEditar.ToolTipText = controladorSelecionado.ToolTipEditar;
            btnExcluir.ToolTipText = controladorSelecionado.ToolTipExcluir;

            if (controladorSelecionado is IControladorVisualizavel controladorVisualizavel)
                btnVisualizar.ToolTipText = controladorVisualizavel.ToolTipVisualizar;

            if (controladorSelecionado is IControladorDuplicavel controladorDuplicavel)
                btnDuplicar.ToolTipText = controladorDuplicavel.ToolTipDuplicar;

            if (controladorSelecionado is IControladorPDF controladorPDF)
                btnPDF.ToolTipText = controladorPDF.ToolTipPDF;
        }

        private void ConfigurarListagem(ControladorBase controladorSelecionado)
        {
            UserControl listagem = controladorSelecionado.ObterListagem();
            listagem.Dock = DockStyle.Fill;

            pnlRegistros.Controls.Clear();
            pnlRegistros.Controls.Add(listagem);
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            controlador.Adicionar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            controlador.Editar();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            controlador.Excluir();
        }
        private void btnDuplicar_Click(object sender, EventArgs e)
        {
            if (controlador is IControladorDuplicavel controladorDuplicavel)
                controladorDuplicavel.Duplicar();
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            if (controlador is IControladorVisualizavel controladorVisualizavel)
                controladorVisualizavel.Visualizar();
        }
        private void btnPDF_Click(object sender, EventArgs e)
        {
            if (controlador is IControladorPDF controladorPDF)
                controladorPDF.GerarPDF();
        }
        private void TelaPrincipalForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


    }
}
