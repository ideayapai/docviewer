/**
    20150814

**/
(function (window) {
    //var ypUpload = {};//初始化命名空间
    var UploadResult = [];//上传成功返回的结果
    var selector = { buttons: [], options: [] }; //上传按钮对象
    var statusCode = {0: "操作成功",105:"上传失败"};//错误码 TODO
    var plugIn = ["imgbox"];//插件列表
    var isFlash = false;//是否支持flash播放器
    ypUpload.defaults = {};
    
    function log(msg){
        if (window["console"]){
            // console.log(msg);
        }
        return "";
    }
        
    //检测浏览器是否有flash播放器
    (function () {
        function flashChecker() {
            var hasFlash = 0; //是否安装了flash
            var flashVersion = 0; //flash版本
            if (document.all) {
                try{
                    //代码区
                    var swf = new ActiveXObject('ShockwaveFlash.ShockwaveFlash');
                }catch(Exception){
                    //异常处理
                    log(Exception);
                }
                
                if (swf) {
                    hasFlash = 1;
                    VSwf = swf.GetVariable("$version");
                    flashVersion = parseInt(VSwf.split(" ")[1].split(",")[0]);
                } else {
                    hasFlash = 0;
                }
            } else {
                if (navigator.plugins && navigator.plugins.length > 0) {
                    var swf = navigator.plugins["Shockwave Flash"];
                    if (swf) {
                        hasFlash = 1;
                        var words = swf.description.split(" ");
                        for (var i = 0; i < words.length; ++i) {
                            if (isNaN(parseInt(words[i]))) continue;
                            flashVersion = parseInt(words[i]);
                        }
                    } else {
                        hasFlash = 0;
                    }
                }
            }
            return { f: hasFlash, v: flashVersion };
        }
        var fls = flashChecker();
        if (fls.f){
            log("您安装了flash,当前flash版本为: " + fls.v + ".x");
            return isFlash = true;
        } else {
            log("您没有安装flash");
        }
    })();

    

    //获取本身JS的src
    (function () {
        var uploadScriptTag = document.getElementById("ypUpload");
        var regex = /.*\:\/\/([^\/]*).*/;
        var match = uploadScriptTag.src.match(regex);
            if (typeof match != "undefined" && null != match)
                ypUpload.BathUrl = 'http://'+match[1];
    })();



    //public method
    //设定上传按钮
    //支持多个按钮
    ypUpload.setUp = function (maker, options) {
        if (typeof maker === "string" && $("#" + maker).size() > 0) {
            selector.buttons.push(maker);
            //$.each(options, function(k, v) {
            //    console.log(k);
            //    switch (typeof v) {
            //        case "object":

            //    }
            //});
            options = $.extend(false, ypUpload.defaults, options);
            selector.options.push(options);
        } else {
            ypUpload.error("makeUp传入的第一个参数必须为上传按钮的id");
        }
    };

    //支持flash播放器的时候
    if (isFlash) {
        //资源地址
        ypUpload.Resources = {
            "uploadScript": ypUpload.BathUrl + '/Resource/uploadify/jquery.uploadify.js',//uploadfy js地址
            "uploadCss": ypUpload.BathUrl + '/Resource/uploadify/uploadify.css'//uploadfy css地址
        };

        //载入css
        var head = document.getElementsByTagName('head')[0];
        var link = document.createElement('link');
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
            swf: ypUpload.BathUrl + '/Resource/uploadify/uploadify.swf',//uploadfy flash组件地址
            height: 30,
            width: 120,
            uploader: ypUpload.BathUrl + '/api/Document/Add',
            fileSizeLimit: "10240KB",
            formData: {
                userId: '',
                userName: '',
                spaceId: '',
                depId:'',
                visible:'',
                path:''
            },
            onUploadSuccess: function (file, data, response) {
                log("<div>" + data.FileName + "</div>");
            },
            onUploadComplete: function () {
            }

        };

        //private method
        //flash版本装配
        ypUpload.make = function (options) {
            $.getScript(ypUpload.Resources.uploadScript, function() {
                if (selector.buttons.length > 0) {
                    $.each(selector.buttons, function(k, v) {
                        //if (selector.options[k] !== null || typeof selector.options[k] !== 'undefined') {
                        //    $.extend(false,ypUpload.defaults,selector.options[k]); //合并自定义的参数
                        //};
                        $('#' + v).uploadify(selector.options[k]);
                    });
                }
            });
        }
    } else {
        ypUpload.Resources = {
            uploadCss: ypUpload.BathUrl + '/Resource/uploadify/uploadify.css'//uploadfy css地址
        }

        //载入css
        var head = document.getElementsByTagName('head')[0];
        var link = document.createElement('link');
        link.href = ypUpload.Resources.uploadCss;
        link.rel = 'stylesheet';
        link.type = 'text/css';
        head.appendChild(link);

        ypUpload.defaults = {
            //TODO
            auto: true,//自动上传
            buttonText: '选择文件',
            swf: ypUpload.BathUrl + '/Resource/uploadify/uploadify.swf',//uploadfy flash组件地址
            height: 30,
            width: 120,
            fileSizeLimit: "10240KB",
            uploader: ypUpload.BathUrl + '/api/Document/Add',
            formData: {
                userId: '',
                userName: '',
                spaceId: '',
                depId: '',
                visible: '',
                path: '',
                sessionid:Math.round(Math.random())
            },
            onUploadSuccess: function (file, data, response) {
                
            }

        };

        var TimeStamp = new Date().getTime();

        //private method
        //form版本  装配

        ypUpload.make = function (options) {
            if (selector.buttons.length > 0) {
                $.each(selector.buttons, function (k, v) {
                    if (selector.options[k] !== null || typeof selector.options[k] !== 'undefined') {
                        $.extend(ypUpload.defaults, selector.options[k]);//合并自定义的参数
                    };

                    var button = $("<button/>").css({
                        width: ypUpload.defaults.width,
                        height: ypUpload.defaults.height,
                        "display": 'block',
                        "line-height": ypUpload.defaults.height + 'px',
					    position: 'absolute',
					    zIndex: 1
                    }).attr({
                        'class': 'uploadify-button',
                        'id': v + '_button'
                    }).text((ypUpload.defaults.buttonText != "") ? ypUpload.defaults.buttonText : "选择文件");


                    var form = $("<form/>").attr({
                        id: v + '_form',
                        action: ypUpload.defaults.uploader,
                        target: v + '_iframe',
                        enctype: "multipart/form-data",
                        method: 'post'
                    }).css({
                         width: ypUpload.defaults.width,
                        height: ypUpload.defaults.height
                    });



                    var iframe = $("<iframe/>").attr({
                        id: v + '_iframe',
                        name: v + '_iframe',
                        width: 0,
                        height: 0
                    }).css({
                        display: 'none'
                    });


                    $("#" + v).css({
                        opacity: 0,
                        width: ypUpload.defaults.width,
                        height: ypUpload.defaults.height,
                        cursor: 'pointer',
                        opacity: 0,
					    position: 'absolute',
					    zIndex: 9999
                    }).attr({
                        name: v + '_file'
                    });



                    $("#" + v).wrap(form);

                    $("#" + v).after(button);
                    $.each(ypUpload.defaults.formData, function (k, v) {
                        button.after("<input type='hidden' name='" + k + "' value='" + v + "' />");
                    });

                    button.after("<input type='hidden' name='TimeStamp' value='" + TimeStamp + "'/>");

                    $("body").append(iframe);

                    $("#" + v).on('change',function () {
                        
                        button.text("上传中...");
                        var checktimes = 0;
                        var setTime = setInterval(function () {
                            if (checktimes > 4) {
                                clearInterval(setTime);
                                checktimes = 0;
                            }
                            $.ajax({
                                Type: "GET",
                                url: ypUpload.BathUrl + "/api/Document/GetTemp?timeStamp=" + TimeStamp,
                                dataType: "text",
                                success: function (data) {
                                    if (data) {
                                        clearInterval(setTime);
                                        if (selector.options[k].onUploadSuccess && typeof selector.options[k].onUploadSuccess == "function") {
                                            selector.options[k].onUploadSuccess("", data, true);
                                        }
                                        button.text("选择文件");
                                    }
                                    $("#" + v).val("");
                                }
                            });
                            
                            checktimes++;
                        }, 2000);
                        $("#" + v + '_form').submit();
                    });
                })
            }
        }

    }
    



    //错误提示
    ypUpload.error = function (ErrMsg) {
        $("<div>" + ErrMsg + "</div>").css({
            width: 350,
            height: 40,
            background: 'rgb(255, 237, 237)',
            position: 'absolute',
            border: '1px #F7DDDD solid',
            color:'#DE5F5F',
            'border-radius': 6,
            right: '50%',
            "margin-left":'-175px',
            top: 200,
            display:'none',
            'text-align': 'center',
            'line-height': '40px',
            'font-size':'16px'
        }).animate({
            top: 220,
            display:'show'
        }, 300, function () {
            var _this = $(this);
            setTimeout(function () {
                _this.fadeOut(function () {
                    _this.remove();
                });
            },3000)
        }).appendTo($("body"));
    }

    ypUpload.info = function (InfoMsg) {
        $("<div>" + InfoMsg + "</div>").css({
            width: 350,
            height: 40,
            background: '#fff',
            position: 'absolute',
            border: '1px #ccc solid',
            color: '#333',
            'border-radius': 6,
            right: '50%',
            "margin-left": '-175px',
            top: 200,
            display: 'none',
            'text-align': 'center',
            'line-height': '40px',
            'font-size': '16px'
        }).animate({
            top: 220,
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


    window.ypUpload = ypUpload;

})(window);


  