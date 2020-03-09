/**
 *http://src.zctya.com/demo/datetime/2013052701/
 * jQuery DatePicker
 * author: admin@laoshu133.com
 * create: 2012.05.11
 * update: 2015.01.22
 *
 * -- 接口说明 --
 *    @Options {Object}
 *        - {
 *            shell: null,                          //触发元素，如果为input类，onselect会自动设置值
 *            shellTriggerEvent: 'click.DatePicker focus.DatePicker', //显示DatePicker的默认触发事件
 *            follow: null,                         //跟随元素，如果没有，则为 shell
 *            followOffset: [0, 0],                 //跟随偏移值[x, y]
 *            showMode: 0,                          //显示模式：0 - 年月日, 1 - 年月, 2 - 年
 *            autoHide: true,                       //是否自动隐藏，如果为 true，当触发 document的click时，会自动隐藏
 *            effect: 'show',                       //默认打开动画，基于JQ默认动画，默认：show
 *            effectDuration: 0,                    //动画时长，和effect配合使用
 *            altFormat: 'yyyy-mm-dd',              //返回时间格式
 *            unitYearSize: 12,                     //选择年份时，每页显示个数
 *            defaultDate: null,                    //默认值
 *            minDate: null,                        //最小值，默认：1680-1-1
 *            maxDate: null,                        //最大值，默认：2999-12-12
 *            onselect: ds.noop,                    //选择时，回调函数
 *            onmouseenter: ds.noop,                //鼠标进入DatePicker区域，触发
 *            onmouseleave: ds.noop                 //鼠标离开DatePicker区域，触发
 *        }
 * -- 接口说明 end --
 */

