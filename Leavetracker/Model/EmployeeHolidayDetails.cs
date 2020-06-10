using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveTracker.Model
{
    public class EmployeeHolidayDetails
    {
        public int EmployeeId { get; set; }
        public string Creator { get; set; }
        public string Manager { get; set; }
        public int ManagerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }

    }
}
