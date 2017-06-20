<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="ZoomSeal.Sealservice.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" title="currentStyle" media="screen" mce_bogus="1">
        #loginDiv
        {
            position: absolute; /*层漂浮*/
            top: 50%;
            left: 50%;
            width: 400px;
            height: 180px;
            margin-top: -90px; /*注意这里必须是DIV高度的一半*/
            margin-left: -200px; /*这里是DIV宽度的一半*/
            background: #edf8fe;
            border: 1px solid #d0eaf7;
            color:SteelBlue;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="loginDiv">
    <div style=" border-bottom:solid 1px #d0eaf7; font-size:12px; padding-left:10px; height:22px; vertical-align:middle; padding-top:8px;   ">印章管理</div>
        <table style="margin:18px auto auto auto; display: table-cell;vertical-align:middle;">

            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="用户名：" Font-Size="9pt"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtUserName" TabIndex="1"  runat="server" Font-Size="9pt" Height="20px" Width="120px"></asp:TextBox>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="密&nbsp;&nbsp;&nbsp;&nbsp;码：" Font-Size="9pt"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtUserPw" runat="server" Font-Size="9pt" Height="20px" 
                        Width="120px" TabIndex="2" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
            <td colspan=3 height=10 > </td>
            </tr>
            <tr>
                <td>
                </td>
                <td align="left">
                    <asp:Button ID="btnLogin" runat="server" Text="登录" style=" color:#003D79;" 
                        Height="22px" OnClick="btnLogin_Click" TabIndex="3" />
                    &nbsp;<asp:Button ID="btnCancel" runat="server" Text="取消" style=" color:#003D79;" 
                        Height="22px" TabIndex="4" />
                </td>
                <td>
                </td>
            </tr>

        </table>
    </div>
    </form>
</body>
</html>
