/**
 * Copyright (c) 2006-2012, JGraph Ltd
 */
Format = function (editorUi, container) {
    this.editorUi = editorUi;
    this.container = container;
};

/**
 * Returns information about the current selection.
 */
Format.prototype.labelIndex = 0;

/**
 * Returns information about the current selection.
 */
Format.prototype.currentIndex = 0;

/**
 * Returns information about the current selection.
 */
Format.prototype.showCloseButton = true;

/**
 * Background color for inactive tabs.
 */
Format.prototype.inactiveTabBackgroundColor = '#d7d7d7';

/**
 * Adds the label menu items to the given menu and parent.
 */
Format.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    this.update = mxUtils.bind(this, function (sender, evt) {
        this.clearSelectionState();
        this.refresh();
    });

    graph.getSelectionModel().addListener(mxEvent.CHANGE, this.update);
    graph.addListener(mxEvent.EDITING_STARTED, this.update);
    graph.addListener(mxEvent.EDITING_STOPPED, this.update);
    graph.getModel().addListener(mxEvent.CHANGE, this.update);
    graph.addListener(mxEvent.ROOT, mxUtils.bind(this, function () {
        this.refresh();
    }));

    this.refresh();
};

/**
 * Returns information about the current selection.
 */
Format.prototype.clearSelectionState = function () {
    this.selectionState = null;
};

/**
 * Returns information about the current selection.
 */
Format.prototype.getSelectionState = function () {
    if (this.selectionState == null) {
        this.selectionState = this.createSelectionState();
    }

    return this.selectionState;
};

/**
 * Returns information about the current selection.
 */
Format.prototype.createSelectionState = function () {
    var cells = this.editorUi.editor.graph.getSelectionCells();
    var result = this.initSelectionState();

    for (var i = 0; i < cells.length; i++) {
        this.updateSelectionStateForCell(result, cells[i], cells);
    }

    return result;
};

/**
 * Returns information about the current selection.
 */
Format.prototype.initSelectionState = function () {
    return {
        vertices: [], edges: [], x: null, y: null, width: null, height: null, style: {},
        containsImage: false, containsLabel: false, fill: true, glass: true, rounded: true,
        comic: true, autoSize: false, image: true, shadow: true, lineJumps: true
    };
};

/**
 * Returns information about the current selection.
 */
