using LeaveTracker.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveTracker.Interface
{
    public interface ILeaveMaster
    {
        void GetEmployeeDetails();
        bool CreateHoliday(LeaveMainModel objLeaveMainModel);
        void UpdateHoliday(LeaveMainModel objLeaveMainModel);
        void ShowAllHolidays(LeaveMainModel objLeaveMainModel);
        void Searchby(LeaveMainModel objLeaveMainModel);
    }
}
