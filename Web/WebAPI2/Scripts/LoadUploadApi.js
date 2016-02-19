//载入上传组件所需
var apiurl = "http://localhost:23427";


var link = document.createElement('link');
link.href = apiurl + '/Content/uploadify.css';
link.setAttribute('rel', 'stylesheet');
document.getElementsByTagName('head')[0].appendChild(link);
var uploadify = document.createElement('script');
uploadify.src = apiurl + '/Scripts/uploadify.js';
document.getElementsByTagName('head')[0].appendChild(uploadify);
var swfobject = document.createElement('script');
swfobject.src = apiurl + '/Scripts/swfobject.js';
document.getElementsByTagName('head')[0].appendChild(swfobject);
var prettify = document.createElement('script');
prettify.src = apiurl + '/Scripts/prettify.js';
document.getElementsByTagName('head')[0].appendChild(prettify);

document.write('<div id="upload-queue"></div><div style="float:left;position:absolute;"><div style="position:absolute;left:0px;top:0px;"><button class="btn btn-primary" type="submit" onclick=""><span>文件上传</span></button>&nbsp;<br /></div><div style="position:absolute;left:0px;top:0px;"><input id="file_upload" type="file" name="Filedata" /></div></div>');


$(document).ready(function () {
    setTimeout(
        function () {

            //绑定上传插件
            $('#file_upload').uploadify({
                'swf': apiurl + 'Content/uploadify.swf',//设置上传组件uploadify.swf的路径
                'uploader': apiurl + '/api/Document/Add', //设置上传的Url接口Document/Add
                'cancelImg': apiurl + '/images/cancel.png',
                'removeCompleted': true,
                'hideButton': true,
                'auto': true,
                'multi': true,
                'queueID': 'upload-queue',
                'fileTypeDesc': 'PDF Files (.PDF)',
                'onUploadStart': function (file) {
                    $("#file_upload").uploadify("settings", "formData", { 'userId': '10001', 'UserName': '测试用户', 'SpaceId': '' });
                },
                //设置上传完成后刷新本页
                'onQueueComplete': function (queueData) {
                    //返回结果 DocumentObject JSON
                    alert(queueData.toString());
                }
            });
        }, 10);
});