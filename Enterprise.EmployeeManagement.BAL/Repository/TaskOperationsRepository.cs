using Enterprise.EmployeeManagement.BAL.CustomModels;
using Enterprise.EmployeeManagement.DAL.Models;
using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using ZeroFormatter;
using Enterprise.EmployeeManagement.DAL.RedisHelper;
using StackExchange.Redis;
using System.Linq.Dynamic;
using System.Net;
using NLog;
namespace Enterprise.EmployeeManagement.BAL.Repository
{
    public class TaskOperationsRepository
    {
        private readonly AppDbContext _context;
        private readonly EmployeeRepository _emprepo;
        private readonly IDatabase cache;
        private readonly Logger _logger;

        string hashKey = "TaskList";
        List<HashEntry> cacheTasks = new List<HashEntry>() { };
        public TaskOperationsRepository(AppDbContext context, EmployeeRepository emprepo)
        {
            _context = context;
            _emprepo = emprepo;
            cache = RedisHelper.database;
            _logger = LogManager.GetLogger("EmployeeLoggerRules");
        }

        public bool AssignTask(TaskModel task)
        {
            try
            {
                task.AssignedDate = DateTime.Now;
                task.TaskStatus = CalculateTaskStatus(task);
                _context.Tasks.Add(task);
                _context.SaveChanges();
                CacheTask(task);
                _logger.Info($" Task : '{task.TaskName}' Added in the tasks database");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return false;
            }

        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                TaskModel task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);

