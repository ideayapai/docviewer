$.extend({
    highlight:function(a, j, g, f) {
    if (a.nodeType === 3) {
        var d = a.data.match(j);
        if (d && d[0].length) {
            var b = document.createElement(g || "span");
            b.className = f || "highlight";
            var h = a.splitText(d.index);
            h.splitText(d[0].length);
            var e = h.cloneNode(true);
            b.appendChild(e);
            h.parentNode.replaceChild(b, h);
            return 1
        }
    } else {
        if ((a.nodeType === 1 && a.childNodes) && !/(script|style)/i.test(a.tagName) && !(a.tagName === g.toUpperCase() && a.className === f)) {
            for (var c = 0; c < a.childNodes.length; c++) {
                c += $.highlight(a.childNodes[c], j, g, f)
            }
        }
    }
    return 0
}});
$.fn.unhighlight = function(a) {
    var b = {className:"keyword",element:"span"};
    $.extend(b, a);
    return this.find(b.element + "." + b.className).each(
        function() {
            var c = this.parentNode;
            c.replaceChild(this.firstChild, this);
            c.normalize()
        }).end()
};
$.fn.highlight = function(f, b) {
    var d = {className:"keyword",element:"span",caseSensitive:false,wordsOnly:false};
    $.extend(d, b);
    if (f.constructor === String) {
        f = [f]
    }
    f = $.grep(f, function(h, g) {
        return h != ""
    });

    if (f.length == 0) {
        return this
    }
    var a = d.caseSensitive ? "" : "i";
    var ea = [];
    $.each(f, function(i, ed) {
        ed = decodeURIComponent(ed);
        var eed = ed.replace(/([!~@`<>~！，。,.；;’'\[\]\/\\\?])/g, "\\$1");
        if (eed) {
            ea.push(eed);
        }
    });
    var e = "(" + ea.join("|") + ")";
    if (d.wordsOnly) {
        e = "\\b" + e + "\\b"
    }
    var c = new RegExp(e, a);
    return this.each(function() {
        $.highlight(this, c, d.element, d.className)
    })
};
