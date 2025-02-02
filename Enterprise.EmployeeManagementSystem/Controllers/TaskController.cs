using Enterprise.EmployeeManagement.BAL.CustomModels;
using Enterprise.EmployeeManagement.BAL.Repository;
using Enterprise.EmployeeManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Enterprise.EmployeeManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        public readonly TaskOperationsRepository _taskrepo;
        public readonly EmployeeRepository _emprepo;

        public TaskController(TaskOperationsRepository taskrepo, EmployeeRepository emprepo)
        {
            _taskrepo = taskrepo;
            _emprepo = emprepo;
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult ManageTasks()
        {
            return View();
        }

        public List<CustomTaskModel> GetAllTasks()
        {
            return _taskrepo.GetAllTasks();
        }

        public IActionResult GetEmployees()
        {
            var (users, _, _) = _emprepo.GetAllEmployees("", 0, 20, "", "");
            if (users != null)
            {
                return Json(new
                {
                    data = users,
                    success = true
                });
            }
            return Json(new
            {
               data = users ,
               success = false
            });
        }

        [HttpPost]
        public IActionResult CreateTask(TaskModel task)
        {
            SiteUserModel user = _emprepo.GetEmpByEmail(User.Identity.Name);
            task.AssignedBy = user.SiteUserId;

            bool status = _taskrepo.AssignTask(task);
            if (status == true)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, errors = "An error occured while adding the task" });
            }
        }
        public IActionResult UpdateTask(TaskModel task)
        {
            bool status = _taskrepo.UpdateTask(task);
            if (status == true)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, errors = "An error occured while adding the task" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteTask(int taskId)
        {
           
            bool status = _taskrepo.DeleteTask(taskId);
            if (status == true)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, errors = "An error occured while adding the task" });
            }
        }

        public IActionResult GetTaskByID(int taskId)
        {
            CustomTaskModel task = _taskrepo.GetTaskById(taskId);
            if (task == null)
            {
                return Json(new { success = false, errors = "An error occured while adding the task" });
            }
            return Json(new { success = true ,data = task});
        }
        public IActionResult DelayedTasks()
        {
            return View();
        }

        public List<CustomTaskModel> GetDelayedTasksOfUser(int userId)
        {
            List<CustomTaskModel> delayedTaskList =  _taskrepo.GetTasksByAssigneeUserId(userId);
            foreach (CustomTaskModel task in delayedTaskList)
            {
                Debug.WriteLine(task.AssignedToName);
            }
            return delayedTaskList;
        }
        public List<CustomSiteUserModel> GetUsersWithDelayedTasks()
        {
            List<CustomSiteUserModel> delayedTaskList = _taskrepo.GetUsersWithDelayedTasks();
            foreach (CustomSiteUserModel task in delayedTaskList)
            {
                Debug.WriteLine(task);
            }
            return delayedTaskList;
        }

        public IActionResult SendEmailAlert(int taskId)
        {
            try
            {
                _taskrepo.SendEmailAlert(taskId);
                return Json(new { success = true });
            }
            catch(Exception e)
            {

            return Json(new { success = false, errors = e });
            }
        }
    }
}