                _context.Tasks.Remove(task);
                _context.SaveChanges();
                var result = cache.HashDelete(hashKey, taskId);
                if (result)
                {
                    _logger.Info($"Task : '{task.TaskName}' Deleted from the tasks database");

                }
                return true;
            }
            catch (Exception ex)
            {

                _logger.Error(ex.ToString());
                return false;
            }

        }
        public bool UpdateTask(TaskModel task)
        {
            try
            {
                if (task != null)
                {
                    task.TaskStatus = CalculateTaskStatus(task);
                    _context.Tasks.Update(task);
                    _context.SaveChanges();
                    CacheTask(task);
                    _logger.Info($" Task : '{task.TaskName}' Updated in the tasks database");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return false;
            }
        }


        public CustomTaskModel GetTaskById(int taskId)
        {
            try
            {
                TaskModel task = new TaskModel();
                if (cache.HashExists(hashKey, taskId))
                {
                    var bytes = cache.HashGet(hashKey, taskId);
                    task = ZeroFormatterSerializer.Deserialize<TaskModel>(bytes);

                }
                else
                {
                    task = _context.Tasks.FirstOrDefault(t => t.TaskId == taskId);
                    CacheTask(task);

                }
                CustomTaskModel customTask = ConvertTaskToCustomTask(task);
                return customTask;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }

        public List<CustomTaskModel> GetAllTasks()
        {
            List<TaskModel> taskList = new List<TaskModel>();
            List<CustomTaskModel> customTaskList = new List<CustomTaskModel>();
            try
            {
                try
                {
                    if (cache.KeyExists(hashKey))
                    {
                        var bytes = cache.HashValues(hashKey);
                        if (bytes.Length > 0)
                        {
                            foreach (var b in bytes)
                            {
                                var deserialized = ZeroFormatterSerializer.Deserialize<TaskModel>(b);
                                taskList.Add(deserialized);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                }
                if (taskList.Count == 0)
                {
                    taskList = _context.Tasks.OrderByDescending(t => t.TaskId).ToList();
                    foreach (TaskModel task in taskList)
                    {
                        CacheTask(task);
                    }
                }
                foreach (TaskModel task in taskList)
                {
                    task.TaskStatus = CalculateTaskStatus(task);
                    CustomTaskModel CustomTask = ConvertTaskToCustomTask(task);
                    customTaskList.Add(CustomTask);
                }
                _context.SaveChanges();
                return customTaskList.OrderByDescending(t => t.TaskId).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Calculates Task status based on currentDate & dates provided in the task object (which includes startDate , endDate , assignedDate and deadline).
        /// </summary>
        /// <param name="task">Takes TaskModel object as a parameter</param>
        /// <returns>string taskStatus</returns>
        public string CalculateTaskStatus(TaskModel task)
        {

            if (task.EndDate != null)
            {
                if (task.EndDate > task.Deadline)
                {
                    return "Delayed complete";
                }
                else
                {
                    return "Completed";
                }
            }
            else
            {
                if (DateTime.Now > task.Deadline)
                {
                    return "Delayed";
                }
                else if (DateTime.Now > task.StartDate)
                {
                    return "In Progress";
                }
                return "Pending";
            }
        }

        /// <summary>
        /// Get all the delayed task of a user from the userId
        /// </summary>
        /// <param name="userId">userId of employee</param>
        /// <returns>List of delayed Tasks as objects of CustomTaskModel</returns>
        public List<CustomTaskModel> GetTasksByAssigneeUserId(int userId)
        {
            List<TaskModel> taskList = new List<TaskModel>();
            List<CustomTaskModel> customTaskList = new List<CustomTaskModel>();
            try
            {
                if (cache.KeyExists(hashKey))
                {
                    var bytes = cache.HashValues(hashKey);
                    if (bytes.Length > 0)
                    {
                        foreach (var b in bytes)
                        {
                            var deserialized = ZeroFormatterSerializer.Deserialize<TaskModel>(b);
                            if (deserialized.TaskStatus == "Delayed" && deserialized.AssignedTo == userId)
                            {
                                taskList.Add(deserialized);
                            }
                        }
                    }
                }
                if (taskList.Count == 0)
                {
                    return null;
                }
                foreach (TaskModel task in taskList)
                {
                    task.TaskStatus = CalculateTaskStatus(task);
                    CustomTaskModel customTask = ConvertTaskToCustomTask(task);
                    customTaskList.Add(customTask);
                }
                return customTaskList.OrderByDescending(t => t.TaskId).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }
        public List<CustomSiteUserModel> GetUsersWithDelayedTasks()
        {
            List<TaskModel> TaskList = new List<TaskModel>();
            List<CustomSiteUserModel> usersWithDelayedTasks = new List<CustomSiteUserModel>();

            try
            {
                if (cache.KeyExists(hashKey))
                {
                    var bytes = cache.HashValues(hashKey);
                    if (bytes.Length > 0)
                    {
                        foreach (var b in bytes)
                        {
                            var deserialized = ZeroFormatterSerializer.Deserialize<TaskModel>(b);
                            if (deserialized.TaskStatus == "Delayed")
                            {
                                usersWithDelayedTasks.Add(_emprepo.GetEmployeeById(deserialized.AssignedTo));
                            }
                        }
                    }
                }
                if (TaskList.Count == 0)
                {
                    Debug.WriteLine($"------------------------- No Delayed Task ---------------------------------");

                }


                return usersWithDelayedTasks.GroupBy(u => u.SiteUserId).Select(u => u.First()).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Sends Email alert to the employee whose task is delayed 
        /// </summary>
        /// <param name="taskId">Takes taskID of the particular delayed task</param>
        public void SendEmailAlert(int taskId)
        {
            CustomTaskModel taskModel = GetTaskById(taskId);
            CustomSiteUserModel assignedToUser = _emprepo.GetEmployeeById(taskModel.AssignedTo);
            try
            {
                string fromMail = "preetmmorbia04@gmail.com";
                string fromPassword = "aakw txxu rvhc ljmb";

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(fromMail),
                    Subject = "Reminder to complete assigned task",
                    IsBodyHtml = true

                };
                message.To.Add(assignedToUser.EmailAddress);
                message.Body = "<html>" +
                  "<body style='font-family: Arial, sans-serif; color: #333333; background-color: #f9f9f9; padding: 20px;'>" +
                    "<div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0,0,0,0.1);'>" +
                      "<h2 style='color: #d9534f; text-align: center;'>📅 Deadline Missed: Immediate Attention Needed!</h2>" +
                      "<p style='font-size: 16px; line-height: 1.6;'>" +
                        $"Dear {assignedToUser.FirstName + " " + assignedToUser.LastName}," +
                      "</p>" +
                      "<p style='font-size: 16px; line-height: 1.6;'>" +
                        "This is a reminder for you to complete the task assigned by <strong>" +
                        $"{taskModel.AssignedByName}</strong> on <strong>{taskModel.FormattedAssignedDate}</strong>." +
                      "</p>" +
                      "<p style='font-size: 16px; line-height: 1.6; color: #555555;'>" +
                        "<strong>Task Details:</strong><br>" +
                        $"<strong>Task Name:</strong> {taskModel.TaskName}<br>" +
                        $"<strong>Description:</strong> {taskModel.TaskDescription}<br>" +
                        $"<strong>Assigned By:</strong> {taskModel.AssignedByName}<br>" +
                        $"<strong>Assigned To:</strong> {taskModel.AssignedToName}<br>" +
                        $"<strong>Assigned Date:</strong> {taskModel.FormattedAssignedDate}<br>" +
                        $"<strong>Start Date:</strong> {taskModel.FormattedStartDate}<br>" +
                        $"<strong>Deadline:</strong> {taskModel.FormattedDeadline}<br>" +
                      "</p>" +
                      "<p style='font-size: 16px; line-height: 1.6; color: #d9534f; text-align: center;'>" +
                        "Please complete the due task as soon as possible!" +
                      "</p>" +
                      "<footer style='text-align: center; font-size: 14px; color: #777777; margin-top: 20px;'>" +
                        "Thank you for your attention." +
                      "</footer>" +
                    "</div>" +
                  "</body>" +
                "</html>";



                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.Send(message);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());

            }
        }
        public CustomTaskModel ConvertTaskToCustomTask(TaskModel task)
        {
            try
            {

                CustomTaskModel customTask = new CustomTaskModel()
                {

                    TaskId = task.TaskId,
                    TaskName = task.TaskName,
                    TaskDescription = task.TaskDescription,
                    AssignedTo = task.AssignedTo,
                    AssignedBy = task.AssignedBy,
                    AssignedByName = _emprepo.GetEmployeeNameById(task.AssignedBy),
                    AssignedToName = _emprepo.GetEmployeeNameById(task.AssignedTo),
                    Priority = task.Priority,
                    StartDate = task.StartDate,
                    AssignedDate = task.AssignedDate,
                    TaskStatus = task.TaskStatus,
                    EndDate = task.EndDate,
                    Deadline = task.Deadline
                };
                return customTask;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
        }

        public void CacheTask(TaskModel task)
        {
            try
            {
                var bytes = ZeroFormatterSerializer.Serialize(task);
                //var deserialized = ZeroFormatterSerializer.Deserialize<TaskModel>(bytes);
                cacheTasks.Add(new HashEntry(task.TaskId, bytes));
                cache.HashSet(hashKey, cacheTasks.ToArray());
                cacheTasks.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());

            }
        }

    }
}