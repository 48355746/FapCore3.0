(function($){
  $.fn.limitTextarea = function(opts){
	  var defaults = {
        maxNumber:140,//允许输入的最大字数
		position:'top',//提示文字的位置，top：文本框上方，bottom：文本框下方
		onOk:function(){},//输入后，字数不足调用的函
		onLimit:function(){},//输入后，字数刚好相等调用的函数  
		onOver:function(){},//输入后，字数超出时调用的函数
	  }
	  var option = $.extend(defaults,opts);
	  this.each(function(){
		  var _this = $(this);
		  var info = '<div id="info">还可以输入<b>'+(option.maxNumber- _this.val().length)+'</b>字</div>';
		  var fn = function(){
			var extraNumber = option.maxNumber - _this.val().length;
			var $info = $('#info');
			if(extraNumber>0){
			  $info.html('');	
			  option.onOk();
			}else if(extraNumber==0){
				$info.html('最多输入100个字');
				option.onLimit();
			}else{
			  //$info.html('已经超出<b style="color:red;">'+(-extraNumber)+'</b>个字'); 
			  option.onOver();
			}  
		  };
		  switch(option.position){
			  case 'top' :
			    _this.before(info);
			  break;
			  case 'bottom' :
			  default :
			    _this.after(info);
		  }
		  //绑定输入事件监听器
		  if(window.addEventListener) { //先执行W3C
			_this.get(0).addEventListener("input", fn, false);
		  } else {
			_this.get(0).attachEvent("onpropertychange", fn);
		  }
		  if(window.VBArray && window.addEventListener) { //IE9
			_this.get(0).attachEvent("onkeydown", function() {
			  var key = window.event.keyCode;
			  (key == 8 || key == 46) && fn();//处理回退与删除
			});
			_this.get(0).attachEvent("oncut", fn);//处理粘贴
		  }		  
	  });   
  }	
})(jQuery)