$(document).ready(function () {
    document.body.style.overflow = 'hidden';
    $(".tydvrg_time_show").live("click", (function () {
        $(this).parent().siblings("#tydvrg_hf_cp").slideToggle("slow");
    }));
    $(".xslw_manven_project").live('click', (function () { $(this).siblings(".xslw_manven_project_content").slideToggle("slow"); }));
    $(".tyhg1 dd a").live('click', function () {
        $(".tyhg1 dd a").each(function () { $(this).removeClass() });
        $(this).attr("class", "ty_tcurrt");
    });
    //var hei_min_max = document.documentElement.clientHeight;

    //if (hei_min_max <= 702) {

    //    $(".nt_list ul li").css(
    //        {
    //            'height': '25px',
    //            'line-height': '25px',
    //            'border-bottom': '1px solid #ccc'
    //        }

    //        );
    //}
    $(".menu").click(function () {
        if ($(this).hasClass("cura")) {
            $(this).children(".new-sub").hide(); //当前菜单下的二级菜单隐藏
            $(".menu").removeClass("cura"); //同一级的菜单项
        } else {
            $(".menu").removeClass("cura"); //移除所有的样式
            $(this).addClass("cura"); //给当前菜单添加特定样式
            $(".menu").children(".new-sub").slideUp("fast"); //隐藏所有的二级菜单
            $(this).children(".new-sub").slideDown("fast"); //展示当前的二级菜单
        }
    });
})