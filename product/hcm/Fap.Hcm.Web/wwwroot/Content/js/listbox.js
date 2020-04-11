/**
 * Listbox.js is a simple jQuery plugin that provides a more powerful
 * alternative to the standard `<select>` tag.
 *
 * The main problem of <select> tag is that last one isn't flexible for
 * customization with CSS. Listbox.js solves this problem. This component
 * runs on top of <select> tag and creates an alternative to the last one
 * based on <div> tags. It opens up great possibilities for customization.
 *
 * @copyright   (c) 2012, Igor Kalnitsky <igor@kalnitsky.org>
 * @version     0.2.0
 * @license     BSD
 */

(function ($) {
    'use strict';


    /**
     * Create an instance of Listbox.
     *
     * @constructor
     * @this {Listbox}
     * @param {object} domelement DOM element to be converted to the Listbox
     * @param {object} options an object with Listbox settings
     */
    function Listbox(domelement, options) {
        // css classes used by plugin
        this.MAIN_CLASS      = 'lbjs';
        this.LIST_CLASS      = 'lbjs-list';
        this.LIST_ITEM_CLASS = 'lbjs-item';
        this.SEARCHBAR_CLASS = 'lbjs-searchbar';

        /** @private */
        this._parent   = domelement;
        this._settings = options;

        // construct a fake listbox
        this._createListbox();                // create a fake listbox
        this._parent.css('display', 'none');  // hide a parent element
    }


    /**
     * Creates a `div`-based listbox.
     *
     * @private
     * @this {Listbox}
     */
    Listbox.prototype._createListbox = function () {
        this._listbox = $('<div>')
            .addClass(this.MAIN_CLASS)
            .addClass(this._settings['class'])
            .insertAfter(this._parent)
        ;

        if (this._settings['searchbar'])
            this._createSearchbar();
        this._createList();
    }

    /**
     * Creates a Listbox's searchbar.
     *
     * @private
     * @this {Listbox}
     */
    Listbox.prototype._createSearchbar = function () {
        // searchbar wrapper is needed for properly stretch
        // the seacrhbar over the listbox width
        var searchbarWrapper = $('<div>')
            .addClass(this.SEARCHBAR_CLASS + '-wrapper')
            .appendTo(this._listbox);

        var searchbar = $('<input>')
            .addClass(this.SEARCHBAR_CLASS)
            .appendTo(searchbarWrapper)
            .attr('placeholder', 'search...');

        // set filter handler
        var self = this;
        searchbar.keyup(function () {
            var searchQuery = $(this).val().toLowerCase();

            if (searchQuery !== '') {
                // hide list items which are not matched search query
                self._list.children().each(function (index) {
                    var text = $(this).text().toLowerCase();

                    if (text.search('^' + searchQuery) != -1) {
                        $(this).css('display', 'block');
                    } else {
                        $(this).css('display', 'none');
                    }
                });
            } else {
                // make visible all list items
                self._list.children().each(function () {
                    $(this).css('display', 'block')
                });
            }

            // @hack: call special handler which is used only for SingleSelectListbox
            //        to prevent situation when none of items are selected
            if (self.onFilterChange) {
                self.onFilterChange();
            }
        });

        // save for using in _resizeListToListbox()
        this._searchbarWrapper = searchbarWrapper;
    }


    /**
     * Creates a list. The List is an element with list items.
     *
     * @private
     */
    Listbox.prototype._createList = function () {
        // create container
        this._list = $('<div>')
            .addClass(this.LIST_CLASS)
            .appendTo(this._listbox);

        this._resizeListToListbox();

        // create items
        var self = this;
        this._parent.children().each(function () {
            var item = $('<div>')
                .addClass(self.LIST_ITEM_CLASS)
                .appendTo(self._list)
                .text($(this).text())
                .click(function () {
                    self.onItemClick($(this))
                });

            if ($(this).attr('disabled'))
                item.attr('disabled', '');

            if ($(this).attr('selected'))
                self.onItemClick(item);
        });
    }




    /**
     * Resize list to lisbox. It's a small hack since I can't
     * do it with CSS.
     *
     * @private
     */
    Listbox.prototype._resizeListToListbox = function () {
        var listHeight = this._listbox.height();

        if (this._settings['searchbar'])
            listHeight -= this._searchbarWrapper.outerHeight(true);

        this._list.height(listHeight);
    }




    /**
     * Create an instance of SingleSelectListbox.
     *
     * Inherit a {Listbox} class.
     *
     * @constructor
     * @this {SingleSelectListbox}
     * @param {object} domelement DOM element to be converted to the Listbox
     * @param {object} options an object with Listbox settings
     */
    function SingleSelectListbox(domelement, options) {
        Listbox.call(this, domelement, options);

        // select first item if none selected
        if (!this._selected)
            this.onItemClick(this._list.children().first());
    }
    SingleSelectListbox.prototype = Object.create(Listbox.prototype);
    SingleSelectListbox.prototype.constructor = SingleSelectListbox;


    /**
     * Reset all items and select a given one.
     *
     * @this {SingleSelectListbox}
     * @param {object} item a DOM object
     */
    SingleSelectListbox.prototype.onItemClick = function (item) {
        if (item.attr('disabled')) return;

        // select a fake item
        if (this._selected)
            this._selected.removeAttr('selected');
        this._selected = item.attr('selected', '');

        // select a real item
        var itemToSelect = $(this._parent.children().get(item.index()));
        this._parent.val(itemToSelect.val());

        this._parent.trigger('change');
    }


    /**
     * Select first visible item if none selected.
     *
     * @this {SingleSelectListbox}
     */
    SingleSelectListbox.prototype.onFilterChange = function () {
        if (!this._selected || !this._selected.is(':visible'))
            this.onItemClick(this._list.children(':visible').first());
    }




    /**
     * Create an instance of MultiSelectListbox.
     *
     * Inherit a {Listbox} class.
     *
     * @constructor
     * @this {MultiSelectListbox}
     * @param {object} domelement DOM element to be converted to the Listbox
     * @param {object} options an object with Listbox settings
     */
    function MultiSelectListbox(domelement, options) {
        Listbox.call(this, domelement, options);
    }
    MultiSelectListbox.prototype = Object.create(Listbox.prototype);
    MultiSelectListbox.prototype.constructor = MultiSelectListbox;


    /**
     * Toggle item status.
     *
     * @this {MultiSelectListbox}
     * @param {object} item a DOM object
     */
    MultiSelectListbox.prototype.onItemClick = function (item) {
        if (item.attr('disabled')) return;

        var parentItem = $(this._parent.children().get(item.index()));
        var parentValue = this._parent.val();

        if (item.attr('selected')) {
            item.removeAttr('selected');

            var removeIndex = parentValue.indexOf(parentItem.val());
            parentValue.splice(removeIndex, 1);
        } else {
            item.attr('selected', '');

            if (!parentValue)
                parentValue = [];
            parentValue.push(parentItem.val());
        }

        this._parent.val(parentValue);
        this._parent.trigger('change');
    }




    /**
     * jQuery plugin definition.
     *
     * @param {object} options an object with Listbox settings
     */
    $.fn.listbox = function (options) {
        var settings = $.extend({
            'class':      null,
            'searchbar':  false
        }, options);

        return this.each(function () {
            $(this).attr('multiple')
                ? new MultiSelectListbox($(this), settings)
                : new SingleSelectListbox($(this), settings)
            ;
        });
    }
})(jQuery);
