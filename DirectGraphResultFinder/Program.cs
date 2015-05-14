using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace DirectGraphResultFinder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            var pipedInput = String.Empty;
            if (IsPipedInput())
            {
                while (Console.In.Peek() != -1)
                {
                    pipedInput = Console.In.ReadLine();
                }
            }
            var commandLineOptions = CommandLineParser.parseInputData(args);
            commandLineOptions.piped_input = pipedInput;
            var givenInput = String.IsNullOrEmpty(pipedInput) ? commandLineOptions .input_file?? String.Empty: pipedInput;
            if (commandLineOptions.show_help)
            {
                Console.WriteLine(commandLineOptions.help_message);
            }
            else if (commandLineOptions.output_to_console)
            {
                var output = ProcessData.exportInformation(givenInput);
                Console.WriteLine(output);
            }
            else if (commandLineOptions.output_to_file)
            {
                var output = ProcessData.exportInformation(givenInput);
                File.WriteAllText(commandLineOptions.output_file, output);
            }
            else if (commandLineOptions.show_form)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BasicIODisplay(givenInput));
            }
            return 0;
        }

        private static bool IsPipedInput()
        {
            try
            {
                bool isKey = Console.KeyAvailable;
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
