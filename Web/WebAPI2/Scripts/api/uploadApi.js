
; (function ($) {

    //初始化命名空间
    var Upload = {};
    var checkInt;//循环检测定时器id
    var isReady = false;//资源是否载入成功
    var timeOut = 5;//循环检测5次


    Upload.method = {
        uploadScript: '/Content/uploadfy/jquery.uploadify.min.js',//uploadfy js地址
        uploadCss: '/Content/uploadfy/uploadify.css',//uploadfy css地址
    }


    Upload.default = {
        //TODO
        swf: '/Content/uploadfy/uploadify.swf',//uploadfy flash组件地址
        height: 30,
        width: 120,
        uploader: 'http://localhost:18889/api/Document/Add'

    };


    //初始化
    //加载资源
    Upload.init = function () {
        loadStyle(Upload.res.uploadCss);
        $.getScript(Upload.res.uploadScript, function () {
            isReady = true;
        });
    };

    //创建上传按钮
    Upload.create = function (selector, options) {
        checkInt = setInterval(function () {
            if (isReady) {
                clearInterval(checkInt);
                $(selector).uploadify(Upload.default);
            } else {
                Upload.init();
            }
        }, 600);
    }


})($);