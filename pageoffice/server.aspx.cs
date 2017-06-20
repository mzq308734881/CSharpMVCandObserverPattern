// 重要提示：本页代码是卓正PageOffice开发平台的系统保留代码，请勿修改。
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace pageoffice
{
    public partial class server : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 重要提示：本页代码是卓正PageOffice开发平台的系统保留代码，请勿修改。
            PageOffice.POServer.Server ServerObj;
            try
            {
                // 如果出现“未将对象引用设置到对象的实例”的调试信息，请检查您在Web服务器上是否正确安装了卓正PageOffice开发平台的Setup.exe安装程序。
                ServerObj = new PageOffice.POServer.Server();
                ServerObj.DBConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=|DataDirectory|seal.mdb";
                ServerObj.Run();
            }
            catch
            {
                LabelReg.Text = "<span style=\"color:Red;\">请检查您在Web服务器上是否正确安装了卓正PageOffice开发平台的Setup.exe安装程序。</span>";
                return;
            }

            //出于安全考虑，您只能在Web服务器上通过localhost方式来查看本页显示的PageOffice系统信息。
            string ServerIP = Request.ServerVariables["HTTP_HOST"].ToLower();
            if ((ServerIP.StartsWith("localhost")) || (ServerIP.StartsWith("127.0.0.1")))
            {
                if (ServerObj.SerialNumber != "")
                {
                    LabelReg.Text = "注册码：" + ServerObj.SerialNumber + "<br>" + "版 本：" + ServerObj.VersionInfo + "<br>" + "用 户：" + ServerObj.Organization;
                    if (ServerObj.TrialEndTime != "")
                        LabelReg.Text = LabelReg.Text + "<br>" + "此产品是<span style=\"color:Red;\">试用版</span>，试用结束时间是 " + ServerObj.TrialEndTime + "。";
                }
                else
                {
                    LabelReg.Text = "<span style=\"color:Red;\">此产品尚未注册激活。</span>";
                }
                LabelLog.Text = ServerObj.GetStatusLog();
            }
            else
            {
                LabelLog.Text = ServerObj.GetStatusLog();
                Response.Write("出于安全考虑，您只能在Web服务器上通过localhost方式来查看本页显示的PageOffice系统信息。");
                Response.End();
            }
        }
    }
}
