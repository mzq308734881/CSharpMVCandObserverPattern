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
            $(this).children(".new-sub").hide(); //��ǰ�˵��µĶ����˵�����
            $(".menu").removeClass("cura"); //ͬһ���Ĳ˵���
        } else {
            $(".menu").removeClass("cura"); //�Ƴ����е���ʽ
            $(this).addClass("cura"); //����ǰ�˵�����ض���ʽ
            $(".menu").children(".new-sub").slideUp("fast"); //�������еĶ����˵�
            $(this).children(".new-sub").slideDown("fast"); //չʾ��ǰ�Ķ����˵�
        }
    });
})