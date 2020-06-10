using LeaveTracker.Interface;
using LeaveTracker.Model;
using System;
using System.Reflection.PortableExecutable;

namespace LeaveTracker
{
    class LeaveTracker
    {
        static void Main(string[] args)
        {
            try
            {
                SelectOption();
            }
            catch (Exception ee)
            {
                Console.WriteLine("Invalid input");
            }

        }

        public static void SelectOption()
        {
            try
            {
                LeaveMainModel objLeaveMainModel = new LeaveMainModel();
                string SearchFilter = "";
                Console.WriteLine("Enter the empoyee id");
                int employeeid = Convert.ToInt32(Console.ReadLine());
                if (employeeid != 0)
                {
                    Console.WriteLine("Enter the option you want : ");
                    Console.WriteLine("1. Create a leave");
                    Console.WriteLine("2. Update leaves");
                    Console.WriteLine("3. List all hoildays");
                    Console.WriteLine("4. Search Leave by : - Title");
                    Console.WriteLine("                     - Status");
                    Console.WriteLine("Enter the option");
                    int option = Convert.ToInt32(Console.ReadLine());
                    if (option == 4)
                    {
                        Console.WriteLine("Select Search Option : Title | Status");
                        SearchFilter = Console.ReadLine();
                    }

                    objLeaveMainModel.employeeid = employeeid;
                    objLeaveMainModel.option = option;
                    objLeaveMainModel.SearchFilteroption = SearchFilter;
                    PromptOption(objLeaveMainModel);

                }
            }catch(Exception ee)
            {

            }
        }

        public static void PromptOption(LeaveMainModel objLeaveMainModel)
        {
            LeaveMaster objLeaveMaster = new LeaveMaster();
            ILeaveMaster objILeaveMaster = (ILeaveMaster) objLeaveMaster;   

            int option = objLeaveMainModel.option;
                
            objILeaveMaster.GetEmployeeDetails();

            switch (option)
            {
                case 1:
                    bool result =objILeaveMaster.CreateHoliday(objLeaveMainModel);
                    if (result == true)
                    {
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine(" ");
                        SelectOption();
                    }
                    break;
                case 2:
                    objILeaveMaster.UpdateHoliday(objLeaveMainModel);
                    break;
                case 3:
                    objILeaveMaster.ShowAllHolidays(objLeaveMainModel);
                    Console.WriteLine(" ");
                    SelectOption();
                    break;
                case 4:
                    objILeaveMaster.Searchby(objLeaveMainModel);
                    Console.WriteLine(" ");
                    SelectOption();
                    break;
                default:
                    return;


            }
        }
    }
}
