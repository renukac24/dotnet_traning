using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveTracker.Model
{
    public class FormatConsole
    {
        static int tablewidth = 73;
        public void PrintLine()
        {
            Console.WriteLine(new string('-', tablewidth));
        }

        public void PrintRow(params string[] columns)
        {
            int width = (tablewidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
               row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }
        public  string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}
