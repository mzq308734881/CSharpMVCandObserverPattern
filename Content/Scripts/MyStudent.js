$(document).ready(function(){
	document.body.style.overflow='hidden';
	  $(".tydvrg_time_show").click(function () {
      $(this).parent().siblings("#tydvrg_hf_cp").slideToggle("slow");
  	});

	  $("#nt_ul_click").click(function(){
	  		$(".nt_bn_sjq").css("display","none");
	  });


$(".lunwen_pos_nav dd").click(function(){
	var $this = $(this);
	var tindex = $this.index();
	// alert(tindex);
	$(".dis_o_"+tindex).addClass('disblock').removeClass('disnone');
});
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


