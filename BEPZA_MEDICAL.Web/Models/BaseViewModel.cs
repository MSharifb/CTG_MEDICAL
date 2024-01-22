using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web
{
    public class BaseViewModel
    {
        public int Id { get; set; }
        public string ActionType { get; set; }
        public bool IsSuccessful { get; set; }

        //------------------
        private String _message;
        public string Message { get { return _message; } set { _message = value; ErrMsg = value; } }

        public string ErrMsg { get; set; }

        private int _isError;
        public int IsError { get { return _isError; } set { _isError = value; errClass = value == 0 ? "success" : "failed"; } }

        public string errClass { get; set; }
        //------------------

        public bool IsButtonHide { get; set; }

        public string strMode { get; set; }

        public bool IsInEditMode { get; set; }

        public string SideBarClassName { get; set; }


        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }

        public bool IsUsed { get; set; }

        public string IUser { get; set; }
        public Nullable<System.DateTime> IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }

    public class LongBaseViewModel
    {
        public long Id { get; set; }
        public string ActionType { get; set; }
        public bool IsSuccessful { get; set; }

        //------------------------------------------
        private String _message;
        public string Message { get { return _message; } set { _message = value; ErrMsg = value; } }

        public string ErrMsg { get; set; }

        private int _isError;
        public int IsError { get { return _isError; } set { _isError = value; errClass = value == 0 ? "success" : "failed"; } }

        public string errClass { get; set; }
        //------------------------------------------

        public string strMode { get; set; }

        public bool IsInEditMode { get; set; }

        public string SideBarClassName { get; set; }

        
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public bool IsUsed { get; set; }
        public string IUser { get; set; }
        public Nullable<System.DateTime> IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }

}