using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DirectGraphResultFinder
{
    public static class CommandLineParser
    {
        public static DirectGraphResultFinderCommandArguments parseInputData(string[] args)
        {
            var showHelp = false;
            var helpMessage = String.Empty;
            var inputFile = String.Empty;
            var outputFile = String.Empty;
            var p = new OptionSet() {
                { "i|input=", "the {INPUTFILE} to read",v => inputFile =v },
                { "o|output=", "the {OUTPUTFILE} to write to",v => outputFile =v },
                { "h|help",  "show this message and exit",v => showHelp = v != null },
            };
            try
            {
               p.Parse(args);
            }
            catch (OptionException)
            {
                showHelp = true;
            }

            if (showHelp)
            {
                using (var writer = new StringWriter())
                {
                    writer.Write("Start Up Options:" + Environment.NewLine);
                    p.WriteOptionDescriptions(writer);
                    helpMessage = writer.ToString();
                }
            }
            return new DirectGraphResultFinderCommandArguments(inputFile, outputFile, showHelp, helpMessage);
        }
    }
}
