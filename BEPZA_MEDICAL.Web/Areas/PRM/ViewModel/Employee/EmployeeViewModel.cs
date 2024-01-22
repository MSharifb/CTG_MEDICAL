using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee
{
    public class EmployeeViewModel
    {
        #region Ctor
                
        public EmployeeViewModel()
        {
            EmploymentInfo = new EmploymentInfoViewModel();
            EmploymentContractInfo = new EmploymentContractPeriodViewModel();
            EmployeePhotograph = new EmployeePhotoGraphViewModel();
            EmployeeSalary = new EmployeeSalaryStructureViewModel();
            UserAccountModel = new CreateUserAccountViewModel();
        }

        #endregion

        #region Standerd Property

        public int Id { get; set; }

        public string EmpId { get; set; }        
  
        public EmploymentInfoViewModel EmploymentInfo
        {
            get;
            set;
        }

        public EmploymentContractPeriodViewModel EmploymentContractInfo
        {
            get;
            set;
        }

        public EmployeeSalaryStructureViewModel EmployeeSalary { get; set; }

        public EmployeePhotoGraphViewModel EmployeePhotograph { get; set; }

        public CreateUserAccountViewModel UserAccountModel { get; set; } 

        #endregion

        #region Others

        public string ViewType
        {
            get;
            set;
        }

        #endregion
    }

    public class CreateUserAccountViewModel
    {
        public CreateUserAccountViewModel() 
        { 
        }

        [Required]
        [DisplayName("User ID")]
        public string UserID { get; set; }

        [Required]
        [DisplayName("User Email")]
        public string Email { get; set; }

    }
}