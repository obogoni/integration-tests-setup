using DbTestBed.Helper.Cli.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace DbTestBed.Helper.Cli
{
	internal static class Extensions
	{
		public static void AddListCommand(this CommandLineApplication app)
		{
			app.Command(List.Name, List.AddCommand);
		}

		public static void AddDropCommand(this CommandLineApplication app)
		{
			app.Command(Drop.Name, Drop.AddCommand);
		}

		public static void AddHelpOption(this CommandLineApplication app)
		{
			app.HelpOption("-?|-h|--help");
		}
	}
}