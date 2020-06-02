using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConvertLogsToCSV
{
    public class ConvertCSV
    {
        public bool convertToCSV(string inputfilepath, List<string> LogLevel, string outputfilepath)
        {
            try
            {
                List<string> lstofLines = new List<string>();
                List<string> lstofLinesend = new List<string>();

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("No,Level,Date,Time,Text");
                stringBuilder.Append("\n");
                int count = 0;
                foreach (var level in LogLevel)
                {
                    
                    var files = from file in Directory.EnumerateFiles(inputfilepath, "*.log")
                                from line in File.ReadLines(file)
                                where line.Contains(level)
                                select line.Split(":.");

                    foreach (var item in files)
                    {
                        lstofLines.Add(item.First());
                        lstofLinesend.Add(item.Last());
                    }
                    
                    foreach (var first in lstofLines.Zip(lstofLinesend, Tuple.Create))
                    {
                        
                        string FirstSplitPart = first.Item1;
                        string Firsttemp = FirstSplitPart;
                        count = count + 1;
                        stringBuilder.Append(count);
                        stringBuilder.Append(",");
                        string[] temp = Firsttemp.Split(" ");
                        string date = ConvertToDate(temp[0]);
                        string Time = ConvertToTime(temp[1]);
                        string loglevel = temp[2];
                        stringBuilder.Append(loglevel);
                        stringBuilder.Append(",");
                        stringBuilder.Append(date);
                        stringBuilder.Append(",");
                        stringBuilder.Append(Time);
                        stringBuilder.Append(",");

                        string LastSpiltPart = first.Item2;
                        //Console.WriteLine(LastSpiltPart);
                        if (LastSpiltPart != null)
                        {
                            string sTextinLine = "";
                            
                                sTextinLine = "\""+LastSpiltPart+"\"";
                            stringBuilder.Append(sTextinLine);
                            stringBuilder.Append("\n");
                        }
                    }
                }

                File.WriteAllText(outputfilepath, stringBuilder.ToString());
                
                return true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message.ToString());
                return false;
            }
        }
        public static string ConvertToDate(string a)
        {
            string sMonthFullName = Convert.ToDateTime(a).ToString("dd MMM yyyy");
            return sMonthFullName;
        }

        public static string ConvertToTime(string b)
        {
            string time = Convert.ToDateTime(b).ToString("hh:mm tt");
            return time;

        }
    }
}