;(function(global, document, $, undefined){
    //base
    var
    rword = /([^,| ]+)/g,
    rblock = /\{([^\}]*)\}/ig,
    ds = {
        noop: function(){},
        mix: function(target, source, cover){
            if(typeof source !== 'object'){
                cover = source;
                source = target;
                target = this;
            }
            for(var k in source){
                if(cover || target[k] === undefined){
                    target[k] = source[k];
                }
            }
            return target;
        },
        mixStr: function(sStr){
            var args = Array.prototype.slice.call(arguments, 1);
            return sStr.replace(rblock, function(a, i){
                return args[i] != null ? args[i] : a;
            });
        },
        paddStr: function(str, len){
            return ('00000000000' + str).slice(-len);
        },
        nodeName: function(elem, name){
            var n = elem.nodeName.toUpperCase();
            return typeof name === 'string' ? n === name.toUpperCase() : n;
        }
    };

    //DatePicker
    var
    _uuid = 0,
    DOC = $(document),
    DatePicker = global.DatePicker = function(ops){
        this.init(ops || {});
    },
    _panelTmpl = '<div class="date_picker">{0}</div>',
    _fixTmpl = '<i class="tl"></i><i class="tr"></i><i class="br"></i><i class="bl"></i><i class="line"></i>',
    _yearTmpl = '<div class="picker_year"><strong title="点击以选择年份">{0}</strong><em class="arrow_t" title="前一年"></em><em class="arrow_b" title="后一年"></em><div class="picker_panel"><div class="picker_inner">{1}</div><strong class="prev" title="前{2}年"><em class="arrow_l"></em></strong><strong class="next" title="后{2}年"><em class="arrow_r"></em></strong><i class="tab"></i></div>{3}</div>',
    _monthTmpl = '<div class="picker_month"><strong title="点击以选择月份">{0}</strong><em class="arrow_t" title="前一月"></em><em class="arrow_b" title="后一月"></em><div class="picker_panel"><div class="picker_inner">{1}</div><strong class="prev" title="前一年"><em class="arrow_l"></em></strong><strong class="next" title="后一年"><em class="arrow_r"></em></strong><i class="tab"></i></div>{2}</div>',
    _dayTmpl = '<div class="picker_day"><strong title="点击以选择日期">{0}</strong><em class="arrow_t" title="前一天"></em><em class="arrow_b" title="后一天"></em><div class="picker_panel"><div class="picker_inner">{1}</div><strong class="prev" title="前一月"><em class="arrow_l"></em></strong><strong class="next" title="后一月"><em class="arrow_r"></em></strong><i class="tab"></i></div>{2}</div>';

    DatePicker.defaultOptions = {
        shell: null,
        shellTriggerEvent: 'click.DatePicker focus.DatePicker',
        follow: null,
        followOffset: [0, 0],
        showMode: 0,    //0 - 年月日, 1 - 年月, 2 - 年
        autoHide: true,
        effect: 'show',
        effectDuration: 0,
        altFormat: 'yyyy-mm-dd',
        unitYearSize: 12,
        defaultDate: null,
        minDate: null,
        maxDate: null,
        onselect: ds.noop,
        onmouseenter: ds.noop,
        onmouseleave: ds.noop
    };
    DatePicker.formatDate = function(date, format){
        if(typeof date === 'string'){
            date = date.split('-');
            return ds.paddStr(date[0], 4) + '-' + ds.paddStr(date[1], 2) + '-' + ds.paddStr(date[2], 2);
        }
        var
        year = date.getFullYear() + '', month = date.getMonth() + 1, day = date.getDate(),
        ret = format.replace(/yyyy/g, year).replace(/yy/g, year.slice(-2));
        ret = ret.replace(/mm/g, ds.paddStr(month, 2)).replace(/m/g, month);
        ret = ret.replace(/dd/g, ds.paddStr(day, 2)).replace(/d/g, day);
        return ret;
    };
    DatePicker.parseDate = function(formatDate, splitStr){
        splitStr = splitStr || '-';
        var
        date = new Date(),
        rDate = new RegExp('(\\d+)\\' + splitStr + '(\\d+)\\' + splitStr + '(\\d+)');
        if(rDate.test(formatDate)){
            date.setFullYear(RegExp.$1);
            date.setMonth(parseInt(RegExp.$2, 10) - 1);
            date.setDate(RegExp.$3);
        }
        return date;
    };
    DatePicker.prototype = {
        constructor : DatePicker,
        init: function(ops){
        	var _ops = DatePicker.defaultOptions;
            for(var k in _ops){
                if(ops[k] === void 0){
                    ops[k] = _ops[k];
                }
            }

            var
            self = this,
            shell = this.shell = $(ops.shell).bind(ops.shellTriggerEvent, function(e){
                if(e.type === 'focus' && self.preventFocusShow) {
                    self.preventFocusShow = false;
                    return;
                }

            	self.show();
            });

            shell[0] && (this.shellGetMethod = 'value' in shell[0] ? 'val' : 'html');
            this.follow = !!ops.follow ? $(ops.follow) : shell;

            this.ops = ops;
            this.id = ++_uuid;
            this.currDate = DatePicker.parseDate(ops.defaultDate);
            this.minDate = DatePicker.parseDate(ops.minDate || '1680-1-1');
            this.maxDate = DatePicker.parseDate(ops.maxDate || '2999-12-12');
            this.initField().position();
        },
        destroy: function(){
            this.shell.unbind(this.ops.shellTriggerEvent).removeData('DatePicker');
            this.shell = this.follow = null;
            this.DOM.remove();
        },
        position: function(x, y){
            if(typeof x !== 'number'){
                var ops = this.ops, pos = this.follow.offset();
                x = pos.left + ops.followOffset[0];
                y = pos.top + ops.followOffset[1];
            }
            this.DOM.css({left:x, top:y});
            return this;
        },
        opened: false,
        show: function(){
            if(this.opened) {
                return this;
            }

            var self = this, ops = this.ops;
            this.hidePanel().position();
            this.DOM[ops.effect](ops.effectDuration > 0 ? ops.effectDuration : undefined);

            DOC.unbind('click.datePicker_' + this.id);
            if(ops.autoHide) {
            	setTimeout(function() {
            		DOC.bind('click.datePicker_' + self.id, function(e){
            			if(e.target !== self.shell[0]) {
            				DOC.unbind('click.datePicker_' + self.id);
            				self.hide();
            			}
            		});
            	}, 32);
            }

            this.opened = true;
            return this;
        },
        hide: function(){
            if(!this.opened) {
                return this;
            }

            this.hidePanel(); //必须先hidePanel, IE6, 7 display:none，在某些情况下有BUG
            this.DOM.hide();
            DOC.unbind('click.datePicker_' + this.id);
            this.opened = false;
            return this;
        },
        initField: function(){
            var
            self = this,
            ops = this.ops,
            date = this.currDate,
            html = ds.mixStr(_yearTmpl, date.getFullYear(), '', ops.unitYearSize, '');
            if(ops.showMode < 2){
                html += ds.mixStr(_monthTmpl, ds.paddStr(date.getMonth() + 1, 2), '', '');
            }
            if(ops.showMode < 1){
                html += ds.mixStr(_dayTmpl, ds.paddStr(date.getDate(), 2), '', '');
            }
            //Elems
            var
            dom = this.DOM = $(ds.mixStr(_panelTmpl, html)).hide(),
            yearElem = this.yearElem = dom.find('.picker_year'),
            monthElem = this.monthElem = dom.find('.picker_month'),
            dayElem = this.dayElem = dom.find('.picker_day');
            this.yearLabel = yearElem.find('strong').eq(0);
            this.yearPanel = yearElem.find('.picker_inner').eq(0);
            this.monthLabel = monthElem.find('strong').eq(0);
            this.monthPanel = monthElem.find('.picker_inner').eq(0);
            this.dayLabel = dayElem.find('strong').eq(0);
            this.dayPanel = dayElem.find('.picker_inner').eq(0);
            function getMethod(elem){
                var method = '';
                if(/picker_([^\s]+)/.test(elem.className)){
                    method = RegExp.$1;
                    return method.charAt(0).toUpperCase() + method.slice(1);;
                }
                return method;
            }
            dom.click(function(e){
                var
                tar = e.target,
                rdir = /next|prev/,
                nodeName = ds.nodeName(tar),
                parentNode = tar.parentNode,
                pClassName = parentNode.className,
                method = getMethod(parentNode);

                //Sub Item
                if(nodeName === 'A' && pClassName.indexOf('picker_inner') > -1){
                    method = getMethod(parentNode.parentNode.parentNode);
                    self['set' + method](tar.innerHTML);
                    if(method === 'Year' && ops.showMode < 2){
                        self.chooseMonth();
                    }
                    else if(method === 'Month' && ops.showMode < 1){
                        self.chooseDay();
                    }
                    else if(!ops.autoHide){
                    	self.hidePanel();
                    }
                    else{
                        self.preventFocusShow = true;
                    	self.hide();
                    }
                }
                //picker_panel next, prev
                else if(pClassName.indexOf('picker_panel') > -1 || nodeName === 'EM' && rdir.test(pClassName)){
                    if(nodeName === 'I'){
                        self.hidePanel();
                    }
                    else if(nodeName === 'EM'){
                        tar = parentNode;
                        nodeName = ds.nodeName(tar);
                    }
                    if(nodeName === 'STRONG'){
                        method = getMethod(tar.parentNode.parentNode);
                        var isNext = tar.className.indexOf('next') > -1;
                        if(method === 'Day'){
                            self[isNext ? 'nextMonth' : 'prevMonth']();
                        }
                        else if(method === 'Month'){
                            self[isNext ? 'nextYear' : 'prevYear']();
                        }
                        else if(method === 'Year'){
                            self[isNext ? 'nextYear' : 'prevYear'](ops.unitYearSize);
                        }
                        self['choose' + method]();
                    }
                }
                //picker_$$
                else if(pClassName.indexOf('picker_') > -1){
                    if(nodeName === 'STRONG'){
                        self['choose' + method]();
                    }
                    else{
                        self[(tar.className.indexOf('_t') > 0 ? 'prev' : 'next') + method]();
                    }
                }
                else{
                    self.hidePanel();
                }

                if(self.lastFocusFrom) {
                    $(self.lastFocusFrom).focus();
                    self.lastFocusFrom = null;
                }

                setTimeout(function() {
                    if(closed) {
                        // self.hidePanel();
                    }
                }, 32);

                //Stop Default
                e.stopPropagation();
                e.preventDefault();
            })
            //mouseenter, mouseleave
            .hover(function(){
                ops.onmouseenter.apply(self, arguments);
            }, function(){
                ops.onmouseleave.apply(self, arguments);
            });

            this.shell.bind('blur.DatePicker', function(e) {
                self.lastFocusFrom = e.target;
            });

            document.body ? dom.appendTo('body') : $(function(){
                dom.appendTo('body');
            });
            return this;
        },
        getDate: function(format){
            return typeof format === 'string' ? DatePicker.formatDate(this.currDate, format) : this.currDate;
        },
        setDate: function(date){
            var
            ops = this.ops,
            prevDate = this.currDate,
            currDate = typeof date === 'object' ? date : DatePicker.parseDate(date);
            //Fix currDate
            currDate = this.currDate = new Date(Math.min(+this.maxDate, Math.max(+this.minDate, +currDate)));
            var
            prevFormatDate = this.currFormatDate,
            currFormatDate = this.currFormatDate = DatePicker.formatDate(currDate, ops.altFormat);

            this.yearLabel.html(ds.paddStr(currDate.getFullYear(), 4));
            if(ops.showMode < 2 && prevDate.getMonth() !== currDate.getMonth()){
                this.monthLabel.html(ds.paddStr(currDate.getMonth() + 1, 2));
            }
            if(ops.showMode < 1 && prevDate.getDate() !== currDate.getDate()){
                this.dayLabel.html(ds.paddStr(currDate.getDate(), 2));
            }
            //Callback & Set shell property
            if(currFormatDate !== prevFormatDate && ops.onselect.call(this, currDate, currFormatDate) !== false){
                this.shell[0] && this.shell[this.shellGetMethod](currFormatDate);
            }
            return this;
        }
    };
    //Extend Funs
    var
    keys = 'Year,Month,Day',
    fn = DatePicker.prototype,
    hooks = {Day:'Date', Year:'FullYear'},
    prevFns = {
        Year: function(val){
            val = parseInt(val, 10) || 1;
            var date = new Date(+this.currDate);
            date.setFullYear(date.getFullYear() - val);
            return this.setDate(date);
        },
        Month: function(val){
            val = parseInt(val, 10) || 1;
            var
            date = new Date(+this.currDate),
            year = date.getFullYear(),
            month = date.getMonth() - val;
            if(month < 0){
                year--;
                month = month + 12;
                date.setFullYear(year);
            }
            date.setMonth(month);
            return this.setDate(date);
        },
        Day: function(val){
            val = parseInt(val, 10) || 1;
            var date = new Date(+this.currDate);
            date.setTime(+date - val * 24 * 60 * 60 * 1000);
            return this.setDate(date);
        }
    },
    itemTmpl = '<a href="javascript:;" title="{1}"{2}>{0}</a>',
    htmlFns = {
        Year: function(date, minDate, maxDate, unitYearSize){
            var
            ret = '',
            currYear = date.getFullYear(),
            endYear = maxDate.getFullYear(),
            startYear = Math.max(minDate.getFullYear(), currYear - parseInt(unitYearSize / 2, 10));
            for(var i=0; i<unitYearSize && (startYear + i) <= endYear; i++){
                ret += ds.mixStr(itemTmpl, startYear + i, (startYear + i) + '年', startYear + i !== currYear ? '' : ' class="hover"');
            }
            return ret;
        },
        Month: function(date, minDate, maxDate){
            var
            ret = '',
            currYear = date.getFullYear(),
            currMonth = date.getMonth() + 1,
            startMonth = currYear > minDate.getFullYear() ? 1 : minDate.getMonth() + 1,
            endMonth = currYear < maxDate.getFullYear() ? 12 : maxDate.getMonth() + 1;
            for(var i = startMonth; i <= endMonth; i++){
                ret += ds.mixStr(itemTmpl, i, i + '月', i !== currMonth ? '' : ' class="hover"');
            }
            return ret;
        },
        Day: function(date, minDate, maxDate){
            var
            ret = '',
            tmpDate = new Date(+date);
            currYear = date.getFullYear(),
            currMonth = date.getMonth() + 1,
            currDay = date.getDate();
            //week
            tmpDate.setDate(1);
            var
            startWeek = tmpDate.getDay(),
            weeks = ['天', '一', '二', '三', '四', '五', '六'];
            //Build
            var
            nowFormat = DatePicker.formatDate(new Date(), 'yyyy-m-d'),
            startDay = currYear > minDate.getFullYear() || currMonth > minDate.getMonth() + 1 ? 1 : minDate.getDate(),
            endDay = currYear < maxDate.getFullYear() || currMonth < maxDate.getMonth() + 1 ? 31 : maxDate.getDate();
            for(var isNow, cName, title, formatDate, i=startDay; i<=endDay; i++){
                formatDate = currYear + '-' + currMonth + '-' + i;
                if(i > 28){
                    tmpDate.setDate(i);
                    if(formatDate !== DatePicker.formatDate(tmpDate, 'yyyy-m-d')){
                        break;
                    }
                }
                isNow = formatDate === nowFormat;
                //title = i + '日\n星期' + weeks[(i + startWeek - 1) % 7];
                title = ds.mixStr('星期{3} &#013;&#010;{0}年{1}月{2}日{4}', currYear, currMonth, currDay, weeks[(i + startWeek - 1) % 7], isNow ? '(今天)' : '');
                cName = (isNow || i === currDay ? ' hover' : ' ') + (isNow ? ' now' : '');
                cName = cName.length > 1 ? 'class="' + cName.slice(1) + '"' : '';

                ret += ds.mixStr(itemTmpl, i, title, cName);
            }
            return ret;
        }
    };
    keys.replace(rword, function(a, k){
        //Set
        var rKey = hooks[k] || k;
        fn['set' + k] = function(val){
            var date = new Date(+this.currDate);
            if(k !== 'Month'){
                date['set' + rKey](val);
            }
            else{
                val = (parseInt(val, 10) || 1) - 1;
                date.setMonth(val);
                val < date.getMonth() && date.setDate(0);
            }
            return this.setDate(date);
        };
        //Next
        fn['next' + k] = function(val){
            val = parseInt(val, 10) || 1;
            var date = new Date(+this.currDate);
            date['set' + rKey](date['get' + rKey]() + val);
            return this.setDate(date);
        };
        //Prev
        fn['prev' + k] = prevFns[k] || ds.noop;
        //Choose
        fn['choose' + k] = function(){
            var
            key = k.toLowerCase(),
            shell = this[key + 'Elem'],
            panel = this[key + 'Panel'],
            html = htmlFns[k](this.currDate, this.minDate, this.maxDate, this.ops.unitYearSize);
            this.hidePanel(function(k){ return k !== key; });

            panel.html(html);
            shell.addClass('focus');
            panel.parent().show();
            return this;
        }
    });
    //Hideanel, showPanel
    ds.mix(fn, {
        hidePanel: function(filter){
            var self = this;
            filter = filter || ds.noop;
            keys.replace(rword, function(a, k){
                k = k.toLowerCase();
                var shell = self[k + 'Elem'], panel = self[k + 'Panel'];
                if(filter.call(shell, k, panel) !== false){
                    panel.parent().hide();
                    shell.removeClass('focus');
                }
            });
            return this;
        },
        showPanel: function(name){
            name = name.toLowerCase();
            this.hidePanel(function(k){ return k !== name; });
            this[name + 'Elem'].addClass('focus');
            this[name + 'Panel'].parent().show();
            return this;
        }
    });
    //Extend jQuery
    var rdate = /\d+\-\d+\-\d+/;
    $.fn.datePicker = function(ops){
        typeof ops !== 'object' && (ops = {});
        ops.shell = this;
        var
        minDate = this.attr('min'),
        maxDate = this.attr('max'),
        defaultDate = this[0].value;

        rdate.test(minDate) && (ops.minDate = minDate);
        rdate.test(maxDate) && (ops.maxDate = maxDate);
        rdate.test(defaultDate) && (ops.defaultDate = defaultDate);
        return this.data('DatePicker', new DatePicker(ops));
    };
    //Include CSS
    var css = '.date_picker{ font:14px/1.5 "5FAE8F6F96C59ED1","Microsoft Yahei",Arial; position:absolute;}.date_picker, .picker_panel{ width:183px;}.picker_year, .picker_month, .picker_day{ float:left; display:inline; background:#fff; border:1px solid #d3d3d3; border-radius:5px; box-shadow:0 0 4px rgba(0,0,0,.2); margin-right:1px; height:60px; width:58px; position:relative; z-index:100000000; white-space:nowrap;}.date_picker .focus{ z-index:100000002;}.date_picker .focus .picker_panel{ display:block;}.date_picker strong{ color:#666; cursor:pointer; margin-top:-10px; height:20px; width:100%; position:absolute; left:0; top:50%; text-align:center; z-index:100000003;}.date_picker .line{ border-top:1px solid #d3d3d3; font-size:0; height:0; width:100%; position:absolute; left:0; top:50%;}.date_picker .arrow_t, .date_picker .arrow_r, .date_picker .arrow_b, .date_picker .arrow_l{ border-color:#cbcbcb transparent; border-style:solid dashed; border-width:0 6px 6px; cursor:pointer; font-size:0; height:0; width:0; position:absolute; z-index:100000000;}.date_picker .arrow_t{ border-width:6px 6px 0; left:22px; bottom:6px;}.date_picker .arrow_b{ left:22px; top:6px;}.date_picker .arrow_r, .date_picker .arrow_l{ border-color:transparent #cbcbcb; border-style:dashed solid; border-width:6px 0 6px 6px;}.date_picker .arrow_l{ border-width:6px 6px 6px 0;}.picker_panel{ display:none; margin-left:-1px; position:absolute; left:0; top:36px; z-index:100000009;}.picker_panel .picker_inner{ background:#fff; border:1px solid #d3d3d3; border-radius:5px; padding:6px 0 5px 5px; position:relative; z-index:100000002; overflow:hidden; *zoom:1;}.picker_panel .picker_inner a{ float:left; border-radius:3px; color:#666; font:14px/25px Arial; height:25px; width:56px; text-align:center; text-decoration:none;}.picker_panel .picker_inner a:hover, .picker_panel .picker_inner a.hover{ background:#ddd; color:#333; text-decoration:none;}.picker_panel .picker_inner a:hover{ background:#d0d0d0;}.picker_panel .tab{ background:#fff; border:1px solid #d3d3d3; border-bottom-width:0; border-radius:5px 5px 0 0; font-size:0; height:18px; width:58px; position:absolute; left:0; top:-18px; z-index:100000003;}.picker_panel .prev, .picker_panel .next{ background:#fff; border:1px solid #d3d3d3; border-radius:12px; cursor:pointer; margin:-16px 0 0 -14px; height:24px; width:24px; position:absolute; left:0; top:50%; z-index:100000000; box-shadow:0 0 3px rgba(0,0,0,.2);}.picker_panel .next{ margin-left:0; margin-right:-14px; left:auto; right:0;}.picker_panel .arrow_l{ margin:-6px 0 0 4px; left:0; top:50%;}.picker_panel .arrow_r{ margin:-6px 4px 0 0; left:auto; right:0; top:50%;}.picker_panel .prev:hover, .picker_panel .next:hover{ border-color:#999;}.picker_panel .prev:hover .arrow_l, .picker_panel .next:hover .arrow_r{ border-color:transparent #999;}.picker_year .picker_inner{ border-radius:0 5px 5px 5px;}.picker_month .picker_panel{ margin-left:-62px;}.picker_month .picker_inner{ border-radius:5px 0 5px 5px;}.picker_month .tab{ margin-left:61px;}.picker_day .picker_panel{ margin-left:-123px;}.picker_day .picker_inner{ border-radius:5px 0 5px 5px;}.picker_day .tab{ margin-left:122px;}.picker_inner{ box-shadow:0 0 4px rgba(0,0,0,.2); margin:0 1px 0 0; overflow:hidden; *zoom:1;}.picker_main a{ float:left; cursor:pointer;}.picker_main a:hover{ background:#ccc;}';
    $(function(){
        var style = document.createElement('style'), head = document.head || document.getElementsByTagName('head')[0];
        style.type = 'text/css';
        !style.styleSheet ? (style.innerHTML = css) : (style.styleSheet.cssText = css);
        head.insertBefore(style, head.firstChild);
    });
})(this, document, jQuery);