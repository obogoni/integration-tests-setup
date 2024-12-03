using DbTestBed.Helper.Cli;
using Microsoft.Extensions.CommandLineUtils;

var app = new CommandLineApplication();

app.Name = "ith";
app.Description = "This is the CLI tool to help manage databases created for integration tests";
app.AddHelpOption();
app.AddListCommand();
app.AddDropCommand();

app.OnExecute(() =>
{
	Console.WriteLine();
	Console.WriteLine(app.Description);
	Console.WriteLine(app.GetHelpText());

	return 0;
});

app.Execute(args);