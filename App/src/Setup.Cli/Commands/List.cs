using ConsoleTables;
using DbTestBed.Helper.Cli.Databases;
using Microsoft.Extensions.CommandLineUtils;

namespace DbTestBed.Helper.Cli.Commands
{
	internal class List
	{
		private record class Database
		{
			public string Name { get; set; } = null!;

			public long Size { get; set; }

			public DateTime CreatedAt { get; set; }
		}

		public static string Name { get; } = "ls";

		public static void AddCommand(CommandLineApplication command)
		{
			command.Description = "Lists the databases created for integration-testing purposes.";
			command.AddHelpOption();

			command.OnExecute(() =>
			{
				var repo = new Repository(Configuration.ConnectionString);
				var databases = repo.ListDatabases();

				var table = new ConsoleTable("Name", "Created at", "Size (MB)");

				foreach (var db in databases)
				{
					table.AddRow(db.Name, db.CreatedAt.ToLongTimeString(), db.Size.ToString());
				}

				table.Write();

				return 0;
			});
		}
	}
}