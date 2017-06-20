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


