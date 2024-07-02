using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using Microsoft.Data.SqlClient;

namespace GeradorDeTestes2024.Infra.Sql.ModuloDisciplina
{
    public class RepositorioDisciplinaEmSql : IRepositorioDisciplina
    {
        private string enderecoBanco;

        public RepositorioDisciplinaEmSql()
        {
            enderecoBanco = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=GeradorTesteDB;Integrated Security=True;Pooling=False";
        }

        public void Cadastrar(Disciplina novaDisciplina)
        {
            string sqlInserir =
                @"INSERT INTO [TBDISCIPLINA]
                    (
                        [NOME]
                    )
                    VALUES
                    (
                        @NOME
                    ); SELECT SCOPE_IDENTITY();";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosDisciplina(novaDisciplina, comandoInsercao);

            conexaoComBanco.Open();

            object id = comandoInsercao.ExecuteScalar();

            novaDisciplina.Id = Convert.ToInt32(id);

            conexaoComBanco.Close();
        }

        public bool Editar(int id, Disciplina contatoEditado)
        {
            string sqlEditar =
                @"UPDATE [TBDISCIPLINA]	
		            SET
			            [NOME] = @NOME
		            WHERE
			            [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            contatoEditado.Id = id;

            ConfigurarParametrosDisciplina(contatoEditado, comandoEdicao);

            conexaoComBanco.Open();

            int numeroRegistrosAfetados = comandoEdicao.ExecuteNonQuery();

            conexaoComBanco.Close();

            if (numeroRegistrosAfetados < 1)
                return false;

            return true;
        }

        public bool Excluir(int id)
        {
            string sqlExcluir =
                @"DELETE FROM [TBDISCIPLINA]
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

        public Disciplina SelecionarPorId(int idSelecionado)
        {
            string sqlSelecionarPorId =
                @"SELECT 
		            [ID], 
		            [NOME]
	            FROM 
		            [TBDISCIPLINA]
                WHERE
                    [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", idSelecionado);

            conexaoComBanco.Open();

            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            Disciplina contato = null;

            if (leitor.Read())
                contato = ConverterParaDisciplina(leitor);

            conexaoComBanco.Close();

            return contato;
        }

        public List<Disciplina> SelecionarTodos()
        {
            string sqlSelecionarTodos =
                @"SELECT 
		            [ID], 
		            [NOME]
	            FROM 
		            [TBDISCIPLINA]";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();

            SqlDataReader leitorDisciplina = comandoSelecao.ExecuteReader();

            List<Disciplina> contatos = new List<Disciplina>();

            while (leitorDisciplina.Read())
            {
                Disciplina contato = ConverterParaDisciplina(leitorDisciplina);

                contatos.Add(contato);
            }

            conexaoComBanco.Close();

            return contatos;
        }

        private Disciplina ConverterParaDisciplina(SqlDataReader leitor)
        {
            Disciplina contato = new Disciplina()
            {
                Id = Convert.ToInt32(leitor["ID"]),
                Nome = Convert.ToString(leitor["NOME"]),
            };

            return contato;
        }

        private void ConfigurarParametrosDisciplina(Disciplina disciplina, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("ID", disciplina.Id);
            comando.Parameters.AddWithValue("NOME", disciplina.Nome);
        }
    }
}