Format.prototype.updateSelectionStateForCell = function (result, cell, cells) {
    var graph = this.editorUi.editor.graph;

    if (graph.getModel().isVertex(cell)) {
        result.vertices.push(cell);
        var geo = graph.getCellGeometry(cell);

        if (geo != null) {
            if (geo.width > 0) {
                if (result.width == null) {
                    result.width = geo.width;
                }
                else if (result.width != geo.width) {
                    result.width = '';
                }
            }
            else {
                result.containsLabel = true;
            }

            if (geo.height > 0) {
                if (result.height == null) {
                    result.height = geo.height;
                }
                else if (result.height != geo.height) {
                    result.height = '';
                }
            }
            else {
                result.containsLabel = true;
            }

            if (!geo.relative || geo.offset != null) {
                var x = (geo.relative) ? geo.offset.x : geo.x;
                var y = (geo.relative) ? geo.offset.y : geo.y;

                if (result.x == null) {
                    result.x = x;
                }
                else if (result.x != x) {
                    result.x = '';
                }

                if (result.y == null) {
                    result.y = y;
                }
                else if (result.y != y) {
                    result.y = '';
                }
            }
        }
    }
    else if (graph.getModel().isEdge(cell)) {
        result.edges.push(cell);
    }

    var state = graph.view.getState(cell);

    if (state != null) {
        result.autoSize = result.autoSize || this.isAutoSizeState(state);
        result.glass = result.glass && this.isGlassState(state);
        result.rounded = result.rounded && this.isRoundedState(state);
        result.lineJumps = result.lineJumps && this.isLineJumpState(state);
        result.comic = result.comic && this.isComicState(state);
        result.image = result.image && this.isImageState(state);
        result.shadow = result.shadow && this.isShadowState(state);
        result.fill = result.fill && this.isFillState(state);

        var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);
        result.containsImage = result.containsImage || shape == 'image';

        for (var key in state.style) {
            var value = state.style[key];

            if (value != null) {
                if (result.style[key] == null) {
                    result.style[key] = value;
                }
                else if (result.style[key] != value) {
                    result.style[key] = '';
                }
            }
        }
    }
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isFillState = function (state) {
    return state.view.graph.model.isVertex(state.cell) ||
        mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null) == 'arrow' ||
        mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null) == 'filledEdge' ||
        mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null) == 'flexArrow';
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isGlassState = function (state) {
    var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

    return (shape == 'label' || shape == 'rectangle' || shape == 'internalStorage' ||
        shape == 'ext' || shape == 'umlLifeline' || shape == 'swimlane' ||
        shape == 'process');
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isRoundedState = function (state) {
    var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

    return (shape == 'label' || shape == 'rectangle' || shape == 'internalStorage' || shape == 'corner' ||
        shape == 'parallelogram' || shape == 'swimlane' || shape == 'triangle' || shape == 'trapezoid' ||
        shape == 'ext' || shape == 'step' || shape == 'tee' || shape == 'process' || shape == 'link' ||
        shape == 'rhombus' || shape == 'offPageConnector' || shape == 'loopLimit' || shape == 'hexagon' ||
        shape == 'manualInput' || shape == 'curlyBracket' || shape == 'singleArrow' || shape == 'callout' ||
        shape == 'doubleArrow' || shape == 'flexArrow' || shape == 'card' || shape == 'umlLifeline');
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isLineJumpState = function (state) {
    var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

    return shape == 'connector' || shape == 'filledEdge';
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isComicState = function (state) {
    var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

    return mxUtils.indexOf(['label', 'rectangle', 'internalStorage', 'corner', 'parallelogram', 'note', 'collate',
        'swimlane', 'triangle', 'trapezoid', 'ext', 'step', 'tee', 'process', 'link', 'rhombus',
        'offPageConnector', 'loopLimit', 'hexagon', 'manualInput', 'singleArrow', 'doubleArrow',
        'flexArrow', 'filledEdge', 'card', 'umlLifeline', 'connector', 'folder', 'component', 'sortShape',
        'cross', 'umlFrame', 'cube', 'isoCube', 'isoRectangle', 'partialRectangle'], shape) >= 0;
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isAutoSizeState = function (state) {
    return mxUtils.getValue(state.style, mxConstants.STYLE_AUTOSIZE, null) == '1';
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isImageState = function (state) {
    var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

    return (shape == 'label' || shape == 'image');
};

/**
 * Returns information about the current selection.
 */
Format.prototype.isShadowState = function (state) {
    var shape = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

    return (shape != 'image');
};

/**
 * Adds the label menu items to the given menu and parent.
 */
Format.prototype.clear = function () {
    this.container.innerHTML = '';

    // Destroy existing panels
    if (this.panels != null) {
        for (var i = 0; i < this.panels.length; i++) {
            this.panels[i].destroy();
        }
    }

    this.panels = [];
};

/**
 * Adds the label menu items to the given menu and parent.
 */
Format.prototype.refresh = function () {
    // Performance tweak: No refresh needed if not visible
    if (this.container.style.width == '0px') {
        return;
    }

    this.clear();
    var ui = this.editorUi;
    var graph = ui.editor.graph;

    var div = document.createElement('div');
    div.style.whiteSpace = 'nowrap';
    div.style.color = 'rgb(112, 112, 112)';
    div.style.textAlign = 'left';
    div.style.cursor = 'default';

    var label = document.createElement('div');
    label.style.border = '1px solid #c0c0c0';
    label.style.borderWidth = '0px 0px 1px 0px';
    label.style.textAlign = 'center';
    label.style.fontWeight = 'bold';
    label.style.overflow = 'hidden';
    label.style.display = (mxClient.IS_QUIRKS) ? 'inline' : 'inline-block';
    label.style.paddingTop = '8px';
    label.style.height = (mxClient.IS_QUIRKS) ? '34px' : '25px';
    label.style.width = '100%';
    this.container.appendChild(div);
    var addClickHandler = mxUtils.bind(this, function (elt, panel, index) {
        var clickHandler = mxUtils.bind(this, function (evt) {
            if (currentLabel != elt) {
                if (containsLabel) {
                    this.labelIndex = index;
                }
                else {
                    this.currentIndex = index;
                }
                if (currentLabel != null) {
                    currentLabel.style.backgroundColor = this.inactiveTabBackgroundColor;
                    currentLabel.style.borderBottomWidth = '1px';
                }
                currentLabel = elt;
                currentLabel.style.backgroundColor = '';
                currentLabel.style.borderBottomWidth = '0px';

                if (currentPanel != panel) {
                    if (currentPanel != null) {
                        currentPanel.style.display = 'none';
                    }

                    currentPanel = panel;
                    currentPanel.style.display = '';
                }
            }
        });

        mxEvent.addListener(elt, 'click', clickHandler);

        if (index == ((containsLabel) ? this.labelIndex : this.currentIndex)) {
            // Invokes handler directly as a workaround for no click on DIV in KHTML.
            clickHandler();
        }
    });
    if (graph.isSelectionEmpty()) {
        var containsLabel = this.getSelectionState().containsLabel;
        var currentLabel = null;
        var currentPanel = null;
        let idx = 0;
        label.style.backgroundColor = this.inactiveTabBackgroundColor;
        label.style.borderLeftWidth = '1px';
        label.style.width = '50%';
        var label2 = label.cloneNode(false);
        // Workaround for ignored background in IE
        label2.style.backgroundColor = this.inactiveTabBackgroundColor;
        // Style
        if (containsLabel) {
            label2.style.borderLeftWidth = '0px';
        }
        else {
            label.style.borderLeftWidth = '0px';
            mxUtils.write(label, mxResources.get('diagram'));
            div.appendChild(label);

            var diagramPanel = div.cloneNode(false);
            diagramPanel.style.display = 'none';
            this.panels.push(new DiagramFormatPanel(this, ui, diagramPanel));
            this.container.appendChild(diagramPanel);

            addClickHandler(label, diagramPanel, idx++);
        }



        //this.panels.push(new BillFormatPanel(this, ui, billPanel));

        // 样式
        mxUtils.write(label2, '样式');
        div.appendChild(label2);

        var stylePanel = div.cloneNode(false);
        stylePanel.style.display = 'none';
        this.panels.push(new StyleFormatPanel(this, ui, stylePanel));
        this.container.appendChild(stylePanel);
        addClickHandler(label2, stylePanel, idx++);

    }
    else if (graph.isEditing()) {
        //mxUtils.write(label, mxResources.get('text'));
        //div.appendChild(label);
        //this.panels.push(new TextFormatPanel(this, ui, div));
    }
    else {
        var cell = graph.getSelectionCell();
        if (graph.getSelectionCount() == 1) {
            if (graph.getModel().isEdge(cell)) {
                //条件
                mxUtils.write(label, '流转设置');
                if (this.showCloseButton) {
                    var img = document.createElement('img');
                    img.setAttribute('border', '0');
                    img.setAttribute('src', Dialog.prototype.closeImage);
                    img.setAttribute('title', mxResources.get('hide'));
                    img.style.position = 'absolute';
                    img.style.display = 'block';
                    img.style.right = '0px';
                    img.style.top = '8px';
                    img.style.cursor = 'pointer';
                    img.style.marginTop = '1px';
                    img.style.marginRight = '17px';
                    img.style.border = '1px solid transparent';
                    img.style.padding = '1px';
                    img.style.opacity = 0.5;
                    label.appendChild(img)

                    mxEvent.addListener(img, 'click', function () {
                        ui.actions.get('formatPanel').funct();
                    });
                }
                var value = cell.value;
                if (value == null) {
                    var doc = mxUtils.createXmlDocument();
                    var obj = doc.createElement('transition');
                    obj.setAttribute('label', '无');
                    obj.setAttribute('placeholders', '1');
                    //名称
                    obj.setAttribute('nodeName', '流转设置');
                    //描述
                    obj.setAttribute('nodeDesc', '');
                    //活动类型
                    obj.setAttribute('nodeType', '100');
                    //优先级
                    obj.setAttribute('priority', '1');
                    //条件
                    obj.setAttribute('condition', '');
                    obj.setAttribute('conditionDesc', '');
                    graph.getModel().beginUpdate();
                    cell.value = obj;
                    try {
                        var edit = new mxCellAttributeChange(
                            cell, 'label',
                            '无');
                        graph.getModel().execute(edit);
                        graph.updateCellSize(cell);
                    }
                    finally {
                        graph.getModel().endUpdate();
                    }
                }
                div.appendChild(label);
                this.panels.push(new TransferFormatPanel(this, ui, div));
            } else if (graph.getModel().isVertex(cell)) {
                if (cell.value === '') {
                    graph.setSelectionCell(cell.parent);
                    return;
                }
                if (!mxUtils.isNode(cell.value)) {
                    return;
                }
                if (cell.value.nodeName === 'swimlane') {
                    //任务
                    mxUtils.write(label, '泳道设置');
                    div.appendChild(label);
                    this.panels.push(new SwimlaneFormatPanel(this, ui, div));
                } else if (cell.value.nodeName === 'activity') {
                    //任务
                    mxUtils.write(label, '节点设置');
                    div.appendChild(label);
                    let idx = 0;
                    var nodeType = cell.value.getAttribute('nodeType');
                    if (nodeType == "0") {
                        //开始节点
                        this.panels.push(new StartFormatPanel(this, ui, div));
                    } else if (nodeType == "99") {
                        //结束节点
                        this.panels.push(new EndFormatPanel(this, ui, div));
                    } else if (nodeType === '98') {
                        this.panels.push(new SubProcessFormatPanel(this, ui, div));
                    } else if (nodeType === '8') {
                        //网关
                        this.panels.push(new GatewayFormatPanel(this, ui, div));
                    }
                    else {
                        label.style.backgroundColor = this.inactiveTabBackgroundColor;
                        label.style.borderLeftWidth = '1px';
                        label.style.width = '50%';
                        var label2 = label.cloneNode(false);
                        // Workaround for ignored background in IE
                        label2.style.backgroundColor = this.inactiveTabBackgroundColor;
                        // Style
                        if (containsLabel) {
                            label2.style.borderLeftWidth = '0px';
                        }
                        else {
                            label.style.borderLeftWidth = '0px';

                            var taskPanel = div.cloneNode(false);
                            taskPanel.style.display = 'none';
                            if (nodeType == "2") {
                                this.panels.push(new CounterTaskFormatPanel(this, ui, taskPanel));
                            } else if (nodeType == "1") {
                                //普通任务
                                this.panels.push(new TaskFormatPanel(this, ui, taskPanel));
                            } else if (nodeType == "3") {
                                //定时任务
                                this.panels.push(new TimerTaskFormatPanel(this, ui, taskPanel));
                            }
                            this.container.appendChild(taskPanel);

                            addClickHandler(label, taskPanel, idx++);
                        }
                        // 单据
                        mxUtils.write(label2, '单据权限');
                        div.appendChild(label2);

                        var billPanel = div.cloneNode(false);
                        billPanel.style.display = 'none';
                        this.panels.push(new TaskBillFormatPanel(this, ui, billPanel));
                        this.container.appendChild(billPanel);
                        addClickHandler(label2, billPanel, idx++);
                    }
                }
            }
        }

    }
};

/**
 * Base class for format panels.
 */
BaseFormatPanel = function (format, editorUi, container) {
    this.format = format;
    this.editorUi = editorUi;
    this.container = container;
    this.listeners = [];
};

/**
 * 
 */
BaseFormatPanel.prototype.buttonBackgroundColor = 'white';

/**
 * Adds the given color option.
 */
BaseFormatPanel.prototype.getSelectionState = function () {
    var graph = this.editorUi.editor.graph;
    var cells = graph.getSelectionCells();
    var shape = null;

    for (var i = 0; i < cells.length; i++) {
        var state = graph.view.getState(cells[i]);

        if (state != null) {
            var tmp = mxUtils.getValue(state.style, mxConstants.STYLE_SHAPE, null);

            if (tmp != null) {
                if (shape == null) {
                    shape = tmp;
                }
                else if (shape != tmp) {
                    return null;
                }
            }

        }
    }

    return shape;
};

/**
 * Install input handler.
 */
BaseFormatPanel.prototype.installInputHandler = function (input, key, defaultValue, min, max, unit, textEditFallback, isFloat) {
    unit = (unit != null) ? unit : '';
    isFloat = (isFloat != null) ? isFloat : false;

    var ui = this.editorUi;
    var graph = ui.editor.graph;

    min = (min != null) ? min : 1;
    max = (max != null) ? max : 999;

    var selState = null;
    var updating = false;

    var update = mxUtils.bind(this, function (evt) {
        var value = (isFloat) ? parseFloat(input.value) : parseInt(input.value);

        // Special case: angle mod 360
        if (!isNaN(value) && key == mxConstants.STYLE_ROTATION) {
            // Workaround for decimal rounding errors in floats is to
            // use integer and round all numbers to two decimal point
            value = mxUtils.mod(Math.round(value * 100), 36000) / 100;
        }

        value = Math.min(max, Math.max(min, (isNaN(value)) ? defaultValue : value));

        if (graph.cellEditor.isContentEditing() && textEditFallback) {
            if (!updating) {
                updating = true;

                if (selState != null) {
                    graph.cellEditor.restoreSelection(selState);
                    selState = null;
                }

                textEditFallback(value);
                input.value = value + unit;

                // Restore focus and selection in input
                updating = false;
            }
        }
        else if (value != mxUtils.getValue(this.format.getSelectionState().style, key, defaultValue)) {
            if (graph.isEditing()) {
                graph.stopEditing(true);
            }

            graph.getModel().beginUpdate();
            try {
                graph.setCellStyles(key, value, graph.getSelectionCells());

                // Handles special case for fontSize where HTML labels are parsed and updated
                if (key == mxConstants.STYLE_FONTSIZE) {
                    graph.updateLabelElements(graph.getSelectionCells(), function (elt) {
                        elt.style.fontSize = value + 'px';
                        elt.removeAttribute('size');
                    });
                }

                ui.fireEvent(new mxEventObject('styleChanged', 'keys', [key],
                    'values', [value], 'cells', graph.getSelectionCells()));
            }
            finally {
                graph.getModel().endUpdate();
            }
        }

        input.value = value + unit;
        mxEvent.consume(evt);
    });

    if (textEditFallback && graph.cellEditor.isContentEditing()) {
        // KNOWN: Arrow up/down clear selection text in quirks/IE 8
        // Text size via arrow button limits to 16 in IE11. Why?
        mxEvent.addListener(input, 'mousedown', function () {
            if (document.activeElement == graph.cellEditor.textarea) {
                selState = graph.cellEditor.saveSelection();
            }
        });

        mxEvent.addListener(input, 'touchstart', function () {
            if (document.activeElement == graph.cellEditor.textarea) {
                selState = graph.cellEditor.saveSelection();
            }
        });
    }

    mxEvent.addListener(input, 'change', update);
    mxEvent.addListener(input, 'blur', update);

    return update;
};

/**
 * Adds the given option.
 */
BaseFormatPanel.prototype.createPanel = function () {
    var div = document.createElement('div');
    div.style.padding = '12px 0px 12px 18px';
    div.style.borderBottom = '1px solid #c0c0c0';

    return div;
};
/**
 *添加一个普通label wyf
 */
BaseFormatPanel.prototype.createLabel = function (div, title) {
    var label = document.createElement('div');
    mxUtils.write(label, title);
    label.style.marginTop = '6px';
    div.appendChild(label);
};
/**
 *添加一个普通textbox wyf
 */
BaseFormatPanel.prototype.createTextbox = function (container, title, value, callback) {
    var input = document.createElement('input');
    input.style.marginTop = '6px';
    input.style.width = '90%';
    input.placeholder = title;
    input.value = value;
    function update(evt) {
        var value = input.value;
        callback && callback(value)
        mxEvent.consume(evt);
    };
    mxEvent.addListener(input, 'blur', update);
    mxEvent.addListener(input, 'change', update);
    container.appendChild(input);
    return input;
};
BaseFormatPanel.prototype.createSelect = function (container, graph, value, callback) {
    container.style.marginLeft = '0px';
    container.style.paddingTop = '4px';
    container.style.paddingBottom = '4px';
    container.style.fontWeight = 'normal';
    var cell = graph.getSelectionCell();
    var select = document.createElement('select');
    var option = document.createElement('option');
    option.value = '-1';
    option.text = '--请选择--';
    select.appendChild(option);

    select.style.width = '200px';
    select.style.marginTop = '2px';
    var cells = graph.getModel().cells;
    for (var i in cells) {
        let c = cells[i];
        let nodeType = c.getAttribute('nodeType');
        if (nodeType && cell.id != c.id && (nodeType == '1' || nodeType == '2' || nodeType == '3')) {
            var dirOption = document.createElement('option');
            dirOption.value = c.id;
            if (c.id == value) {
                dirOption.selected = true;
            }
            dirOption.text = c.getAttribute('label');
            select.add(dirOption);
        }
    }
    container.appendChild(select);


    function update(evt) {
        var value = select.value;
        callback && callback(value);
    }
    mxEvent.addListener(select, 'change', update);
    this.listeners.push({
        destroy: function () {
            mxEvent.removeListener(select, 'change');
        }
    });
    return select;
};
BaseFormatPanel.prototype.createTextarea = function (container, title, value, callback) {
    var input = document.createElement('textarea');
    input.style.marginTop = '2px';
    input.style.width = '90%';
    input.placeholder = title;
    input.rows = 3;
    input.value = value;
    function update(evt) {
        var value = input.value;
        callback && callback(value);
        mxEvent.consume(evt);
    };
    mxEvent.addListener(input, 'blur', update);
    mxEvent.addListener(input, 'change', update);
    mxEvent.addListener(input, 'mouseup', function (evt) {
        input.focus();
        mxEvent.consume(evt);
    });
    mxEvent.addListener(input, 'touchend', function (evt) {
        input.focus();
        mxEvent.consume(evt);
    });
    this.listeners.push({
        destroy: function () {
            // No need to remove listener since textarea is destroyed after edit
        }
    });
    container.appendChild(input);
};
/**
*创建审批人Tag
*/
var createParticipantTags = function (tags, values, callback) {

    if (values) {
        for (let i = 0; i < values.length; i++) {
            let value = values[i];
            let tag = document.createElement('div');
            //tag.style.display = 'inline-block';
            tag.style.position = 'relative';
            tag.style.fontSize = '13px';
            tag.style.fontWeight = 'normal';
            tag.style.verticalAlign = 'baseline';
            tag.style.whiteSpace = 'nowrap';
            let name = value.name;
            tag.title = name;
            if (name.length > 7) {
                name = name.substring(0, 7) + '...';
            }
            if (value.type == '1') {
                tag.style.backgroundColor = 'rgb(148, 142, 142)';
                tag.innerHTML = '用户: ' + name;
            } else if (value.type == '2') {
                tag.style.backgroundColor = '#797979';
                tag.innerHTML = '指派角色: ' + name;
            } else {
                tag.style.backgroundColor = '#444444';
                tag.innerHTML = '动态角色: ' + name;
            }
            tag.style.color = '#FFF';
            tag.style.textShadow = '1px 1px 1px rgba(0, 0, 0, 0.15)';
            tag.style.padding = '4px 22px 5px 9px';
            tag.style.marginBottom = '3px';
            tag.style.marginRight = '3px';
            tag.style.webkitTransition = 'all 0.2s';
            tag.style.transition = 'all 0.2s';
            let btn = document.createElement('button');
            btn.innerHTML = '×';
            btn.style.fontSize = '15px';
            btn.style.opacity = '1';
            btn.style.filter = 'alpha(opacity = 100)';
            btn.style.color = '#FFF';
            btn.style.textShadow = 'none';
            btn.style.float = 'none';
            btn.style.position = 'absolute';
            btn.style.right = '0';
            btn.style.top = '0';
            btn.style.bottom = '0';
            btn.style.width = '18px';
            btn.style.lineHeight = '20px';
            btn.style.textAlign = 'center';
            btn.style.padding = '0';
            btn.style.cursor = 'pointer';
            btn.style.background = 'transparent';
            btn.style.border = '0';
            btn.style.fontWeight = 'bold';

            tag.appendChild(btn);

            mxEvent.addListener(btn, 'click', function (evt) {
                btn.parentNode.remove();
                values.splice(i, 1);
                callback && callback(values);
            })
            tags.appendChild(tag);
        }
    }
}
/**
*创建流转条件Tag
*/
var createTransitionTags = function (tags, value, callback) {
    if (value) {
        let tag = document.createElement('div');
        //tag.style.display = 'inline-block';
        tag.style.position = 'relative';
        tag.style.fontSize = '13px';
        tag.style.fontWeight = 'normal';
        tag.style.verticalAlign = 'baseline';
        tag.style.whiteSpace = 'nowrap';
        tag.title = value;
        if (value.length > 20) {
            value = value.substring(0, 20) + '...';
        }
        tag.style.backgroundColor = 'rgb(148, 142, 142)';

        tag.innerHTML = value;

        tag.style.color = '#FFF';
        tag.style.textShadow = '1px 1px 1px rgba(0, 0, 0, 0.15)';
        tag.style.padding = '4px 22px 5px 9px';
        tag.style.marginBottom = '3px';
        tag.style.marginRight = '3px';
        tag.style.webkitTransition = 'all 0.2s';
        tag.style.transition = 'all 0.2s';
        let btn = document.createElement('button');
        btn.innerHTML = '×';
        btn.style.fontSize = '15px';
        btn.style.opacity = '1';
        btn.style.filter = 'alpha(opacity = 100)';
        btn.style.color = '#FFF';
        btn.style.textShadow = 'none';
        btn.style.float = 'none';
        btn.style.position = 'absolute';
        btn.style.right = '0';
        btn.style.top = '0';
        btn.style.bottom = '0';
        btn.style.width = '18px';
        btn.style.lineHeight = '20px';
        btn.style.textAlign = 'center';
        btn.style.padding = '0';
        btn.style.cursor = 'pointer';
        btn.style.background = 'transparent';
        btn.style.border = '0';
        btn.style.fontWeight = 'bold';

        tag.appendChild(btn);

        mxEvent.addListener(btn, 'click', function (evt) {
            btn.parentNode.remove();
            callback && callback(value);
        });
        tags.appendChild(tag);
    }

}
/**
 * 刷新审批人
 */
var participantsRefresh = function (participantsNode, values) {
    var participants = participantsNode;
    var childs = participants.childNodes;
    var doc = mxUtils.createXmlDocument();
    if (childs) {
        for (var i = childs.length - 1; i >= 0; i--) {
            let participant = childs[i];
            if (participant.nodeType == 1) {
                let ptype = participant.getAttribute('pType');
                if (ptype === '1' || ptype === '2' || ptype === '3') {
                    //participantsNode.removeChild(participant);
                    participant.parentNode.removeChild(participant);
                }
            }
        }
    }
    if (values) {
        for (var i = 0; i < values.length; i++) {
            var v = values[i];
            let pNode = doc.createElement('participant');
            participants.appendChild(pNode);
            pNode.setAttribute('pValue', v.id);
            pNode.setAttribute('pName', v.name);
            pNode.setAttribute('pType', v.type);
            pNode.setAttribute('pColumn', v.colname);
        }
    }
}

/**
 * Adds the given option.
 */
BaseFormatPanel.prototype.createTitle = function (title) {
    var div = document.createElement('div');
    div.style.padding = '0px 0px 6px 0px';
    div.style.whiteSpace = 'nowrap';
    div.style.overflow = 'hidden';
    div.style.width = '200px';
    div.style.fontWeight = 'bold';
    mxUtils.write(div, title);

    return div;
};

/**
 * 
 */
BaseFormatPanel.prototype.createStepper = function (input, update, step, height, disableFocus, defaultValue) {
    step = (step != null) ? step : 1;
    height = (height != null) ? height : 8;

    if (mxClient.IS_QUIRKS) {
        height = height - 2;
    }
    else if (mxClient.IS_MT || document.documentMode >= 8) {
        height = height + 1;
    }

    var stepper = document.createElement('div');
    mxUtils.setPrefixedStyle(stepper.style, 'borderRadius', '3px');
    stepper.style.border = '1px solid rgb(192, 192, 192)';
    stepper.style.position = 'absolute';

    var up = document.createElement('div');
    up.style.borderBottom = '1px solid rgb(192, 192, 192)';
    up.style.position = 'relative';
    up.style.height = height + 'px';
    up.style.width = '10px';
    up.className = 'geBtnUp';
    stepper.appendChild(up);

    var down = up.cloneNode(false);
    down.style.border = 'none';
    down.style.height = height + 'px';
    down.className = 'geBtnDown';
    stepper.appendChild(down);

    mxEvent.addListener(down, 'click', function (evt) {
        if (input.value == '') {
            input.value = defaultValue || '2';
        }

        var val = parseInt(input.value);

        if (!isNaN(val)) {
            input.value = val - step;

            if (update != null) {
                update(evt);
            }
        }

        mxEvent.consume(evt);
    });

    mxEvent.addListener(up, 'click', function (evt) {
        if (input.value == '') {
            input.value = defaultValue || '0';
        }

        var val = parseInt(input.value);

        if (!isNaN(val)) {
            input.value = val + step;

            if (update != null) {
                update(evt);
            }
        }

        mxEvent.consume(evt);
    });

    // Disables transfer of focus to DIV but also :active CSS
    // so it's only used for fontSize where the focus should
    // stay on the selected text, but not for any other input.
    if (disableFocus) {
        var currentSelection = null;

        mxEvent.addGestureListeners(stepper,
            function (evt) {
                // Workaround for lost current selection in page because of focus in IE
                if (mxClient.IS_QUIRKS || document.documentMode == 8) {
                    currentSelection = document.selection.createRange();
                }

                mxEvent.consume(evt);
            },
            null,
            function (evt) {
                // Workaround for lost current selection in page because of focus in IE
                if (currentSelection != null) {
                    try {
                        currentSelection.select();
                    }
                    catch (e) {
                        // ignore
                    }

                    currentSelection = null;
                    mxEvent.consume(evt);
                }
            }
        );
    }

    return stepper;
};

/**
 * Adds the given option.
 */
BaseFormatPanel.prototype.createOption = function (label, isCheckedFn, setCheckedFn, listener) {
    var div = document.createElement('div');
    div.style.padding = '6px 0px 1px 0px';
    div.style.whiteSpace = 'nowrap';
    div.style.overflow = 'hidden';
    div.style.width = '200px';
    div.style.height = (mxClient.IS_QUIRKS) ? '27px' : '18px';

    var cb = document.createElement('input');
    cb.setAttribute('type', 'checkbox');
    cb.style.margin = '0px 6px 0px 0px';
    div.appendChild(cb);

    var span = document.createElement('span');
    mxUtils.write(span, label);
    div.appendChild(span);

    var applying = false;
    var value = isCheckedFn();

    var apply = function (newValue) {
        if (!applying) {
            applying = true;
            //debugger
            if (newValue == '1') {
                cb.setAttribute('checked', 'checked');
                cb.defaultChecked = true;
                cb.checked = true;
            }
            else {
                cb.removeAttribute('checked');
                cb.defaultChecked = false;
                cb.checked = false;
            }

            if (value != newValue) {
                value = newValue;

                // Checks if the color value needs to be updated in the model
                if (isCheckedFn() != value) {
                    setCheckedFn(value ? 1 : 0);
                }
            }

            applying = false;
        }
    };

    mxEvent.addListener(div, 'click', function (evt) {
        if (cb.getAttribute('disabled') != 'disabled') {
            // Toggles checkbox state for click on label
            var source = mxEvent.getSource(evt);

            if (source == div || source == span) {
                cb.checked = !cb.checked;
            }

            apply(cb.checked);
        }

        mxEvent.consume(evt);
    });

    apply(value);

    if (listener != null) {
        listener.install(apply);
        this.listeners.push(listener);
    }

    return div;
};

/**
 * The string 'null' means use null in values.
 */
BaseFormatPanel.prototype.createCellOption = function (label, key, defaultValue, enabledValue, disabledValue, fn, action, stopEditing) {
    enabledValue = (enabledValue != null) ? ((enabledValue == 'null') ? null : enabledValue) : '1';
    disabledValue = (disabledValue != null) ? ((disabledValue == 'null') ? null : disabledValue) : '0';

    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    return this.createOption(label, function () {
        // Seems to be null sometimes, not sure why...
        var state = graph.view.getState(graph.getSelectionCell());

        if (state != null) {
            return mxUtils.getValue(state.style, key, defaultValue) != disabledValue;
        }

        return null;
    }, function (checked) {
        if (stopEditing) {
            graph.stopEditing();
        }

        if (action != null) {
            action.funct();
        }
        else {
            graph.getModel().beginUpdate();
            try {
                var value = (checked) ? enabledValue : disabledValue;
                graph.setCellStyles(key, value, graph.getSelectionCells());

                if (fn != null) {
                    fn(graph.getSelectionCells(), value);
                }

                ui.fireEvent(new mxEventObject('styleChanged', 'keys', [key],
                    'values', [value], 'cells', graph.getSelectionCells()));
            }
            finally {
                graph.getModel().endUpdate();
            }
        }
    },
        {
            install: function (apply) {
                this.listener = function () {
                    // Seems to be null sometimes, not sure why...
                    var state = graph.view.getState(graph.getSelectionCell());

                    if (state != null) {
                        apply(mxUtils.getValue(state.style, key, defaultValue) != disabledValue);
                    }
                };

                graph.getModel().addListener(mxEvent.CHANGE, this.listener);
            },
            destroy: function () {
                graph.getModel().removeListener(this.listener);
            }
        });
};

/**
 * Adds the given color option.
 */
BaseFormatPanel.prototype.createColorOption = function (label, getColorFn, setColorFn, defaultColor, listener, callbackFn, hideCheckbox) {
    var div = document.createElement('div');
    div.style.padding = '6px 0px 1px 0px';
    div.style.whiteSpace = 'nowrap';
    div.style.overflow = 'hidden';
    div.style.width = '200px';
    div.style.height = (mxClient.IS_QUIRKS) ? '27px' : '18px';

    var cb = document.createElement('input');
    cb.setAttribute('type', 'checkbox');
    cb.style.margin = '0px 6px 0px 0px';

    if (!hideCheckbox) {
        div.appendChild(cb);
    }

    var span = document.createElement('span');
    mxUtils.write(span, label);
    div.appendChild(span);

    var applying = false;
    var value = getColorFn();

    var btn = null;

    var apply = function (color, disableUpdate, forceUpdate) {
        if (!applying) {
            applying = true;
            btn.innerHTML = '<div style="width:' + ((mxClient.IS_QUIRKS) ? '30' : '36') +
                'px;height:12px;margin:3px;border:1px solid black;background-color:' +
                ((color != null && color != mxConstants.NONE) ? color : defaultColor) + ';"></div>';

            // Fine-tuning in Firefox, quirks mode and IE8 standards
            if (mxClient.IS_QUIRKS || document.documentMode == 8) {
                btn.firstChild.style.margin = '0px';
            }

            if (color != null && color != mxConstants.NONE) {
                cb.setAttribute('checked', 'checked');
                cb.defaultChecked = true;
                cb.checked = true;
            }
            else {
                cb.removeAttribute('checked');
                cb.defaultChecked = false;
                cb.checked = false;
            }

            btn.style.display = (cb.checked || hideCheckbox) ? '' : 'none';

            if (callbackFn != null) {
                callbackFn(color);
            }

            if (!disableUpdate) {
                value = color;

                // Checks if the color value needs to be updated in the model
                if (forceUpdate || hideCheckbox || getColorFn() != value) {
                    setColorFn(value);
                }
            }

            applying = false;
        }
    };

    btn = mxUtils.button('', mxUtils.bind(this, function (evt) {
        this.editorUi.pickColor(value, function (color) {
            apply(color, null, true);
        });
        mxEvent.consume(evt);
    }));

    btn.style.position = 'absolute';
    btn.style.marginTop = '-4px';
    btn.style.right = (mxClient.IS_QUIRKS) ? '0px' : '20px';
    btn.style.height = '22px';
    btn.className = 'geColorBtn';
    btn.style.display = (cb.checked || hideCheckbox) ? '' : 'none';
    div.appendChild(btn);

    mxEvent.addListener(div, 'click', function (evt) {
        var source = mxEvent.getSource(evt);

        if (source == cb || source.nodeName != 'INPUT') {
            // Toggles checkbox state for click on label
            if (source != cb) {
                cb.checked = !cb.checked;
            }

            // Overrides default value with current value to make it easier
            // to restore previous value if the checkbox is clicked twice
            if (!cb.checked && value != null && value != mxConstants.NONE &&
                defaultColor != mxConstants.NONE) {
                defaultColor = value;
            }

            apply((cb.checked) ? defaultColor : mxConstants.NONE);
        }
    });

    apply(value, true);

    if (listener != null) {
        listener.install(apply);
        this.listeners.push(listener);
    }

    return div;
};

/**
 * 
 */
BaseFormatPanel.prototype.createCellColorOption = function (label, colorKey, defaultColor, callbackFn, setStyleFn) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    return this.createColorOption(label, function () {
        // Seems to be null sometimes, not sure why...
        var state = graph.view.getState(graph.getSelectionCell());

        if (state != null) {
            return mxUtils.getValue(state.style, colorKey, null);
        }

        return null;
    }, function (color) {
        graph.getModel().beginUpdate();
        try {
            if (setStyleFn != null) {
                setStyleFn(color);
            }

            graph.setCellStyles(colorKey, color, graph.getSelectionCells());
            ui.fireEvent(new mxEventObject('styleChanged', 'keys', [colorKey],
                'values', [color], 'cells', graph.getSelectionCells()));
        }
        finally {
            graph.getModel().endUpdate();
        }
    }, defaultColor || mxConstants.NONE,
        {
            install: function (apply) {
                this.listener = function () {
                    // Seems to be null sometimes, not sure why...
                    var state = graph.view.getState(graph.getSelectionCell());

                    if (state != null) {
                        apply(mxUtils.getValue(state.style, colorKey, null));
                    }
                };

                graph.getModel().addListener(mxEvent.CHANGE, this.listener);
            },
            destroy: function () {
                graph.getModel().removeListener(this.listener);
            }
        }, callbackFn);
};

/**
 * 
 */
BaseFormatPanel.prototype.addArrow = function (elt, height) {
    height = (height != null) ? height : 10;

    var arrow = document.createElement('div');
    arrow.style.display = (mxClient.IS_QUIRKS) ? 'inline' : 'inline-block';
    arrow.style.padding = '6px';
    arrow.style.paddingRight = '4px';

    var m = (10 - height);

    if (m == 2) {
        arrow.style.paddingTop = 6 + 'px';
    }
    else if (m > 0) {
        arrow.style.paddingTop = (6 - m) + 'px';
    }
    else {
        arrow.style.marginTop = '-2px';
    }

    arrow.style.height = height + 'px';
    arrow.style.borderLeft = '1px solid #a0a0a0';
    arrow.innerHTML = '<img border="0" src="' + ((mxClient.IS_SVG) ? 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAHBJREFUeNpidHB2ZyAGsACxDRBPIKCuA6TwCBB/h2rABu4A8SYmKCcXiP/iUFgAxL9gCi8A8SwsirZCMQMTkmANEH9E4v+CmsaArvAdyNFI/FlQ92EoBIE+qCRIUz168DBgsU4OqhinQpgHMABAgAEALY4XLIsJ20oAAAAASUVORK5CYII=' :
        IMAGE_PATH + '/dropdown.png') + '" style="margin-bottom:4px;">';
    mxUtils.setOpacity(arrow, 70);

    var symbol = elt.getElementsByTagName('div')[0];

    if (symbol != null) {
        symbol.style.paddingRight = '6px';
        symbol.style.marginLeft = '4px';
        symbol.style.marginTop = '-1px';
        symbol.style.display = (mxClient.IS_QUIRKS) ? 'inline' : 'inline-block';
        mxUtils.setOpacity(symbol, 60);
    }

    mxUtils.setOpacity(elt, 100);
    elt.style.border = '1px solid #a0a0a0';
    elt.style.backgroundColor = this.buttonBackgroundColor;
    elt.style.backgroundImage = 'none';
    elt.style.width = 'auto';
    elt.className += ' geColorBtn';
    mxUtils.setPrefixedStyle(elt.style, 'borderRadius', '3px');

    elt.appendChild(arrow);

    return symbol;
};

/**
 * 
 */
BaseFormatPanel.prototype.addUnitInput = function (container, unit, right, width, update, step, marginTop, disableFocus) {
    marginTop = (marginTop != null) ? marginTop : 0;

    var input = document.createElement('input');
    input.style.position = 'absolute';
    input.style.textAlign = 'right';
    input.style.marginTop = '-2px';
    input.style.right = (right + 12) + 'px';
    input.style.width = width + 'px';
    container.appendChild(input);

    var stepper = this.createStepper(input, update, step, null, disableFocus);
    stepper.style.marginTop = (marginTop - 2) + 'px';
    stepper.style.right = right + 'px';
    container.appendChild(stepper);

    return input;
};

/**
 * 
 */
BaseFormatPanel.prototype.createRelativeOption = function (label, width, handler, init) {
    width = (width != null) ? width : 44;

    var div = this.createPanel();
    div.style.paddingTop = '10px';
    div.style.paddingBottom = '10px';
    mxUtils.write(div, label);
    div.style.fontWeight = 'bold';

    function update(evt) {
        if (handler != null) {
            handler(input);
        }

        mxEvent.consume(evt);
    };

    var input = this.addUnitInput(div, '%', 20, width, update, 10, -15, handler != null);

    mxEvent.addListener(input, 'blur', update);
    mxEvent.addListener(input, 'change', update);

    if (init != null) {
        init(input);
    }

    return div;
};

/**
 * 
 */
BaseFormatPanel.prototype.addLabel = function (div, title, right, width) {
    width = (width != null) ? width : 61;

    var label = document.createElement('div');
    mxUtils.write(label, title);
    label.style.position = 'absolute';
    label.style.right = right + 'px';
    label.style.width = width + 'px';
    label.style.marginTop = '6px';
    label.style.textAlign = 'center';
    div.appendChild(label);
};

/**
 * 
 */
BaseFormatPanel.prototype.addKeyHandler = function (input, listener) {
    mxEvent.addListener(input, 'keydown', mxUtils.bind(this, function (e) {
        if (e.keyCode == 13) {
            this.editorUi.editor.graph.container.focus();
            mxEvent.consume(e);
        }
        else if (e.keyCode == 27) {
            if (listener != null) {
                listener(null, null, true);
            }

            this.editorUi.editor.graph.container.focus();
            mxEvent.consume(e);
        }
    }));
};

/**
 * 
 */
BaseFormatPanel.prototype.styleButtons = function (elts) {
    for (var i = 0; i < elts.length; i++) {
        mxUtils.setPrefixedStyle(elts[i].style, 'borderRadius', '3px');
        mxUtils.setOpacity(elts[i], 100);
        elts[i].style.border = '1px solid #a0a0a0';
        elts[i].style.padding = '4px';
        elts[i].style.paddingTop = '3px';
        elts[i].style.paddingRight = '1px';
        elts[i].style.margin = '1px';
        elts[i].style.width = '24px';
        elts[i].style.height = '20px';
        elts[i].className += ' geColorBtn';
    }
};

/**
 * Adds the label menu items to the given menu and parent.
 */
BaseFormatPanel.prototype.destroy = function () {
    if (this.listeners != null) {
        for (var i = 0; i < this.listeners.length; i++) {
            this.listeners[i].destroy();
        }

        this.listeners = null;
    }
};

/**
 * Adds the label menu items to the given menu and parent.
 */
ArrangePanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(ArrangePanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
ArrangePanel.prototype.init = function () {
    var graph = this.editorUi.editor.graph;
    var ss = this.format.getSelectionState();

    this.container.appendChild(this.addLayerOps(this.createPanel()));
    // Special case that adds two panels
    this.addGeometry(this.container);
    this.addEdgeGeometry(this.container);

    if (!ss.containsLabel || ss.edges.length == 0) {
        this.container.appendChild(this.addAngle(this.createPanel()));
    }

    if (!ss.containsLabel && ss.edges.length == 0) {
        this.container.appendChild(this.addFlip(this.createPanel()));
    }

    if (ss.vertices.length > 1) {
        this.container.appendChild(this.addAlign(this.createPanel()));
        this.container.appendChild(this.addDistribute(this.createPanel()));
    }

    this.container.appendChild(this.addGroupOps(this.createPanel()));
};

/**
 * 
 */
ArrangePanel.prototype.addLayerOps = function (div) {
    var ui = this.editorUi;

    var btn = mxUtils.button(mxResources.get('toFront'), function (evt) {
        ui.actions.get('toFront').funct();
    })

    btn.setAttribute('title', mxResources.get('toFront') + ' (' + this.editorUi.actions.get('toFront').shortcut + ')');
    btn.style.width = '100px';
    btn.style.marginRight = '2px';
    div.appendChild(btn);

    var btn = mxUtils.button(mxResources.get('toBack'), function (evt) {
        ui.actions.get('toBack').funct();
    })

    btn.setAttribute('title', mxResources.get('toBack') + ' (' + this.editorUi.actions.get('toBack').shortcut + ')');
    btn.style.width = '100px';
    div.appendChild(btn);

    return div;
};

/**
 * 
 */
ArrangePanel.prototype.addGroupOps = function (div) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;
    var cell = graph.getSelectionCell();
    var ss = this.format.getSelectionState();
    var count = 0;
    var btn = null;

    div.style.paddingTop = '8px';
    div.style.paddingBottom = '6px';

    if (graph.getSelectionCount() > 1) {
        btn = mxUtils.button(mxResources.get('group'), function (evt) {
            ui.actions.get('group').funct();
        })

        btn.setAttribute('title', mxResources.get('group') + ' (' + this.editorUi.actions.get('group').shortcut + ')');
        btn.style.width = '202px';
        btn.style.marginBottom = '2px';
        div.appendChild(btn);
        count++;
    }
    else if (graph.getSelectionCount() == 1 && !graph.getModel().isEdge(cell) && !graph.isSwimlane(cell) &&
        graph.getModel().getChildCount(cell) > 0) {
        btn = mxUtils.button(mxResources.get('ungroup'), function (evt) {
            ui.actions.get('ungroup').funct();
        })

        btn.setAttribute('title', mxResources.get('ungroup') + ' (' + this.editorUi.actions.get('ungroup').shortcut + ')');
        btn.style.width = '202px';
        btn.style.marginBottom = '2px';
        div.appendChild(btn);
        count++;
    }

    if (graph.getSelectionCount() == 1 && graph.getModel().isVertex(cell) &&
        graph.getModel().isVertex(graph.getModel().getParent(cell))) {
        if (count > 0) {
            mxUtils.br(div);
        }

        btn = mxUtils.button(mxResources.get('removeFromGroup'), function (evt) {
            ui.actions.get('removeFromGroup').funct();
        })

        btn.setAttribute('title', mxResources.get('removeFromGroup'));
        btn.style.width = '202px';
        btn.style.marginBottom = '2px';
        div.appendChild(btn);
        count++;
    }
    else if (graph.getSelectionCount() > 0) {
        if (count > 0) {
            mxUtils.br(div);
        }

        btn = mxUtils.button(mxResources.get('clearWaypoints'), mxUtils.bind(this, function (evt) {
            this.editorUi.actions.get('clearWaypoints').funct();
        }));

        btn.setAttribute('title', mxResources.get('clearWaypoints') + ' (' + this.editorUi.actions.get('clearWaypoints').shortcut + ')');
        btn.style.width = '202px';
        btn.style.marginBottom = '2px';
        div.appendChild(btn);

        count++;
    }

    if (graph.getSelectionCount() == 1) {
        if (count > 0) {
            mxUtils.br(div);
        }

        btn = mxUtils.button(mxResources.get('editData'), mxUtils.bind(this, function (evt) {
            this.editorUi.actions.get('editData').funct();
        }));

        btn.setAttribute('title', mxResources.get('editData') + ' (' + this.editorUi.actions.get('editData').shortcut + ')');
        btn.style.width = '100px';
        btn.style.marginBottom = '2px';
        div.appendChild(btn);
        count++;

        btn = mxUtils.button(mxResources.get('editLink'), mxUtils.bind(this, function (evt) {
            this.editorUi.actions.get('editLink').funct();
        }));

        btn.setAttribute('title', mxResources.get('editLink'));
        btn.style.width = '100px';
        btn.style.marginLeft = '2px';
        btn.style.marginBottom = '2px';
        div.appendChild(btn);
        count++;
    }

    if (count == 0) {
        div.style.display = 'none';
    }

    return div;
};

/**
 * 
 */
ArrangePanel.prototype.addAlign = function (div) {
    var graph = this.editorUi.editor.graph;
    div.style.paddingTop = '6px';
    div.style.paddingBottom = '12px';
    div.appendChild(this.createTitle(mxResources.get('align')));

    var stylePanel = document.createElement('div');
    stylePanel.style.position = 'relative';
    stylePanel.style.paddingLeft = '0px';
    stylePanel.style.borderWidth = '0px';
    stylePanel.className = 'geToolbarContainer';

    if (mxClient.IS_QUIRKS) {
        div.style.height = '60px';
    }

    var left = this.editorUi.toolbar.addButton('geSprite-alignleft', mxResources.get('left'),
        function () { graph.alignCells(mxConstants.ALIGN_LEFT); }, stylePanel);
    var center = this.editorUi.toolbar.addButton('geSprite-aligncenter', mxResources.get('center'),
        function () { graph.alignCells(mxConstants.ALIGN_CENTER); }, stylePanel);
    var right = this.editorUi.toolbar.addButton('geSprite-alignright', mxResources.get('right'),
        function () { graph.alignCells(mxConstants.ALIGN_RIGHT); }, stylePanel);

    var top = this.editorUi.toolbar.addButton('geSprite-aligntop', mxResources.get('top'),
        function () { graph.alignCells(mxConstants.ALIGN_TOP); }, stylePanel);
    var middle = this.editorUi.toolbar.addButton('geSprite-alignmiddle', mxResources.get('middle'),
        function () { graph.alignCells(mxConstants.ALIGN_MIDDLE); }, stylePanel);
    var bottom = this.editorUi.toolbar.addButton('geSprite-alignbottom', mxResources.get('bottom'),
        function () { graph.alignCells(mxConstants.ALIGN_BOTTOM); }, stylePanel);

    this.styleButtons([left, center, right, top, middle, bottom]);
    right.style.marginRight = '6px';
    div.appendChild(stylePanel);

    return div;
};

/**
 * 
 */
ArrangePanel.prototype.addFlip = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    div.style.paddingTop = '6px';
    div.style.paddingBottom = '10px';

    var span = document.createElement('div');
    span.style.marginTop = '2px';
    span.style.marginBottom = '8px';
    span.style.fontWeight = 'bold';
    mxUtils.write(span, mxResources.get('flip'));
    div.appendChild(span);

    var btn = mxUtils.button(mxResources.get('horizontal'), function (evt) {
        graph.toggleCellStyles(mxConstants.STYLE_FLIPH, false);
    })

    btn.setAttribute('title', mxResources.get('horizontal'));
    btn.style.width = '100px';
    btn.style.marginRight = '2px';
    div.appendChild(btn);

    var btn = mxUtils.button(mxResources.get('vertical'), function (evt) {
        graph.toggleCellStyles(mxConstants.STYLE_FLIPV, false);
    })

    btn.setAttribute('title', mxResources.get('vertical'));
    btn.style.width = '100px';
    div.appendChild(btn);

    return div;
};

/**
 * 
 */
ArrangePanel.prototype.addDistribute = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    div.style.paddingTop = '6px';
    div.style.paddingBottom = '12px';

    div.appendChild(this.createTitle(mxResources.get('distribute')));

    var btn = mxUtils.button(mxResources.get('horizontal'), function (evt) {
        graph.distributeCells(true);
    })

    btn.setAttribute('title', mxResources.get('horizontal'));
    btn.style.width = '100px';
    btn.style.marginRight = '2px';
    div.appendChild(btn);

    var btn = mxUtils.button(mxResources.get('vertical'), function (evt) {
        graph.distributeCells(false);
    })

    btn.setAttribute('title', mxResources.get('vertical'));
    btn.style.width = '100px';
    div.appendChild(btn);

    return div;
};

/**
 * 
 */
ArrangePanel.prototype.addAngle = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var ss = this.format.getSelectionState();

    div.style.paddingBottom = '8px';

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.width = '70px';
    span.style.marginTop = '0px';
    span.style.fontWeight = 'bold';

    var input = null;
    var update = null;
    var btn = null;

    if (ss.edges.length == 0) {
        mxUtils.write(span, mxResources.get('angle'));
        div.appendChild(span);

        input = this.addUnitInput(div, '°', 20, 44, function () {
            update.apply(this, arguments);
        });

        mxUtils.br(div);
        div.style.paddingTop = '10px';
    }
    else {
        div.style.paddingTop = '8px';
    }

    if (!ss.containsLabel) {
        var label = mxResources.get('reverse');

        if (ss.vertices.length > 0 && ss.edges.length > 0) {
            label = mxResources.get('turn') + ' / ' + label;
        }
        else if (ss.vertices.length > 0) {
            label = mxResources.get('turn');
        }

        btn = mxUtils.button(label, function (evt) {
            ui.actions.get('turn').funct();
        })

        btn.setAttribute('title', label + ' (' + this.editorUi.actions.get('turn').shortcut + ')');
        btn.style.width = '202px';
        div.appendChild(btn);

        if (input != null) {
            btn.style.marginTop = '8px';
        }
    }

    if (input != null) {
        var listener = mxUtils.bind(this, function (sender, evt, force) {
            if (force || document.activeElement != input) {
                ss = this.format.getSelectionState();
                var tmp = parseFloat(mxUtils.getValue(ss.style, mxConstants.STYLE_ROTATION, 0));
                input.value = (isNaN(tmp)) ? '' : tmp + '°';
            }
        });

        update = this.installInputHandler(input, mxConstants.STYLE_ROTATION, 0, 0, 360, '°', null, true);
        this.addKeyHandler(input, listener);

        graph.getModel().addListener(mxEvent.CHANGE, listener);
        this.listeners.push({ destroy: function () { graph.getModel().removeListener(listener); } });
        listener();
    }

    return div;
};

/**
 * 
 */
ArrangePanel.prototype.addGeometry = function (container) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;
    var rect = this.format.getSelectionState();

    var div = this.createPanel();
    div.style.paddingBottom = '8px';

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.width = '50px';
    span.style.marginTop = '0px';
    span.style.fontWeight = 'bold';
    mxUtils.write(span, mxResources.get('size'));
    div.appendChild(span);

    var widthUpdate, heightUpdate, leftUpdate, topUpdate;
    var width = this.addUnitInput(div, 'pt', 84, 44, function () {
        widthUpdate.apply(this, arguments);
    });
    var height = this.addUnitInput(div, 'pt', 20, 44, function () {
        heightUpdate.apply(this, arguments);
    });

    var autosizeBtn = document.createElement('div');
    autosizeBtn.className = 'geSprite geSprite-fit';
    autosizeBtn.setAttribute('title', mxResources.get('autosize') + ' (' + this.editorUi.actions.get('autosize').shortcut + ')');
    autosizeBtn.style.position = 'relative';
    autosizeBtn.style.cursor = 'pointer';
    autosizeBtn.style.marginTop = '-3px';
    autosizeBtn.style.border = '0px';
    autosizeBtn.style.left = '52px';
    mxUtils.setOpacity(autosizeBtn, 50);

    mxEvent.addListener(autosizeBtn, 'mouseenter', function () {
        mxUtils.setOpacity(autosizeBtn, 100);
    });

    mxEvent.addListener(autosizeBtn, 'mouseleave', function () {
        mxUtils.setOpacity(autosizeBtn, 50);
    });

    mxEvent.addListener(autosizeBtn, 'click', function () {
        ui.actions.get('autosize').funct();
    });

    div.appendChild(autosizeBtn);
    this.addLabel(div, mxResources.get('width'), 84);
    this.addLabel(div, mxResources.get('height'), 20);
    mxUtils.br(div);

    var wrapper = document.createElement('div');
    wrapper.style.paddingTop = '8px';
    wrapper.style.paddingRight = '20px';
    wrapper.style.whiteSpace = 'nowrap';
    wrapper.style.textAlign = 'right';
    var opt = this.createCellOption(mxResources.get('constrainProportions'),
        mxConstants.STYLE_ASPECT, null, 'fixed', 'null');
    opt.style.width = '100%';
    wrapper.appendChild(opt);
    div.appendChild(wrapper);

    var constrainCheckbox = opt.getElementsByTagName('input')[0];
    this.addKeyHandler(width, listener);
    this.addKeyHandler(height, listener);

    widthUpdate = this.addGeometryHandler(width, function (geo, value) {
        if (geo.width > 0) {
            var value = Math.max(1, value);

            if (constrainCheckbox.checked) {
                geo.height = Math.round((geo.height * value * 100) / geo.width) / 100;
            }

            geo.width = value;
        }
    });
    heightUpdate = this.addGeometryHandler(height, function (geo, value) {
        if (geo.height > 0) {
            var value = Math.max(1, value);

            if (constrainCheckbox.checked) {
                geo.width = Math.round((geo.width * value * 100) / geo.height) / 100;
            }

            geo.height = value;
        }
    });

    container.appendChild(div);

    var div2 = this.createPanel();
    div2.style.paddingBottom = '30px';

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.width = '70px';
    span.style.marginTop = '0px';
    span.style.fontWeight = 'bold';
    mxUtils.write(span, mxResources.get('position'));
    div2.appendChild(span);

    var left = this.addUnitInput(div2, 'pt', 84, 44, function () {
        leftUpdate.apply(this, arguments);
    });
    var top = this.addUnitInput(div2, 'pt', 20, 44, function () {
        topUpdate.apply(this, arguments);
    });

    mxUtils.br(div2);
    this.addLabel(div2, mxResources.get('left'), 84);
    this.addLabel(div2, mxResources.get('top'), 20);

    var listener = mxUtils.bind(this, function (sender, evt, force) {
        rect = this.format.getSelectionState();

        if (!rect.containsLabel && rect.vertices.length == graph.getSelectionCount() &&
            rect.width != null && rect.height != null) {
            div.style.display = '';

            if (force || document.activeElement != width) {
                width.value = rect.width + ((rect.width == '') ? '' : ' pt');
            }

            if (force || document.activeElement != height) {
                height.value = rect.height + ((rect.height == '') ? '' : ' pt');
            }
        }
        else {
            div.style.display = 'none';
        }

        if (rect.vertices.length == graph.getSelectionCount() &&
            rect.x != null && rect.y != null) {
            div2.style.display = '';

            if (force || document.activeElement != left) {
                left.value = rect.x + ((rect.x == '') ? '' : ' pt');
            }

            if (force || document.activeElement != top) {
                top.value = rect.y + ((rect.y == '') ? '' : ' pt');
            }
        }
        else {
            div2.style.display = 'none';
        }
    });

    this.addKeyHandler(left, listener);
    this.addKeyHandler(top, listener);

    graph.getModel().addListener(mxEvent.CHANGE, listener);
    this.listeners.push({ destroy: function () { graph.getModel().removeListener(listener); } });
    listener();

    leftUpdate = this.addGeometryHandler(left, function (geo, value) {
        if (geo.relative) {
            geo.offset.x = value;
        }
        else {
            geo.x = value;
        }
    });
    topUpdate = this.addGeometryHandler(top, function (geo, value) {
        if (geo.relative) {
            geo.offset.y = value;
        }
        else {
            geo.y = value;
        }
    });

    container.appendChild(div2);
};

