using Microsoft.AspNetCore.Mvc;
using Enterprise.EmployeeManagement.DAL.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Enterprise.EmployeeManagement.BAL.Repository;
using Enterprise.EmployeeManagement.BAL.CustomModels;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Enterprise.EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _userRepository;
        private readonly ILogger<EmployeeController> _logger;


        public EmployeeController(EmployeeRepository userRepository, ILogger<EmployeeController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult ManageEmployee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetEmployees()
        {

            string searchStr = Request.Form["search[value]"];
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            int skip = Convert.ToInt32(Request.Form["start"]);
            int take = Convert.ToInt32(Request.Form["length"]);
            var (users, totalRecords, filteredRecords) = _userRepository.GetAllEmployees(searchStr, skip, take, sortColumn, sortColumnDirection);

            return Json(new { data = users, recordsTotal = totalRecords, recordsFiltered = filteredRecords });
        }
        [HttpPost]
        public IActionResult CreateEmployee(SiteUserModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    var role = _userRepository.GetRoleById(model.RoleId);
                    if (role == null)
                    {
                        return Json(new { success = false, errors = "Invalid RoleId" });
                    }

                    bool status = _userRepository.AddEmployee(model);
                    if (status == true)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, errors = "Unable to create user !!" });
                    }
                }

                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex });
            }
        }


        [HttpGet]
        public IActionResult GetEmployee(int id)
        {
            _logger.LogInformation("GetUser method called with id: {id}", id);
            var user = _userRepository.GetEmployeeById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Json(user);
        }

        [HttpPost]
        public IActionResult EditEmployee(CustomSiteUserModel user)
        {
            _logger.LogInformation("Updating user with ID {UserId}.", user.SiteUserId);

            if (ModelState.IsValid)
            {
                bool status = _userRepository.UpdateEmployee(user);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errors = "You can not use this email address as it already exists !!" });
                }
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        public IActionResult DeleteEmployee(int id)
        {
            _userRepository.DeleteEmployee(id);
            return Json(new { success = true });
        }

        public IActionResult UserDashboard()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserDashboard");
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginPage(SiteUserModel formData)
        {
            string emailAddress = formData.EmailAddress;
            string password = formData.Password;
            (SiteUserModel userdata, bool status) = _userRepository.ValidateUser(emailAddress, password, out string roleName);

            if (status == true)
            {
                List<Claim> claimList = new List<Claim>()
                {
                    new Claim(ClaimTypes.Role , roleName),
                    new Claim(ClaimTypes.NameIdentifier , emailAddress),
                    new Claim(ClaimTypes.Name , emailAddress)

                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                        claimList, CookieAuthenticationDefaults.AuthenticationScheme);


                AuthenticationProperties authProperties = new AuthenticationProperties();

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("UserDashboard");
            }
            TempData["Error"] = "Invalid Username or Password !! ";
            return View();
        }


        public IActionResult LogoutUser()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("LoginPage", "Employee");
        }

    }
}
