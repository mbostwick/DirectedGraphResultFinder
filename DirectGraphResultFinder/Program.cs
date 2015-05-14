using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

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
            var listener = new ConsoleTraceListener();
            Trace.Listeners.Add(listener);

            var pipedInput = String.Empty;
            try
            {
                if (IsPipedInput())
                {
                    while (Console.In.Peek() != -1)
                    {
                        pipedInput = Console.In.ReadLine();
                    }
                }
            }
            catch { }
            
            var commandLineOptions = CommandLineParser.parseInputData(args);
            commandLineOptions.piped_input = pipedInput;
            var givenInput = String.IsNullOrEmpty(pipedInput) ? (commandLineOptions.input_file ?? String.Empty) : pipedInput;
            if (commandLineOptions.show_help)
            {
                writeMessage(commandLineOptions.help_message);
            }
            else if (commandLineOptions.output_to_console)
            {
                try
                {
                    var output = ProcessData.exportInformation(givenInput);
                    writeMessage(output);
                }catch
                {
                    writeMessage("Unknown error exporting groups!");
                }
                
            }
            else if (commandLineOptions.output_to_file)
            {
                try
                {
                    var output = ProcessData.exportInformation(givenInput);
                    File.WriteAllText(commandLineOptions.output_file, output);
                }
                catch(Exception ex)
                {
                    writeMessage("Unknown error exporting groups:" + ex.ToString()+ "!");
                }
                
            }
            else if (commandLineOptions.show_form)
            {
                HideConsole();
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

        private static void writeMessage(string messageToWrite)
        {
            Trace.WriteLine(messageToWrite);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);

        private static void HideConsole()
        {
            var ptr = GetStdHandle(StdHandle.Stdout);
            if (!CloseHandle(ptr))
                throw new Win32Exception();

            ptr = IntPtr.Zero;

            if (!FreeConsole())
                throw new Win32Exception();
        }
    }
}
