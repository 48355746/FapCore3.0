/*!
 * Copyright (c) 2012 twairball@yahoo.com
 * Dual licensed under the MIT and GPL licenses.
 * use at your own risk.
 */

// GitHub          - http://github.com/twairball/jquery-citypicker
// jQuery Versions - 1.8.2
// Browsers Tested - Chrome, IE, 360, Safari, Firefox

// Usage 
// <input class="citypicker" type="text">
// $('.citypicker').citypicker();
;


$.fn.cityselect = function () {
    //twitter bootstrap dropdown div
    var _dropdownDiv = $('<div class="dropdown"></div>');
    var _dropdownMenu = $('<dl class="dropdown-menu  dl-horizontal" style="position:fixed;margin:auto;left:0; right:0; top:0; bottom:0;padding:20px;"></dl>');
    var _cities = {
        "华东华北": ["北京", "天津", "沈阳", "大连", "长春", "哈尔滨", "石家庄", "太原", "呼和浩特"],
        "华东地区": ["上海", "杭州", "南京", "苏州", "无锡", "济南", "厦门", "宁波", "福州", "青岛", "合肥", "常州"],
        "中部西部": ["武汉", "郑州", "长沙", "南昌", "贵阳", "南宁", "成都", "重庆", "西安", "昆明", "兰州", "乌鲁木齐"],
        "华南地区": ["广州", "深圳", "佛山", "珠海", "东莞", "厦门", "海口", "南宁"],
        "港澳台": ["香港", "台湾", "澳门"]
    };


    var $this = $(this);
    $.each(_cities, function (area, citylist) {

        _dropdownMenu.append('<dt>' + area + '</dt>');
        var _ul = $('<ul class="list-inline"></ul>');
        // add cells for each city
        $.each(citylist, function (index, city) {
            var _a = $('<a href="#">' + city + '</a>');
            _a.click(function () {
                $this.val(city);
            });
            _ul.append(_a);
            _a.wrap('<li></li>');
        });
        _dropdownMenu.append(_ul);
        _ul.wrap($('<dd></dd>'));
    });

    // wraps and appends
    this.wrap(_dropdownDiv);
    this.parent().append(_dropdownMenu);
    this.wrap('<a class="dropdown-toggle" data-toggle="dropdown" href="#"></a>');

};
