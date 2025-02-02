using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace Enterprise.EmployeeManagement.BAL.CustomModels
{
    public class CustomTaskModel
    {
        [Key]
        public int TaskId { get; set; }


        public string TaskName { get; set; }


        public string TaskDescription { get; set; }



        public int AssignedBy { get; set; }
        public string AssignedByName { get; set; }


        public int AssignedTo { get; set; }
        public string AssignedToName { get; set; }

        public DateTime AssignedDate { get; set; }
        public string FormattedAssignedDate
        {
            get
            {
                return FormateDate(this.AssignedDate);
            }
        }

        public DateTime StartDate { get; set; }
        public string FormattedStartDate
        {
            get
            {
                return FormateDate(this.StartDate);
            }
        }
        public DateTime? EndDate { get; set; }
        public string FormattedEndDate
        {
            get
            {
                if (this.EndDate.HasValue)
                {
                    DateTime date = this.EndDate.Value;
                    return FormateDate(date);
                }
                else
                {
                    return "";
                }

            }
        }



        public DateTime Deadline { get; set; }
        public string FormattedDeadline
        {
            get
            {
                return FormateDate(this.Deadline);
            }
        }


        public string TaskStatus { get; set; }

       
        public string? Priority { get; set; }

        public static string FormateDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
