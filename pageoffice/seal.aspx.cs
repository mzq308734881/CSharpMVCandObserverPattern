using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ZoomSeal.Sealservice
{
    public partial class seal : System.Web.UI.Page
    {
        protected bool flg = true;//标识是否有印章
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userName"] == null || Session["userName"].ToString() == "" || Session["userPw"] == null || Session["userPw"].ToString() == "")
            {
                Response.Redirect("login.aspx");
            }
            lblUserName.Text = Session["userName"].ToString();
            lblNowTime.Text = DateTime.Now.ToString("yyyy/MM/dd");
            if (!IsPostBack)
            {
                PageOffice.ZoomSeal.SealManager sealMg = new PageOffice.ZoomSeal.SealManager(Server.MapPath("."));
                sealMg.DBConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=|DataDirectory|seal.mdb";
                //显示印章列表
                if (sealMg.GetQueryCollection("").Count > 0)
                {
                    flg = true;
                    ShowList(sealMg);
                }
                else
                {
                    flg = false;
                    ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowDelAll();</script>");
                }

            }
        }

        /// <summary>
        /// 显示印章列表
        /// </summary>
        /// <param name="sealMg"></param>
        private void ShowList(PageOffice.ZoomSeal.SealManager sealMg)
        {
            GridView1.AutoGenerateColumns = false;
            GridView1.DataSource = sealMg.GetQueryCollection(" order by ID desc");
            GridView1.DataBind();
        }

        /// <summary>
        /// 删除印章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {

                if (GridView1.Rows[e.RowIndex].Cells[0].Text != null && GridView1.Rows[e.RowIndex].Cells[0].Text.Trim().Length > 0)
                {
                    string id = GridView1.Rows[e.RowIndex].Cells[0].Text.Trim();

                    PageOffice.ZoomSeal.SealManager sealMg = new PageOffice.ZoomSeal.SealManager(Server.MapPath("."));
                    sealMg.DBConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=|DataDirectory|seal.mdb";

                    if (string.IsNullOrEmpty(id) || !sealMg.Exists(int.Parse(id)))
                    {
                        // Literal_JS.Text = "<script type='text/javascript'>alert('为获得印章的ID号，删除失败！');</script>";
                        ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('未获得印章的ID号，删除失败！');</script>");
                        return;
                    }

                    if (sealMg.Delete(int.Parse(id)))
                    {
                        ShowList(sealMg);
                        //Literal_JS.Text = "<script type='text/javascript'>alert('删除成功！');</script>";
                        if (sealMg.GetQueryCollection("").Count == 0)
                        {
                            flg = false;
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowDelAll();alert('删除成功！');</script>");
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('删除成功！');</script>");
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('删除失败！');</script>");
                    }


                }
            }
            catch (Exception ex)
            {
                // Literal_JS.Text = "<script type='text/javascript'>alert('删除失败！失败原因：" + ex.Message + "');</script>";
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('删除失败！失败原因：" + ex.Message + "');</script>");
                return;
            }

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
                {
                    ((LinkButton)e.Row.Cells[6].Controls[0]).Attributes.Add("onclick", "javascript:return confirm('你确认要删除\"编号\"为： " + e.Row.Cells[0].Text + " 的印章吗?')");
                }
            }
        }

        /// <summary>
        /// 添加印章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddSeal_Click(object sender, EventArgs e)
        {
            if (Session["imageByte"] == null || Session["imageByte"].ToString() == "")
            {
                //防止层跳转
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();alert('未获得上传图片路径，请重新上传！');</script>");
                Image1.ImageUrl = null;
                return;
            }

            if (string.IsNullOrEmpty(txtSealName.Text))
            {
                //防止层跳转
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();alert('请输入印章名称！');</script>");
                Image1.ImageUrl = null;
                return;
            }
            if (string.IsNullOrEmpty(txtSignerName.Text))
            {
                //防止层跳转
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();alert('请输入签章人姓名！');</script>");
                Image1.ImageUrl = null;
                return;
            }

            bool flg = false;//标识是否添加了用户 false：没有；true 有
            int userId = 0;//记录添加的用户Id
            PageOffice.ZoomSeal.SealManager sealManager = new PageOffice.ZoomSeal.SealManager(Server.MapPath("."));
            sealManager.DBConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=|DataDirectory|seal.mdb";

            PageOffice.ZoomSeal.UserManager userManager = new PageOffice.ZoomSeal.UserManager();
            userManager.DBConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=|DataDirectory|seal.mdb";
            PageOffice.ZoomSeal.Seal seal = new PageOffice.ZoomSeal.Seal();

            try
            {
                seal.SealName = txtSealName.Text.Trim();
                string signerName = txtSignerName.Text.Trim();

                if (userManager.Exists(signerName) <= 0)
                {
                    PageOffice.ZoomSeal.User user = new PageOffice.ZoomSeal.User();
                    user.DeptID = 1;
                    user.DeptName = sealManager.GetLicOrg();
                    user.Password = "111111";
                    user.UserName = signerName;
                    userId = userManager.Add(user);

                    if (userId <= 0)
                    {
                        //防止层跳转
                        ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();alert('用户添加失败！');</script>");
                        return;
                    }
                    else
                    {
                        flg = true;//标识为添加用户
                    }
                }

                seal.SignerID = userManager.Exists(signerName);
                seal.SignerName = signerName;
                seal.SealImage = (System.Byte[])Session["imageByte"];
                seal.SealImageType = "image/" + Session["fileEx"];
                seal.SealType = dropSealType.SelectedValue;
                seal.DeptID = 1;
                seal.DeptName = sealManager.GetLicOrg();
                seal.AuthType = "密码";
                int id = sealManager.Add(seal);//添加印章信息
                if (id != -1)
                {
                    sealManager.Grant(id);//颁发印章
                    flg = true;
                    ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script type='text/javascript'>ShowList();alert('印章添加成功！');</script>");
                    ShowList(sealManager);
                    txtSealName.Text = "";
                    txtSignerName.Text = "";
                    dropSealType.SelectedValue = "印章";
                    Image1.ImageUrl = "";
                }
                else
                {
                    //防止层跳转
                    ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();alert('印章添加失败！');</script>");
                    if (flg)
                    {
                        userManager.Delete(userId);//删除添加的用户
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                //防止层跳转
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('印章添加失败！失败原因：" + ex.Message + "');</script>");
                if (flg)
                {
                    //删除添加的用户
                    if (!userManager.Delete(userId))
                    {
                        ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>alert('新添加的用户删除失败，请在数据库中手动删除！');</script>");
                    }
                }

                Image1.ImageUrl = "";
                txtSealName.Text = "";
                txtSignerName.Text = "";
                dropSealType.SelectedValue = "印章";

                return;
            }

            Image1.ImageUrl = "";
            txtSealName.Text = "";
            txtSignerName.Text = "";
            dropSealType.SelectedValue = "印章";
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddPic_Click(object sender, EventArgs e)
        {
            #region   //图片存储
            if (!FileUpload1.HasFile)
            {
                //防止层跳转
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script type='text/javascript'>ShowAdd();alert('您还没有上传图片！');</script>");

                return;
            }
            else
            {
                HttpPostedFile hp = FileUpload1.PostedFile;//创建访问客户端上传文件的对象
                int uplength = FileUpload1.PostedFile.ContentLength;//图片大小

                string filepath = FileUpload1.PostedFile.FileName;//取得路径
                string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);//文件名称带后缀名
                string name = filename.Substring(0, filename.Length - 4);//文件名不带后缀名
                string fileEx = filepath.Substring(filepath.LastIndexOf(".") + 1).ToLower();//后缀名
                //判断图片格式
                if (fileEx == "jpg" || fileEx == "jpeg" || fileEx == "bmp" || fileEx == "gif" || fileEx == "png")
                {
                    string path = System.Environment.GetEnvironmentVariable("TEMP") + @"\" + filename;
                    FileUpload1.PostedFile.SaveAs(path);//先将文件上传到系统临时文件夹

                    Stream sr = hp.InputStream;//创建数据流对象
                    byte[] imageByte = new byte[uplength];//定义byte型数组
                    sr.Read(imageByte, 0, uplength);//将图片数据放到b数组对象实例中，其中0代表数组指针的起始位置，uplength表示要读取流的长度（指针的结束位置）

                    Session["fileEx"] = fileEx;
                    Session["imageByte"] = imageByte;

                    Session["imagePic"] = imageByte;
                    Session["fileExPic"] = fileEx;
                    Image1.ImageUrl = "sealPic.aspx";//动态页显示图片
                    //Session["path"] = Server.MapPath("lic/") + filename;
                }
                else
                {
                    //提示信息并防止层跳转
                    ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();alert('上传文件类型错误！\\r\\n请上传 .jpg|.jpeg|.bmp|.gif|.png 类型的图片！');</script>");

                    return;
                }

                //防止层跳转
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ShowAdd();</script>");
            }
            #endregion
        }
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("login.aspx");
        }
    }
}
