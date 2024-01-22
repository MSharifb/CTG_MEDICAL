using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MFS_IWM.DAL.PRM;
using System.ComponentModel.DataAnnotations;

namespace MFS_IWM.Web.Areas.PRM.ViewModel
{
    public class EmployeePhotoGraphViewModel
    {

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsPhoto { get; set; }   
        public byte[] PhotoSignature { get; set; }     
        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
    }
}