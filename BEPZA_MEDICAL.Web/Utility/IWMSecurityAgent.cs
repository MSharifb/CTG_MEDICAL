using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.Web.SecurityService;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class IWMSecurityAgent
    {
        #region User Info Operation
        
        public static Int32 InsertUserData(User user)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.InsertUserData(user);
            }            
            
        }

        public static void DeleteUserData(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                umService.DeleteUserData(id);
            }            
        }

        public static int UpdateUserData(User user)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateUserData(user);
            }
        }

        public static int InserUserRoles(int userId, List<Role> roleList,int applicationId,int moduleId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.InsertUserRoles(userId, roleList,applicationId,moduleId);
            }
        }
        
        public static List<User> GetUserList()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                List<User> list = umService.GetUserList();
                return list;
            }

        }

        
        public static User GetUser(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetUser(id);
            }
        }

        public static User GetUserByLoginId(string loginId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetUserByLoginId(loginId);
            }
        }

        public static List<User> GetUserListByCraiteria(User user)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetUserListByCraiteria(user);
            }
        }
        


        #endregion
        
        #region Group Operation

        public static Int32 CreateGroup(UserGroup item,List<Role> roleList)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.CreateGroup(item,roleList);
            }
        }

        public static Int32 UpdateGroup(UserGroup item,List<Role> roleList)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateGroup(item, roleList);
            }
        }


        public static Int32 DeleteGroup(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.DeleteGroup(id);
            }
        }

        public static UserGroup GetUserGroupById(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetGroupById(id);
            }
        }

        public static List<UserGroup> GetGroupList()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetGroupList();
            }
        }

        #endregion

        #region Roles
        public static List<Role> GetRoleList()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRolesList();
            }
        }

        public static List<Role> GetRoleListByCraiteria(Role role)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRolesListByCraiteria(role);
            }
        }

        public static List<RoleGroup> GetRoleGroupList()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRoleGroups();
            }
        }

        public static List<Role> GetRoleListByUser(int userId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRolesListByUser(userId);
            }
        }

        public static List<Role> GetRolesListByUserGroup(int userGroupId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRolesListByUserGroup(userGroupId);
            }
        }

        public static Role GetRoleById(int roleId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRoleById(roleId);
            }
        }

        public static int InsertRole(Role role, List<Menu> menuList, List<Right> rightList)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.InsertRolesMenusRight(role, menuList, rightList);
            }
        }

        public static int UpdateRole(Role role, List<Menu> menuList, List<Right> rightList)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateRolesMenusRight(role, menuList, rightList);
            }
        }

        public static  Int32 CreateRoleGroup(RoleGroup item)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.CreateRoleGroup(item);
            }
        }


        public static  Int32 UpdateRoleGroup(RoleGroup item)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateRoleGroup(item);
            }
        }


        public static  Int32 DeleteRoleGroup(int Id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.DeleteRoleGroup(Id);
            }
        }


        public static RoleGroup GetRoleGroupById(int Id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRoleGroupById(Id);
            }
        }

       

        #endregion

        #region Menus


        public static int InsertMenuData(Menu menu)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.InsertMenuData(menu);
            }
        }


        public static int UpdateMenuData(Menu menu)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateMenuData(menu);
            }
        }


        public static int DeleteMenuData(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.DeleteMenuData(id);
            }
        }

        public static List<Menu> GetAllMenus()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetAllMenus();
            }
        }

        public static List<Menu> GetMenusByParent(int parentId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenusByParent(parentId);
            }
        }

        public static List<Menu> GetMenusByRoleId(int roleId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenuListByRoleId(roleId);
            }
        }


        public static Menu GetMenusByMenuId(int menuId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenuByMenuId(menuId);
            }
        }

        public static Menu GetMenuByMenuNameAndLoginId(string loginId, string MenuName)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenuByMenuNameAndLoginId(loginId, MenuName);
            }

        }

        public static List<Menu> GetMenusByApplicationAndModuleId(int roleId,int applicationId, int moduleId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenusByApplicationAndModuleId(roleId,applicationId, moduleId);
            }
        }


        public static List<Menu> GetMenusByApplicationAndModuleName(string appName, string modName)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenusByApplicationAndModuleName(appName, modName);
            }
        }

        public static List<Menu> GetMenus(string loginId,string appName, string modName)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetMenus(loginId, appName, modName);
            }
        }


        

        #endregion

        #region Rights
        public static List<Right> GetAllRights()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetAllRights();
            }
        }

        public static List<Right> GetAllRightsMapedByRole(int roleId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetAllRightsMapedByRole(roleId);
            }
        }

        public static Right GetRightByLoginIdAndRightName(string loginId, string rightName)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRightByLoginIdAndRightName(loginId, rightName);
            }

        }

        public static List<Right> GetRightByRoleAndAppAndModule(int roleId,int appId,int moduleId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetRightsByRoleAndAppAndModule(roleId, appId, moduleId);
            }

        }


        #endregion


        #region "Department"
            public static List<Department> GetAllDepartment()
            {
                using (UserManagementServiceClient umService = new UserManagementServiceClient())
                {
                    return umService.GetAllDepartment();
                }
            }
        #endregion

        #region "Employee"
        public static List<Employee> GetAllEmployee()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
               return umService.GetAllEmployee();
            }
        }

        public static List<Employee> GetAllEmployeeWithPaging(int startRowIndex,int maxRow)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetAllEmployeeWithPaging(startRowIndex, maxRow);
            }
        }


        public static List<Employee> GetSearchEmployeeWithPaging(Employee obj, int startRowIndex, int maxRow, out int intTotalRowCount)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetSearchEmployeeWithPaging(out intTotalRowCount,obj, startRowIndex, maxRow);
            }
        }
       

        public static Int32 GetTotalEmployeeCount()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetTotalEmployeeCount();
            }
        }
        
        #endregion

        #region Application
        public static List<Application> GetAllApplications()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetAllApplications();
            }
        }


        public static Application GetApplicationById(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetApplicationById(id);
            }
        }


        public static int CreateApplication(Application app)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.InsertApplicationData(app);
            }
        }

        public static int UpdateApplication(Application app)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateApplicationData(app);
            }
        }
        #endregion


        #region Module
        public static List<Module> GetAllModules()
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetAllModules();
            }
        }

        public static List<Module> GetModulesByApplicationId(int appId)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetModulesByApplicationId(appId);
            }
        }



        public static Module GetModuleById(int id)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.GetModuleById(id);

            }
        }


        public static int CreateModule(Module module)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.InsertModuleData(module);
            }
        }

        public static int UpdateModule(Module module)
        {
            using (UserManagementServiceClient umService = new UserManagementServiceClient())
            {
                return umService.UpdateModuleData(module);
            }
        }
        #endregion

        #region Validation

        //public static IEnumerable<RuleViolation> GetUserRuleViolations(User user)
        //{
        //    return null;
        //}

        public void ValidateUser(User user)
        {

        }

        #endregion
    }    
}