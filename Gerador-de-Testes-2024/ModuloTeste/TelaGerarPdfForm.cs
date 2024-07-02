using GeradorDeTestes2024.Compartilhado;
using GeradorDeTestes2024.Dominio.ModuloTeste;

namespace GeradorDeTestes2024.ModuloTeste
{
    public partial class TelaGerarPdfForm : Form
    {
        private string caminho = "";
        public string Caminho { get { return caminho; } }
        private bool gabarito;
        public bool Gabarito { get { return gabarito; } }

        public TelaGerarPdfForm(Teste testeSelecionado)
        {
            InitializeComponent();
            this.ConfigurarDialog();
            lblTeste.Text = testeSelecionado.Titulo;
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCaminho.Text))
            {
                TelaPrincipalForm.Instancia.AtualizarRodape("O campo \"Caminho\" é obrigatório");
                DialogResult = DialogResult.None;
                return;
            }
            caminho = txtCaminho.Text;
            gabarito = cbGabarito.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diretorioSaida = new FolderBrowserDialog();
            DialogResult resultado = diretorioSaida.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                string caminho = diretorioSaida.SelectedPath;
                txtCaminho.Text = caminho;
            }
        }
    }
}
