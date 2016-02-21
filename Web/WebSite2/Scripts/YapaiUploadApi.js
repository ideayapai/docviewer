/**
    20150814

**/
//var ypUpload = {};
(function ($,window) {
    'use strict';

    window.ypUpload = {};//初始化命名空间
    var UploadResult = [];//上传成功返回的结果
    var selector = { buttons: [], options: [] }; //上传按钮对象
    var statusCode = { 0: "操作成功", 105: "上传失败" };//错误码 TODO
    var plugIn = ["imgbox"];//插件列表
    //var isFlash = false;//是否支持flash播放器
    ypUpload.defaults = {};
    var console = console || window.console;

    ////检测浏览器是否有flash播放器
    //(function () {
    //    function flashChecker() {
    //        var hasFlash = 0; //是否安装了flash
    //        var flashVersion = 0; //flash版本
    //        if (document.all) {
    //            var swf = new ActiveXObject('ShockwaveFlash.ShockwaveFlash');
    //            if (swf) {
    //                hasFlash = 1;
    //                VSwf = swf.GetVariable("$version");
    //                flashVersion = parseInt(VSwf.split(" ")[1].split(",")[0]);
    //            }
    //        } else {
    //            if (navigator.plugins && navigator.plugins.length > 0) {
    //                var swf = navigator.plugins["Shockwave Flash"];
    //                if (swf) {
    //                    hasFlash = 1;
    //                    var words = swf.description.split(" ");
    //                    for (var i = 0; i < words.length; ++i) {
    //                        if (isNaN(parseInt(words[i]))) continue;
    //                        flashVersion = parseInt(words[i]);
    //                    }
    //                }
    //            }
    //        }
    //        return { f: hasFlash, v: flashVersion };
    //    }
    //    var fls = flashChecker();
    //    if (fls.f) {
    //        console.log("您安装了flash,当前flash版本为: " + fls.v + ".x");
    //        return isFlash = true;
    //    } else {
    //        console.log("您没有安装flash");
    //    }
    //})();



    //获取本身JS的src
    (function () {
        var uploadScriptTag = document.getElementById("ypUpload");
        var regex = /.*\:\/\/([^\/]*).*/;
        var match = uploadScriptTag.src.match(regex);
        if (typeof match != "undefined" && null != match)
            ypUpload.BathUrl = 'http://' + match[1];
    })();



    //public method
    //设定上传按钮
    //支持多个按钮
    ypUpload.setUp = function (maker, options) {
        if (typeof maker === "string" && $("#" + maker).size() > 0) {
            selector.buttons.push(maker);
            selector.options.push(options);
        } else {
            ypUpload.error("makeUp传入的第一个参数必须为上传按钮的id");
        }
    };


    var head;
    var link;
    //if (isFlash) {
        //资源地址
        ypUpload.Resources = {
            uploadScript: ypUpload.BathUrl + '/Resource/js/SlickUpload.js',//上传组件js地址
            uploadCss: ypUpload.BathUrl + '/Resource/css/upload.css',//uploadfy css地址
        }

        //载入css
        head = document.getElementsByTagName('head')[0];
        link = document.createElement('link');
        link.href = ypUpload.Resources.uploadCss;
        link.rel = 'stylesheet';
        link.type = 'text/css';
        head.appendChild(link);
        //public property
        //默认设置
        ypUpload.defaults = {
            //TODO
            auto: true,//自动上传
            buttonText: '选择文件',
            //swf: ypUpload.BathUrl + '/Resource/uploadify/uploadify.swf',//uploadfy flash组件地址
            height: 30,
            width: 120,
            uploader: ypUpload.BathUrl + '/Document/Upload',
            formData: {
                userId: '',
                userName: '',
                spaceId: '',
                sessionId:'?_='+Math.random()
            },
            onSelect: function(data) {
                //console.log(data);
            },

            onFileRemove: function(data) {
                
            },

            onUploadStart: function() {
                
            },
            onUploadSuccess: function (file, data, response) {

            },

            onUploadComplete: function () {

            }

        };

   

        //private method
        //flash版本装配
        ypUpload.make = function () {
            $.when($.getScript(ypUpload.Resources.uploadScript)).then(function() {
                if (selector.buttons.length > 0) {
                    $.each(selector.buttons, function (k, v) {
                        $.extend(ypUpload.defaults, selector.options[k]); //合并自定义的参数
                        $('#' + v).wrap("<div id='" + v + "_slickUpload'>");
                        var uploadWrapper = $('#' + v + '_slickUpload');
                        uploadWrapper.append("<div id='" + v + "_slickUpload_selector' >选择文件</div>");
                        uploadWrapper.append("<div id='" + v + "_slickUpload_list'></div>");
                        if (!ypUpload.defaults.auto) {
                            uploadWrapper.append("<button id='" + v + "_uploadButton' class='button startupload'>开始上传</button>");
                        }
                       
                        uploadWrapper.append("<div id='" + v + "_slickUpload_selector_dropzone'>");
                        uploadWrapper.append("<div id='" + v + "_slickUpload_connector'>");
                        uploadWrapper.append("<div id='" + v + "_slickUpload_list_template'>");
                        uploadWrapper.find('#' + v + '_slickUpload_list').append("<div id='" + v + "_slickUpload_list_template' style=\"display:none;\" class=\"su-filelisttemplate\"><div class=\"filedata\"><span class=\"su-filename\"></span><span class=\"su-filesize\">(calculating)</span></div><a class=\"su-removecommand\">[x]</a><span class=\"su-validationmessage\"></span></div>");
                        uploadWrapper.append("<div id='" + v + "_slickUpload_progress'>");
                        uploadWrapper.find('#' + v + '_slickUpload_progress').append('<div id="' + v + '_duringUpload"><div class="uploadhead">上传状态</div><div class="uoloadBody"><div>上传文件 <span class="su-filecount"></span> 个,<span class="su-contentlengthtext">(calculating)</span>.</div><div>正在上传: <span class="su-currentfilename"></span>第 <span class="su-currentfileindex"></span> 个 <span class="su-filecount"></span>.</div><div>上传速度: <span class="su-speedtext">(calculating)</span></div><div><span class="su-timeremainingtext">(calculating)</span></div><div class="progressBarContainer"><div class="su-progressbar"></div><div class="progressBarText"><span class="su-percentcompletetext">(calculating)</span></div></div></div></div>');
                        $('#' + v).hide();
                        kw.debug = true;
                        kw._registerInit(function () {
                            var component = new kw.UploadConnector({
                                id: v + "_slickUpload_connector",
                                uploadHandlerUrl: ypUpload.BathUrl+"/SlickUpload.axd",
                                completeHandlerUrl: ypUpload.defaults.uploader,
                                completionMethod: "POST",
                                completionContentType: "application/json",
                                uploadForm: null,
                                confirmNavigateDuringUploadMessage: null,
                                uploadProfile: "mongodb",
                                allowPartialError: true,
                                onBeforeSessionEnd: function () {
                                    var requestBody = ypUpload.defaults.onUploadStart();
                                    if (typeof requestBody != "undefined" && requestBody != null) {
                                        kw(v + "_slickUpload").set_CompletionBody(JSON.stringify(requestBody));
                                    }
                                    
                                },
                                onUploadSessionEnded: function (data, responseBody) {
                                    $('#' + v + '_slickUpload_selector').show();
                                    ypUpload.defaults.onUploadSuccess(null,responseBody,true);
                                },

                            });

                            var component = new kw.FileSelector({
                                id: v + "_slickUpload_selector",
                                uploadConnector: v + "_slickUpload_connector",
                                folderElement: null,
                                dropZone: v + "_slickUpload_selector_dropzone",
                                showDropZoneOnDocumentDragOver: true,
                                onFileAdded: function (data) {
                                    ypUpload.defaults.onSelect(data);
                                    if (ypUpload.defaults.auto) {
                                        var slickUpload = kw(v + "_slickUpload");
                                        if (slickUpload.get_Files().length > 0) {
                                           slickUpload.start();
                                        } else {
                                            ypUpload.error("请选择文件");
                                        }
                                    }
                                    
                                    // document.getElementById(v + "_uploadButton").className = "button" + (kw(v + "_slickUpload").get_Files().length > 0 ? "" : " disabled");
                                },
                                onFileRemoved: function (data) {
                                    
                                    //document.getElementById(v + "_uploadButton").className = "button" + (kw(v + "_slickUpload").get_Files().length > 0 ? "" : " disabled");
                                }
                            });

                            var component = new kw.UploadProgressDisplay({
                                id: v + "_slickUpload_progress",
                                uploadConnector: v + "_slickUpload_connector"
                            });

                            var component = new kw.FileList({
                                id: v + "_slickUpload_list",
                                templateElement: v + "_slickUpload_list_template",
                                fileSelector: v + "_slickUpload_selector"
                            });

                            var component = new kw.SlickUpload({
                                id: v + "_slickUpload",
                                fileSelector: v + "_slickUpload_selector",
                                fileList: v + "_slickUpload_list",
                                uploadProgressDisplay: v + "_slickUpload_progress",
                                uploadConnector: v + "_slickUpload_connector"
                            });
                        });

                        if (!ypUpload.defaults.auto) {
                            $('body').on('click', "#" + v + "_uploadButton", function () {
                                var slickUpload = kw(v + "_slickUpload");
                                console.log(slickUpload.get_Files());
                                if (slickUpload.get_Files().length > 0) {
                                    slickUpload.start();
                                } else {
                                    ypUpload.error("请选择文件");
                                }

                            });
                        }
                        

                    });
                } else {
                    alert("没有上传按钮");
                }
            });
            
        }
  

    //错误提示
    ypUpload.error = function (ErrMsg) {
        $("<div>" + ErrMsg + "</div>").css({
            width: 150,
            height: 40,
            background: 'rgb(255, 237, 237)',
            position: 'absolute',
            border: '1px #F7DDDD solid',
            color: '#DE5F5F',
            right: '50%',
            "margin-left": '-75px',
            top: 10,
            display: 'none',
            'text-align': 'center',
            'line-height': '40px',
            'font-size': '14px'
        }).animate({
            top: 30,
            display: 'show'
        }, 300, function () {
            var _this = $(this);
            setTimeout(function() {
                _this.fadeOut(function() {
                    _this.remove();
                });
            }, 3000);
        }).appendTo($("body"));
    }

    ypUpload.info = function (InfoMsg) {
        $("<div>" + InfoMsg + "</div>").css({
            width: 150,
            height: 35,
            background: "green",
            position: "absolute",
            border: "1px #ccc solid",
            color: "#fff",
            'border-radius': 6,
            right: '50%',
            "margin-left": '-75px',
            top: 0,
            display: 'none',
            'text-align': 'center',
            'line-height': '35px',
            'font-size': '14px'
        }).animate({
            top: 10,
            display: 'show'
        }, 300, function () {
            var _this = $(this);
            setTimeout(function() {
                _this.animate({
                    top:0,
                }, function () {
                    _this.remove();
                });
            }, 3000);
        }).appendTo($("body"));
    }


})(jQuery,window);