/**
 * 
 */
ArrangePanel.prototype.addGeometryHandler = function (input, fn) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;
    var initialValue = null;

    function update(evt) {
        if (input.value != '') {
            var value = parseFloat(input.value);

            if (value != initialValue) {
                graph.getModel().beginUpdate();
                try {
                    var cells = graph.getSelectionCells();

                    for (var i = 0; i < cells.length; i++) {
                        if (graph.getModel().isVertex(cells[i])) {
                            var geo = graph.getCellGeometry(cells[i]);

                            if (geo != null) {
                                geo = geo.clone();
                                fn(geo, value);

                                graph.getModel().setGeometry(cells[i], geo);
                            }
                        }
                    }
                }
                finally {
                    graph.getModel().endUpdate();
                }

                initialValue = value;
                input.value = value + ' pt';
            }
            else if (isNaN(value)) {
                input.value = initialValue + ' pt';
            }
        }

        mxEvent.consume(evt);
    };

    mxEvent.addListener(input, 'blur', update);
    mxEvent.addListener(input, 'change', update);
    mxEvent.addListener(input, 'focus', function () {
        initialValue = input.value;
    });

    return update;
};

ArrangePanel.prototype.addEdgeGeometryHandler = function (input, fn) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;
    var initialValue = null;

    function update(evt) {
        if (input.value != '') {
            var value = parseFloat(input.value);

            if (isNaN(value)) {
                input.value = initialValue + ' pt';
            }
            else if (value != initialValue) {
                graph.getModel().beginUpdate();
                try {
                    var cells = graph.getSelectionCells();

                    for (var i = 0; i < cells.length; i++) {
                        if (graph.getModel().isEdge(cells[i])) {
                            var geo = graph.getCellGeometry(cells[i]);

                            if (geo != null) {
                                geo = geo.clone();
                                fn(geo, value);

                                graph.getModel().setGeometry(cells[i], geo);
                            }
                        }
                    }
                }
                finally {
                    graph.getModel().endUpdate();
                }

                initialValue = value;
                input.value = value + ' pt';
            }
        }

        mxEvent.consume(evt);
    };

    mxEvent.addListener(input, 'blur', update);
    mxEvent.addListener(input, 'change', update);
    mxEvent.addListener(input, 'focus', function () {
        initialValue = input.value;
    });

    return update;
};

