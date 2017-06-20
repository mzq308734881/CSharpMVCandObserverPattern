<%@ Page Language="C#" AutoEventWireup="true" CodeFile="seal.aspx.cs" Inherits="ZoomSeal.Sealservice.seal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>印章管理</title>
    <style type="text/css">
        body, div, ul, li, h2, h5, h6, form, input, p, td
        {
            margin: 0;
            padding: 0;
            font-size: 12px;
            list-style: none;
        }
        ul
        {
            list-style: none;
        }
        h2, h5, h6
        {
            font-size: 100%;
        }
        h2
        {
            color: #5b3510;
            padding-bottom: 10px;
            font-size: 14px;
        }
        p
        {
            color: #5b3510;
            line-height: 22px;
        }
        .mc
        {
            margin: 0 auto;
        }
        .fs-12
        {
            font-size: 12px;
        }
        a:focus
        {
            outline: 0;
        }
        a
        {
            blr: expression(this.onFocus=this.blur());
        }
        /* for IE7.0及以下版本*/:focus
        {
            outline-style: none;
        }
        /* for Firefox，IE8.0等 */　 table
        {
            border-collapse: collapse;
            border-spacing: 0;
        }
        body
        {
            font: 12px/1.6 "宋体" ,Arial, Helvetica, sans-serif;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
        a
        {
            text-decoration: none;
            cursor: pointer;
            color: #666;
        }
        a:hover
        {
            text-decoration: none;
        }
        a:active, a:focus
        {
            outline: none;
        }
        .clearfix:after
        {
            content: ".";
            display: block;
            height: 0;
            clear: both;
            visibility: hidden;
            font-size: 0;
        }
        .clearfix
        {
            zoom: 1;
        }
        .clear
        {
            clear: both;
            height: 0;
            line-height: 0;
            font-size: 0;
        }
        .fl
        {
            float: left;
        }
        .fr
        {
            float: right;
        }
        .zz-contentRight
        {
            width: 780px;
            height: auto !important;
            height: 452px;
            _height: expression(this.scrollHeight < 452 ? "652px" : "auto");
            min-height: 225px;
            margin: 0;
            padding-bottom: 50px;
        }
        /*head----------------------------------style*/.zz-headBox
        {
            width: 100%;
            height: 94px;
        }
        .zz-head
        {
            width: 986px;
        }
        .zz-head .logo
        {
            width: 450px;
            height: 61px;
            padding-top: 30px;
        }
        .zz-head .headRight
        {
            width: 248px;
        }
        .zz-head .headRight
        {
            width: 493px;
            height: 92px;
            background-color: Gray;
        }
        .zz-head .headRight
        {
            width: 140px;
            height: 24px;
            background-color: Gray;
            margin: 0 10px 0 0;
            _margin: 0 5px 0 0;
        }
        .zz-head .headRight ul
        {
            width: 130px;
        }
        .zz-head .headRight ul li
        {
            width: 42px;
            height: 14px;
            line-height: 14px;
            margin-top: 5px;
            float: left;
            text-align: center;
            border-left: 1px solid #999;
        }
        .zz-head .headRight ul li.bor
        {
            border: none;
        }
        .zz-head .headRight ul li a
        {
            color: #656565;
            text-decoration: none;
        }
        .zz-head .headRight ul li a:hover
        {
            color: #656565;
            text-decoration: none;
        }
        /*head right*/.head-rightUl
        {
            width: 248px;
            margin-top: 35px;
        }
        .head-rightUl li
        {
            width: 78px;
            height: 14px;
            line-height: 14px;
            float: left;
            text-align: center;
            border-right: 1px solid #d8d8d8;
        }
        .head-rightUl li.bor-0
        {
            border: none;
        }
        /*a title*/.topTitle
        {
            width: 100%;
            height: 28px;
            background: #00517e;
        }
        .topTitle ul
        {
            width: 986px;
            margin: 0 auto;
        }
        .topTitle ul li
        {
            overflow: hidden;
            padding: 0 16px;
            margin-top: 7px;
            float: left;
            line-height: 16px;
            color: #fff;
            border-left: 1px solid #2d7ca7;
        }
        .topTitle ul li.pd-left
        {
            border: none;
            padding-left: 0;
            color: #d0eaf8;
        }
        .topTitle ul li font
        {
            font-size: 12px;
            font-weight: bold;
            margin-right: 10px;
            color: #d0eaf8;
        }
        /*left*/.left-ul
        {
            width: 162px;
            border: 1px solid #a4c8de;
        }
        .left-ul h2
        {
            width: 162px;
            height: 34px;
            line-height: 34px;
            text-indent: 10px;
            background: #6096b2;
            padding: 0;
            color: #fff;
        }
        .left-ul li
        {
            width: 162px;
            height: 34px;
            margin: 0;
            padding: 0;
            line-height: 34px;
            background: #edf8fe;
            border-bottom: 1px solid #a4c8de;
            display: block;
            text-indent: 10px;
        }
        .left-ul li.bo-n
        {
            border-bottom: none;
        }
        .left-ul li a
        {
            text-decoration: none;
            display: block;
        }
        .left-ul li a:hover
        {
            text-decoration: none;
            background: #d0eaf7;
            display: block;
        }
        .zz-content
        {
            width: 986px;
            position: relative;
        }
        .zz-content.pd-28
        {
            padding: 28px 0;
        }
        /*图片居中*//*图片居中*/.frame
        {
            float: left;
            margin: 2px;
            height: 160px;
            width: 160px;
            overflow: hidden;
            background: white;
            position: static !important;
            position: relative;
            display: table !important;
            top: 0px;
            left: 0px;
            border: solid 1px gray;
        }
        /* 图片居中结束*/.addtr
        {
            text-align: center;
            color: #333333;
            font-size: 12px;
        }
        .spanAdd
        {
            font-size: 12px;
        }
        .inputAdd
        {
            font-size: 12px;
            width: 160px;
        }
        .titleAdd
        {
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            border-bottom: dotted 1px #ccc;
        }
        .sealAdd
        {
            width: 70px;
            text-align: left;
        }
    </style>

    <script type="text/javascript">
        //显示列表
        function ShowList() {
            if ('<%=flg %>'.toLowerCase()=="true") {
                document.getElementById("deleteAll").style.display = "none";
                document.getElementById("listDiv").style.display = "";
                document.getElementById("addDiv").style.display = "none";
                document.getElementById("addLi").style.backgroundColor = "#edf8fe";
                document.getElementById("listLi").style.backgroundColor = "#d0eaf7";
            }
            else{
                ShowDelAll();
            }
        }

        //显示添加印章信息
        function ShowAdd() {
            document.getElementById("deleteAll").style.display = "none";
            document.getElementById("addDiv").style.display = "";
            document.getElementById("listDiv").style.display = "none";
            document.getElementById("addLi").style.backgroundColor = "#d0eaf7";
            document.getElementById("listLi").style.backgroundColor = "#edf8fe";
        }

        //无印章时显示印章信息
        function ShowDelAll() {
            document.getElementById("deleteAll").style.display = "";
            document.getElementById("listDiv").style.display = "none";
            document.getElementById("addDiv").style.display = "none";
            document.getElementById("addLi").style.backgroundColor = "#edf8fe";
            document.getElementById("listLi").style.backgroundColor = "#d0eaf7";
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <!--header-->
    <div class="zz-headBox clearfix">
        <div class="zz-head mc">
            <!--logo-->
            <div class="logo fl">
                <span style="color: Gray; font: italic bolder 35px 宋体;">卓正电子印章简易管理平台</span></div>
            <!--logo end-->
            <ul class="head-rightUl fr">
                <li><a href="http://www.zhuozhengsoft.com">卓正软件网站</a></li>
                <li><a target="_blank" href="http://www.zhuozhengsoft.com/poask/index.asp">客户问吧</a></li>
                <li class="bor-0"><a href="http://www.zhuozhengsoft.com/contact-us.html">联系我们</a></li>
            </ul>
        </div>
    </div>
    <!--header end-->
    <!--a title-->
    <div class=" topTitle">
        <ul>
            <li class="pd-left">印章管理系统</li>
            <li><font>当前用户：</font><asp:Label ID="lblUserName" runat="server" Text="Label"></asp:Label></li>
            <li><font>当前系统日期：</font><asp:Label ID="lblNowTime" runat="server" Text="Label"></asp:Label></li>
        </ul>
    </div>
    <!--content-->
    <div class="zz-content mc clearfix pd-28">
        <!--left-->
        <div class="fl">
            <ul class="left-ul">
                <h2 class="fs-12">
                    用户功能区</h2>
                <li id="listLi" style="background: #d0eaf7"><a href="#" onclick="ShowList()">印章列表</a></li>
                <li id="addLi"><a href="#" onclick="ShowAdd()">添加印章</a></li>
                <li class="bo-n"><a href="#">
                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">退出系统</asp:LinkButton></a></li>
            </ul>
        </div>
        <!--right-->
        <div class="zz-contentRight fl" id="listDiv">
            <!--表格内容-->
            <div style="margin-left: 40px; width: 698px; clear: both;">
                <asp:GridView ID="GridView1" runat="server" OnRowDeleting="GridView1_RowDeleting"
                    OnRowDataBound="GridView1_RowDataBound" BackColor="White" BorderColor="#A4C8DE"
                    BorderStyle="Solid" BorderWidth="1px" CellPadding="3">
                    <RowStyle ForeColor="#666666" BorderColor="#A4C8DE" BorderStyle="Solid" BorderWidth="1px" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="编号">
                            <HeaderStyle Font-Bold="True" Font-Names="宋体" Font-Size="12px" Height="32" HorizontalAlign="Center"
                                VerticalAlign="Middle" Width="120px" />
                            <ItemStyle HorizontalAlign="Center" Height="32" BorderColor="#A4C8DE" BorderStyle="Solid"
                                BorderWidth="1px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SealName" HeaderText="印章名称">
                            <ControlStyle Font-Size="10pt" Width="200px" />
                            <FooterStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <HeaderStyle Font-Bold="True" Font-Names="宋体" Font-Size="12px" HorizontalAlign="Center"
                                VerticalAlign="Middle" Width="160px" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" BorderColor="#A4C8DE" BorderStyle="Solid"
                                BorderWidth="1px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SealType" HeaderText="印章类型">
                            <ControlStyle Font-Size="10pt" Width="60px" />
                            <HeaderStyle Font-Bold="True" Font-Names="宋体" Font-Size="12px" HorizontalAlign="Center"
                                VerticalAlign="Middle" Width="100px" />
                            <ItemStyle BorderColor="#A4C8DE" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DeptName" HeaderText="部门名称">
                            <ControlStyle Font-Size="10pt" />
                            <HeaderStyle Font-Bold="True" Font-Names="宋体" Font-Size="12px" HorizontalAlign="Center"
                                VerticalAlign="Middle" Width="140px" />
                            <ItemStyle BorderColor="#A4C8DE" BorderStyle="Solid" BorderWidth="1px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Status" HeaderText="印章状态">
                            <ControlStyle Font-Size="10pt" />
                            <HeaderStyle Font-Bold="True" Font-Names="宋体" Font-Size="12px" HorizontalAlign="Center"
                                VerticalAlign="Middle" Width="100px" />
                            <ItemStyle BorderColor="#A4C8DE" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SignerName" HeaderText="签章人">
                            <ControlStyle Font-Size="10pt" />
                            <HeaderStyle Font-Bold="True" Font-Names="宋体" Font-Size="12px" HorizontalAlign="Center"
                                VerticalAlign="Middle" Width="120px" />
                            <ItemStyle BorderColor="#A4C8DE" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:CommandField CancelText="" EditText="" InsertText="" NewText="" SelectText=""
                            ShowCancelButton="False" ShowDeleteButton="True" UpdateText="" HeaderText="操作">
                            <HeaderStyle Font-Names="宋体" Font-Size="12px" HorizontalAlign="Center" VerticalAlign="Middle"
                                Width="60px" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#3366CC" BorderColor="#A4C8DE"
                                BorderStyle="Solid" BorderWidth="1px" />
                        </asp:CommandField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BorderColor="#A2C5D9" />
                </asp:GridView>
            </div>
        </div>
        <div id="addDiv" class="zz-contentRight fl" style="display: none;">
            <!--表格内容-->
            <div style="border: solid 1px #6096b2; margin-left: 40px; text-align: center; width: 580px;
                vertical-align: middle;">
                <table style="margin: 20px 0px 20px 50px;">
                    <!-添加印章-->
                    <tr class="addtr">
                        <td colspan="2" style="vertical-align: top;">
                            <div class="titleAdd">
                                印章信息</div>
                        </td>
                        <td rowspan="4" style="text-align: center; width: 220px; padding-left: 50px;">
                            <div class="frame">
                                <table width="100%" style="height: 155px;">
                                    <tr>
                                        <td>
                                            <asp:Image ID="Image1" runat="server" Style="vertical-align: middle;" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr class="addtr">
                        <td class="sealAdd">
                            <span class="spanAdd">印章名称</span>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSealName" runat="server" CssClass="inputAdd"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="addtr">
                        <td class="sealAdd">
                            <span class="spanAdd">签&nbsp;章&nbsp;人</span>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSignerName" runat="server" CssClass="inputAdd"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="addtr">
                        <td class="sealAdd">
                            <span class="spanAdd">印章类型</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="dropSealType" runat="server" Font-Size="9" Width="163px">
                                <asp:ListItem>印章</asp:ListItem>
                                <asp:ListItem>手写</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="addtr">
                        <td colspan="2" style="text-align: right; vertical-align: bottom; height: 50px;">
                            <asp:Button ID="btnAddSeal" runat="server" Font-Size="9" Text="添加印章" Width="80px"
                                OnClick="btnAddSeal_Click" TabIndex="5" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <input id="Reset1" type="reset" style="width: 60px;" value="重置" />
                        </td>
                        <td style="padding-left: 10px; vertical-align: top;">
                            <asp:FileUpload ID="FileUpload1" runat="server" Width="140px" />
                            &nbsp;<asp:Button ID="btnAddPic" runat="server" Text="上传" Width="35px" OnClick="btnAddPic_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="zz-contentRight fl" id="deleteAll" style="display: none;">
            <!--表格内容-->
            <div style="margin-left: 40px; width: 698px; clear: both;">
                <table cellspacing="0" cellpadding="3" rules="all" border="1" id="Table1" style="background-color: White;
                    border-color: #A4C8DE; border-width: 1px; border-style: solid; font-size: 12px;
                    width: 100%; border-collapse: collapse;">
                    <tr style="color: White; background-color: #006699; font-weight: bold; height: 32px;">
                        <th scope="col" style="width: 120px;">
                            编号
                        </th>
                        <th scope="col" style="width: 160px;">
                            印章名称
                        </th>
                        <th scope="col" style="width: 100px;">
                            印章类型
                        </th>
                        <th scope="col" style="width: 140px;">
                            部门名称
                        </th>
                        <th scope="col" style="width: 100px;">
                            印章状态
                        </th>
                        <th scope="col" style="width: 120px;">
                            签章人
                        </th>
                        <th scope="col" style="width: 60px;">
                            操作
                        </th>
                    </tr>
                    <tr style="color: #000066; height: 32px;">
                        <td colspan="7" style="text-align: center; color: #666666">
                            无印章。
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <!--content end-->
    </form>
</body>
</html>
