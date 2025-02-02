using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ZeroFormatter;

namespace Enterprise.EmployeeManagement.DAL.Models
{
    [ZeroFormattable]
    public class TaskModel
    {

        [Key]
        [Index(0)]
        public  virtual int TaskId { get; set; }

        [Index(1)]
        public virtual string TaskName { get; set; }

        [Index(2)]
        public virtual string TaskDescription { get; set; }


        [Index(3)]
        public virtual int AssignedBy { get; set; }

        [Index(4)]
        public virtual int AssignedTo { get; set; }
        [Index(5)]
        public virtual DateTime AssignedDate { get; set; }
        [Index(6)]
        public virtual DateTime StartDate { get; set; }
        [Index(7)]
        public virtual DateTime? EndDate { get; set; }

        [Index(8)]
        public virtual DateTime Deadline { get; set; }

        [Index(9)]
        public virtual string TaskStatus { get; set; }
        [Index(10)]
        public virtual string? Priority { get; set; }




    }
}