/**
 * 
 */
ArrangePanel.prototype.addEdgeGeometry = function (container) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;
    var rect = this.format.getSelectionState();

    var div = this.createPanel();

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.width = '70px';
    span.style.marginTop = '0px';
    span.style.fontWeight = 'bold';
    mxUtils.write(span, mxResources.get('width'));
    div.appendChild(span);

    var widthUpdate, xtUpdate, ytUpdate, xsUpdate, ysUpdate;
    var width = this.addUnitInput(div, 'pt', 20, 44, function () {
        widthUpdate.apply(this, arguments);
    });

    mxUtils.br(div);
    this.addKeyHandler(width, listener);

    function widthUpdate(evt) {
        // Maximum stroke width is 999
        var value = parseInt(width.value);
        value = Math.min(999, Math.max(1, (isNaN(value)) ? 1 : value));

        if (value != mxUtils.getValue(rect.style, 'width', mxCellRenderer.defaultShapes['flexArrow'].prototype.defaultWidth)) {
            graph.setCellStyles('width', value, graph.getSelectionCells());
            ui.fireEvent(new mxEventObject('styleChanged', 'keys', ['width'],
                'values', [value], 'cells', graph.getSelectionCells()));
        }

        width.value = value + ' pt';
        mxEvent.consume(evt);
    };

    mxEvent.addListener(width, 'blur', widthUpdate);
    mxEvent.addListener(width, 'change', widthUpdate);

    container.appendChild(div);

    var divs = this.createPanel();
    divs.style.paddingBottom = '30px';

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.width = '70px';
    span.style.marginTop = '0px';
    span.style.fontWeight = 'bold';
    mxUtils.write(span, 'Start');
    divs.appendChild(span);

    var xs = this.addUnitInput(divs, 'pt', 84, 44, function () {
        xsUpdate.apply(this, arguments);
    });
    var ys = this.addUnitInput(divs, 'pt', 20, 44, function () {
        ysUpdate.apply(this, arguments);
    });

    mxUtils.br(divs);
    this.addLabel(divs, mxResources.get('left'), 84);
    this.addLabel(divs, mxResources.get('top'), 20);
    container.appendChild(divs);
    this.addKeyHandler(xs, listener);
    this.addKeyHandler(ys, listener);

    var divt = this.createPanel();
    divt.style.paddingBottom = '30px';

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.width = '70px';
    span.style.marginTop = '0px';
    span.style.fontWeight = 'bold';
    mxUtils.write(span, 'End');
    divt.appendChild(span);

    var xt = this.addUnitInput(divt, 'pt', 84, 44, function () {
        xtUpdate.apply(this, arguments);
    });
    var yt = this.addUnitInput(divt, 'pt', 20, 44, function () {
        ytUpdate.apply(this, arguments);
    });

    mxUtils.br(divt);
    this.addLabel(divt, mxResources.get('left'), 84);
    this.addLabel(divt, mxResources.get('top'), 20);
    container.appendChild(divt);
    this.addKeyHandler(xt, listener);
    this.addKeyHandler(yt, listener);

    var listener = mxUtils.bind(this, function (sender, evt, force) {
        rect = this.format.getSelectionState();
        var cell = graph.getSelectionCell();

        if (rect.style.shape == 'link' || rect.style.shape == 'flexArrow') {
            div.style.display = '';

            if (force || document.activeElement != width) {
                var value = mxUtils.getValue(rect.style, 'width',
                    mxCellRenderer.defaultShapes['flexArrow'].prototype.defaultWidth);
                width.value = value + ' pt';
            }
        }
        else {
            div.style.display = 'none';
        }

        if (graph.getSelectionCount() == 1 && graph.model.isEdge(cell)) {
            var geo = graph.model.getGeometry(cell);

            if (geo.sourcePoint != null && graph.model.getTerminal(cell, true) == null) {
                xs.value = geo.sourcePoint.x;
                ys.value = geo.sourcePoint.y;
            }
            else {
                divs.style.display = 'none';
            }

            if (geo.targetPoint != null && graph.model.getTerminal(cell, false) == null) {
                xt.value = geo.targetPoint.x;
                yt.value = geo.targetPoint.y;
            }
            else {
                divt.style.display = 'none';
            }
        }
        else {
            divs.style.display = 'none';
            divt.style.display = 'none';
        }
    });

    xsUpdate = this.addEdgeGeometryHandler(xs, function (geo, value) {
        geo.sourcePoint.x = value;
    });

    ysUpdate = this.addEdgeGeometryHandler(ys, function (geo, value) {
        geo.sourcePoint.y = value;
    });

    xtUpdate = this.addEdgeGeometryHandler(xt, function (geo, value) {
        geo.targetPoint.x = value;
    });

    ytUpdate = this.addEdgeGeometryHandler(yt, function (geo, value) {
        geo.targetPoint.y = value;
    });

    graph.getModel().addListener(mxEvent.CHANGE, listener);
    this.listeners.push({ destroy: function () { graph.getModel().removeListener(listener); } });
    listener();
};

