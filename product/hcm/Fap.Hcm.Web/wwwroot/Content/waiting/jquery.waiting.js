/*! jQuery Waiting - v0.1.1 - 2013-06-08
* https://github.com/Novascreen/jquery.waiting
* Copyright (c) 2013 Thomas Hermann; Licensed MIT */
(function ($, window, document, undefined) {
 'use strict';

  var waiting = 'waiting',
    defaults = {
      waitingClass: waiting,
      position: "center",
      overlay: true,
      loadingtext:'',
      fixed: false
    };

  function Plugin(element, options) {
    this.element = element;
    this.$el = $(element);

    this.options = $.extend({}, defaults, options);

    this._defaults = defaults;
    this._name = waiting;
    this._addPositionRelative = false;

    this.init();
  }


  Plugin.prototype = {

    init: function () {
      this.$container = $('<div class="waiting-container hidden" />');
      this.$indicator = $('<div class="waiting-indicator h3"><i class="ajax-loading-icon fa fa-spin fa-spinner fa-2x orange bigger-125"></i> ' + this.options.loadingtext + '</div>').appendTo(this.$container);

      if (this.options.overlay) {
        this.$container.addClass('overlay');
        this.$overlay = $('<div class="waiting-overlay" />').appendTo(this.$container);
      }

      if (this.options.overlay && this.options.position !== 'custom') {
        this.$indicator.addClass(this.options.position);
      }

      if (this.options.fixed) {
        this.$container.addClass('fixed');
      }

      if(this.element.style.position === '') {
        this._addPositionRelative = true;
      }

      this.show();
    },


    show: function () {

      if (this._addPositionRelative) {
        this.element.style.position = 'relative';
      }

      this.$el.addClass(this.options.waitingClass);
      this.$container.appendTo(this.$el).removeClass('hidden');
    },


    hide: function () {
      this.$container.addClass('hidden');
      this.$container.detach();
      this.$el.removeClass(this.options.waitingClass);

      if (this._addPositionRelative) {
        this.element.style.position = '';
      }
    },


    again: function () {
      this.show();
    },


    done: function () {
      this.hide();
    }
  };

  // A really lightweight plugin wrapper around the constructor,
  // preventing against multiple instantiations
  $.fn[waiting] = function (options) {
    return this.each(function () {
      var plugin, method, expose;
      if (!$.data(this, 'plugin_' + waiting)) {
        $.data(this, 'plugin_' + waiting, new Plugin(this, options));
      }
      else {
        plugin = $.data(this, 'plugin_' + waiting);
        method = 'again';
        expose = {
          again: true,
          done: true
        };

        if (typeof options === 'string') {
          if (!expose[options]) {
            return false;
          }
          method = options;
          options = null;
        }

        plugin[method].call(plugin, options);
      }
    });
  };

})(jQuery, window, document);