using DbTestBed.Helper.Cli.Databases;
using Microsoft.Extensions.CommandLineUtils;

namespace DbTestBed.Helper.Cli.Commands;

internal class Drop
{
	public static string Name = "drop";

	public static void AddCommand(CommandLineApplication command)
	{
		command.Description = "Drops all databases created for integration-testing purposes.";
		command.AddHelpOption();

		command.OnExecute(() =>
		{
			Console.WriteLine("Fetching db names ...");

			var repo = new Repository(Configuration.ConnectionString);
			var databases = repo.ListDatabases();
			var i = 0;

			foreach (var db in databases)
			{
				repo.Drop(db);

				Console.WriteLine($"Database [{db.Name}] droped.");

				i++;
			}

			if (i == 0)
			{
				Console.WriteLine("No database found");
			}
			else
			{
				Console.WriteLine($"Total of databases droped: {i}");
			}

			return 0;
		});
	}
}