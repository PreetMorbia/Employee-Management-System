using Enterprise.EmployeeManagement.BAL.CustomModels;
using Enterprise.EmployeeManagement.BAL.Enums;
using Enterprise.EmployeeManagement.DAL;
using Enterprise.EmployeeManagement.DAL.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using System.Text;

namespace Enterprise.EmployeeManagement.BAL.Repository
{
    public class EmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public (List<CustomSiteUserModel>, int, int) GetAllEmployees(string searchStr, int skip, int take, string sortCol, string sortColDir)
        {
            try
            {
                List<SiteUserModel> users = _context.SiteUsers.OrderByDescending(u => u.SiteUserId).ToList();
                int totalRecords = users.Count;
                List<CustomSiteUserModel> userList = new List<CustomSiteUserModel>();
                IQueryable<SiteUserModel> searchFilteredSiteUserData = _context.Set<SiteUserModel>().AsQueryable().OrderByDescending(u => u.SiteUserId);
                int filteredRecords = totalRecords;

                if (!string.IsNullOrEmpty(searchStr))
                {
                    searchStr = searchStr.ToLower();
                    List<int> roleIdsFromEnum = GetRoleIdFromSearchString(searchStr);
                    searchFilteredSiteUserData = searchFilteredSiteUserData.Where(
                        u => u.FirstName.ToLower().Contains(searchStr) ||
                        u.LastName.ToLower().Contains(searchStr) ||
                        u.EmailAddress.ToLower().Contains(searchStr) ||
                        roleIdsFromEnum.Contains(u.RoleId));
                    filteredRecords = searchFilteredSiteUserData.Count();
                }

                if (!string.IsNullOrEmpty(sortCol) && !string.IsNullOrEmpty(sortColDir))
                {
                    searchFilteredSiteUserData = searchFilteredSiteUserData.OrderBy(sortCol + " " + sortColDir);
                }

                users = searchFilteredSiteUserData.Skip(skip).Take(take).ToList();
                foreach (SiteUserModel user in users)
                {
                    CustomSiteUserModel customSiteUserModel = ConvertSiteUserToCustomSiteUser(user);
                    userList.Add(customSiteUserModel);
                }
                return (userList, totalRecords, filteredRecords);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (null, 0, 0);
            }
        }

        public bool AddEmployee(SiteUserModel user)
        {
            try
            {
                SiteUserModel existingUser = _context.SiteUsers.FirstOrDefault(u => u.EmailAddress == user.EmailAddress);
                if (existingUser != null)
                {
                    return false;
                }
                user.Password = HashPassword(user.Password);
                _context.SiteUsers.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public RoleModel GetRoleById(int roleId)
        {
            return _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
        }
        public bool UpdateEmployee(CustomSiteUserModel user)
        {
            try
            {
                SiteUserModel existingUserWithSameEmail = _context.SiteUsers.FirstOrDefault(u => u.EmailAddress == user.EmailAddress);
                var existingUser = _context.SiteUsers.Find(user.SiteUserId);

                if (existingUserWithSameEmail != null && existingUserWithSameEmail.SiteUserId != user.SiteUserId)
                {
                    return false;
                }
                if (existingUser != null)
                {
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.EmailAddress = user.EmailAddress;
                    existingUser.RoleId = user.RoleId;
                    existingUser.Password = existingUser.Password;

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public CustomSiteUserModel GetEmployeeById(int id)
        {
            try
            {
                SiteUserModel user = _context.SiteUsers.FirstOrDefault(u => u.SiteUserId == id);
                CustomSiteUserModel customSiteUserModel = ConvertSiteUserToCustomSiteUser(user);
                return customSiteUserModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
        public string GetEmployeeNameById(int id)
        {
            SiteUserModel user = _context.SiteUsers.FirstOrDefault(u => u.SiteUserId == id);
            return user.FirstName + " " + user.LastName;
        }

        public void DeleteEmployee(int id)
        {
            SiteUserModel user = _context.SiteUsers.FirstOrDefault(u => u.SiteUserId == id);
            _context.SiteUsers.Remove(user);
            _context.SaveChanges();
        }

        public static List<int> GetRoleIdFromSearchString(string searchStr)
        {
            string[] roleNamesFromEnum = Enum.GetNames(typeof(CustomEnums.RoleEnum));
            List<int> roleIdsFromEnum = new List<int>();
            foreach (string roleName in roleNamesFromEnum)
            {
                if (roleName.ToLower().Contains(searchStr))
                {
                    roleIdsFromEnum.Add((int)Enum.Parse(typeof(CustomEnums.RoleEnum), roleName));
                }
            }
            return new HashSet<int>(roleIdsFromEnum).ToList();
        }

        private static string HashPassword(string password)
        {
            //Using MD5 to encrypt a string
            using MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            UTF8Encoding utf8 = new UTF8Encoding();
            //Hash data
            byte[] data = md5.ComputeHash(utf8.GetBytes(password));
            return Convert.ToBase64String(data);
        }

        public (SiteUserModel, bool) ValidateUser(string emailAddress, string password, out string roleName)
        {
            SiteUserModel user = _context.SiteUsers.Where(x => x.EmailAddress == emailAddress && x.Password == HashPassword(password)).FirstOrDefault();
            roleName = string.Empty;
            if (user != null)
            {
                CustomSiteUserModel customSiteUserModel = ConvertSiteUserToCustomSiteUser(user);
                roleName = customSiteUserModel.RoleName;
                return (user, true);
            }
            return (null, false);
        }

        public SiteUserModel GetEmpByEmail(string emailAddress)
        {
            try
            {
                SiteUserModel user = _context.SiteUsers.FirstOrDefault(u => u.EmailAddress == emailAddress);
                return user;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public CustomSiteUserModel ConvertSiteUserToCustomSiteUser(SiteUserModel user)
        {

            CustomSiteUserModel customSiteUserModel = new CustomSiteUserModel
            {
                SiteUserId = user.SiteUserId,
                CreateDate = user.CreateDate,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                RoleId = user.RoleId
            };
            return customSiteUserModel;
        }
    }

}
