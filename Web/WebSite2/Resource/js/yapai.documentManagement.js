$(function () {
    /**
     * 整体布局
     * @returns {} 
     */
    function layout() {
        var setLaout = {
            mainHeight: $(window).height() - $(".main").position().top,
            showStyle: $(".showstyle").height(),
            mainWidth: $(window).width() - $(".sidebar").width() - 1
        };
        $(".main").height(setLaout.mainHeight);
        $(".content").height(setLaout.mainHeight - setLaout.showStyle);
        $(".file-list, .file-pic").height($(".con_list").height() - $(".file-list").siblings().height());
        $(".viewbox").width(setLaout.mainWidth);
    }

    //布局初始化
    layout();
    //浏览器窗口状态发生变动时再次布局
    window.onresize = layout;
});

//树状菜单变量值初始化
var folder_setting = {
        treeRoot: undefined,
        parentNode: {}
    },
    treeObj,
    node,
    uploadTree;

//在当前窗口浏览图片或文档的必要层
$(document.body).append('<div class="picShow"></div>');
//操作按钮栏权限 true为开启
$(".showstyle span").data("disabled", true);

/**
 * 处理加载失败的图片
 */
$(".con_list img").error(function () {
    $(this).attr("src", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAABmJLR0QA/wD/AP+gvaeTAAAFq0lEQVRYw8WXbWwcxRnHf7Mzs7t35/Pd9ewkThy/0RI1Ji9gxT4bQ5uo/YCbF6BUqlQwRAlKsaUIjCqBSD+RD0hp06K2UZsEVaIEobSRAImCUBqoqE2TgkogdSsaAgQSmji2ia/4zr67nX6YNblYOInTACM9Wt2z/5vn9zwzz+6sMMbwZQ7nS40OiMsFf/U+fhSVPAIwXuKB9p+xDQi+EOqNGaqPPIgxJ5415sSz5siDmI0Zqi9nLnkZ/9E7vstztZnv1ccKb8DoYUTlNTSWBjO7D/LEbKsw2z0gnuymPZ6K3lDd0gVH98HRfVS3dBFPRW94spt2ZrmsswVwF6V5qn5VLwxuB1dbG9xO/apeFqV5CnA/LwDn+U10Jxc21sSq4jD2Nijf2tjbxKriJBY21jy/ie7ZzDsbAK8mxs6aju/D0V3g+VChrHk+/HsXdSs3UhNjJ+BdaQD5Ui8PpxevIOKehWIWIopkTzPJnmaIKChl0aWjfGXxCl7q5WEucYNfEkBPJ+mqSu5f0NoFJ56xZXc1FV5AhRfYfaB8+OhFalu/Q1WluL+nk/SVAlC3X8fOucu/jcj/CwhAKVASKQxSGFDS+ggQ+X8yd/m3uP06dgLq/wUQe++iI5Hy11UvaYOhfnA8cFxwNKUASgHg6NDnwVA/1UvaSKT8dXvvooOLtOXFANyGBDtqM2tgdACkazOVEoQkNwG5CUBI61PKakYHWJBZQ0OCHVykLS8E4LywiTsTc1LNlQ11MH7cTu5oEAqMZvjpEYafHgET+hxtNePHSTTUkV44v/mFTdx5oTgXAvDmRHm07hu3wuhrIDXIMHslwZUk16ZIrk2BG/qkDDUahl+lJnMTc6I8ygXaciYA+XIvW1NNjb6fEjA5Eq6xOmdCEwSGIDAg9Pn3HBeKWSIpQaqp0X+5l63M0JYzAfgJl776G1fD2KBdW63KspRgJGN/LDL2XBECCU7ZvSl9dpD6G1eTcOkD/EsFkAOb2bOgvQMhh0EUIOJCNHzuawWetj7tgnJBTy1PGDiiIeqBKCDkMAvaOxjYzJ7PqsJ0APGr27g6HmFd9dIWmPgAYi4/eSKPWHEaYjbwgUNFkquGEC2nSK4c4sDBIvjagpyndyH/AdVLWqiIsO6Xt3E109pyOoBureUPNe3fhNJJkJD7WLDlsf/iRgV4CnzJhm1jrOn0MccaWNPps2HbGPgKPEXurGHLY1mrdzUIA6WT1K+8ibZafg/omQDE4z8gE63Ui9PXLoPSCMQ8bv7xCGs7I4Cxa+tq3jtZ4KHuFOQlD3WneO+jgi17hebmLeX6sC0nh6lcdBXRSt38zAZWl1ehHMBdMpcXm7pugfH3wVX8+ZUc+w/l2PvT+RSL5txGNIaiEYAMr1AYMvS/kj9fr8L2dBTkPqSp6xbqEuwpb8spAGf/PfTF51d5/rwkBBPguazfeprfPDAXGmMoCeQdu/EEKG2robQCDPqrMdZvPXW+fiLUOxpKE/jzksTnV/n77+G+qdhTpYj+/V4+WfbDjQgxDEEBog6i8Q2k7yAd0ErwSbaEOdXCVW1H6Fga43ePN3HHHe8w8NY47/ztGsSc15Fumf5sCXNsOeQCMIDSGJPm8K93c+3PiQHjApD9m9nV1Nq8fl5nK+RHLZIS4Dv26glE6q+YoQxMBhz408fceu+7nM0WqayQvLXv69R9LWLTKdefboNsAJPhOVUAfor//OUQxw7+47fX/4K7BVBxuI/s0r4eKAxDqWSVgnMmhbWSgcDYIK5jJ69wbICisVlO1xdCP9iNKSXoNG9u38Gy7cQV4FvySVAxMJ91qg6BnHM/MUA8vKrQPo1UptfTPv2EA0yiIgIwvgLEmTz9b27bfb0plcF+TkNg395n8vQDQoR51AFVXMIJ5gqNInAGOC7CoBHsy+JyvxVnOwyQB3JfULyZx/8A/IimYLc8/4YAAAAldEVYdGRhdGU6Y3JlYXRlADIwMTUtMDctMjVUMjE6NDk6MzErMDg6MDBI0w9NAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDEyLTA5LTI5VDExOjE2OjM1KzA4OjAwZ3q66AAAAE50RVh0c29mdHdhcmUASW1hZ2VNYWdpY2sgNi44LjgtMTAgUTE2IHg4Nl82NCAyMDE1LTA3LTE5IGh0dHA6Ly93d3cuaW1hZ2VtYWdpY2sub3JnBQycNQAAABh0RVh0VGh1bWI6OkRvY3VtZW50OjpQYWdlcwAxp/+7LwAAABd0RVh0VGh1bWI6OkltYWdlOjpIZWlnaHQAMzIo9Pj0AAAAFnRFWHRUaHVtYjo6SW1hZ2U6OldpZHRoADMy0Fs4eQAAABl0RVh0VGh1bWI6Ok1pbWV0eXBlAGltYWdlL3BuZz+yVk4AAAAXdEVYdFRodW1iOjpNVGltZQAxMzQ4ODg4NTk1QckW3wAAABN0RVh0VGh1bWI6OlNpemUAMi4zMUtCQrqVh5kAAABadEVYdFRodW1iOjpVUkkAZmlsZTovLy9ob21lL3d3d3Jvb3Qvd3d3LmVhc3lpY29uLm5ldC9jZG4taW1nLmVhc3lpY29uLmNuL3NyYy8xMDg2Mi8xMDg2MjMyLnBuZ0HKK44AAAAASUVORK5CYII=");
    $(this).closest("li").data("loadFailed", true).addClass("loadFailed");
});

/**
 * 左侧树状目录及上传树状目录对象初始化
 */
$(function () {
    treeObj = $.fn.zTree.getZTreeObj("tree");
    uploadTree = $.fn.zTree.getZTreeObj("uploadTree");
    if (treeObj != null) {
        node = treeObj.getNodeByTId("tree_1");
        folder_setting.treeRoot = node;
        folder_setting.parentNode.xid = $("#uploadSpaceId").val();
    }
});

/**
 * 显示消息提示框
 * @param text 需要显示的消息内容 string
 * @param cla 需要显示的消息的类型 string 值“ok”、“info”、“error”或“warning”
 * @param status 消息提示框持续的状态 string 默认值“time”、缺省状态。 值“time”：显示一段时间后自动消失、“always”：手动控制消失，控制方式：调用hideDialog()函数。
 * @returns {*|jQuery|HTMLElement}
 */
function showDialog(text, cla, status) {
    this.text = text;
    this.cla = cla;
    var dialog = $("<span class='information " + this.cla + "'>" + this.text + "</span>");
    dialog.appendTo($(".breadcrumb"));

    dialog.animate({
        "top": "0"
    }, 300, function () {
        if (typeof status == "undefined" || status === "time") {
            setTimeout(function () {
                hideDialog(dialog);
            }, 4000);
        }
    });
    return dialog;
}

/**
 * 关闭消息提示框
 * @param dialog 需要关闭的消息提示框jQuery对象，缺省或者其他值为关闭所有已打开的消息框
 */
function hideDialog(dialog) {
    if (typeof dialog != "undefined") {
        if (typeof dialog == "object" || dialog.indexOf(".")) {
            dialog = $(dialog);
        } else {
            dialog = $("." + dialog);
        }
    } else {
        dialog = $(".information");
    }
    dialog.animate({
        "top": "-35px"
    }, 500, function () {
        dialog.remove();
    });
}

/**
 * 上传文件对话框的显示与隐藏
 */
function show_upload_box() {
    $(".upload-box").stop(true, true).animate({opacity: "show"}, 200);
}
function cancle_upload() {
    if (typeof nowPageUploadCount != "undefined" && nowPageUploadCount > 0)
        location.href = location.href;
    $(".upload-box").stop(true, true).animate({opacity: "hide"}, 200);
}

/**
 * 左侧树状目录的显示与隐藏
 */
$("#showtree").on("click", function () {
    var text = $(this).data("show");
    if (text === "hide") {
        $(this).attr("title", "隐藏目录树");
        $(this).data("show", "show");
        $(".tree-wrap").toggle();
        $(".con_list").width($(".content").width() - $(".tree-wrap").width() - 2);
        addCookie("showTree", "show", 24 * 7);
    } else {
        $(this).attr("title", "显示目录树");
        $(this).data("show", "hide");
        $(".tree-wrap").toggle();
        $(".con_list").width("100%");
        addCookie("showTree", "hide", 24 * 7);
    }
});

/**
 * 列表和缩略图之间的切换
 */
$("#showtoggle").on("click", function () {
    if ($("#showtype").val() === "list") {
        $(this).attr("title", "显示为缩略图");
        $("#showtype").val("pic");
        $(this).css("background-position", "0 -217px");
        $(".file-pic").hide();
        $(".file-list").show();
        addCookie("showType", "list", 24 * 7);
    } else {
        $(this).attr("title", "显示为列表");
        $("#showtype").val("list");
        $(this).css("background-position", "-1px -272px");
        $(".file-pic").show();
        $(".file-list").hide();
        addCookie("showType", "pic", 24 * 7);
    }
});

/**
 * 设置文件（夹）的初始显示方式
 */
if (getCookie("showType") === "pic") {
    $("#showtoggle").click();
}

/**
 * 设置目录树初始是否显示
 */
if (getCookie("showTree") === "show") {
    $("#showtree").click();
}

/**
 * 列表和缩略图文件（夹）的勾选与取消勾选
 */
$("#chk-all").on("click", function () {
    if ($(this).attr("checked")) {
        $(".file-list li[data-visibility='True'] input[type=checkbox]").attr("checked", true);
        $(".file-pic .chk-wrap").remove();
        $(".file-pic li[data-visibility='True']:not(.recently_name)").append('<div class="chk-wrap"> <input type="checkbox" checked/> <i class="chk"></i> </div>').addClass("checked");
        $(".file-pic .chk").css("background-position", "0 -36px");
    } else {
        $(".file-list input[type=checkbox]").attr("checked", false);
        $(".file-pic li").removeClass("checked");
        $(".file-pic .chk-wrap").remove();
    }
});
$("#chk-all-trash").on("click", function () {
    if ($(this).attr("checked")) {
        $(".file-list li input[type=checkbox]").attr("checked", true);
        $(".file-pic .chk-wrap").remove();
        $(".file-pic li").append('<div class="chk-wrap"> <input type="checkbox" checked/> <i class="chk"></i> </div>').addClass("checked");
        $(".file-pic .chk").css("background-position", "0 -36px");
    } else {
        $(".file-list input[type=checkbox]").attr("checked", false);
        $(".file-pic li").removeClass("checked");
        $(".file-pic .chk-wrap").remove();
    }
});
$(".file-pic").on("click", "li:not(.recently_name) input[type=checkbox]", function (e) {
    var _this = $(this),
        $chkPic = _this.next(),
        $pic = _this.closest("li"),
        e = e || window.event;
    if (_this.attr("checked")) {
        $chkPic.css("background-position", "0 -36px");
        $pic.addClass("checked");
        $(".file-list").find("li[data-id=" + $pic.data("id") + "] input[type=checkbox]").attr("checked", true);
    } else {
        $chkPic.css("background-position", "0 0");
        $pic.removeClass("checked");
        $(".file-list").find("li[data-id=" + $pic.data("id") + "] input[type=checkbox]").attr("checked", false);
    }
    e.stopPropagation();
});
$(".file-list").on("click", "li input[type=checkbox]", function (e) {
    var _this = $(this),
        e = e || window.event,
        selector = ".file-pic li[data-id=" + _this.closest("li").data("id") + "]";
    if (_this.attr("checked")) {
        $(selector + " .chk-wrap").remove();
        $(selector + " input[type=checkbox]")
            .attr("checked", true);
        $(selector)
            .append('<div class="chk-wrap"> <input type="checkbox" checked/> <i class="chk" style="background-position : 0 -36px"></i> </div>')
            .addClass("checked");
    } else {
        $(selector + " input[type=checkbox]").attr("checked", false);
        $(selector).removeClass("checked");
        $(selector + " .chk-wrap").remove();
    }
    e.stopPropagation();
});

/**
 * 列表和缩略图的hover事件
 */
$(".file-list").on("mouseenter", "li:not(:first-child, .recently_name)", function (e) {
    var $this = $(this);
    $this.addClass("hover");
    $this.siblings().removeClass("hover");
    if ($this.is(".trash, .row-creating")) return;
    var e = e || window.event;
    if ($(e.target).closest("li").data("id") != $(".moreCon").data("id")) {
        $(".list-oper-wrap").remove();
        $(".moreCon").hide();
    }

    if ($this.data("visibility") && $this.data("visibility") != "False") {//判断权限
        if ($this.data("file")) {
            $this.find(">div:first-child").append('<span class="list-oper-wrap"><i class="download" title="下载"></i> <i class="reName_per" title="重命名"></i> <i class="remove_per" title="删除"></i> <i class="more"></i></span>');
        } else {
            $this.find(">div:first-child").append('<span class="list-oper-wrap"><i class="reName_per" title="重命名"></i>  <i class="remove_per" title="删除"></i> <i class="more"></i></span>');
        }
        $(this).find("input[type=checkbox]").attr("disabled", false);
    } else {
        if ($this.data("file")) {
            $this.find(">div:first-child").append('<span class="list-oper-wrap"><i class="download" title="下载"></i></span>');
        }
        $(this).find("input[type=checkbox]").attr({"checked": false, "disabled": true});
    }
    if ($this.data("folder") && typeof span != "undefined" && span.hasClass("clone")) {
        span.val("drag");
    }
});
$(".file-list").on("mouseleave", "li", function () {
    var $this = $(this);
    if ($(".moreCon").css("display") == "none") {
        $this.find(".list-oper-wrap").remove();
        $this.removeClass("hover");
    }
    if (typeof span != "undefined" && span.hasClass("clone")) {
        span.val("");
    }
});
$(".file-pic").on("mouseenter", "li:not(.recently_name)", function (e) {
    var $this = $(this);
    $this.addClass("hover");
    $this.siblings().removeClass("hover");
    if ($this.is(".trash, .row-creating")) return;
    var e = e || window.event;
    if ($(e.target).closest("li").data("id") !== $(".moreCon").data("id")) {
        $(".pic-oper-wrap").remove();
        $(".moreCon").hide();
    }
    if ($this.data("visibility") && $this.data("visibility") !== "False") {//判断权限
        if ($this.data("file")) {
            $this.append('<div class="pic-oper-wrap"><i class="download" title="下载"></i><i class="reName_per" title="重命名"></i><i class="remove_per" title="删除"></i><i class="more"></i></div>');
        } else {
            $this.append('<div class="pic-oper-wrap"><i class="reName_per" title="重命名"></i><i class="remove_per" title="删除"></i><i class="more"></i></div>');
        }
        if (!$this.find(".chk-wrap").length) {
            $this.append('<div class="chk-wrap"> <input type="checkbox"/> <i class="chk"></i> </div>');
        }
    } else {
        if ($this.data("file")) {
            $this.append('<div class="pic-oper-wrap"><i class="download" title="下载"></i></div>');
        }
    }
    if ($this.data("folder") && typeof span != "undefined" && span.hasClass("clone")) {
        span.val("drag");
    }
});
$(".file-pic").on("mouseleave", "li", function (e) {
    var e = e || window.event,
        $this = $(this);
    if (!$this.find(".chk-wrap input").attr("checked") && !$(e.target).is(".more")) {
        $this.find(".chk-wrap").remove();
    }
    if ($(".moreCon").css("display") === "none") {
        $this.find(".pic-oper-wrap").remove();
        $this.removeClass("hover");
    }
    if (typeof span != "undefined" && span.hasClass("clone")) {
        span.val("");
    }
});

/**
 * 列表和缩略图拖拽移动文件
 * ---------------------------------------------------------------------------------------------------------------------
 */
var span,
    startClass,
    eTarget = {
        start: "",
        end: ""
    },
    ePagePosition = {
        oldPageX: 0,
        newPageX: 0,
        oldPageY: 0,
        newPageY: 0
    };

$(".con_list").on("mousedown", "li[data-file='true'], li[data-folder='true']", function (e) {
    if ($(this).is(".trash, .row-creating, .loadFailed") || $(this).closest("li").is(".trash, .row-creating, .loadFailed")) return;
    var e = e || window.event,
        $target = $(e.target);
    if (!$.browser.msie && e.button === 0 || $.browser.msie && e.button === 1) {
        eTarget.start = $target.closest("li");
        ePagePosition.oldPageX = e.pageX;
        ePagePosition.newPageX = e.pageX;
        ePagePosition.oldPageY = e.pageY;
        ePagePosition.newPageY = e.pageY;
        if (!$target.is("input, .pic-oper-wrap i, .list-oper-wrap i")) {
            //create drag DOM
            startClass = $(this).find("i").attr("class");
            span = $('<span class="clone ' + startClass + '"></span>');
            $(this).append(span);
            if ($(this).data("folder")) {
                span.val("drag");
            }
        }

        $(".con_list").on("mousemove", function (event) {
            var event = event || window.event;
            $target = $(event.target);
            if (!$.browser.msie && e.button === 0 || $.browser.msie && e.button === 1) {
                ePagePosition.newPageX = event.pageX;
                ePagePosition.newPageY = event.pageY;
                eTarget.end = $target.closest("li");
                $("span.clone").css({
                    "left": event.pageX + 15,
                    "top": event.pageY
                });
                if (eTarget.start.data("id") !== eTarget.end.data("id")) {
                    $("span.clone").css("opacity", 0.7);
                } else {
                    $("span.clone").css("opacity", 0);
                }
            }
        });
    }
});

$(document.body).on("mouseup", function (e) {
    var e = e || window.event,
        $target = $(e.target);
    eTarget.end = $target.closest("li");
    $(".con_list").off("mousemove");
    if ($target.is("i")) return;
    if ((!$.browser.msie && e.button === 0 || $.browser.msie && e.button === 1) && typeof span != "undefined" && span.hasClass("clone")) {
        if (span.val() === "drag") {
            $("span.clone").remove();
            var eStart = eTarget.start.data("id"),
                eEnd = eTarget.end.data("id");
            if (eStart !== eEnd) {
                var dialog = showDialog("正在移动文件（夹），请勿做其他操作", "info", "always");
                //禁用其他操作按钮
                $(".showstyle > span")
                    .data("disabled", false)
                    .css("opacity", "0.3");

                //准备请求参数
                var type = "";
                if (eTarget.start.data("folder")) {
                    type = "folder";
                } else {
                    type = "file";
                }

                $.ajax({
                    type: "POST",
                    url: "/Document/Move",
                    data: {
                        "id": eStart,
                        "type": type,
                        "spaceid": eEnd
                    },
                    dataType: 'JSON',
                    success: function (data) {
                        if (data.Status === 0) {
                            if (type === "folder") {
                                //更新树状目录文件夹结构
                                var treeStart = treeObj.getNodeByParam("xid", eStart, null),
                                    treeEnd = treeObj.getNodeByParam("xid", eEnd, null);
                                treeObj.addNodes(treeEnd, treeStart);
                                treeObj.removeNode(treeStart);

                                //更新上传树目录文件夹结构
                                treeStart = uploadTree.getNodeByParam("xid", eStart, null);
                                treeEnd = uploadTree.getNodeByParam("xid", eEnd, null);
                                uploadTree.addNodes(treeEnd, treeStart, true);
                                uploadTree.removeNode(treeStart);
                            }

                            //从页面上删除被移动的文件（夹）
                            //$(eTarget.start).remove();
                            $(".con_list li[data-id='" + eStart + "']").remove();

                            showDialog("移动文件（夹）成功", "ok");
                        } else {
                            showDialog("移动文件（夹）失败", "error");
                        }
                        hideDialog(dialog);
                        $(".showstyle > span").data("disabled", true).css("opacity", "1");
                    },
                    error: function () {
                        showDialog("发生未知错误，移动文件（夹）失败", "error");
                        hideDialog(dialog);
                        $(".showstyle > span").data("disabled", true).css("opacity", "1");
                    }
                });
            } else {
                if (ePagePosition.newPageX === ePagePosition.oldPageX && ePagePosition.newPageY === ePagePosition.oldPageY && !$(e.target).closest("li").is(".trash") && $("#delFiles").data("disabled")) {//鼠标单击后，不能移动鼠标
                    //打开文件夹
                    window.location = $(e.target).closest("li").find("span").data("url");
                }
            }
        } else {
            if (eTarget.start.data("id") !== eTarget.end.data("id") && $("span.clone").length > 0) {
                showDialog("移动目标无效", "error");
            }
            $("span.clone").remove();
        }
    }
});
//-------------------------------------------------拖拽移动文件结束-----------------------------------------------------

/**
 * 列表中每项的下载、重命名和删除触发按钮
 */
$(".con_list").on("click", "[class$='oper-wrap'] i", function (e) {
    var $this = $(this),
        li = $this.closest("li");
    if ($this.is(".download")) {//下载
        window.open(li.find("span").data("url"));
    } else if ($this.is(".reName_per")) {//重命名
        if ($("#reName").data("disabled")) {
            var id = li.data("id"),
                listLi = $(".file-list li[data-id='" + id + "']"),
                picLi = $(".file-pic li[data-id='" + id + "']"),
                $list = listLi.find(".name"),
                $pic = picLi.find("p");
            //禁用该项的点击事件
            listLi.addClass("row-creating");
            picLi.addClass("row-creating");

            //禁用操作按钮
            listLi.find(".list-oper-wrap").remove();
            picLi.find(".pic-oper-wrap").remove();
            $(".showstyle > span")
                .data("disabled", false)
                .css("opacity", "0.3");

            //在当前项添加输入框（启用编辑状态）
            $list.html('<input class="newText" type="text" value="' + $.trim($list.text()) + '"/> <img class="loading list-loading" src="/images/loading.gif" />');
            $pic.find("span").hide();
            $pic.append('<input class="newText" type="text" value="' + $.trim($pic.find("span").text()) + '"/>');
            $pic.before('<img class="loading pic-loading" src="/images/loading.gif" />');
            $(".newText").select();

            //监听回车
            $(".newText").on("keydown", function (e) {
                var e = e || event;
                if (e.keyCode === 13) {
                    $(this).blur();
                }
            });

            //移除编辑状态
            function editClose(name) {
                //显示文件名
                $list.html(name);
                $pic.find("span").show();
                //删除编辑框和加载图标
                $(".newText").remove();
                $(".loading").remove();
                //恢复该项的点击事件
                listLi.removeClass("row-creating");
                picLi.removeClass("row-creating");
            }

            $(".newText").one("blur", function () {
                $(".newText").attr("readonly", true);
                $(".row-creating .loading").fadeIn(300);
                var reg = new RegExp('^[^\\\\\\/:*?\\"<>|]+$'),
                    type = listLi.data("folder") && picLi.data("folder") ? "folder" : "file",
                    newText = $.trim($(this).val()),
                    oldText = $.trim($pic.find("span").text());
                if (newText === "") {
                    showDialog("文件夹名称不能为空", "error");
                    editClose(oldText);
                    setTimeout(function () {
                        $(".showstyle > span").data("disabled", true).css("opacity", "1");
                    }, 300);
                } else if (!reg.test(newText)) {
                    showDialog("文件夹名称不能包含 / \\ : * ? < > | \" ", "error");
                    editClose(oldText);
                    setTimeout(function () {
                        $(".showstyle > span").data("disabled", true).css("opacity", "1");
                    }, 300);
                } else {
                    //将列表和缩略图已输入的名称同步
                    $(".newText").val(newText);
                    //检查是否重名
                    if (oldText !== newText) {
                        var dialog = showDialog("正在重命名文件（夹）", "info", "always");
                        $.ajax({
                            type: "POST",
                            url: "/Document/ReName",
                            data: {
                                "id": listLi.data("id"),
                                "type": type,
                                "name": newText
                            },
                            dataType: "JSON",
                            success: function (data) {
                                if (data.Status === 0) {
                                    //更新文件（夹）名称
                                    $list.text(newText);
                                    $pic.find("span").text(newText);

                                    if (type === "folder") {
                                        //更新树状目录中结点名称
                                        var root = treeObj.getNodesByParam("name", oldText, null);
                                        root[0].name = newText;
                                        treeObj.updateNode(root[0]);

                                        //更新上传树目录结点名称
                                        root = uploadTree.getNodesByParam("name", oldText, null);
                                        root[0].name = newText;
                                        uploadTree.updateNode(root[0]);
                                    }

                                    editClose(newText);
                                    showDialog("重命名文件（夹）成功", "ok");
                                } else {
                                    editClose(oldText);
                                    showDialog("重命名文件（夹）失败", "error");
                                }
                                hideDialog(dialog);
                                $(".showstyle > span").data("disabled", true).css("opacity", "1");
                            },
                            error: function () {
                                showDialog("发生未知错误，重命名文件（夹）失败", "error");
                                hideDialog(dialog);
                                editClose(oldText);
                                $(".showstyle > span").data("disabled", true).css("opacity", "1");
                            }
                        });
                    } else {
                        showDialog("未定义新名称，重命名已取消", "warning");
                        editClose(oldText);
                        setTimeout(function () {
                            $(".showstyle > span").data("disabled", true).css("opacity", "1");
                        }, 300);
                    }
                }
            });
        }
    } else if ($this.is(".remove_per")) {//删除
        var prom = confirm("确定要删除选中的文件（夹）？");
        if (prom) {
            if (li.data("visibility") && li.data("visibility") !== "False") {
                var dialog = showDialog("正在删除中", "info", "always");
                var id = li.data("id"),
                    type = "";
                if ($this.closest("li").data("file")) {
                    type = "file";
                } else {
                    type = "folder";
                }
                $.ajax({
                    type: "POST",
                    url: "/Document/DeleteList",
                    data: { "ids": id + "|", "types": type + "|" },
                    dataType: "JSON",
                    success: function() {
                        ////注意以下的代码片段的顺序不可改变
                        if (type === "folder") {
                            //删除目录树中对应的文件夹
                            if (treeObj != null) {
                                nodes = treeObj.getNodeByParam("xid", id, null);
                                treeObj.removeNode(nodes);
                            }
                            //删除上传树状菜单中的文件夹
                            if (uploadTree != null) {
                                nodes = uploadTree.getNodeByParam("xid", id, null);
                                uploadTree.removeNode(nodes);
                            }
                        }

                        //删除选中的文件
                        $(".con_list li[data-id='" + id + "']").remove();

                        //最近页面 如果该时间段内没有文件，则删除该时间段的标题
                        var i = 0,
                            $rec = $(".recently_name"),
                            length = $rec.length,
                            preN,
                            curN,
                            nodes;
                        for (i; i < length; i++) {
                            curN = $rec.eq(i).index();
                            if (typeof preN != "undefined" && preN + 1 === curN) {
                                $rec.eq(i - 1).addClass("rn_remove");
                            }
                            preN = curN;
                        }
                        $(".rn_remove").remove();
                        showDialog("删除成功", "ok");
                        hideDialog(dialog);
                    },
                    error: function() {
                        showDialog("发生未知错误，删除文件（夹）失败", "error");
                        hideDialog(dialog);
                        $(".row-creating").remove();
                    }
                });
            } else {
                showDialog("您没有权限删除此文件（夹）", "warning");
            }

        }
    }
    var e = e || window.event;
    e.stopPropagation();
});

/**
 * 列表和缩略图每项的选择权限下拉框
 */
$(".con_list").on("mouseenter", "li .more", function () {
    //TODO 查询权限 设置权限按钮的禁用或解除的显示样式


    var _this = $(this),
        height = _this.height(),
        _top = parseFloat(_this.offset().top),
        _left = parseFloat(_this.offset().left),
        id = _this.closest("li").data("id"),
        moreConHeight = $(".moreCon").height(),
        winHeight = $(window).height();
    if(_top + moreConHeight > winHeight){
        _top = _top - moreConHeight + height;
    }
    $(".moreCon").data("id", id).css({
        "left": _left,
        "top": _top
    }).show();

    $(".moreCon").mouseleave(function () {
        $(".moreCon").data("id", "").hide();
        $(".list-oper-wrap, .pic-oper-wrap").remove();
    });
});

/**
 * 列表和缩略图每项的下拉框点击事件
 */
$(".moreCon").on("click", "li", function (e) {
    var _this = $(this),
        e = e || window.event;
    if (!_this.is(".disabled")) {
        var id = $(".moreCon").data("id"),
            type = "",
            visibility = _this.data("para"),
            dialog = showDialog("正在操作中", "info", "always");
        if ($(".file-list li[data-id='" + id + "']").data("file")) {
            type = "file";
        } else {
            type = "folder";
        }
        $.ajax({
            type: "POST",
            url: "/Document/SetVisiblity",
            data: {"id": id, "type": type, "visiblity": visibility},
            dataType: 'JSON',
            success: function () {
                var returnInfo = "此文档可见度已修改为";
                _this.addClass("disabled");
                _this.siblings().removeClass("disabled");
                if (visibility === "0") {
                    returnInfo += "所有人可见";
                } else if (visibility === "1") {
                    returnInfo += "仅部门可见";
                } else if (visibility === "2") {
                    returnInfo += "仅自己可见";
                }
                showDialog(returnInfo, "ok");
                hideDialog(dialog);
            },
            error: function () {
                showDialog("发生未知错误，操作失败", "error");
                hideDialog(dialog);
            }
        });
    }
    e.stopPropagation();
});

/**
 * 列表和缩略图每项的下拉框鼠标移除事件
 */
$(".moreCon").on("mouseleave", function (e) {
    var e = e || window.event,
        $ele = $(".con_list li[data-id='" + $(this).data("id") + "']");
    $ele.removeClass("hover");
    if (!$ele.find(".chk-wrap input").attr("checked") && !$(e.target).is(".more")) {
        $ele.find(".chk-wrap").remove();
    }
});

/**
 * 批量删除文件
 */
$("#delFiles").on("click", function () {
    if ($(this).data("disabled")) {
        var chk = $(".con_list input[type=checkbox]:not([class=chk-all]):checked"),
            chk_length = chk.length;
        if (chk_length < 1) {
            showDialog("请先选中您要删除的文件（夹）", "warning");
        } else {
            var prom = confirm("确定要删除选中的文件（夹）？");
            if (prom) {
                var dialog = showDialog("正在获取要删除的文件（夹）", "info", "always");
                setTimeout(function() {
                    var allId = "",
                    types = "",
                    ids = [],
                    visibility = true;
                    chk.closest("li").each(function (i, e) {
                        if (allId.indexOf($(e).data("id")) === -1) {
                            if ($(e).data("visibility") && $(e).data("visibility") != "False") {
                                allId += ($(e).data("id") + "|");
                                if ($(e).find("i").hasClass("file-small-icon-folders")) {
                                    types += "folder|";
                                } else {
                                    types += "file|";
                                }
                            } else {
                                visibility = false;
                            }
                        }
                    });
                    if (!visibility) {
                        if (allId === "") {
                            showDialog("没有删除这些文件（夹）的权限！", "warning");
                            prom = false;
                        } else {
                            prom = confirm("您要删除的文件（夹）中包含至少一项没有操作权限，已自动过滤！\n\n 是否继续执行批量删除？");
                        }
                    }
                    if (prom) {
                        dialog = showDialog("正在删除中", "info", "always");
                        hideDialog(dialog);
                        $.ajax({
                            type: "POST",
                            url: "/Document/DeleteList",
                            data: { "ids": allId, "types": types },
                            dataType: "JSON",
                            success: function() {
                                ids = allId.split("|");
                                ids.pop();
                                types = types.split("|");
                                types.pop();
                                $.each(ids, function(i, e) {
                                    if (types[e] === "folder") {
                                        //删除目录树中对应的文件夹
                                        if (treeObj != null) {
                                            nodes = treeObj.getNodeByParam("xid", e, null);
                                            treeObj.removeNode(nodes);
                                        }
                                        //删除上传树状菜单中的文件夹
                                        if (uploadTree != null) {
                                            nodes = uploadTree.getNodeByParam("xid", e, null);
                                            uploadTree.removeNode(nodes);
                                        }
                                    }
                                });
                                //删除选中的文件
                                chk.closest("li").remove();
                                //最近页面 如果该时间段内没有文件，则删除该时间段的标题
                                var i = 0,
                                    $rec = $(".recently_name"),
                                    length = $rec.length,
                                    preN,
                                    curN,
                                    nodes;
                                for (i; i < length; i++) {
                                    curN = $rec.eq(i).index();
                                    if (typeof preN != "undefined" && preN + 1 === curN) {
                                        $rec.eq(i - 1).addClass("rn_remove");
                                    }
                                    preN = curN;
                                }
                                $(".rn_remove").remove();
                                showDialog("删除成功", "ok");
                                hideDialog(dialog);
                            },
                            error: function() {
                                showDialog("发生未知错误，删除文件（夹）失败", "error");
                                hideDialog(dialog);
                                $(".row-creating").remove();
                            }
                        });
                    } else {
                        hideDialog(dialog);
                    }
                }, 300);
            }
        }
    }
});

/**
 * 批量还原文件
 */
$("#redFiles").on("click", function () {
    var chk = $(".con_list input[type=checkbox]:not([class=chk-all]):checked"),
        chk_length = chk.length,
        types;
    if (chk_length < 1) {
        showDialog("请先选中您要还原的文件（夹）", "warning");
    } else {
        var dialog = showDialog("正在还原中", "info", "always");

        var allId = "",
            types = "";
        chk.closest("li").each(function (i, e) {
            if (allId.indexOf($(e).data("id")) === -1) {
                allId += ($(e).data("id") + "|");
                if ($(e).find("i").hasClass("file-small-icon-folders")) {
                    types += "folder|";
                } else {
                    types += "file|";
                }
            }
        });
        $.ajax({
            type: "POST",
            url: "/Document/RecoveryList",
            data: {"ids": allId, "types": types},
            dataType: "JSON",
            success: function () {
                chk.closest("li").remove();
                showDialog("文件（夹）已还原到原位置", "ok");
                hideDialog(dialog);
            },
            error: function () {
                showDialog("发生未知错误，还原文件（夹）失败", "error");
                hideDialog(dialog);
                $(".row-creating").remove();
            }
        });
    }
});

/**
 * 回收站 彻底删除选中文件
 */
$("#delSelected").on("click", function () {
    var $ele = $(".con_list input[type=checkbox]:not([class=chk-all]):checked"),
        len = $ele.length,
        types;
    if (len < 1) {
        showDialog("请先选中您要彻底删除的文件（夹）", "warning");
    } else {
        var prom = confirm("确定要彻底删除所有选中的文件（夹）吗？");
        if (prom) {
            var dialog = showDialog("正在删除选中文件中", "info", "always");
            var allId = "";
            types = "";
            $ele.closest("li").each(function (i, e) {
                if (allId.indexOf($(e).data("id")) === -1) {
                    allId += ($(e).data("id") + "|");
                    if ($(e).find("i").hasClass("file-small-icon-folders")) {
                        types += "folder|";
                    } else {
                        types += "file|";
                    }
                }
            });
            $.ajax({
                type: "POST",
                url: "/Document/Clear",
                data: {"ids": allId, "types": types},
                dataType: "JSON",
                success: function () {
                    $ele.closest("li").remove();
                    showDialog("删除选中文件（夹）成功", "ok");
                    hideDialog(dialog);
                },
                error: function () {
                    showDialog("发生未知错误，删除文件（夹）失败", "error");
                    hideDialog(dialog);
                    $(".row-creating").remove();
                }
            });
        }
    }
});

/**
 * 清空回收站
 */
$("#delAll").on("click", function () {
    var $ele = $(".con_list li:not(.file-list-title)"),
        len = $ele.length;

    if (len < 1) {
        showDialog("您的回收站内没有文件（夹）", "warning");
    } else {
        var prom = confirm("确定要清空回收站所有文件（夹）吗？");
        if (prom) {
            var dialog = showDialog("正在清空回收站", "info", "always");

            $.ajax({
                type: "POST",
                url: "/Document/Clear",
                dataType: "JSON",
                success: function (data) {
                    if (data) {
                        $(".con_list li:not(.file-list-title)").remove();
                        showDialog("回收站已清空", "ok");
                    } else {
                        showDialog("发生未知错误，清空文件（夹）失败", "error");
                        $(".row-creating").remove();
                    }
                    hideDialog(dialog);
                },
                error: function () {
                    showDialog("发生未知错误，清空文件（夹）失败", "error");
                    hideDialog(dialog);
                    $(".row-creating").remove();
                }
            });
        }
    }
});

/**
 * 新建文件夹
 */
$("#newFolders").on("click", function () {
    if ($(this).data("disabled")) {
        //禁用其他操作按钮
        $(".showstyle > span")
            .data("disabled", false)
            .css("opacity", "0.3");

        //在页面写入创建编辑框
        $(".file-list-title").after("<li class='row-fluid row-creating'>" +
            "<div class='span8'>" +
            "<input type='checkbox'/>" +
            "<i class='file-small-icon file-small-icon-folders'></i>" +
            "<span class='name'>" +
            "<input class='newText' type='text'/>" +
            "<img class='loading list-loading' src='/images/loading.gif' />" +
            "</span>" +
            "</div>" +
            "<div class='span2'></div>" +
            "<div class='span2'></div>" +
            "</li>");
        $(".file-pic").prepend("<li class='row-creating'>" +
            "<i class='file-icon file-icon-folders'></i>" +
            "<img class='loading pic-loading' src='/images/loading.gif' />" +
            "<p class='file-name'>" +
            "<input class='newText' type='text' />" +
            "</p>" +
            "</li>");

        $(".newText").focus();
        $(".newText").on("keydown", function (e) {
            var e = e || event;
            if (e.keyCode === 13) {
                $(this).blur();
            }
        });

        $(".newText").one("blur", function () {
            $(".newText").attr("readonly", true);
            $(".row-creating .loading").fadeIn(300);
            var reg = new RegExp('^[^\\\\\\/:*?\\"<>|]+$'),
                val = $(this).val();
            if (val === "") {
                showDialog("文件夹名称不能为空", "error");
                $(".row-creating").remove();
                setTimeout(function () {
                    $(".showstyle > span").data("disabled", true).css("opacity", "1");
                }, 300);
            } else if (!reg.test(val)) {
                showDialog("文件夹名称不能包含 / \\ : * ? < > | \" ", "error");
                $(".row-creating").remove();
                setTimeout(function () {
                    $(".showstyle > span").data("disabled", true).css("opacity", "1");
                }, 300);
            } else {
                var bool = true;
                $(".con_list li").each(function (i, e) {
                    if ($(e).find(".file-small-icon-folders").next().text() === val) {
                        bool = false;
                        return false;
                    }
                });
                if (bool) {
                    var newText = "";
                    var dialog = showDialog("正在创建文件夹", "info", "always");
                    $(".newText").each(function (i, e) {
                        if ($(e).val() !== "") {
                            newText = $(e).val();
                            return false;
                        }
                        return true;
                    });
                    $(".newText").val(newText);
                    var selectedNodes = treeObj.getSelectedNodes();
                    var spaceid;
                    if (typeof selectedNodes[0] == "undefined" || selectedNodes[0] === "") {
                        spaceid = folder_setting.treeRoot.xid;
                    } else {
                        spaceid = selectedNodes[0].xid;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Space/Add",
                        data: {
                            "UserId": $("#currentUserId").val(),
                            "UserName": $("#currentUserNickName").val(),
                            "SpaceName": $.trim(newText),
                            "ParentId": spaceid
                        },
                        dataType: "JSON",
                        success: function (data) {
                            $(".row-creating").remove();
                            if (data.SpaceName !== "") {
                                $(".file-list-title").after('<li class="row-fluid" data-id="' + data.SpaceObject.Id + '" data-folder="true" data-visibility="' + data.CanEdit + '">' +
                                    '<div class="span8">' +
                                    '<input type="checkbox"/> ' +
                                    '<i class="file-small-icon file-small-icon-folders"></i> ' +
                                    '<span class="name" data-url="/Home/Space/' + data.SpaceObject.Id + '/page" title="' + data.SpaceObject.SpaceName + '">' + data.SpaceObject.SpaceName + '</span>' +
                                    '</div>' +
                                    '<div class="span2"></div>' +
                                    '<div class="span2">' + getDate() + '</div>' +
                                    '</li>');
                                $(".file-pic").prepend('<li data-id="' + data.SpaceObject.Id + '" data-folder="true" data-visibility="' + data.CanEdit + '">' +
                                    '<i class="file-icon file-icon-folders"></i>' +
                                    '<p title="' + data.SpaceObject.SpaceName + '"><span data-url="/Home/Space/' + data.SpaceObject.Id + '/page">' + data.SpaceObject.SpaceName + '</span></p>' +
                                    '</li>');

                                //在树状目录中新增结点
                                var rootNode;
                                if (folder_setting.parentNode == undefined) {
                                    rootNode = folder_setting.treeRoot.xid;
                                } else {
                                    rootNode = folder_setting.parentNode.xid;
                                }
                                var root = treeObj.getNodeByParam("xid", rootNode, null);
                                var newNode = {name: val, xid: data.Id, iconSkin: "ind"};
                                treeObj.addNodes(root, newNode);

                                //在上传树目录中新增结点
                                root = uploadTree.getNodeByParam("xid", rootNode, null);
                                uploadTree.addNodes(root, newNode);

                                showDialog("创建文件夹成功", "ok");
                            } else {
                                showDialog("创建文件夹失败", "error");
                            }
                            hideDialog(dialog);
                            $(".showstyle > span").data("disabled", true).css("opacity", "1");
                        },
                        error: function () {
                            showDialog("发生未知错误，创建文件夹失败", "error");
                            hideDialog(dialog);
                            $(".row-creating").remove();
                            $(".showstyle > span").data("disabled", true).css("opacity", "1");
                        }
                    });
                } else {
                    showDialog("文件夹名称冲突", "error");
                    $(".row-creating").remove();
                    setTimeout(function () {
                        $(".showstyle > span").data("disabled", true).css("opacity", "1");
                    }, 300);
                }
            }
        });
    }
});

