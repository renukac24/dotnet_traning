using System;
using System.Collections.Generic;
using System.IO;

namespace ConvertLogsToCSV
{
    class LogParse
    {
        static void Main(string[] args)
        {
            try
            {
                List<string> InputArgs = new List<string>();
                foreach (var item in args)
                {
                    InputArgs.Add(item);
                }
                if (CommandLineInput(InputArgs))
                {
                    Console.WriteLine("File created successfully.");
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message.ToString());
            }
        }

        public static bool CommandLineInput(List<string> InputArgs)
        {
            try
            {
                List<string> LogLevel = new List<string>();
                string inputfilepath = null;
                string outputfilepath = null;
                bool isValid = false;
                List<string> lstFilePath = new List<string>();
                for (int i = 0; i < InputArgs.Count; i++)
                {                    
                    if (InputArgs[i] == "--log-dir")
                    {
                        inputfilepath = InputArgs[i + 1];
                        if (!Directory.Exists(inputfilepath))
                        {
                            Console.WriteLine("Log directory does not exist.");
                            isValid = false;                            
                            break;
                        }
                        isValid = true;

                    }
                    else if (InputArgs[i] == "--csv")
                    {
                        outputfilepath = InputArgs[i + 1];
                        isValid = true;

                    }
                    else if(InputArgs[i] == "--log-level"){
                    if (Enum.IsDefined(typeof(enumLogLevel), InputArgs[i+1].ToUpper()))
                    {
                        LogLevel.Add(InputArgs[i+1]);
                     
                    }else{
                        Console.WriteLine("Invalid Log Level");
                        isValid = false;
                        break;
                    }
                    isValid = true;
                    }
                }
                if (inputfilepath == null || outputfilepath == null || LogLevel.Count == 0 || isValid == false)
                {
                    ShowUsage();
                    return false;
                }
                ConvertCSV objConvertCSV = new ConvertCSV();
                if (objConvertCSV.convertToCSV(inputfilepath, LogLevel, outputfilepath))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                //Console.WriteLine(ee.Message.ToString());
                ShowUsage();
                return false;
            }
        }
        public static void ShowUsage()
        {
            Console.WriteLine("Inavild option files, we can refer below Usage: ");
            Console.WriteLine("Usage: logParser --log-dir <dir> --log-level <level> --csv <out>");
            Console.WriteLine("--log-dir    Directory to parse recursively for .log files");
            Console.WriteLine("--csv        Output file-path (absolute/relative)");
            Console.WriteLine("--log-level  <INFO/WARN/DEBUG>");
        }
    }
}
