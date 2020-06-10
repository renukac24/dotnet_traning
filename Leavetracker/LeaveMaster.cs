using CsvHelper;
using LeaveTracker.Interface;
using LeaveTracker.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LeaveTracker
{
    public class LeaveMaster : ILeaveMaster
    {
        Dictionary<int, EmployeeManagerDetails> dictempmanager = null;
        Dictionary<int, List<EmployeeHolidayDetails>> dictEmployeeDetails = null;
        Constant objConstant;

        public LeaveMaster()
        {
            objConstant = new Constant();
        }

        public bool CreateHoliday(LeaveMainModel objLeaveMainModel)
        {
            EmployeeHolidayDetails objEmployeeHolidayDetails = new EmployeeHolidayDetails();
            objEmployeeHolidayDetails.EmployeeId = objLeaveMainModel.employeeid;
            objEmployeeHolidayDetails.Status = "Pending";

            //objEmployeeHolidayDetails.Manager = Console.ReadLine();
            int ManagerId = dictempmanager.GetValueOrDefault(objEmployeeHolidayDetails.EmployeeId).ManagerId;
            if(ManagerId == 0)
            {
                Console.WriteLine("Invalid Employee Id.");
                return false;
            }
            string ManagerName = dictempmanager.GetValueOrDefault(ManagerId).EmployeeName;
            objEmployeeHolidayDetails.Creator = dictempmanager.GetValueOrDefault(objLeaveMainModel.employeeid).EmployeeName;
            Console.WriteLine("Assign To : " + ManagerName);
            objEmployeeHolidayDetails.Manager = ManagerName;
            Console.Write("Title :");
            objEmployeeHolidayDetails.Title = Console.ReadLine();
            Console.Write("Description :");
            objEmployeeHolidayDetails.Description = Console.ReadLine();
            Console.Write("Start-Date :");
            objEmployeeHolidayDetails.StartDate = Console.ReadLine();
            Console.Write("End-Date :");
            objEmployeeHolidayDetails.EndDate = Console.ReadLine();

            if (DateValidate(objEmployeeHolidayDetails))
            {
                var csv = new StringBuilder();
                string headerline = "ID,Creator,Manager,Title,Description,Start - Date,End - Date,Status";
                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                    objEmployeeHolidayDetails.EmployeeId,
                    objEmployeeHolidayDetails.Creator,
                    objEmployeeHolidayDetails.Manager,
                    objEmployeeHolidayDetails.Title,
                    objEmployeeHolidayDetails.Description,
                    objEmployeeHolidayDetails.StartDate,
                    objEmployeeHolidayDetails.EndDate,
                    objEmployeeHolidayDetails.Status);

                if (!File.Exists(objConstant.LeavesFilePath))
                {
                    csv.AppendLine(headerline);
                }
                csv.AppendLine(newLine);
                File.AppendAllText(objConstant.LeavesFilePath, csv.ToString());
                Console.WriteLine("Record Successfully inserted");
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Update Holiday only be manager
        /// </summary>
        /// <param name="objLeaveMainModel"></param>
        public void UpdateHoliday(LeaveMainModel objLeaveMainModel)
        {
            List<EmployeeManagerDetails> lstEmployeeManagerDetails = dictempmanager.Where(x => x.Value.ManagerId == objLeaveMainModel.employeeid).Select(y => y.Value).ToList();
            if(lstEmployeeManagerDetails.Count == 0)
            {
                Console.WriteLine("Invalid Manager or No Data To Show.");
                return;
            }
            String path = objConstant.LeavesFilePath;
            foreach (var item in lstEmployeeManagerDetails)
            {
                List<EmployeeHolidayDetails> lstEmployeeHolidayDetails;
                int employeeid = item.EmployeeId;
                if (dictEmployeeDetails.TryGetValue(employeeid, out lstEmployeeHolidayDetails))
                {
                    foreach (var empHoildayDetails in lstEmployeeHolidayDetails)
                    {
                        if (empHoildayDetails.Status.ToUpper() != "APPROVED" && empHoildayDetails.Status.ToUpper() != "REJECTED")
                        {
                            FormatConsole objFormatConsole = new FormatConsole();
                            objFormatConsole.PrintLine();
                            string[] arrHeader = new string[] { "ID", "Creator", "Manager", "Title", "Description", "Start-Date", "End-Date", "Status" };
                            objFormatConsole.PrintRow(arrHeader);
                            objFormatConsole.PrintLine();
                            string[] arrEmployeeHolidayDetails = new string[] { empHoildayDetails.EmployeeId.ToString(), empHoildayDetails.Creator, empHoildayDetails.Manager, empHoildayDetails.Title, empHoildayDetails.Description, empHoildayDetails.StartDate, empHoildayDetails.EndDate, empHoildayDetails.Status };
                            objFormatConsole.PrintRow(arrEmployeeHolidayDetails);
                            objFormatConsole.PrintLine();
                            Console.Write("Modify Status to : ");
                            string Status = Console.ReadLine();
                            if (Status.ToUpper() == "APPROVED" || Status.ToUpper() == "REJECTED")
                            {
                                empHoildayDetails.Status = Status;
                                UpdateRecord(empHoildayDetails);
                                Console.WriteLine("Record Modified successfully.");
                            }
                        }

                    }

                }
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Update the record and save the file
        /// </summary>
        /// <param name="empHoildayDetails"></param>
        public void UpdateRecord(EmployeeHolidayDetails empHoildayDetails)
        {
            string filename = objConstant.LeavesFilePath;
            List<string> quotelist = File.ReadAllLines(filename).ToList();
            string searchFilter = empHoildayDetails.Creator + "," + empHoildayDetails.Title + "," + empHoildayDetails.Description;
            string insertData = empHoildayDetails.EmployeeId + "," + empHoildayDetails.Creator + "," + empHoildayDetails.Manager+ "," + empHoildayDetails.Title + "," + empHoildayDetails.Description + ","
            + empHoildayDetails.StartDate + "," + empHoildayDetails.EndDate + "," + empHoildayDetails.Status;
            int id = quotelist.FindIndex(x => x.Contains(empHoildayDetails.Creator) && x.Contains(empHoildayDetails.Title)
            && x.Contains(empHoildayDetails.Description));
            quotelist.RemoveAt(id);
            quotelist.Insert(id, insertData);
            File.WriteAllLines(filename, quotelist.ToArray());
        }
        
        /// <summary>
        /// List All the holidays
        /// </summary>
        /// <param name="objLeaveMainModel"></param>
        public void ShowAllHolidays(LeaveMainModel objLeaveMainModel)
        {
            try
            {
                FormatConsole objFormatConsole = new FormatConsole();
                List<EmployeeHolidayDetails> lstEmployeeHolidayDetails;
                if (dictEmployeeDetails.TryGetValue(objLeaveMainModel.employeeid, out lstEmployeeHolidayDetails))
                {
                    DisplayDataonConsole(lstEmployeeHolidayDetails);
                }
                else
                {
                    Console.WriteLine("No record to Display");
                }
            }
            catch (Exception ee)
            {
		Console.WriteLine(ee.Message);
            }
            Console.ReadLine();

        }

        /// <summary>
        /// Search With Filter 
        /// </summary>
        /// <param name="objLeaveMainModel"></param>
        public void Searchby(LeaveMainModel objLeaveMainModel)
        {
            try
            {
                //To Display in table Format
                FormatConsole objFormatConsole = new FormatConsole();
                List<EmployeeHolidayDetails> lstEmployeeHolidayDetails;
                List<EmployeeHolidayDetails> lstNewEmployeeHolidayDetails = new List<EmployeeHolidayDetails>();

                Console.WriteLine("Enter Search Parameter");
                string SearchParam = Console.ReadLine();
                if (dictEmployeeDetails.TryGetValue(objLeaveMainModel.employeeid, out lstEmployeeHolidayDetails))
                {

                    if (objLeaveMainModel.SearchFilteroption.ToUpper() == "TITLE")
                        lstNewEmployeeHolidayDetails = lstEmployeeHolidayDetails.Where(x => x.Title.ToUpper() == SearchParam.ToUpper()).ToList();
                    else if (objLeaveMainModel.SearchFilteroption.ToUpper() == "STATUS")
                        lstNewEmployeeHolidayDetails = lstEmployeeHolidayDetails.Where(x => x.Status.ToUpper() == SearchParam.ToUpper()).ToList();
                    else
                        Console.WriteLine("Invalid option.");
                    
                    if (lstNewEmployeeHolidayDetails.Count != 0)
                    {
                        DisplayDataonConsole(lstNewEmployeeHolidayDetails);
                    }
                    else
                    {
                        Console.WriteLine("No record found");
                    }
                }
            }
            catch (Exception ee)
            {

            }
            Console.ReadLine();
        }

        public void DisplayDataonConsole(List<EmployeeHolidayDetails> lstEmployeeHolidayDetails)
        {
            FormatConsole objFormatConsole = new FormatConsole();
            objFormatConsole.PrintLine();
            string[] arrHeader = new string[] { "ID", "Creator", "Manager", "Title", "Description", "Start-Date", "End-Date", "Status" };
            objFormatConsole.PrintRow(arrHeader);
            objFormatConsole.PrintLine();

            foreach (var item in lstEmployeeHolidayDetails)
            {

                string[] arrEmployeeHolidayDetails = new string[] { item.EmployeeId.ToString(), item.Creator, item.Manager, item.Title, item.Description, item.StartDate, item.EndDate, item.Status };
                objFormatConsole.PrintRow(arrEmployeeHolidayDetails);
                objFormatConsole.PrintLine();

            }
        }

        public void GetEmployeeDetails()
        {
            Constant objConstant = new Constant();
            try
            {
                if (dictempmanager == null)
                {
                    dictempmanager = new Dictionary<int, EmployeeManagerDetails>();
                    if (File.Exists(objConstant.EmployeeFilePath))
                    {
                        using (var reader = new StreamReader(objConstant.EmployeeFilePath))
                        {
                            string headerline = reader.ReadLine();
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                EmployeeManagerDetails objEmployeManagerDetails = new EmployeeManagerDetails();
                                string[] splits = line.Split(',');
                                objEmployeManagerDetails.EmployeeId = Convert.ToInt32(splits[0]);
                                objEmployeManagerDetails.EmployeeName = splits[1];
                                if (splits[2] != "")
                                    objEmployeManagerDetails.ManagerId = Convert.ToInt32(splits[2]);
                                else
                                    objEmployeManagerDetails.ManagerId = 0;
                                dictempmanager?.Add(objEmployeManagerDetails.EmployeeId, objEmployeManagerDetails);
                            }
                        }
                    }
                }
                if (dictEmployeeDetails == null)
                {
                    dictEmployeeDetails = new Dictionary<int, List<EmployeeHolidayDetails>>();
                    if (File.Exists(objConstant.LeavesFilePath))
                    {
                        using (var reader = new StreamReader(objConstant.LeavesFilePath))
                        {
                            string headerline = reader.ReadLine();
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                EmployeeHolidayDetails objEmployeeHolidayDetails = new EmployeeHolidayDetails();
                                string[] splits = line.Split(',');
                                objEmployeeHolidayDetails.EmployeeId = Convert.ToInt32(splits[0]);
                                objEmployeeHolidayDetails.Creator = splits[1];
                                objEmployeeHolidayDetails.Manager = splits[2];
                                objEmployeeHolidayDetails.Title = splits[3];
                                objEmployeeHolidayDetails.Description = splits[4];
                                objEmployeeHolidayDetails.StartDate = splits[5];
                                objEmployeeHolidayDetails.EndDate = splits[6];
                                objEmployeeHolidayDetails.Status = splits[7];
                                List<EmployeeHolidayDetails> lstEmployeeHolidayDetails;
                                if (!dictEmployeeDetails.TryGetValue(objEmployeeHolidayDetails.EmployeeId, out lstEmployeeHolidayDetails))
                                {
                                    lstEmployeeHolidayDetails = new List<EmployeeHolidayDetails>();
                                    lstEmployeeHolidayDetails.Add(objEmployeeHolidayDetails);
                                    dictEmployeeDetails.Add(objEmployeeHolidayDetails.EmployeeId, lstEmployeeHolidayDetails);
                                }
                                else
                                {
                                    lstEmployeeHolidayDetails.Add(objEmployeeHolidayDetails);
                                    dictEmployeeDetails[objEmployeeHolidayDetails.EmployeeId]= lstEmployeeHolidayDetails;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message.ToString(), "GetEmployeeDetails");
                return;
            }

        }

        public bool DateValidate(EmployeeHolidayDetails objEmployeeHolidayDetails)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(objEmployeeHolidayDetails.StartDate, "dd-MM-yyyy", null, DateTimeStyles.None, out dt) == true)
            {
                Console.WriteLine("Invalid Start Date");
                return false;
            }
            if (!DateTime.TryParseExact(objEmployeeHolidayDetails.EndDate, "dd-MM-yyyy", null, DateTimeStyles.None, out dt) == true)
            {
                Console.WriteLine("Invalid End Date");
                return false;
            }

            if(DateTime.Parse(objEmployeeHolidayDetails.StartDate) > DateTime.Parse(objEmployeeHolidayDetails.EndDate))
            {
                Console.WriteLine("Start Date is greater than End Date");
                return false;
            }

            return true;
        }

    }
}
