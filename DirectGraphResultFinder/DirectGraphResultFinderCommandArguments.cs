using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectGraphResultFinder
{
    public class DirectGraphResultFinderCommandArguments
    {
        public DirectGraphResultFinderCommandArguments() { }

        public DirectGraphResultFinderCommandArguments(string input_file, string output_file, bool show_help, string help_message)
        {
            this.input_file = input_file;
            this.output_file = output_file;
            this.show_help = show_help;
            this.help_message = help_message;
        }

        public string input_file { get; set; }

        public string output_file { get; set; }

        public bool output_to_console
        {
            get
            {
                return !String.IsNullOrEmpty(input_file) && String.IsNullOrEmpty(output_file);
            }
        }

        public bool output_to_file
        {
            get
            {
                return input_give && !String.IsNullOrEmpty(output_file);
            }
        }

        public bool show_help { get; set; }

        public string help_message { get; set; }

        public string piped_input { get; set; }

        public bool input_give
        {
            get
            {
                return !String.IsNullOrEmpty(piped_input) ||
                    !String.IsNullOrEmpty(input_file);
            }
        }

        public bool show_form
        {
            get
            {
                return String.IsNullOrEmpty(input_file) && !show_help;
            }
        }
    }
}
