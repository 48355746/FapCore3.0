;
(function () {
    "use strict";
    (function () {
        var NGUeditor;
        NGUeditor = angular.module("ng.ueditor", []);
        NGUeditor.directive("ueditor", [
            function () {
                return {
                    restrict: "C",
                    require: "ngModel",
                    scope: {
                        config: "=",
                        ready: "=",
                        focus: "=",
                        blur: "="
                    },
                    link: function ($S, element, attr, ctrl) {
                        var _NGUeditor, _updateByRender;
                        _updateByRender = false;
                        _NGUeditor = (function () {
                            function _NGUeditor() {
                                this.bindRender();
                                this.initEditor();
                                return;
                            }


                            /**
                             * 初始化编辑器
                             * @return {[type]} [description]
                             */

                            _NGUeditor.prototype.initEditor = function () {
                                var _UEConfig, _editorId, _self;
                                _self = this;
                                if (typeof UE === 'undefined') {
                                    console.error("Please import the local resources of ueditor!");
                                    return;
                                }
                                _UEConfig = $S.config ? $S.config : {};
                                _editorId = attr.id ? attr.id : "_editor" + (Date.now());
                                element[0].id = _editorId;
                                this.editor = new UE.getEditor(_editorId, _UEConfig);
                                return this.editor.ready(function () {
                                    _self.editorReady = true;
                                    _self.editor.addListener("contentChange", function () {
                                        ctrl.$setViewValue(_self.editor.getContent());
                                        if (!_updateByRender) {
                                            if (!$S.$$phase) {
                                                $S.$apply();
                                            }
                                        }
                                        _updateByRender = false;
                                    });
                                    if (_self.modelContent && _self.modelContent.length > 0) {
                                        _self.setEditorContent();
                                    }
                                    if (typeof $S.ready === "function") {
                                        $S.ready(_self.editor);
                                    }
                                    if (typeof $S.focus === "function") {
                                        _self.editor.addListener("focus", function() {
                                            $S.focus(_self.editor);
                                            if (!_updateByRender) {
                                                if (!$S.$$phase) {
                                                    $S.$apply();
                                                }
                                            }
                                            _updateByRender = false;
                                        });
                                    }
                                    if (typeof $S.blur === "function") {
                                        _self.editor.addListener("blur", function() {
                                            console.log('blured');
                                            $S.blur(_self.editor);
                                            if (!_updateByRender) {
                                                if (!$S.$$phase) {
                                                    $S.$apply();
                                                }
                                            }
                                            _updateByRender = false;
                                        });
                                    }
                                });
                            };

                            _NGUeditor.prototype.setEditorContent = function (content) {
                                if (content == null) {
                                    content = this.modelContent;
                                }
                                if (this.editor && this.editorReady) {
                                    this.editor.setContent(content);
                                }
                            };

                            _NGUeditor.prototype.bindRender = function () {
                                var _self;
                                _self = this;
                                ctrl.$render = function () {
                                    _self.modelContent = (ctrl.$isEmpty(ctrl.$viewValue) ? "" : ctrl.$viewValue);
                                    _updateByRender = true;
                                    _self.setEditorContent();
                                };
                            };

                            return _NGUeditor;

                        })();
                        new _NGUeditor();
                    }
                };
            }
        ]);
    })();

}).call(this);