/**
 * 获取并处理系统时间
 * @returns {string}
 */
function getDate() {
    var date = new Date();
    var year = date.getFullYear() + "-",
        month = "" + (date.getMonth() + 1) + "-",
        day = "" + date.getDate();
    return year + dateLen(month, 3) + dateLen(day, 2);
}

/**
 * 时间字符串处理（例如，将2014-2-2处理为2014-02-02）
 * @param strTime 待处理的时间字符串
 * @param len 处理字符串的最低长度标准
 * @returns {*}
 */
function dateLen(strTime, len) {
    if (strTime.length < len) {
        return "0" + strTime;
    }
    return strTime;
}

/**
 * 类型分类选择
 */
$(".cla").on("click", "a", function () {
    $(this).siblings().removeClass("checked");
    $(this).addClass("checked");

    $("#documentType").val($(this).data("text"));

    $("#searchAgain").submit();
});

/**
 *  在当前页面预览图片或文件
 */
$(".content, .search_result_con").on("click", "li:not(.row-creating, .ztree li, .safe_con li, .addInfo li, .trash), .article", function () {
    var $this = $(this),
        $parent = $this.parent(),
        $ele = $this.find("i"),
        click_index = $this.index();
    if (ePagePosition.newPageX === ePagePosition.oldPageX && ePagePosition.newPageY === ePagePosition.oldPageY) {
        //图片预览
        if ($ele.is(".file-small-icon-JPG") ||
            $ele.is(".file-small-icon-JPEG") ||
            $ele.is(".file-small-icon-PNG") ||
            $ele.is(".file-small-icon-GIF") ||
            $ele.is(".file-small-icon-BMP") ||
            $ele.is(".file-small-icon-CAD") ||
            $ele.is(".file-icon-JPG") ||
            $ele.is(".file-icon-JPEG") ||
            $ele.is(".file-icon-PNG") ||
            $ele.is(".file-icon-GIF") ||
            $ele.is(".file-icon-BMP") ||
            $ele.is(".file-icon-CAD")) {
            if (!$this.data("loadFailed")) {
                //图片浏览模块初始化准备
                $(".picShow").show()
                    .append('<i class="picClose">×</i>' +
                    '<table class="pictures">' +
                    '<tr>' +
                    '<td class="prev" align="center"><i></i></td>' +
                    '<td class="mainShow" align="center"><div>' +
                    '<img class="m" src="Resource/images/bg.jpg"/>' +
                    '</div></td>' +
                    '<td class="next" align="center"><i></i></td>' +
                    '</tr>' +
                    '</table>' +
                    '<div class="showNav">' +
                    '<div class="hideNav"><i></i></div>' +
                    '<div class="title">' +
                    '第 <span id="picIndex">0</span>/<span id="picTotal">0</span> 张' +
                    '<a class="downloadPicM" href="javascript:void(0);">下 载</a>' +
                    '<a class="CADPreview" href="javascript:void(0);">查看高清图</a>' +
                    '</div>' +
                    '<div class="pic_min_len">' +
                    '<ul class="pic_min"></ul>' +
                    '</div>' +
                    '<span class="prev"><i></i></span>' +
                    '<span class="next"><i></i></span>' +
                    '</div>');
                $(".picClose").on("click", function () {
                    $(".picShow").html("").hide();
                });
                $(".showNav .hideNav").on("click", function () {
                    if ($(this).data("toggle")) {
                        $(".showNav").stop(true).animate({"bottom": "0"}, 300, function () {
                            $(".hideNav i").removeClass("top");
                            $(".showNav .hideNav").data("toggle", false);
                        });
                    } else {
                        $(".showNav").stop(true).animate({"bottom": "-110px"}, 300, function () {
                            $(".hideNav i").addClass("top");
                            $(".showNav .hideNav").data("toggle", true);
                        });
                    }
                });
                var showNav = "",
                    _src = $ele.find("img").data("displaypath"),
                    src,
                    name,
                    svg;
                $(".mainShow img.m").attr("src", _src);
                $parent.find("li, .article").each(function (i, e) {
                    src = $(e).find("img").attr("src");
                    _src = $(e).find("img").data("displaypath");
                    name = $(e).find(".name").text() || $(e).find("p span").text();

                    if ($(e).find("i").is(".file-small-icon-CAD") || $(e).find("i").is(".file-icon-CAD")) {
                        svg = $(e).find("img").data("svg");
                    } else {
                        svg = "";
                    }

                    if (src && src.match(/[^\s]+\.(jpg|jpeg|gif|png|bmp|dwg)/g) && src !== "../images/loadFailed.png") {
                        showNav += "<li data-dlu='" + $(e).find('span').data('url') + "'";
                        if (svg === "") {
                            if (click_index === i) {
                                showNav += ' class="checked"><img src="' + src + '" data-displaypath="' + _src + '" title="' + name + '"/></li>';
                            } else {
                                showNav += '><img src="' + src + '" data-displaypath="' + _src + '" title="' + name + '"/></li>';
                            }
                        } else {
                            if (click_index === i) {
                                showNav += ' class="checked"><img src="' + src + '" data-displaypath="' + _src + '" data-svg="' + svg + '" title="' + name + '"/></li>';
                            } else {
                                showNav += '><img src="' + src + '" data-displaypath="' + _src + '" data-svg="' + svg + '" title="' + name + '"/></li>';
                            }
                        }
                    }
                });

                //图片浏览模块初始化
                var conMarLeft = 0,
                    $picCon = $(".pic_min");
                $picCon.append(showNav);
                $picCon.find("li").append("<img class='loadingimg' src='../Resource/images/loading2.gif' title='正在加载中...'/>");
                var $pic = $picCon.find("li"),
                    curIndex = $picCon.find("li.checked").index(),
                    maxIndex = $pic.length,
                    picConWidth = maxIndex * 70,
                    picWidth = $pic.width() + parseInt($pic.css("margin-right")),
                    curPicMarLeft = curIndex * picWidth,
                    viewableArea = viewableAreaCalc();

                $("#picIndex").text(curIndex + 1);
                $("#picTotal").text(maxIndex);
                $picCon.width(maxIndex * 70);

                /**
                 * 图片浏览模块 主显示图片功能
                 */
                function setPic() {
                    $(".mainShow img").hide();
                    $(".mainShow div").append("<img class='mloading' src='../Resource/images/loading2.gif' />");
                    $("#picIndex").text(curIndex + 1);
                    $(".mainShow img.m")
                        .attr("src", $(".pic_min li").eq(curIndex).addClass("checked").find("img").data("displaypath"))
                        .load(function () {
                            var viewableArea = viewableAreaCalc();
                            $(".mainShow img.mloading").remove();
                            $(".mainShow img.m").show();
                            if ($(".mainShow img.m").width() >= viewableArea) {
                                $(".mainShow img.m").width(viewableArea);
                            } else {
                                $(".mainShow img.m").css(width, "auto");
                            }
                        }).error(function () {
                            $(".mainShow img.mloading").remove();
                            $(".mainShow img.m").show();
                        });

                    //是否显示CAD图预览链接
                    if (typeof $(".pic_min li").eq(curIndex).find("img").data("svg") != "undefined" && $(".pic_min li").eq(curIndex).find("img").data("svg") !== "") {
                        $(".CADPreview").attr("href", "/View/Index?Id=" + $this.data("id")).show();
                    } else {
                        $(".CADPreview").attr("href", "javascript:alert('该高清图不存在或已被删除');").hide();
                    }

                    //是否显示下载链接
                    if ($(".pic_min li.checked").data("dlu") === "") {
                        $(".downloadPicM").attr("href", "javascript:alert('该图片不存在或已被删除');");
                    } else {
                        if (typeof $(".pic_min li.checked").data("dlu") == "undefined") {
                            $(".downloadPicM").attr("href", "javascript:alert('该图片不存在或已被删除');");
                        } else {
                            $(".downloadPicM").attr("href", "javascript:window.open('" + $(".pic_min li.checked").data("dlu") + "');");
                        }
                    }
                }

                setPic();
                /**
                 * 当显示内容到达首尾时立即禁用相对应的按键并改变其为不可用状态
                 */
                function setMainClick() {
                    if ($(".pic_min li:first-child").is(".checked")) {
                        $(".pictures .prev i").css("background-position", "-50px 0");
                    } else if ($(".pic_min li:last-child").is(".checked")) {
                        $(".pictures .next i").css("background-position", "-125px 0");
                    }
                }

                $(".pictures .prev").hover(function () {
                    if (!$(".pic_min li:first-child").is(".checked")) {
                        $(".pictures .prev i").css("background-position", "-25px 0");
                    }
                }, function () {
                    $(".pictures .prev i").css("background-position", "-50px 0");
                });
                $(".pictures .prev").on("click", function () {
                    curIndex = $(".pic_min li[class=checked]").index();
                    if (curIndex > 0) {
                        $(".pic_min li").eq(curIndex).removeClass("checked");
                        curIndex--;
                        setPic();
                        setMainClick();
                        picFocusPosition(curIndex * picWidth);
                    }
                });
                $(".pictures .next").hover(function () {
                    if (!$(".pic_min li:last-child").is(".checked")) {
                        $(".pictures .next i").css("background-position", "-100px 0");
                    }
                }, function () {
                    $(".pictures .next i").css("background-position", "-125px 0");
                });
                $(".pictures .next").on("click", function () {
                    curIndex = $(".pic_min li[class=checked]").index();
                    if (curIndex < maxIndex - 1) {
                        $(".pic_min li").eq(curIndex).removeClass("checked");
                        curIndex++;
                        setPic();
                        setMainClick();
                        picFocusPosition(curIndex * picWidth);
                    }
                });

                //图片浏览模块 缩略图功能

                /**
                 * 计算缩略图可视区域宽度
                 * @returns {Number}
                 */
                function viewableAreaCalc() {
                    return parseInt($(window).width() * 0.88);
                }

                $pic.on("click", function () {
                    var _this = $(this),
                        index = _this.index();
                    if (index !== curIndex) {
                        $picCon.find("li").eq(curIndex).removeClass("checked");
                        _this.addClass("checked");
                        curIndex = index;
                        setPic();
                    }
                });
                $(".showNav .prev").on("click", function () {
                    picConWidth = maxIndex * 70,
                        conMarLeft = parseInt($picCon.css("left")),
                        viewableArea = viewableAreaCalc();
                    if (picConWidth >= viewableArea && conMarLeft < 0) {
                        $picCon.stop(true).animate({
                            left: conMarLeft += viewableArea
                        }, 400, function () {
                            if (conMarLeft >= 0) {
                                $picCon.stop(true).animate({
                                    left: 0
                                }, 100);
                                $(".showNav .prev i").css("background-position", "-150px 0");
                            }
                        })
                    }
                });
                $(".showNav .next").on("click", function () {
                    picConWidth = maxIndex * picWidth,
                        conMarLeft = parseInt($picCon.css("left")),
                        viewableArea = viewableAreaCalc();

                    if (picConWidth >= viewableArea && conMarLeft > (viewableArea - picConWidth)) {
                        $picCon.stop(true).animate({
                            left: conMarLeft -= viewableArea
                        }, 400, function () {
                            if (conMarLeft <= (viewableArea - picConWidth)) {
                                $picCon.stop(true).animate({
                                    left: viewableArea - picConWidth
                                }, 100);
                                $(".showNav .next i").css("background-position", "-163px -23px");
                            }
                        });
                    }
                });
                $(".showNav .prev").hover(function () {
                    if (picConWidth >= viewableArea && conMarLeft < 0) {
                        $(".showNav .prev i").css("background-position", "-150px -23px");
                    }
                }, function () {
                    $(".showNav .prev i").css("background-position", "-150px 0");
                });
                $(".showNav .next").hover(function () {
                    if (picConWidth >= viewableArea && conMarLeft > (viewableArea - picConWidth)) {
                        $(".showNav .next i").css("background-position", "-176px 0");
                    }
                }, function () {
                    $(".showNav .next i").css("background-position", "-163px -23px");
                });
                /**
                 * 设置当前显示图片的缩略图到可视区域内
                 * @param curPicMarLeft 当前显示图片的缩略图相对于缩略图容器最左边的距离
                 */
                function picFocusPosition(curPicMarLeft) {
                    if (Math.abs(conMarLeft) > curPicMarLeft) {
                        conMarLeft = -curPicMarLeft + viewableArea / 2;
                        $picCon.stop(true).animate({left: conMarLeft}, 400);
                    } else if (Math.abs(conMarLeft) + viewableArea < curPicMarLeft + picWidth) {
                        conMarLeft = -curPicMarLeft + viewableArea / 2;
                        $picCon.stop(true).animate({left: conMarLeft}, 400);
                    }

                    if (conMarLeft > 0){
                        conMarLeft = 0;
                        $picCon.stop(true).animate({left: conMarLeft}, 400);
                    } else if (picConWidth > viewableArea && picConWidth + conMarLeft < viewableArea) {
                        conMarLeft = -(picConWidth - viewableArea);
                        $picCon.stop(true).animate({left: conMarLeft}, 400);
                    }
                }

                picFocusPosition(curPicMarLeft);

                $(".pic_min img:not(.pic_min img.loadingimg)").load(function () {
                    $(this).siblings().remove();
                });

                $(".pic_min img:not(.pic_min img.loadingimg)").error(function () {
                    $(this).siblings().remove();
                    $(this)
                        .css({width: "32px", height: "32px", margin: "14px"})
                        .attr("src", "../images/loadFailed.png")
                        .attr("title", "加载失败")
                        .data("displaypath", "../images/loadFailed.png");
                    var _this = $(this).closest("li");
                    _this.stop(true).animate({width: 0, height: 0, opacity: 0}, 100, function () {
                        if (_this.is(".checked")) {
                            curIndex = 0;
                            $("#picIndex").text(curIndex);
                            $(".mainShow img.m").attr("src", "../images/loadFailed.png");
                            $(".CADPreview").attr("href", "javascript:alert('该高清图不存在或已被删除');").hide();
                            $(".downloadPicM").attr("href", "javascript:alert('该高清图不存在或已被删除');");
                        } else {
                            if (_this.index() < curIndex) {
                                $("#picIndex").text(curIndex--);
                            }
                        }
                        $("#picTotal").text(--maxIndex);
                        picConWidth = maxIndex * picWidth;
                        $picCon.width(picConWidth);
                        _this.remove();
                        picFocusPosition(curPicMarLeft);
                    });
                });
            } else {
                showDialog("该高清图不存在或已被删除", "error");
            }
        } else if ($this.is(".article") && ($ele.is(".file-small-icon-folders") || $ele.is(".file-icon-folders"))) {
            //打开搜索界面文件夹
            window.location = "/Home/Space/" + $this.data("id") + "/page";
        } else {
            //文件预览
            if (!$ele.is(".file-small-icon-folders")
                && !$ele.is(".file-icon-folders")
                && !$this.is(".file-list-title")
                && !$this.is(".recently_name")
                && !$ele.is(".file-small-icon-DAMAGE")
                && !$ele.is(".file-icon-DAMAGE")) {

                //文档跳页面浏览 2015-11-27前版本
                //window.location = "/View/Index?Id=" + $this.data("id");

                //文档浏览模块初始化准备 2015-11-27后版本
                $(".picShow").show()
                    .append('<i class="picClose">×</i>' +
                    '<div class="fileCon">' +
                    '<div class="fileTitleCon">' +
                    '<p class="fileTitle">' + $this.find("span").text() + '</p>' +
                    '<a class="fileDownload" title="下载" href="javascript:window.open(\'' + $this.find("span").data("url") + '\')">下 载</a>' +
                    '</div>' +
                    '<div class="fileView">' +
                    '</div>' +
                    '</div>');
                $(".fileView").height($(window).height() - $(".fileView").position().top);
                $(".picClose").on("click", function () {
                    $(".picShow").html("").hide();
                });

                //文档浏览模块初始化
                $.ajax({
                    type: "GET",
                    url: "/View/Preview",
                    data: "Id=" + $this.data("id"),
                    success: function (data) {
                        $(".fileView").append(data);
                    },
                    error: function () {
                        alert("打开失败");
                        $(".picShow").html("").hide();
                    }
                });
            }
        }
    }
});