/**
 * Adds the label menu items to the given menu and parent.
 */
TransferFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(TransferFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
TransferFormatPanel.prototype.init = function () {
    this.addTransfer(this.container);
};

/**
 * Adds the label menu items to the given menu and parent.
 */
TransferFormatPanel.prototype.addTransfer = function (container) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var div = this.createPanel();
    this.createLabel(div, '流转描述');
    var cell = graph.getSelectionCell();
    //条件
    var condition = cell.getAttribute('condition') || '';
    var conditionDesc = cell.getAttribute('conditionDesc') || '';
    var label = cell.getAttribute('label');
    this.createTextbox(div, label, label, function (value) {
        cell.setAttribute('label', value);
        graph.getModel().beginUpdate();
        try {
            var edit = new mxCellAttributeChange(
                cell, 'label',
                value);
            graph.getModel().execute(edit);
            graph.updateCellSize(cell);
        }
        finally {
            graph.getModel().endUpdate();
        }
    });
    this.container.appendChild(div);
    var conditionPanel = this.createPanel();
    var appPanel = document.createElement("div");
    appPanel.style.marginLeft = '0px';
    appPanel.style.paddingTop = '4px';
    appPanel.style.paddingBottom = '4px';
    appPanel.style.fontWeight = 'normal';
    var btn = mxUtils.button('流转条件设置', function (evt) {
        var billTable = graph.billTable || '';
        if (billTable === '') {
            mxUtils.alert('请在流程图那里设置业务单据');
            return;
        }
        //事件
        top.layer.open({
            type: 2,
            title: '流转条件设置',
            shadeClose: true,
            shade: 0.8,
            area: ['800px', '600px'],
            content: TRANSITION_URL + "?billTable=" + encodeURIComponent(billTable), //iframe的url，
            btn: ['确定', '关闭'],
            success: function (layero, index) {
                var iframeWin =parent.window[layero.find('iframe')[0]['name']];
                iframeWin.initEditor();
                iframeWin.setEditorValue(conditionDesc);
            },
            yes: function (index, layero) {
                var iframeWin = parent.window[layero.find('iframe')[0]['name']];
                var transition = iframeWin.getEditorValue();
                if (transition) {
                    while (tags.hasChildNodes()) //当elem下还存在子节点时 循环继续  
                    {
                        tags.removeChild(tags.firstChild);
                    }
                    createTransitionTags(tags, transition, function (values) {
                        //删除条件
                        cell.setAttribute('condition', '');
                    });
                    cell.setAttribute('condition', transition);
                    cell.setAttribute('conditionDesc', transition);
                    //label
                    graph.getModel().beginUpdate();
                    try {
                        var edit = new mxCellAttributeChange(
                            cell, 'label',
                            transition);
                        graph.getModel().execute(edit);
                        graph.updateCellSize(cell);
                    }
                    finally {
                        graph.getModel().endUpdate();
                    }
                }
                top.layer.close(index);
            }
        });
    });
    btn.setAttribute('title', '设置流转条件');
    btn.style.width = '205px';
    btn.style.marginRight = '2px';
    appPanel.appendChild(btn);
    conditionPanel.appendChild(appPanel);


    //流转条件
    var appPanel2 = document.createElement("div");
    appPanel2.style.display = 'inline-block !important';
    var tags = document.createElement("div");
    tags.style.display = 'block';
    tags.style.width = '180px';
    tags.style.padding = '4px 6px';
    tags.style.color = '#777777';
    tags.style.verticalAlign = 'middle';
    tags.style.backgroundColor = '#FFF';
    tags.style.border = '1px solid #d5d5d5';
    appPanel2.appendChild(tags);
    createTransitionTags(tags, conditionDesc, function (values) {
        cell.setAttribute('condition', '');
    });
    conditionPanel.appendChild(appPanel2);

    container.appendChild(conditionPanel);


    var priorityPanel = this.createPanel();// conditionPanel.cloneNode(false);
    var pdiv = document.createElement("div");
    pdiv.style.fontWeight = 'normal';
    pdiv.style.position = 'relative';
    pdiv.style.paddingLeft = '16px'
    pdiv.style.marginBottom = '2px';
    pdiv.style.marginTop = '6px';
    pdiv.style.borderWidth = '0px';
    pdiv.style.paddingBottom = '18px';

    priorityPanel.appendChild(pdiv);

    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.marginLeft = '3px';
    span.style.marginBottom = '12px';
    span.style.marginTop = '1px';
    span.style.fontWeight = 'normal';
    span.style.width = '120px';
    mxUtils.write(span, '优先级');
    pdiv.appendChild(span);

    var priority = cell.getAttribute('priority') || 1;
    var priorityUpdate;
    var prioritySpacing = this.addUnitInput(pdiv, '', 20, 41, function () {
        priorityUpdate.apply(this, arguments);
    });
    function priorityUpdate(evt) {
        // Maximum stroke width is 10
        var value = parseInt(prioritySpacing.value);
        value = Math.min(10, Math.max(1, (isNaN(value)) ? 1 : value));
        prioritySpacing.value = value;
        cell.setAttribute('priority', value);
    };

    mxEvent.addListener(prioritySpacing, 'blur', priorityUpdate);
    mxEvent.addListener(prioritySpacing, 'change', priorityUpdate);
    prioritySpacing.value = priority;
    container.appendChild(priorityPanel);
    return container;
};

/**
 * 开始节点面板
 */
StartFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(StartFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
StartFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var nodeName = cell.getAttribute('nodeName') || '流程开始';

    var div = this.createPanel();
    this.createLabel(div, '节点名称');
    var input = this.createTextbox(div, nodeName, nodeName, function (value) {
    });
    input.disabled = true;
    this.container.appendChild(div);
};


/**
 * 结束节点面板
 */
EndFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(EndFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
EndFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var nodeName = cell.getAttribute('nodeName') || '流程结束';
    var div = this.createPanel();
    this.createLabel(div, '节点名称');
    var input = this.createTextbox(div, nodeName, nodeName, function (value) {
        //cell.value.setAttribute('nodeName', value);
    });
    input.disabled = true;
    this.container.appendChild(div);
};
/**
*网关
*/
GatewayFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(GatewayFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
GatewayFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var nodeName = cell.getAttribute('nodeName') || '流程结束';
    var gateway = cell.getAttribute('gateway') || 'split';
    var direction = cell.getAttribute('direction');
    var div = this.createPanel();
    this.createLabel(div, '路由选择');

    var directionSelect = document.createElement('select');
    directionSelect.style.marginTop = '8px';
    directionSelect.style.width = '202px';
    directionSelect.options.length = 0;
    var emptyOption = document.createElement('option');
    emptyOption.setAttribute('value', '');
    mxUtils.write(emptyOption, '---请选择---');
    directionSelect.appendChild(emptyOption);
    if (gateway === '1') {
        if (!direction) {
            direction = '1';
        }
        var option1 = document.createElement('option');
        option1.setAttribute('value', '1');
        if (direction == '1') {
            option1.setAttribute('selected', true);
            cell.setAttribute('direction', '1');
        }
        mxUtils.write(option1, '或分支');
        directionSelect.appendChild(option1);
        var option4 = document.createElement('option');
        option4.setAttribute('value', '4');
        if (direction == '4') {
            option4.setAttribute('selected', true);
            cell.setAttribute('direction', '4');
        }
        mxUtils.write(option4, '并行分支');
        directionSelect.appendChild(option4);
    } else {
        if (!direction) {
            direction = '16';
        }
        var option16 = document.createElement('option');
        option16.setAttribute('value', '16');
        if (direction == '16') {
            option16.setAttribute('selected', true);
            cell.setAttribute('direction', '16');
        }
        mxUtils.write(option16, '或合并');
        directionSelect.appendChild(option16);
        var option64 = document.createElement('option');
        option64.setAttribute('value', '64');
        if (direction == '64') {
            option64.setAttribute('selected', true);
            cell.setAttribute('direction', '64');
        }
        mxUtils.write(option64, '并行合并');
        directionSelect.appendChild(option64);
    }
    mxEvent.addListener(directionSelect, 'change', function (evt) {
        var v = directionSelect.value;
        cell.setAttribute("direction", v)
    });


    div.appendChild(directionSelect);
    this.container.appendChild(div);
};
/**
 * 子流程面板
 */
SubProcessFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(SubProcessFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
SubProcessFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var subProcess = cell.getAttribute('subProcess') || '';
    var div = this.createPanel();
    this.createLabel(div, '子流程');

    var wfTemplateSelect = document.createElement('select');
    wfTemplateSelect.style.marginTop = '8px';
    wfTemplateSelect.style.width = '202px';
    wfTemplateSelect.options.length = 0;
    var emptyOption = document.createElement('option');
    emptyOption.setAttribute('value', '');
    mxUtils.write(emptyOption, '---请选择---');
    wfTemplateSelect.appendChild(emptyOption);
    $.get(window.SUBPROCESS_URL, function (data) {
        $.each(data, function (index, value) {
            var option = document.createElement('option');
            option.setAttribute('value', value.Fid);
            if (subProcess === value.Fid) {
                option.setAttribute('selected', true);
                cell.setAttribute('subProcess', subProcess);
            }
            mxUtils.write(option, value.ProcessName);
            wfTemplateSelect.appendChild(option);
        });

    });
    mxEvent.addListener(wfTemplateSelect, 'change', function (evt) {
        var v = wfTemplateSelect.value;
        cell.setAttribute("subProcess", v);

    });
    div.appendChild(wfTemplateSelect);
    this.container.appendChild(div);
};
/**
*泳道名称
*/
SwimlaneFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(SwimlaneFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
SwimlaneFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var swimlaneName = cell.getAttribute('label') || '泳道';

    var div = this.createPanel();
    this.createLabel(div, '泳道名称');
    var input = this.createTextbox(div, swimlaneName, swimlaneName, function (value) {
        cell.setAttribute('label', value);
        graph.getModel().beginUpdate();
        try {
            var edit = new mxCellAttributeChange(
                cell, 'label',
                value);
            graph.getModel().execute(edit);
        }
        finally {
            graph.getModel().endUpdate();
        }
    });
    this.container.appendChild(div);
};
/**
 * 普通任务面板
 */
TaskFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(TaskFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
TaskFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var label = cell.getAttribute('label') || '普通任务';
    var div = this.createPanel();
    this.createLabel(div, '节点名称');
    this.createTextbox(div, label, label, function (value) {
        cell.setAttribute('label', value);
        graph.getModel().beginUpdate();
        try {
            var edit = new mxCellAttributeChange(
                cell, 'label',
                value);
            graph.getModel().execute(edit);
        }
        finally {
            graph.getModel().endUpdate();
        }
    });
    this.createLabel(div, '描述');
    var nodeDesc = cell.getAttribute('nodeDesc') || '';
    this.createTextarea(div, nodeDesc, nodeDesc, function (value) {
        cell.setAttribute('nodeDesc', value);
    });
    this.container.appendChild(div);
    this.container.appendChild(this.addParamOption(this.createPanel()));
    this.container.appendChild(this.addApplierOption(this.createPanel()));
    this.container.appendChild(this.addMessageOption(this.createPanel()));
    //消息方式
    this.container.appendChild(this.addMessageMethodOption(this.createPanel()));
};
TaskFormatPanel.prototype.addParamOption = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var isAssign = cell.getAttribute('isAssign') || 0;
    var approverMethod = cell.getAttribute('approverMethod') || '1';
    div.appendChild(this.createTitle("参数设置"));
    div.appendChild(this.createOption('启用加签', function () {
        return parseInt(isAssign);
    }, function (checked) {
        cell.setAttribute('isAssign', checked);
        }));
    //approverMethod
    //单据模板
    var approverMethodSelect = document.createElement('select');
    approverMethodSelect.style.marginTop = '8px';
    approverMethodSelect.style.marginBottom = '8px';
    approverMethodSelect.style.width = '202px';
    approverMethodSelect.options.length = 0;
    var methods = [{ v: '0', n: '顺序审批' }, {v:'1',n:'任意人审批'}]
    $.each(methods, function (index, value) {
        var option = document.createElement('option');
        option.setAttribute('value', value.v);
        if (approverMethod == value.v) {
            option.setAttribute('selected', true);
            cell.setAttribute('approverMethod', approverMethod);
        }
        mxUtils.write(option, value.n);
        approverMethodSelect.appendChild(option);
    });
    mxEvent.addListener(approverMethodSelect, 'change', function (evt) {
        var v = approverMethodSelect.value;
        cell.setAttribute("approverMethod", v);

    });
    this.createLabel(div, '审批方式');
    //div.appendChild(this.createTitle("审批方式"));
    div.appendChild(approverMethodSelect);
    return div;
}



/**
 * 会签任务面板
 */
CounterTaskFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(CounterTaskFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
CounterTaskFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var label = cell.getAttribute('label') || '会签任务';
    var div = this.createPanel();
    div.appendChild(this.createTitle("基础设置"));
    this.createLabel(div, '节点名称');
    this.createTextbox(div, label, label, function (value) {
        cell.setAttribute('label', value);
        graph.getModel().beginUpdate();
        try {
            var edit = new mxCellAttributeChange(
                cell, 'label',
                value);
            graph.getModel().execute(edit);
        }
        finally {
            graph.getModel().endUpdate();
        }
    });
    this.createLabel(div, '描述');
    var nodeDesc = cell.getAttribute('nodeDesc') || '';
    this.createTextarea(div, nodeDesc, '', function (value) {
        cell.setAttribute('nodeDesc', value);
    });
    this.container.appendChild(div);
    var passRate = cell.getAttribute('passRate') || 100;
    this.container.appendChild(this.createRelativeOption('通过率', 44, function (input) {
        var value = parseInt(input.value);
        value = Math.min(100, Math.max(0, (isNaN(value)) ? 100 : value));
        input.value = ((value != null) ? value : '100') + ' %';
        cell.setAttribute('passRate', value);
    }, function (input) {
        input.value = passRate + '%';
    }));
    //申请人设置
    this.container.appendChild(this.addApplierOption(this.createPanel()));
    //消息
    this.container.appendChild(this.addMessageOption(this.createPanel()));
    //消息方式
    this.container.appendChild(this.addMessageMethodOption(this.createPanel()));
};

//审批人设置
BaseFormatPanel.prototype.addApplierOption = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var doc = mxUtils.createXmlDocument();
    var cell = graph.getSelectionCell();
    //获取所有审批人
    var participants = cell.value.getElementsByTagName('participants')[0];
    var childs = participants.childNodes;
    //用户，角色，动态角色,同其他节点，
    var users, sameOther;
    var usersNode = [], sameOtherNode;
    function getParticipants() {
        users = [];
        if (childs) {
            for (var i = 0; i < childs.length; i++) {
                let participant = childs[i];
                if (participant.nodeType == 1) {//nodetype 1为子节点
                    let ptype = participant.getAttribute('pType');
                    let pvalue = participant.getAttribute('pValue');
                    let pname = participant.getAttribute('pName');
                    let pcolumn = participant.getAttribute('pColumn');
                    if (ptype == '1') {
                        //用户
                        users.push({ 'id': pvalue, 'name': pname, type: '1', colname: pcolumn });
                        usersNode.push(participant);
                    } else if (ptype == '2') {
                        users.push({ 'id': pvalue, 'name': pname, type: '2', colname: pcolumn });
                        usersNode.push(participant);
                    } else if (ptype == '3') {
                        users.push({ 'id': pvalue, 'name': pname, type: '3', colname: pcolumn });
                        usersNode.push(participant);
                    } else if (ptype == '4') {
                        //同其他节点
                        sameOther = pvalue;
                        sameOtherNode = participant;
                    }
                }
            }
        }
        return users;
    }
    getParticipants();
    var isAppoint = cell.getAttribute('isAppoint') || 0;
    div.appendChild(this.createTitle("审批人设置"));
    div.appendChild(this.createOption('是否由上级手工指定', function () {
        return parseInt(isAppoint);
    }, function (checked) {
        cell.setAttribute('isAppoint', checked);
    }));
    var appPanel = document.createElement("div");
    appPanel.style.marginLeft = '0px';
    appPanel.style.paddingTop = '4px';
    appPanel.style.paddingBottom = '4px';
    appPanel.style.fontWeight = 'normal';

    var btn = mxUtils.button('设置审批人', function (evt) {
        var billTable = graph.billTable || '';
        if (billTable === '') {
            mxUtils.alert('请在流程图那里设置业务单据');
            return;
        }
        //事件
        layer.open({
            type: 2,
            title: '审批人选择',
            shadeClose: true,
            shade: 0.8,
            area: ['800px', '600px'],
            content: APPROVER_URL + "?billTable=" + encodeURIComponent(billTable), //iframe的url，
            btn: ['确定', '关闭'],
            success: function (layero, index) {
                var iframeWin = window[layero.find('iframe')[0]['name']];
                var approvers = getParticipants();
                if (approvers && approvers.length > 0) {
                    iframeWin.addApprovers(approvers);
                }
            },
            yes: function (index, layero) {
                var iframeWin = window[layero.find('iframe')[0]['name']];
                var approvers = iframeWin.getApprover();
                while (tags.hasChildNodes()) {
                    tags.removeChild(tags.firstChild);
                }
                createParticipantTags(tags, approvers, function (values) {
                    participantsRefresh(participants, values);
                });
                participantsRefresh(participants, approvers);

                layer.close(index);
            }
        });
    });

    btn.setAttribute('title', '设置审批人');
    btn.style.width = '195px';
    btn.style.marginRight = '2px';
    appPanel.appendChild(btn);

    div.appendChild(appPanel);
    //审批人列表
    var appPanel2 = document.createElement("div");
    appPanel2.style.display = 'inline-block !important';
    var tags = document.createElement("div");
    tags.style.display = 'block';
    tags.style.width = '180px';
    tags.style.padding = '4px 6px';
    tags.style.color = '#777777';
    tags.style.verticalAlign = 'middle';
    tags.style.backgroundColor = '#FFF';
    tags.style.border = '1px solid #d5d5d5';
    appPanel2.appendChild(tags);
    createParticipantTags(tags, users, function (values) {
        participantsRefresh(participants, values);
    });

    div.appendChild(appPanel2);
    //同其他节点审批人
    this.createLabel(div, '同其他节点审批人');
    var appPanel1 = appPanel.cloneNode(false);

    this.createSelect(appPanel1, graph, sameOther, function (v) {
        if (v != '-1') {
            if (sameOtherNode) {
                sameOtherNode.setAttribute('pValue', v);
            } else {
                sameOtherNode = doc.createElement('participant');
                participants.appendChild(sameOtherNode);
                sameOtherNode.setAttribute('pValue', v);
                sameOtherNode.setAttribute('pName', '同其它节点');
                sameOtherNode.setAttribute('pType', '4');
                sameOtherNode.setAttribute('pColumn', '');
            }
        }
    });
    div.appendChild(appPanel1);
    return div;
}
//消息设置
BaseFormatPanel.prototype.addMessageOption = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var noticeApprover = cell.getAttribute('noticeApprover') || 1;
    div.appendChild(this.createTitle("消息设置"));
    div.appendChild(this.createOption('通知处理人', function () {
        return parseInt(noticeApprover);
    }, function (checked) {
        cell.setAttribute('noticeApprover', checked);
    }));
    var noticeApplicant = cell.getAttribute('noticeApplicant') || 0;
    div.appendChild(this.createOption('通知申请人', function () {
        return parseInt(noticeApplicant);
    }, function (checked) {
        cell.setAttribute('noticeApplicant', checked);
    }));
    return div;
}
//消息发送方式
BaseFormatPanel.prototype.addMessageMethodOption = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var cell = graph.getSelectionCell();
    var isMail = cell.getAttribute('isMail') || 1;
    div.appendChild(this.createTitle("通知方式"));
    div.appendChild(this.createOption('邮件', function () {
        return parseInt(isMail);
    }, function (checked) {
        cell.setAttribute('isMail', checked);
    }));
    var isMessage = cell.getAttribute('isMessage') || 1;
    // Connection points
    div.appendChild(this.createOption('站内信', function () {
        return parseInt(isMessage);
    }, function (checked) {
        cell.setAttribute('isMessage', checked);
    }));
    return div;
}

TimerTaskFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(TimerTaskFormatPanel, BaseFormatPanel);

/**
 * Adds the label menu items to the given menu and parent.
 */
TimerTaskFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var div = this.createPanel();
    this.createLabel(div, '节点名称');
    var cell = graph.getSelectionCell();
    var label = cell.getAttribute('label') || '';
    this.createTextbox(div, label, label, function (value) {
        cell.setAttribute('label', value);
        graph.getModel().beginUpdate();
        try {
            var edit = new mxCellAttributeChange(
                cell, 'label',
                value);
            graph.getModel().execute(edit);
        }
        finally {
            graph.getModel().endUpdate();
        }
    });
    this.createLabel(div, '描述');
    var nodeDesc = cell.getAttribute('nodeDesc') || '';
    this.createTextarea(div, nodeDesc, nodeDesc, function (value) {
        cell.setAttribute('nodeDesc', value);
    });
    this.container.appendChild(div);
    this.container.appendChild(this.addParamOption(this.createPanel()));
    this.container.appendChild(this.addApplierOption(this.createPanel()));
    this.container.appendChild(this.addMessageOption(this.createPanel()));
    //消息方式
    this.container.appendChild(this.addMessageMethodOption(this.createPanel()));
};
TimerTaskFormatPanel.prototype.addParamOption = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    var pdiv = document.createElement("div");
    pdiv.style.fontWeight = 'normal';
    pdiv.style.position = 'relative';
    pdiv.style.paddingLeft = '16px'
    pdiv.style.marginBottom = '2px';
    pdiv.style.marginTop = '6px';
    pdiv.style.borderWidth = '0px';
    pdiv.style.paddingBottom = '18px';


    var span = document.createElement('div');
    span.style.position = 'absolute';
    span.style.marginLeft = '3px';
    span.style.marginBottom = '12px';
    span.style.marginTop = '1px';
    span.style.fontWeight = 'normal';
    span.style.width = '120px';
    mxUtils.write(span, '超期时间(小时)');
    pdiv.appendChild(span);
    var cell = graph.getSelectionCell();
    var expiration = cell.getAttribute('expiration');
    var timerSpacing = this.addUnitInput(pdiv, 'h', 20, 41, function () {
        expirationUpdate.apply(this, arguments);
    });
    function expirationUpdate(evt) {
        // Maximum stroke width is 999
        var value = parseInt(timerSpacing.value);
        value = Math.min(999, Math.max(1, (isNaN(value)) ? 1 : value));

        timerSpacing.value = value + ' h';
        cell.setAttribute('expiration', value);
        mxEvent.consume(evt);
    };

    mxEvent.addListener(timerSpacing, 'blur', expirationUpdate);
    mxEvent.addListener(timerSpacing, 'change', expirationUpdate);
    timerSpacing.value = expiration + 'h';
    div.appendChild(pdiv);
    return div;
}



StyleFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(StyleFormatPanel, BaseFormatPanel);

/**
 * Switch to disable page view.
 */
StyleFormatPanel.showPageView = true;

/**
 * Specifies if the background image option should be shown. Default is true.
 */
StyleFormatPanel.prototype.showBackgroundImageOption = true;

/**
 * Adds the label menu items to the given menu and parent.
 */
StyleFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    this.container.appendChild(this.addView(this.createPanel()));

    if (graph.isEnabled()) {
        this.container.appendChild(this.addPaperSize(this.createPanel()));
    }
};





TaskBillFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(TaskBillFormatPanel, BaseFormatPanel);

/**
 * Switch to disable page view.
 */
TaskBillFormatPanel.showPageView = true;

/**
 * Specifies if the background image option should be shown. Default is true.
 */
TaskBillFormatPanel.prototype.showBackgroundImageOption = true;

/**
 * Adds the label menu items to the given menu and parent.
 */
TaskBillFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    this.container.appendChild(this.addBillTemplate(this.createPanel()));
    this.container.appendChild(this.addBillForm(this.createPanel()));
};
/**
*单据模板
*/
TaskBillFormatPanel.prototype.addBillTemplate = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    if (graph.frmType == 'FreeForm') {
        div.style.display = 'inline-block';
    } else {
        div.style.display = 'none';
    }
    var cell = graph.getSelectionCell();
    var billTemplate = cell.getAttribute("billTemplate") || graph.billTemplate;
    //单据模板
    var billTemplateSelect = document.createElement('select');
    billTemplateSelect.style.marginBottom = '8px';
    billTemplateSelect.style.width = '202px';


    billTemplateSelect.options.length = 0;
    $.each(billTemplates, function (index, value) {
        var option = document.createElement('option');
        option.setAttribute('value', value.Fid);
        if (billTemplate == value.Fid) {
            option.setAttribute('selected', true);
            cell.setAttribute('billTemplate', billTemplate);
        }
        mxUtils.write(option, value.FFName);
        billTemplateSelect.appendChild(option);
    });
    mxEvent.addListener(billTemplateSelect, 'change', function (evt) {
        var v = billTemplateSelect.value;
        cell.setAttribute("billTemplate", v);

    });
    div.appendChild(this.createTitle("单据模板"));
    div.appendChild(billTemplateSelect);
    return div;
}
/**
*单据设置
*/
TaskBillFormatPanel.prototype.addBillForm = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var doc = mxUtils.createXmlDocument();
    //div.appendChild(this.createTitle("权限设置"));
    //获取权限设置
    var cell = graph.getSelectionCell();
    var fields = cell.value.getElementsByTagName('fields')[0];
    var childs;
    if (fields) {
        childs = fields.childNodes;
    }
    function findFieldNode(childNodes, attrValue) {
        for (var i = 0; i < childNodes.length; i++) {
           //nodetype 1为子节点
            if (childNodes[i].nodeType == 1&&childNodes[i].getAttribute('name') == attrValue) {
                return childNodes[i];
            }
        }
        return null;
    }
    div.appendChild(this.createTitle("字段权限"));

    var table = document.createElement('table');
    table.setAttribute('width', '100%');
    table.setAttribute('height', '100%');

    var tbody = document.createElement('tbody');
    if (columns && columns.length > 0) {
        $.each(columns, function (index, col) {
            let tr0 = document.createElement('tr');
            let td0 = document.createElement('td');
            td0.style.verticalAlign = 'middle';
            //var lbl = document.createElement('span');
            //lbl.style.fontWeight = 'bold';
            let colName = col.name;
            let colValue = col.key;
            mxUtils.write(td0, colName);
            //td0.appendChild(lbl);
            tr0.appendChild(td0);
            tbody.appendChild(tr0);

            let tr1 = document.createElement('tr');
            let td1 = document.createElement('td');
            //----------
            let formatName = 'format-billform' + index;

            let hideCheckBox = document.createElement('input');
            hideCheckBox.setAttribute('name', formatName);
            hideCheckBox.setAttribute('type', 'radio');
            hideCheckBox.setAttribute('value', '0');

            let editCheckBox = document.createElement('input');
            editCheckBox.setAttribute('name', formatName);
            editCheckBox.setAttribute('type', 'radio');
            editCheckBox.setAttribute('value', '1');

            let readonlyCheckBox = document.createElement('input');
            readonlyCheckBox.setAttribute('name', formatName);
            readonlyCheckBox.setAttribute('type', 'radio');
            //根据配置设置默认选中值
            readonlyCheckBox.setAttribute('value', '2');
            if (childs && childs.length > 0) {
                var field = findFieldNode(childs, colValue);
                if (field != null) {
                    var property = field.getAttribute('property');
                    if (property === '0') {
                        hideCheckBox.checked = true;
                    } else if (property === '1') {
                        editCheckBox.checked = true;
                    } else {
                        readonlyCheckBox.checked = true;
                    }
                } else {
                    readonlyCheckBox.checked = true;
                }
            } else {
                readonlyCheckBox.checked = true;
            }
            let formatDiv = document.createElement('div');
            formatDiv.style.marginLeft = '4px';
            formatDiv.style.width = '210px';
            formatDiv.style.height = '24px';

            hideCheckBox.style.marginRight = '6px';
            formatDiv.appendChild(hideCheckBox);

            let hideSpan = document.createElement('span');
            hideSpan.style.maxWidth = '100px';
            mxUtils.write(hideSpan, '隐藏');
            formatDiv.appendChild(hideSpan);

            editCheckBox.style.marginLeft = '10px';
            editCheckBox.style.marginRight = '6px';
            formatDiv.appendChild(editCheckBox);

            let editSpan = document.createElement('span');
            editSpan.style.width = '100px';
            mxUtils.write(editSpan, '编辑');
            formatDiv.appendChild(editSpan)

            readonlyCheckBox.style.marginLeft = '10px';
            readonlyCheckBox.style.marginRight = '6px';
            formatDiv.appendChild(readonlyCheckBox);

            let readSpan = document.createElement('span');
            readSpan.style.width = '100px';
            mxUtils.write(readSpan, '只读');
            formatDiv.appendChild(readSpan)
            mxEvent.addListener(hideSpan, 'click', function (evt) {
                hideCheckBox.checked = true;
                update();
            });
            mxEvent.addListener(editSpan, 'click', function (evt) {
                editCheckBox.checked = true;
                update();
            });
            mxEvent.addListener(readSpan, 'click', function (evt) {
                readonlyCheckBox.checked = true;
                update();
            });

            function update() {
                var property = '2';
                if (hideCheckBox.checked == true) {
                    property = '0';
                } else if (editCheckBox.checked == true) {
                    property = '1';
                } else {
                    property = '2';
                }
                if (childs && childs.length > 0) {
                    var field = findFieldNode(childs, colValue);
                    if (field != null) {
                        if (property === '2') {
                            fields.removeChild(field);
                        } else {
                            field.setAttribute('property', property);
                        }
                    } else if (property !== '2') {
                        var field = doc.createElement('field');
                        field.setAttribute('name', colValue);
                        field.setAttribute('property', property);
                        fields.appendChild(field);
                    }
                } else if (property !== '2') {
                    var field = doc.createElement('field');
                    field.setAttribute('name', colValue);
                    field.setAttribute('property', property);
                    fields.appendChild(field);
                }
            }

            mxEvent.addListener(hideCheckBox, 'change', update);
            mxEvent.addListener(editCheckBox, 'change', update);
            mxEvent.addListener(readonlyCheckBox, 'change', update);
            td1.appendChild(formatDiv);
            tr1.appendChild(td1);
            tbody.appendChild(tr1);
        });
    } else {
        let tr0 = document.createElement('tr');
        let td0 = document.createElement('td');
        td0.style.verticalAlign = 'middle';

        mxUtils.write(td0, '请在流程图那里设置业务单据！');

        tr0.appendChild(td0);
        tbody.appendChild(tr0);

    }
    table.appendChild(tbody);
    div.appendChild(table);

    return div;
};
/**
 * Adds the label menu items to the given menu and parent.
 */
DiagramFormatPanel = function (format, editorUi, container) {
    BaseFormatPanel.call(this, format, editorUi, container);
    this.init();
};

mxUtils.extend(DiagramFormatPanel, BaseFormatPanel);

/**
 * Switch to disable page view.
 */
DiagramFormatPanel.showPageView = true;

/**
 * Specifies if the background image option should be shown. Default is true.
 */
DiagramFormatPanel.prototype.showBackgroundImageOption = true;

/**
 * Adds the label menu items to the given menu and parent.
 */
DiagramFormatPanel.prototype.init = function () {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    //this.container.appendChild(this.addView(this.createPanel()));

    if (graph.isEnabled()) {
        //this.container.appendChild(this.addOptions(this.createPanel()));
        //this.container.appendChild(this.addPaperSize(this.createPanel()));
        //this.container.appendChild(this.addStyleOps(this.createPanel()));
        //流程基础设置
        this.container.appendChild(this.addDiagram(this.createPanel()));
        //业务单据
        this.container.appendChild(this.addBillForm(this.createPanel()));
        //消息设置
        this.container.appendChild(this.addDiagramMessage(this.createPanel()));
        //消息方式
        this.container.appendChild(this.addDiagramMessageMethod(this.createPanel()));
    }
};
/**
 * 添加流程图设置项wyf
 */
