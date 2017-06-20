using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZoomSeal.Sealservice
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string Errmsg = "";
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtUserPw.Text))
            {
                Errmsg = "请输入用户名和密码！";
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('" + Errmsg + "');</script>");
                return;
            }

            if (!txtUserName.Text.Trim().Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                !txtUserPw.Text.Trim().Equals("111111", StringComparison.OrdinalIgnoreCase))
            {
                Errmsg = "用户名或密码错误！";
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('" + Errmsg + "');</script>");
                return;
            }
            else
            {
                Session["userName"] = txtUserName.Text.Trim();
                Session["userPw"] = txtUserPw.Text.Trim();
                Response.Redirect("seal.aspx");
            }
        }
    }

}