/**
 * 目录树点击事件
 * @param e
 * @param treeId 目录数DOM容器ID
 * @param node 所点击的结点
 */
function onNodeClick(e, treeId, node) {
    if (treeId === "tree") {
        //左侧目录树
        folder_setting.parentNode = node;
        folder_setting.parentNode.xid = node.xid;
        location.href = "/Home/Space/" + node.xid + "/page";
    } else {
        //上传目录树
        $("#uploadSpaceId").val(node.xid);
        //$("#directory").val(getTreeDirStr(treeId, node));
    }
}

function getTreeDirStr(treeId, node) {
    var directory = "",
        pNode = [];
    do {
        pNode.push(node.name);
        node = node.getParentNode();
    } while (node != null);
    pNode.reverse();
    directory = pNode.join(" \\ ");
    return directory;
}

/**
 * 左侧主菜单
 */
$(".sys_toggle").on("click", function () {
    $(this).next(".sys_options").slideToggle(300);
});

/**
 * 添加一个cookie
 * @param name cookie名称
 * @param value cookie值
 * @param expiresHours 过期时间（单位：小时），为0时不设定过期时间，即当浏览器关闭时cookie自动消失。
 */
function addCookie(name, value, expiresHours) {
    var cookieString = name + "=" + value + "; path=/";
    //判断是否设置过期时间
    if (expiresHours > 0) {
        var date = new Date();
        date.setTime(date.getTime + expiresHours * 3600 * 1000);
        cookieString += "; expires=" + date.toGMTString();
    }
    document.cookie = cookieString;
}

/**
 * 获取指定名称的cookie值
 * @param name
 * @returns {*}
 */
function getCookie(name) {
    var strCookie = document.cookie;
    var arrCookie = strCookie.split("; ");
    for (var i = 0; i < arrCookie.length; i++) {
        var arr = arrCookie[i].split("=");
        if (arr[0] === name) return arr[1];
    }
    return "";
}