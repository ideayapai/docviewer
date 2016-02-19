var apiUrl = "";
setUploadApiResources();
function setUploadApiResources() {
    apiUrl = fileapiurl;
    console.log("uploadfileapi资源:(1/5):检查配置是否正确");
    if (apiUrl == "") {
        console.log("uploadfileapi:ERROR:检查配置错误! 参数fileapiurl 不能为空");
        return;
    }
    console.log("uploadfileapi资源:(2/5)创建apiDIV空间");
    setTimeout(function() {
        console.log("uploadfileapi资源:(3/5)引入JavaScript资源");
        var link = document.createElement('link');
        link.href = apiUrl + '/Content/uploadify.css';
        link.setAttribute('rel', 'stylesheet');
        document.getElementsByTagName('head')[0].appendChild(link);
        var uploadify = document.createElement('script');
        uploadify.src = apiUrl + '/Scripts/uploadify.js';
        document.getElementsByTagName('head')[0].appendChild(uploadify);
        var swfobject = document.createElement('script');
        swfobject.src = apiUrl + '/Scripts/swfobject.js';
        document.getElementsByTagName('head')[0].appendChild(swfobject);
        var prettify = document.createElement('script');
        prettify.src = apiUrl + '/Scripts/prettify.js';
        document.getElementsByTagName('head')[0].appendChild(prettify);
        
        var loadinged = 0;
        setTimeout(function verification() {
            console.log("uploadfileapi资源:(4/5)开始判断引入JavaScript资源载入是否成功");
            for (var i = 0; i < 10; i++) {//10秒时间内判断,资源是否加载成功
                setTimeout(function () {
                    if (loadinged == 0) {
                        if (prettifyloadinged == 1 && swfobjectloadinged == 1 && uploadifyloadinged == 1) {
                            loadinged = 1;
                            console.log("uploadfileapi资源:(5/5)JavaScript资源已载入成功");
                            SetUploadObject(uploadapiconfig);
                            //设置上传返回结果集保存处
                        }
                    }
                }, 1000 * (i + 1));
            }
        }, 2000);
    }, 500);
}

function installUploadObject(htmlid, userid, username, spaceid) {
    if ($("#" + htmlid) != undefined) {
        console.log("installUploadObject:(1/2)绑定控件,TO[" + htmlid + "]");
        var htmlContent = '<input id="' + htmlid + '" type="file" name="Filedata" /><div id="' + htmlid + '-queue" class="uploadify-queue"></div>';
        $("#htmlid").html(htmlContent);
        setTimeout(function() {
            settingUpload(htmlid, userid, username, spaceid);
        }, 500);

        //添加result
        $("body").append("<div id='" + htmlid + "_results'></div>");
    } else {
        alert('未找到对象[' + htmlid + ']的依赖项');
    }
}

function settingUpload(htmlid, userid, username, spaceid) {
    console.log("installUploadObject:(1/2)设置AJAX参数,TO[" + htmlid + "]");
    $('#'+htmlid).uploadify
    ({
        'buttonText': '附件上传',
        'swf': apiUrl + '/uploadify.swf', //设置上传组件uploadify.swf的路径
        'uploader': apiUrl + '/api/Document/Add', //设置上传的Url接口Document/Add
        'cancelImg': apiUrl + '/images/cancel.png',
        'removeCompleted': false,
        'hideButton': true,
        'auto': true,
        'multi': true,
        'fileTypeDesc': 'PDF Files (.PDF)',
        'onUploadStart': function (file) {
            $("#" + htmlid).uploadify("settings", "formData", { 'userId': userid, 'UserName': username, 'SpaceId': spaceid });
        },
        'onQueueComplete': function (queueData) {
            //设置上传完成后方法
        },
        'onCancel': function (file) {
            //TODO;取消上传方法

        },
        'onUploadSuccess': function (file, data, responese) {
            //上传成功方法
            var objectData = $.parseJSON(data);
            //绑定删除方法
            $("#" + file.id).find("a").attr("href", "javascript:DeleteUploadedFile('" + htmlid + "','" + file.id + "','" + objectData.Id + "');");
            $("#" + file.id).find("a").attr("id", objectData.Id);
            $("#" + objectData.Id).after('<a href="'+apiUrl+'/plugin/show/' + objectData.Id + '" target="_blank" class="uploadify-show_link"></a>');
            //添加result
            $("#" + htmlid + "_results").append("<input type='hidden' id='" + htmlid + "_uploadresult_" + file.id.toString().split('_')[2] + "' name='" + htmlid + "_uploadresult" + "' value='" + data + "'/>");
        }
    });
    console.log("uploadfileapi:(2/2)控件已设置成功");
}

function DeleteUploadedFile(htmlid,fileid, dataid) {
    //Ajax请求删除ID
    console.log("deleteUploadedFile:(1/2):开始删除文件,id:["+dataid+"]");
    $.ajax({
        url: apiUrl + "/api/Document/Delete?fileId=" + dataid,
        type: 'POST',
        dataType: "text",
        success: function (e) {
            $('#' + htmlid).uploadify('cancel', fileid);
            console.log("deleteUploadedFile:(2/2):删除文件成功,id:[" + dataid + "]");
        },
        error: function (e) {
            console.log("deleteUploadedFile:(2/2):删除文件失败,消息:[" + $.parseJSON(e) + "]");
        }
    });
}

//移入回收站
function RecoverUploadedFile() {
    $("#result").html("");
    if ($.trim($("#recoverfileid").val()) == "") {
        alert("ID ERROR");
        return;
    }
    //Ajax请求删除ID
    $.ajax({
        url: apiUrl + "/api/Document/Recover?fileId=" + $.trim($("#recoverfileid").val()),
        type: 'POST',
        dataType: "json",
        success: function (e) {
            alert('回收成功');
            $("#result").html(e.data);
        },
        error: function (e) {
            alert("失败:" + $.parseJSON(e));
        }
    });
}

function GetUploadedFile() {
    $("#result").html("");
    if ($.trim($("#getfileid").val()) == "") {
        alert("ID ERROR");
        return;
    }
    //Ajax请求删除ID
    $.ajax({
        url: apiUrl + "/api/Document/Get/" + $("#getfileid").val(),
        type: 'GET',
        dataType: "text",
        success: function (e) {
            $("#result").html(JSON.stringify(e.d));
        },
        error: function (e) {
            alert("失败:" + $.parseJSON(e));
        }
    });
}