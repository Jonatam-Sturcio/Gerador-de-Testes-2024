using GeradorDeTestes2024.Dominio.ModuloMateria;
using GeradorDeTestes2024.Dominio.ModuloQuestao;
using Microsoft.Data.SqlClient;

namespace GeradorDeTestes2024.ModuloQuestao
{
    public class RepositorioQuestaoEmSql : IRepositorioQuestao
    {
        private string enderecoBanco;

        public RepositorioQuestaoEmSql()
        {
            enderecoBanco = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=GeradorTesteDB;Integrated Security=True;Pooling=False";
        }

        public void Cadastrar(Questao novaQuestao)
        {
            string sqlInserir =
                @"INSERT INTO [TBQuestao]
                    (
                        [Enunciado],
                        [Materia_Id]
                    )
                    VALUES
                    (
                        @Enunciado, 
                        @Materia_Id
                    ); SELECT SCOPE_IDENTITY();";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosQuestao(novaQuestao, comandoInsercao);

            conexaoComBanco.Open();

            object id = comandoInsercao.ExecuteScalar();

            novaQuestao.Id = Convert.ToInt32(id);

            conexaoComBanco.Close();

            CadastrarAlternativasNoBanco(novaQuestao);
        }

        public bool Editar(int id, Questao questaoEditado)
        {
            string sqlEditar =
                @"UPDATE [TBQuestao]	
		            SET
			            [Enunciado] = @Enunciado,
                        [Materia_Id] = @Materia_Id
		            WHERE
			            [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            questaoEditado.Id = id;

            ConfigurarParametrosQuestao(questaoEditado, comandoEdicao);

            conexaoComBanco.Open();

            int numeroRegistrosAfetados = comandoEdicao.ExecuteNonQuery();

            conexaoComBanco.Close();

            if (numeroRegistrosAfetados < 1)
                return false;

            AtualizarAlternativas(questaoEditado);

            return true;
        }

        private void AtualizarAlternativas(Questao questaoEditado)
        {
            string sqlExcluir =
                 @"DELETE FROM [TBAlternativa]
		            WHERE
			            [Questao_Id] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", questaoEditado.Id);

            conexaoComBanco.Open();

            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            conexaoComBanco.Close();

            CadastrarAlternativasNoBanco(questaoEditado);
        }

        public bool Excluir(int id)
        {
            string sqlExcluir =
                @"DELETE FROM [TBQuestao]
		            WHERE
			            [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", id);

            conexaoComBanco.Open();

            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            conexaoComBanco.Close();

            if (numeroRegistrosExcluidos < 1)
                return false;

            return true;
        }

        public Questao SelecionarPorId(int idSelecionado)
        {
            string sqlSelecionarPorId =
                 @"SELECT 
		            QT.[ID], 
		            QT.[Enunciado],
                    MT.[Nome],
                    MT.[SERIE]
	            FROM 
		            [TBQuestao] AS QT LEFT JOIN
                    [TBMateria] AS MT
                ON
                    MT.ID = QT.Materia_id
                WHERE
                    QT.[ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", idSelecionado);

            conexaoComBanco.Open();

            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            Questao questao = null;

            if (leitor.Read())
            {
                questao = ConverterParaQuestao(leitor);
                CarregarAlternativas(questao);
            }

            conexaoComBanco.Close();

            return questao;
        }

        public List<Questao> SelecionarTodos()
        {
            string sqlSelecionarTodos =
                @"SELECT 
		            QT.[ID], 
		            QT.[Enunciado],
                    MT.[Nome],
                    MT.[SERIE]
	            FROM 
		            [TBQuestao] AS QT LEFT JOIN
                    [TBMateria] AS MT
                ON
                    MT.ID = QT.Materia_id";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();

            SqlDataReader leitorQuestao = comandoSelecao.ExecuteReader();

            List<Questao> questoes = new List<Questao>();

            while (leitorQuestao.Read())
            {
                Questao questao = ConverterParaQuestao(leitorQuestao);

                CarregarAlternativas(questao);

                questoes.Add(questao);
            }

            conexaoComBanco.Close();

            return questoes;
        }

        private void CarregarAlternativas(Questao questao)
        {
            string sqlSelecionarTodos =
                @"SELECT 
		            [ID], 
		            [Descricao],
                    [Correta]
	            FROM 
		            [TBAlternativa]
                WHERE
                    Questao_ID = @ID_Questao";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();

            comandoSelecao.Parameters.AddWithValue("ID_Questao", questao.Id);

            SqlDataReader leitorQuestao = comandoSelecao.ExecuteReader();

            List<Alternativa> alternativas = new List<Alternativa>();

            while (leitorQuestao.Read())
            {
                Alternativa alt = ConverterParaAlternativa(leitorQuestao);

                alternativas.Add(alt);
            }

            conexaoComBanco.Close();

            questao.Alternativas = alternativas;
        }

        private Questao ConverterParaQuestao(SqlDataReader leitor)
        {
            Questao questao = new Questao()
            {
                Id = Convert.ToInt32(leitor["ID"]),
                Enunciado = Convert.ToString(leitor["Enunciado"])
            };

            questao.Materia = ConverterParaMateria(leitor);

            return questao;
        }

        private Materia ConverterParaMateria(SqlDataReader leitor)
        {
            Materia materia = new Materia()
            {
                Nome = Convert.ToString(leitor["Nome"]),
                Serie = Convert.ToString(leitor["Serie"])
            };

            return materia;
        }

        private Alternativa ConverterParaAlternativa(SqlDataReader leitor)
        {
            Alternativa alternativa = new Alternativa()
            {
                Descricao = Convert.ToString(leitor["Descricao"]),
                Correta = Convert.ToBoolean(leitor["Correta"])
            };

            return alternativa;
        }

        private void ConfigurarParametrosQuestao(Questao questao, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("ID", questao.Id);
            comando.Parameters.AddWithValue("Enunciado", questao.Enunciado);
            comando.Parameters.AddWithValue("Materia_Id", questao.Materia.Id);
        }

        private void CadastrarAlternativasNoBanco(Questao questao)
        {
            string sqlInserir =
                @"INSERT INTO [TBAlternativa]
                    (
                        [Descricao],
                        [Correta],
                        [Questao_Id]
                    )
                    VALUES
                    (
                        @Descricao, 
                        @Correta, 
                        @Questao_Id
                    ); SELECT SCOPE_IDENTITY();";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            conexaoComBanco.Open();

            foreach (Alternativa alt in questao.Alternativas)
            {
                ConfigurarParametrosAlternativa(questao.Id, alt, comandoInsercao);
                comandoInsercao.ExecuteNonQuery();
            }

            conexaoComBanco.Close();
        }

        private void ConfigurarParametrosAlternativa(int questaoID, Alternativa alternativa, SqlCommand comandoInsercao)
        {
            comandoInsercao.Parameters.Clear();
            comandoInsercao.Parameters.AddWithValue("Questao_Id", questaoID);
            comandoInsercao.Parameters.AddWithValue("Descricao", alternativa.Descricao);
            comandoInsercao.Parameters.AddWithValue("Correta", alternativa.Correta);
        }
    }

}
