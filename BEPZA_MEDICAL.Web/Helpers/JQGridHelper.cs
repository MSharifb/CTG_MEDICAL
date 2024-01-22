using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Helpers
{
    public class JQGridRowsViewModel
    {
        public string[] cell { get; set; }
    }

    public static class JQGridHelper
    {
        public static string GenerateCheckBox(string name, string id, string text, bool isCheck)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk" + name + "'";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />&nbsp;&nbsp;" + text;


            return strCheckBox;
        }

        public static string GenerateRadioButton(string name, string id, string text, bool isCheck)
        {
            string strRadioButton = "<input type='radio' ";

            strRadioButton += "class='rb" + name + "'";
            strRadioButton += "id='rb" + name + "_" + id;
            strRadioButton += "' name='rb" + name + "_" + id;
            strRadioButton += "' value='" + id;
            if (isCheck)
            {
                strRadioButton += "' checked='checked";
            }
            strRadioButton += "' />&nbsp;&nbsp;" + text;




            return strRadioButton;
        }

        public static string GenerateGroupRadioButton(string name, string id, string text, bool isCheck, int numberButton)
        {
            string strRadioButton = "<ul class='rb-group'>";

            for (int i = 1; i <= numberButton; i++)
            {
                strRadioButton += "<li><input type='radio' ";

                strRadioButton += "class='rb" + name + "'";
                strRadioButton += "id='rb" + name + "_" + id;
                strRadioButton += "' name='rb" + name + "_" + id;
                strRadioButton += "' value='" + id;
                if (isCheck)
                {
                    strRadioButton += "' checked='checked";
                }
                strRadioButton += "' />&nbsp;&nbsp;" + text;

                strRadioButton += "</li>";
            }

            strRadioButton += "</ul>";

            return strRadioButton;
        }

        public static string GenerateValueForProject(string id, string text)
        {
            string strValue = @"<span class='project-no' id='projectNo_" + id + "' >" + text + "</span>";

            return strValue;
        }

        public static string GenerateCheckBoxForProject(string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-projectno' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='ProjectCheck(" + id + ");";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }

            strCheckBox += "' />&nbsp;&nbsp;<label class='lbl-projectno' ";

            strCheckBox += "id='chk" + name + "_" + id;

            strCheckBox += "' for='chk" + name + "_" + id;

            strCheckBox += "' >" + text + "</label>";

            return strCheckBox;
        }

        public static string GenerateCheckBoxForProject(string rowId, string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-projectno' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='ProjectCheck(" + rowId + ", " + id + ");";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }

            strCheckBox += "' />&nbsp;&nbsp;<label class='lbl-projectno' ";

            strCheckBox += "id='chk" + name + "_" + id;

            strCheckBox += "' for='chk" + name + "_" + id;

            strCheckBox += "' >" + text + "</label>";

            return strCheckBox;
        }

        public static string GenerateUpdateCheckBoxForProject(string rowId, string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-projectno' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='ProjectCheckForUpdate(" + rowId + ", " + id + ");";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }

            strCheckBox += "' />&nbsp;&nbsp;<label class='lbl-projectno' ";

            strCheckBox += "id='chk" + name + "_" + id;

            strCheckBox += "' for='chk" + name + "_" + id;

            strCheckBox += "' >" + text + "</label>";

            return strCheckBox;
        }

        public static string GenerateSubmitCheckBoxForProject(string rowId, string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-projectno' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='ProjectCheckForSubmit(" + rowId + ", " + id + ");";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }

            strCheckBox += "' />&nbsp;&nbsp;<label class='lbl-projectno' ";

            strCheckBox += "id='chk" + name + "_" + id;

            strCheckBox += "' for='chk" + name + "_" + id;

            strCheckBox += "' >" + text + "</label>";

            return strCheckBox;
        }

        public static string GenerateGroupRadioButtonForSubmitted(string name, string id, string textPL, string textPS, bool isCheckPL = true, bool isCheckPS = false)
        {
            string strRadioButton = "<ul class='ul-submitted' >";

            if (isCheckPL == true && isCheckPS == true)
            {
                isCheckPL = true;
                isCheckPS = false;
            }

            //PL Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id;
            strRadioButton += "' name='rbSubmittedTo_" + id;
            strRadioButton += "' onclick='SubmittedToCheck(" + id + ", rbSubmittedToPL_" + id + ", rbSubmittedToPS_" + id + ", this);";
            strRadioButton += "' value='" + id;

            if (isCheckPL)
            {
                strRadioButton += "' checked='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPL + "&nbsp;&nbsp;";
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id;
            strRadioButton += "' for='rbSubmittedToPL_" + id;
            strRadioButton += "' > PL (" + textPL + ") </label>";
            strRadioButton += "</li>";

            //PS Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id;
            strRadioButton += "' name='rbSubmittedTo_" + id;
            strRadioButton += "' onclick='SubmittedToCheck(" + id + ", rbSubmittedToPL_" + id + ", rbSubmittedToPS_" + id + ", this);";
            strRadioButton += "' value='" + id;

            if (isCheckPS)
            {
                strRadioButton += "' checked='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPS;
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id;
            strRadioButton += "' for='rbSubmittedToPS_" + id;
            strRadioButton += "' > PS (" + textPS + ") </label>";
            strRadioButton += "</li>";

            strRadioButton += "</ul>";

            return strRadioButton;
        }

        public static string GenerateGroupRadioButtonForSubmitted(string name, string id, string pLId, string pSId, string textPL, string textPS, bool isCheckPL = true, bool isCheckPS = false)
        {
            string strRadioButton = "<ul id='rbSubmitted_" + id + "' class='ul-submitted' >";

            if (isCheckPL == true && isCheckPS == true)
            {
                isCheckPL = true;
                isCheckPS = false;
            }

            //PL Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id;
            strRadioButton += "' name='rbSubmittedTo_" + id;
            strRadioButton += "' onclick='SubmittedToCheck(" + id + ", rbSubmittedToPL_" + id + ", rbSubmittedToPS_" + id + ", this);";
            strRadioButton += "' value='" + pLId;

            if (isCheckPL)
            {
                strRadioButton += "' CHECKED='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPL + "&nbsp;&nbsp;";
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id;
            strRadioButton += "' for='rbSubmittedToPL_" + id;
            strRadioButton += "' > PL (" + textPL + ") </label>";
            strRadioButton += "</li>";

            //PS Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id;
            strRadioButton += "' name='rbSubmittedTo_" + id;
            strRadioButton += "' onclick='SubmittedToCheck(" + id + ", rbSubmittedToPL_" + id + ", rbSubmittedToPS_" + id + ", this);";
            strRadioButton += "' value='" + pSId;

            if (isCheckPS)
            {
                strRadioButton += "' CHECKED='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPS;
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id;
            strRadioButton += "' for='rbSubmittedToPS_" + id;
            strRadioButton += "' > PS (" + textPS + ") </label>";
            strRadioButton += "</li>";

            strRadioButton += "</ul>";

            return strRadioButton;
        }

        public static string GenerateGroupRadioButtonForSubmitted(string rowId, string name, string id, string idPL, string idPS, string textPL, string textPS, bool isCheckPL = true, bool isCheckPS = false)
        {
            string strRadioButton = "<ul id='rbSubmitted_" + id + "' class='ul-submitted' >";

            if (isCheckPL == true && isCheckPS == true)
            {
                isCheckPL = true;
                isCheckPS = false;
            }

            //PL Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id + "_" + rowId;
            strRadioButton += "' name='rbSubmittedTo_" + id + "_" + rowId;
            strRadioButton += "' onclick='SubmittedToCheck(" + id + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", this);";
            strRadioButton += "' value='" + idPL;

            if (isCheckPL)
            {
                strRadioButton += "' CHECKED='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPL + "&nbsp;&nbsp;";
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id + "_" + rowId;
            strRadioButton += "' for='rbSubmittedToPL_" + id + "_" + rowId;
            strRadioButton += "' > PL (" + textPL + ") </label>";
            strRadioButton += "</li>";

            //PS Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id + "_" + rowId;
            strRadioButton += "' name='rbSubmittedTo_" + id + "_" + rowId;
            strRadioButton += "' onclick='SubmittedToCheck(" + id + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", this);";
            strRadioButton += "' value='" + idPS;

            if (isCheckPS)
            {
                strRadioButton += "' CHECKED='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPS;
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id + "_" + rowId;
            strRadioButton += "' for='rbSubmittedToPS_" + id + "_" + rowId;
            strRadioButton += "' > PS (" + textPS + ") </label>";
            strRadioButton += "</li>";

            strRadioButton += "</ul>";

            return strRadioButton;
        }

        public static string GenerateGroupRadioButtonForSubmitted(string rowId, string projectId, string name, string id, string idPL, string idPS, string textPL, string textPS, bool isCheckPL = true, bool isCheckPS = false)
        {
            string strRadioButton = "<ul id='rbSubmitted_" + id + "' class='ul-submitted' >";

            if (isCheckPL == true && isCheckPS == true)
            {
                isCheckPL = true;
                isCheckPS = false;
            }

            //PL Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id + "_" + rowId;
            strRadioButton += "' name='rbSubmittedTo_" + id + "_" + rowId;
            //strRadioButton += "' onclick='SubmittedToCheck(" + projectId + ", " + id + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", this);";
            strRadioButton += "' onclick='SubmittedToCheck(" + projectId + ", " + id + ", " + 1 + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", this);"; //1 is PL
            //strRadioButton += "' onclick='SubmittedToCheck(" + projectId + ", " + id + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", PL, this);";
            strRadioButton += "' value='" + idPL;

            if (isCheckPL)
            {
                strRadioButton += "' CHECKED='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPL + "&nbsp;&nbsp;";
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPL_" + id + "_" + rowId;
            strRadioButton += "' for='rbSubmittedToPL_" + id + "_" + rowId;
            strRadioButton += "' > PL (" + textPL + ") </label>";
            strRadioButton += "</li>";

            //PS Radio
            strRadioButton += "<li>";
            strRadioButton += "<input type='radio' ";
            strRadioButton += "class='rb-submitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id + "_" + rowId;
            strRadioButton += "' name='rbSubmittedTo_" + id + "_" + rowId;
            //strRadioButton += "' onclick='SubmittedToCheck(" + projectId + ", " + id + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", this);";
            strRadioButton += "' onclick='SubmittedToCheck(" + projectId + ", " + id + ", " + 2 + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", this);"; //2 is PS
            //strRadioButton += "' onclick='SubmittedToCheck(" + projectId + ", " + id + ", rbSubmittedToPL_" + id + "_" + rowId + ", rbSubmittedToPS_" + id + "_" + rowId + ", PS, this);";
            strRadioButton += "' value='" + idPS;

            if (isCheckPS)
            {
                strRadioButton += "' CHECKED='checked";
            }

            //strRadioButton += "' />&nbsp;&nbsp;" + textPS;
            strRadioButton += "' />&nbsp;&nbsp;<label class='lblSubmitted' ";
            strRadioButton += "id='rbSubmittedToPS_" + id + "_" + rowId;
            strRadioButton += "' for='rbSubmittedToPS_" + id + "_" + rowId;
            strRadioButton += "' > PS (" + textPS + ") </label>";
            strRadioButton += "</li>";

            strRadioButton += "</ul>";

            return strRadioButton;
        }

        public static string GenerateCheckBoxForComplete(string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-complete' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='CompleteCheck(" + id + ", this);";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />&nbsp;&nbsp;" + text;


            return strCheckBox;
        }

        public static string GenerateCheckBoxForComplete(string rowId, string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-complete' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='CompleteCheck(" + rowId + ", " + id + ", this);";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />&nbsp;&nbsp;" + text;


            return strCheckBox;
        }

        public static string GenerateUpdateCheckBoxForComplete(string rowId, string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-complete' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='CompleteCheckForUpdate(" + rowId + ", " + id + ", this);";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />&nbsp;&nbsp;" + text;


            return strCheckBox;
        }

        public static string GenerateUpdateCheckBoxForComplete(string rowId, string name, string id, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-complete' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='CompleteCheckForUpdate(" + rowId + ", " + id + ", this);";
            strCheckBox += "' value='" + id;
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />";


            return strCheckBox;
        }

        public static string GenerateSubmitCheckBoxForComplete(string rowId, string name, string id, string text, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-complete' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='CompleteCheckForSubmit(" + rowId + ", " + id + ", this);";
            strCheckBox += "' value='" + id;
            strCheckBox += "' disabled='true";
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />&nbsp;&nbsp;" + text;


            return strCheckBox;
        }

        public static string GenerateSubmitCheckBoxForComplete(string rowId, string name, string id, bool isCheck = false)
        {
            string strCheckBox = "<input type='checkbox' ";

            strCheckBox += "class='chk-complete' ";
            strCheckBox += "id='chk" + name + "_" + id;
            strCheckBox += "' name='chk" + name + "_" + id;
            strCheckBox += "' onclick='CompleteCheckForSubmit(" + rowId + ", " + id + ", this);";
            strCheckBox += "' value='" + id;
            strCheckBox += "' disabled='true";
            if (isCheck)
            {
                strCheckBox += "' checked='checked";
            }
            strCheckBox += "' />";


            return strCheckBox;
        }

        public static string GenerateValueForApprovalStatus(string id, string text)
        {
            string strValue = @"<span class='approval-status' id='approvalStatus_" + id + "' >" + text + "</span>";

            return strValue;
        }

        public static string GenerateValueForActivity(string id, string text)
        {
            string strValue = @"<span class='activity-status' id='activity_" + id + "' >" + text + "</span>";

            return strValue;
        }

    }
}