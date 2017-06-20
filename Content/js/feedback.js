var a=true;

$(".fankui_click img").click(function() {
	if (a == true) {
    	$("#feedback").animate({right:0});
     	a=false;
    }else{
    	$("#feedback").animate({right:-350});
    	a=true;
    }
});

$(".mk_fg span").click(function() {
        $(".mk_fg span").removeClass('ysbh_fankui');
     $(this).addClass('ysbh_fankui');
});