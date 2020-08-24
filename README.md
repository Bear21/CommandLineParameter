# Commandline Parameter Parser

Simple interface for using input from the commandline in a structured manor.


## Example Usage:

### First create a list of expected inputs with help
Create a **CommandLineParameter** object for each parameter you would like to take. For example if you want a shortname '-q' or longname '--quiet' you would create an object like `new CommandLineParameter("q", "quiet", "Don't output text and don't ask questions", false);` the false on the end is wether or not to expect a value. An example of taking a value would be `new CommandLineParameter("p", "path", "Path to input directory", true);` this will allow the user to input either `-p [dirpath]` or `--path [dirpath]`.
### Create an instance of CommandLineProcessor
for example: `CommandLineProcessor clp = new CommandLineProcessor();`
And use CommandLineProcessor.Add to add the CommandLineParameter objects to clp as an array. keep in mind that the order in which you add them will change the order they appear in help printout.
### Process
CommandLineProcessor.Process takes the `args` from `main(string[] args)` and if the input looks invalid will return false.

I often write my process code in an if statement checking for `help` start.
```C#
if (!clp.Process(args) || clp.HasParameter("help")) {
    Console.WriteLine("Error parsing commandline");
    clp.PrintHelp(Console.Out);
}
```
### Using the inputs
CommandLineProcessor provides two methods of extracting the data. **HasParameter(string)** and **GetValue(string)**. Both of these methods take a string with the longname of the parameter you're checking for. `HasParameter("quiet")` will work, where as `HasParameter("q")` will fail as there is no long name "q". There is a little extra typing in this approach however being explicit helps when changes occur. HasParameter will return true or false if the longname was entered on the commandline for instance if the user ran `./app --path /tmp` we can check for this with `if (clp.HasParameter("path"))`. The path the user entered can than be accepted with `clp.GetValue("path");`. GetValue will always return either a string or null. You must manually convert string to the value you were expecting if not a string.

### An example program for usage
```C#
internal class Program
{
    private static void Main(string[] args)
    {
        bool quiet = false;
        string path;

        CommandLineProcessor clp = new CommandLineProcessor();
        clp.Add(new CommandLineParameter[] {
            new CommandLineParameter("p", "path", "Path to input directory", true),
            new CommandLineParameter("q", "quiet", "Don't output text and don't ask questions", true),
            new CommandLineParameter("v", "version", "Displays the version of this application", false),
            new CommandLineParameter("h", "help", "Print this help", false) 
            });    
        if (!clp.Process(args) || clp.HasParameter("help")) {
           Console.WriteLine("Error parsing commandline");
           clp.PrintHelp(Console.Out);
        }
        if (clp.HasParameter("version") {
            Console.WriteLine("version 1.0.0");
        }
        if (clp.HasParameter("quiet") {
            quiet = true;
        }
        if (clp.HasParameter("path")) {
            path = clp.GetValue("path");
            if (!quiet) {
                Console.WriteLine($"User specified path: ${path}");
            }
        } else {
            if (!quiet) {
                Console.WriteLine($"User did not specify a path.");
            }
        }
    }
}
```

### Help
CommandLineProcessor.PrintHelp Can be used to put in the list of available parameters. The output looks like:
```
-p    --path                 Path to input directory
-q    --quiet                Don't output text and don't ask questions
-v    --version              Displays the version of this application
-h    --help                 Print this help
```