using GeradorDeTestes2024.Dominio.ModuloDisciplina;
using GeradorDeTestes2024.Dominio.ModuloMateria;
using Microsoft.Data.SqlClient;

namespace GeradorDeTestes2024.ModuloMateria
{
    public class RepositorioMateriaEmSql : IRepositorioMateria
    {
        private string enderecoBanco;

        public RepositorioMateriaEmSql()
        {
            enderecoBanco = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=GeradorTesteDB;Integrated Security=True;Pooling=False";
        }

        public void Cadastrar(Materia novaMateria)
        {
            string sqlInserir =
                @"INSERT INTO [TBMateria]
                    (
                        [NOME],
                        [Serie],
                        [Disciplina_Id]
                    )
                    VALUES
                    (
                        @NOME, 
                        @Serie, 
                        @Disciplina_Id
                    ); SELECT SCOPE_IDENTITY();";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosMateria(novaMateria, comandoInsercao);

            conexaoComBanco.Open();

            object id = comandoInsercao.ExecuteScalar();

            novaMateria.Id = Convert.ToInt32(id);

            conexaoComBanco.Close();
        }

        public bool Editar(int id, Materia materiaEditado)
        {
            string sqlEditar =
                @"UPDATE [TBMateria]	
		            SET
			            [NOME] = @Nome,
                        [Serie] = @Serie,
                        [Disciplina_Id] = @Disciplina_Id
		            WHERE
			            [ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            materiaEditado.Id = id;

            ConfigurarParametrosMateria(materiaEditado, comandoEdicao);

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
                @"DELETE FROM [TBMateria]
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

        public Materia SelecionarPorId(int idSelecionado)
        {
            string sqlSelecionarPorId =
                @"SELECT 
		            MT.[ID], 
		            MT.[NOME],
                    MT.[SERIE],
                    MT.[Disciplina_Id],
                    DS.[NOME]
	            FROM 
		            [TBMateria] AS MT LEFT JOIN
                    [TBDisciplina] AS DS
                ON
                    DS.ID = MT.Disciplina_id
                WHERE
                    MT.[ID] = @ID";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", idSelecionado);

            conexaoComBanco.Open();

            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            Materia materia = null;

            if (leitor.Read())
                materia = ConverterParaMateria(leitor);

            conexaoComBanco.Close();

            return materia;
        }

        public List<Materia> SelecionarTodos()
        {
            string sqlSelecionarTodos =
                @"SELECT 
		            MT.[ID], 
		            MT.[NOME],
                    MT.[SERIE],
                    MT.[Disciplina_Id],
                    DS.[NOME] AS Disciplina_Nome
	            FROM 
		            [TBMateria] AS MT LEFT JOIN
                    [TBDisciplina] AS DS
                ON
                    DS.ID = MT.Disciplina_id";

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao =
                new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();

            SqlDataReader leitorMateria = comandoSelecao.ExecuteReader();

            List<Materia> materias = new List<Materia>();

            while (leitorMateria.Read())
            {
                Materia materia = ConverterParaMateria(leitorMateria);

                materias.Add(materia);
            }

            conexaoComBanco.Close();

            return materias;
        }

        private Materia ConverterParaMateria(SqlDataReader leitor)
        {
            Materia materia = new Materia()
            {
                Id = Convert.ToInt32(leitor["ID"]),
                Nome = Convert.ToString(leitor["NOME"]),
                Serie = Convert.ToString(leitor["Serie"])
            };
            materia.Disciplina = ConverterParaDisciplina(leitor);

            return materia;
        }

        private Disciplina ConverterParaDisciplina(SqlDataReader leitor)
        {
            Disciplina disciplina = new Disciplina()
            {
                Id = Convert.ToInt32(leitor["Disciplina_id"]),
                Nome = Convert.ToString(leitor["Disciplina_Nome"]),
            };

            return disciplina;
        }

        private void ConfigurarParametrosMateria(Materia materia, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("ID", materia.Id);
            comando.Parameters.AddWithValue("NOME", materia.Nome);
            comando.Parameters.AddWithValue("Serie", materia.Serie);
            comando.Parameters.AddWithValue("Disciplina_Id", materia.Disciplina.Id);

        }

    }
}