DiagramFormatPanel.prototype.addDiagram = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    div.appendChild(this.createTitle("基础设置"));
    this.addDiagramOption(div);
    return div;
}
DiagramFormatPanel.prototype.addDiagramMessage = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    div.appendChild(this.createTitle("消息设置"));
    this.addDiagramMessageOption(div);
    return div;
}
DiagramFormatPanel.prototype.addDiagramMessageMethod = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    div.appendChild(this.createTitle("通知方式"));
    this.addDiagramMessageMethodOption(div);
    return div
}
/**
*单据设置
*/
DiagramFormatPanel.prototype.addBillForm = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;
    var frmType = graph.frmType || 'Internal';
    var billTable = graph.billTable || '';
    var billTemplate = graph.billTemplate || '';
    div.appendChild(this.createTitle('业务单据'));

    var formatName = 'format-billform';

    var internalCheckBox = document.createElement('input');
    internalCheckBox.setAttribute('name', formatName);
    internalCheckBox.setAttribute('type', 'radio');
    internalCheckBox.setAttribute('value', 'Internal');

    var freeformCheckBox = document.createElement('input');
    freeformCheckBox.setAttribute('name', formatName);
    freeformCheckBox.setAttribute('type', 'radio');
    freeformCheckBox.setAttribute('value', 'FreeForm');
    //单据模板
    var billTemplateSelect = document.createElement('select');
    billTemplateSelect.style.marginLeft = '8px';
    billTemplateSelect.style.width = '142px';

    var templateDiv = document.createElement('div');
    templateDiv.style.marginLeft = '4px';
    templateDiv.style.width = '210px';
    templateDiv.style.height = '24px';
    var tmplabel = document.createElement('span');
    tmplabel.style.maxWidth = '100px';
    mxUtils.write(tmplabel, '单据模板');
    templateDiv.appendChild(tmplabel);
    templateDiv.appendChild(billTemplateSelect);
    div.appendChild(templateDiv);

    if (frmType == 'Internal') {
        internalCheckBox.checked = true;
        graph.frmType = 'Internal';
        templateDiv.style.display = 'none';
    } else {
        freeformCheckBox.checked = true;
        graph.frmType = 'FreeForm';
        templateDiv.style.display = 'inline';
    }
    //单据
    var billSelect = document.createElement('select');
    billSelect.style.marginBottom = '8px';
    billSelect.style.width = '202px';


    var formatDiv = document.createElement('div');
    formatDiv.style.marginLeft = '4px';
    formatDiv.style.width = '210px';
    formatDiv.style.height = '24px';

    internalCheckBox.style.marginRight = '6px';
    formatDiv.appendChild(internalCheckBox);

    var internalSpan = document.createElement('span');
    internalSpan.style.maxWidth = '100px';
    mxUtils.write(internalSpan, '系统表单');
    formatDiv.appendChild(internalSpan);

    freeformCheckBox.style.marginLeft = '10px';
    freeformCheckBox.style.marginRight = '6px';
    formatDiv.appendChild(freeformCheckBox);

    var freeformSpan = document.createElement('span');
    freeformSpan.style.width = '100px';
    mxUtils.write(freeformSpan, '自由表单');
    formatDiv.appendChild(freeformSpan)


    var tables = billTables;//主页，单据表
    let empOption = document.createElement('option');
    empOption.setAttribute('value', '');
    mxUtils.write(empOption, '--请选择--');
    billSelect.appendChild(empOption);
    for (var i = 0; i < tables.length; i++) {
        var f = tables[i];
        var tableOption = document.createElement('option');
        tableOption.setAttribute('value', f.key);
        if (billTable == f.key) {
            tableOption.setAttribute('selected', true);
        }
        mxUtils.write(tableOption, f.name);
        billSelect.appendChild(tableOption);
    }

    var customSize = false;

    div.appendChild(billSelect);
    mxUtils.br(div);

    div.appendChild(formatDiv);

    //mxUtils.br(div);

    div.appendChild(templateDiv);

    var update = function (evt, selectChanged) {
        var oriTable = graph.billTable;
        var newTabe = billSelect.value;
        graph.billTable = newTabe;
        //获取字段
        $.get(window.FIELDLIST_URL + '/' + graph.billTable, function (data) {
            columns = [];
            $.each(data, function (index, value) {
                //系统默认字段和单据字段不显示
                if ((value.isDefaultCol == 1 || value.colProperty == 3) && value.colName !=='EffectiveTime')
                    return true;
                //仅仅添加业务字段,property 0隐藏，1 可编辑，2只读
                columns.push({ key: value.colName, name: value.colComment, property: 2 });
            });
        })
        //判断table是否更换，如果更换清空所有的单据字段权限
        if (oriTable != '' && oriTable != newTabe) {
            var cells = graph.getModel().cells;
            for (var i in cells) {
                let c = cells[i];
                let nodeType = c.getAttribute('nodeType');
                if (nodeType && (nodeType == '1' || nodeType == '2' || nodeType == '3')) {
                    var fields = c.value.getElementsByTagName('fields')[0];
                    if (fields) {
                        while (fields.hasChildNodes()) {
                            fields.removeChild(fields.firstChild);
                        }
                    }
                }
            }
        }
        //更新单据模板
        $.get(window.BILLTEMPLATE_URL, { tableName: graph.billTable }, function (data) {
            billTemplates = data;
            if (data.length > 0 && graph.billTemplate === '') {
                graph.billTemplate = data[0].Fid;
            }
            billTemplateSelect.options.length = 0;
            $.each(data, function (index, value) {
                var option = document.createElement('option');
                option.setAttribute('value', value.Fid);
                if (billTemplate == value.Fid) {
                    option.setAttribute('selected', true);
                }
                mxUtils.write(option, value.FFName);
                billTemplateSelect.appendChild(option);
            });
        })
    };

    mxEvent.addListener(internalSpan, 'click', function (evt) {
        internalCheckBox.checked = true;
        changFrmType();
    });

    mxEvent.addListener(freeformSpan, 'click', function (evt) {
        freeformCheckBox.checked = true;
        changFrmType();

    });
    function updateFormType() {
        if (internalCheckBox.checked == true) {
            graph['frmType'] = 'Internal';
        } else {
            graph['frmType'] = 'FreeForm';
        }
    }
    mxEvent.addListener(internalCheckBox, 'click', function () {
        changFrmType();
    });
    mxEvent.addListener(freeformCheckBox, 'click', function () {
        changFrmType();
    });
    function changFrmType() {
        if (internalCheckBox.checked == true) {
            graph['frmType'] = 'Internal';
            templateDiv.style.display = 'none';
        } else {
            graph['frmType'] = 'FreeForm';
            templateDiv.style.display = 'inline';
        }
    }
    mxEvent.addListener(internalCheckBox, 'change', updateFormType);
    mxEvent.addListener(freeformCheckBox, 'change', updateFormType);
    //选择单据表事件
    mxEvent.addListener(billSelect, 'change', function (evt) {
        update(evt, true);
    });
    mxEvent.addListener(billTemplateSelect, 'change', function (evt) {
        var v = billTemplateSelect.value;
        graph.billTemplate = v;

    });
    if (billTable !== '') {
        update();
    }
    return div;
};
/**
 * Adds the label menu items to the given menu and parent.
 */
StyleFormatPanel.prototype.addView = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    div.appendChild(this.createTitle(mxResources.get('view')));

    // Grid
    this.addGridOption(div);

    if (graph.isEnabled()) {
        // Page View
        if (DiagramFormatPanel.showPageView) {
            div.appendChild(this.createOption(mxResources.get('pageView'), function () {
                return graph.pageVisible;
            }, function (checked) {
                ui.actions.get('pageView').funct();
            },
                {
                    install: function (apply) {
                        this.listener = function () {
                            apply(graph.pageVisible);
                        };

                        ui.addListener('pageViewChanged', this.listener);
                    },
                    destroy: function () {
                        ui.removeListener(this.listener);
                    }
                }));
        }


    }

    return div;
};

/**
 * Adds the label menu items to the given menu and parent.
 */
DiagramFormatPanel.prototype.addOptions = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    div.appendChild(this.createTitle(mxResources.get('options')));

    if (graph.isEnabled()) {
        // Connection arrows
        div.appendChild(this.createOption(mxResources.get('connectionArrows'), function () {
            return graph.connectionArrowsEnabled;
        }, function (checked) {
            ui.actions.get('connectionArrows').funct();
        },
            {
                install: function (apply) {
                    this.listener = function () {
                        apply(graph.connectionArrowsEnabled);
                    };

                    ui.addListener('connectionArrowsChanged', this.listener);
                },
                destroy: function () {
                    ui.removeListener(this.listener);
                }
            }));

        // Connection points
        div.appendChild(this.createOption(mxResources.get('connectionPoints'), function () {
            return graph.connectionHandler.isEnabled();
        }, function (checked) {
            ui.actions.get('connectionPoints').funct();
        },
            {
                install: function (apply) {
                    this.listener = function () {
                        apply(graph.connectionHandler.isEnabled());
                    };

                    ui.addListener('connectionPointsChanged', this.listener);
                },
                destroy: function () {
                    ui.removeListener(this.listener);
                }
            }));

        // Guides
        div.appendChild(this.createOption(mxResources.get('guides'), function () {
            return graph.graphHandler.guidesEnabled;
        }, function (checked) {
            ui.actions.get('guides').funct();
        },
            {
                install: function (apply) {
                    this.listener = function () {
                        apply(graph.graphHandler.guidesEnabled);
                    };

                    ui.addListener('guidesEnabledChanged', this.listener);
                },
                destroy: function () {
                    ui.removeListener(this.listener);
                }
            }));
    }

    return div;
};

/**
 * 添加流程设置选项wyf
 */
DiagramFormatPanel.prototype.addDiagramOption = function (container) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;
    var wfuid = graph.wfDiagramUid || '';
    var wfname = graph.wfDiagramName || '';
    var wfdesc = graph.wfDesc || '';
    this.createLabel(container, '流程UID');
    var txtwfuid = this.createTextbox(container, '流程UID', wfuid, function (value) {
        graph['wfDiagramUid'] = value;
    });
    txtwfuid.disabled = true;
    this.createLabel(container, '流程名称');
    this.createTextbox(container, '流程名称', wfname, function (value) {
        graph['wfDiagramName'] = value;
    });
    this.createLabel(container, '描述');
    this.createTextarea(container, '描述', wfdesc, function (value) {
        graph['wfDesc'] = value;
    });

}
DiagramFormatPanel.prototype.addDiagramMessageOption = function (div) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;

    if (graph.isEnabled()) {
        // Connection arrows
        div.appendChild(this.createOption('结果反馈给申请人', function () {
            return parseInt(graph.wfResultNotice);
        }, function (checked) {
            graph.wfResultNotice = checked;
        }));

        // Connection points
        div.appendChild(this.createOption('流程中断，通知申请人和处理人', function () {
            return parseInt(graph.wfSuspendNotice);
        }, function (checked) {
            graph.wfSuspendNotice = checked;
        }));
    }

}
DiagramFormatPanel.prototype.addDiagramMessageMethodOption = function (div) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;

    if (graph.isEnabled()) {
        div.appendChild(this.createOption('邮件', function () {
            return parseInt(graph.wfMail);
        }, function (checked) {
            graph.wfMail = checked;
        }));

        // Connection points
        div.appendChild(this.createOption('站内信', function () {
            return parseInt(graph.wfMessage);
        }, function (checked) {
            graph.wfMessage = checked;
        }));
    }

}

/**
 * 
 */
StyleFormatPanel.prototype.addGridOption = function (container) {
    var ui = this.editorUi;
    var graph = ui.editor.graph;

    var input = document.createElement('input');
    input.style.position = 'absolute';
    input.style.textAlign = 'right';
    input.style.width = '38px';
    input.value = graph.getGridSize() + ' pt';

    var stepper = this.createStepper(input, update);
    input.style.display = (graph.isGridEnabled()) ? '' : 'none';
    stepper.style.display = input.style.display;

    mxEvent.addListener(input, 'keydown', function (e) {
        if (e.keyCode == 13) {
            graph.container.focus();
            mxEvent.consume(e);
        }
        else if (e.keyCode == 27) {
            input.value = graph.getGridSize();
            graph.container.focus();
            mxEvent.consume(e);
        }
    });

    function update(evt) {
        var value = parseInt(input.value);
        value = Math.max(1, (isNaN(value)) ? 10 : value);

        if (value != graph.getGridSize()) {
            graph.setGridSize(value)
        }

        input.value = value + ' pt';
        mxEvent.consume(evt);
    };

    mxEvent.addListener(input, 'blur', update);
    mxEvent.addListener(input, 'change', update);

    if (mxClient.IS_SVG) {
        input.style.marginTop = '-2px';
        input.style.right = '84px';
        stepper.style.marginTop = '-16px';
        stepper.style.right = '72px';

        var panel = this.createColorOption(mxResources.get('grid'), function () {
            var color = graph.view.gridColor;

            return (graph.isGridEnabled()) ? color : null;
        }, function (color) {
            if (color == mxConstants.NONE) {
                graph.setGridEnabled(false);
                ui.fireEvent(new mxEventObject('gridEnabledChanged'));
            }
            else {
                graph.setGridEnabled(true);
                ui.setGridColor(color);
            }

            input.style.display = (graph.isGridEnabled()) ? '' : 'none';
            stepper.style.display = input.style.display;
        }, '#e0e0e0',
            {
                install: function (apply) {
                    this.listener = function () {
                        apply((graph.isGridEnabled()) ? graph.view.gridColor : null);
                    };

                    ui.addListener('gridColorChanged', this.listener);
                    ui.addListener('gridEnabledChanged', this.listener);
                },
                destroy: function () {
                    ui.removeListener(this.listener);
                }
            });

        panel.appendChild(input);
        panel.appendChild(stepper);
        container.appendChild(panel);
    }
    else {
        input.style.marginTop = '2px';
        input.style.right = '32px';
        stepper.style.marginTop = '2px';
        stepper.style.right = '20px';


        container.appendChild(input);
        container.appendChild(stepper);

        container.appendChild(this.createOption(mxResources.get('grid'), function () {
            return graph.isGridEnabled();
        }, function (checked) {
            graph.setGridEnabled(checked);

            if (graph.isGridEnabled()) {
                graph.view.gridColor = '#e0e0e0';
            }

            ui.fireEvent(new mxEventObject('gridEnabledChanged'));
        },
            {
                install: function (apply) {
                    this.listener = function () {
                        input.style.display = (graph.isGridEnabled()) ? '' : 'none';
                        stepper.style.display = input.style.display;

                        apply(graph.isGridEnabled());
                    };

                    ui.addListener('gridEnabledChanged', this.listener);
                },
                destroy: function () {
                    ui.removeListener(this.listener);
                }
            }));
    }
};

/**
 * Adds the label menu items to the given menu and parent.
 */
DiagramFormatPanel.prototype.addDocumentProperties = function (div) {
    // Hook for subclassers
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    div.appendChild(this.createTitle(mxResources.get('options')));

    return div;
};

/**
 * Adds the label menu items to the given menu and parent.
 */
StyleFormatPanel.prototype.addPaperSize = function (div) {
    var ui = this.editorUi;
    var editor = ui.editor;
    var graph = editor.graph;

    div.appendChild(this.createTitle(mxResources.get('paperSize')));

    var accessor = PageSetupDialog.addPageFormatPanel(div, 'formatpanel', graph.pageFormat, function (pageFormat) {
        if (graph.pageFormat == null || graph.pageFormat.width != pageFormat.width ||
            graph.pageFormat.height != pageFormat.height) {
            var change = new ChangePageSetup(ui, null, null, pageFormat);
            change.ignoreColor = true;
            change.ignoreImage = true;

            graph.model.execute(change);
        }
    });

    this.addKeyHandler(accessor.widthInput, function () {
        accessor.set(graph.pageFormat);
    });
    this.addKeyHandler(accessor.heightInput, function () {
        accessor.set(graph.pageFormat);
    });

    var listener = function () {
        accessor.set(graph.pageFormat);
    };

    ui.addListener('pageFormatChanged', listener);
    this.listeners.push({ destroy: function () { ui.removeListener(listener); } });

    graph.getModel().addListener(mxEvent.CHANGE, listener);
    this.listeners.push({ destroy: function () { graph.getModel().removeListener(listener); } });

    return div;
};

/**
 * Adds the label menu items to the given menu and parent.
 */
DiagramFormatPanel.prototype.addStyleOps = function (div) {
    var btn = mxUtils.button(mxResources.get('editData'), mxUtils.bind(this, function (evt) {
        this.editorUi.actions.get('editData').funct();
    }));

    btn.setAttribute('title', mxResources.get('editData') + ' (' + this.editorUi.actions.get('editData').shortcut + ')');
    btn.style.width = '202px';
    btn.style.marginBottom = '2px';
    div.appendChild(btn);

    mxUtils.br(div);

    btn = mxUtils.button(mxResources.get('clearDefaultStyle'), mxUtils.bind(this, function (evt) {
        this.editorUi.actions.get('clearDefaultStyle').funct();
    }));

    btn.setAttribute('title', mxResources.get('clearDefaultStyle') + ' (' + this.editorUi.actions.get('clearDefaultStyle').shortcut + ')');
    btn.style.width = '202px';
    div.appendChild(btn);

    return div;
};

/**
 * Adds the label menu items to the given menu and parent.
 */
DiagramFormatPanel.prototype.destroy = function () {
    BaseFormatPanel.prototype.destroy.apply(this, arguments);

    if (this.gridEnabledListener) {
        this.editorUi.removeListener(this.gridEnabledListener);
        this.gridEnabledListener = null;
    }
};
