using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZoomSeal.Sealservice
{
    public partial class sealPic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userName"] == null || Session["userName"].ToString() == "" || Session["userPw"] == null || Session["userPw"].ToString() == "")
            {
                Response.Redirect("login.aspx");
            }
            //上传时
            if (Session["imagePic"] != null && Session["imagePic"].ToString().Trim().Length > 0 && Session["fileExPic"] != null && Session["fileExPic"].ToString().Length > 0)
            {
                System.Byte[] b = (Byte[])Session["imagePic"];
                string imgType = "image/" + Session["fileExPic"].ToString();
                string imgName = "down." + imgType;

                Response.ContentType = imgType;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + imgName);
                Response.AddHeader("Content-Length", b.Length.ToString());
                this.Response.Clear();
                System.IO.Stream fs = this.Response.OutputStream;
                fs.Write(b, 0, b.Length);
                fs.Close();

                Session["imagePic"] = null;
                Session["fileExPic"] = null;
            }
        }
    }
}
