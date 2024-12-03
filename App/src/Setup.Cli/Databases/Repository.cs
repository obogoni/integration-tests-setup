using System.Data.SqlClient;

namespace DbTestBed.Helper.Cli.Databases
{
	internal class Repository
	{
		private readonly string _connectionString;

		public Repository(string connectionString)
		{
			this._connectionString = connectionString;
		}

		public IEnumerable<Database> ListDatabases()
		{
			#region SQL

			const string SQL = @"

				SELECT
					d.[name],
					d.create_date,
					CAST(SUM(CAST(mf.size AS BIGINT) * 8 / 1024.0) AS DECIMAL(18,2)) AS size_mb
				FROM
					sys.databases d
					JOIN sys.master_files mf ON d.database_id = mf.database_id
				WHERE
					d.[name] LIKE '%_[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]'
							  + '[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]'
							  + '[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]'
							  + '[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]'
				GROUP BY
					d.name,
					d.create_date
				ORDER BY
					d.create_date DESC

			";

			#endregion SQL

			using var connection = new SqlConnection(_connectionString);

			connection.Open();

			var command = new SqlCommand(SQL, connection);

			var reader = command.ExecuteReader();

			while (reader.Read())
			{
				var db = new Database()
				{
					Name = reader.GetString(0),
					CreatedAt = reader.GetDateTime(1),
					Size = reader.GetDecimal(2),
				};

				yield return db;
			}

			connection.Close();
		}

		internal void Drop(Database db)
		{
			using var connection = new SqlConnection(_connectionString);

			connection.Open();

			var command = new SqlCommand($"DROP DATABASE [{db.Name}];", connection);

			command.ExecuteNonQuery();

			connection.Close();
		}
	}
}