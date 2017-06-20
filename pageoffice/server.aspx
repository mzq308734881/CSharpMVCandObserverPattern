<%@ Page Language="C#" AutoEventWireup="true" CodeFile="server.aspx.cs" Inherits="pageoffice.server" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>卓正PageOffice开发平台服务</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h3>欢迎使用 PageOffice 产品</h3>
        <p>PageOffice 注册信息：</p>
        <asp:Label ID="LabelReg" runat="server" Text=""></asp:Label>
        <br />
        <p>PageOffice 运行信息：</p>
        <asp:Label ID="LabelLog" runat="server" Text="没有日志信息。"></asp:Label>
    </div>
    </form>
</body>
</html>
