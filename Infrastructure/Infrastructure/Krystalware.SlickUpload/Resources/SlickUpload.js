(function ()
{
    // --- Component registry
    var kw = window.kw || function (value)
    {
        if (typeof value == "string")
        {
            return kw._components[value];
        }
        else if (typeof value == "function")
        {
            if (kw._hasWindowLoaded)
                value();
            else
                kw._initFunctionList.push(value);
        }
        else if (value && value.isLicensed != null)
        {
            handleLicense(value);
        }
        else
        {
            throw new Error("Invalid value argument type. Must be a string (to get a component) or a function (to call on load).");
        }
    };

    kw._registerInit = function (initFunction)
    {
        if (typeof initFunction == "function")
        {
            if (kw._hasWindowLoaded)
                initFunction();
            else
                kw._initFunctionList.splice(0, 0, initFunction);
        }
        else
        {
            throw new Error("Invalid value argument type. Must be a function.");
        }
    };

    // --- Globals
    kw.debug = false;
    kw.log = [];
    kw.createLog = false;
    kw.verboseLog = false;
    kw.dropZoneWindow = window;

    kw._components = kw._components || {};
    kw._hasWindowLoaded = kw._hasWindowLoaded || false;
    kw._initFunctionList = kw._initFunctionList || [];
    kw._frameLoadedHandlers = kw._frameLoadedHandlers || [];
    kw._completionActions = kw._completionActions || [];
    kw._licenseHandlerUrl = null;

    // TODO: make case insensitive
    kw.UploadState =
	{
	    Initializing: "Initializing",
	    Uploading: "Uploading",
	    Completing: "Completing",
	    Complete: "Complete",
	    Error: "Error"
	};

    kw.UploadErrorType =
	{
	    None: "None",
	    Cancelled: "Cancelled",
	    Other: "Other"
	};

    // --- Formatters
    kw.defaultFileSizeFormatter = function (bytes)
    {
        if (bytes != null && isFinite(bytes) && bytes >= 0)
        {
            var suffix = ["bytes", "KB", "MB", "GB", "TB"];

            for (var i = 0; i < suffix.length && bytes / 1024 > 1; i++)
                bytes /= 1024;

            var decimalCount = Math.max(0, 3 - Math.round(bytes).toString().length);

            return bytes.toFixed(decimalCount) + " " + suffix[i];
        }
        else if (bytes == -1)
        {
            return "(unknown size)";
        }
        else
        {
            return null; // "(calculating)";
        }
    };

    kw.defaultPercentFormatter = function (percent)
    {
        if (percent != null)
        {
            var numericPercent = parseFloat(percent);

            if (isFinite(numericPercent))
                return Math.min(Math.max(numericPercent, "0"), "100").toFixed(2) + " %";
        }

        return null; // "(calculating)";
    };

    kw.defaultTimeFormatter = function (seconds)
    {
        seconds = parseFloat(seconds);

        if (isFinite(seconds) && seconds > 0)
        {
            var hours = Math.floor(seconds / (60 * 60));

            seconds -= hours * (60 * 60);

            var minutes = Math.floor(seconds / 60);

            seconds = Math.floor(seconds - minutes * 60);

            var value = "";

            if (hours > 0)
            {
                value += hours;

                if (hours == 1)
                    value += " hour";
                else
                    value += " hours";
            }

            if (minutes > 0)
            {
                if (value.length > 0)
                {
                    if (seconds > 0)
                        value += ", ";
                    else
                        value += " and ";
                }

                value += minutes;

                if (minutes == 1)
                    value += " minute";
                else
                    value += " minutes";
            }

            if (seconds > 0)
            {
                if (value.length > 0)
                    value += " and ";

                value += seconds;

                if (seconds == 1)
                    value += " second";
                else
                    value += " seconds";
            }

            return value;
        }
        else if (seconds == 0)
        {
            return "an instant";
        }

        return null; // "(calculating)";
    };

    kw.defaultFileValidationMessageFormatter = function (file, fileList)
    {
        var message = "";

        if (!file.get_IsExtensionValid() && fileList.get_InvalidExtensionMessage())
        {
            if (message.length > 0)
                message += " ";

            message += fileList.get_InvalidExtensionMessage();
        }
        if (!file.get_IsSizeValid() && fileList.get_InvalidFileSizeMessage())
        {
            if (message.length > 0)
                message += " ";

            message += fileList.get_InvalidFileSizeMessage();
        }

        if (message.length > 0)
            return message;
        else
            return null;
    };

    /*kw.defaultFileTypeFormatter = function (type)
    {
    if (type != null)
    {
    return type;
    }
    else
    {
    return "(unknown)";
    }
    };*/

    // --- Frame trampoline
    kw._uploadFrameLoaded = function (data)
    {
        // setTimeout required for webkit
        window.setTimeout(function ()
        {
            for (var i = 0; i < kw._frameLoadedHandlers.length; i++)
                kw._frameLoadedHandlers[i](data);
        }, 1);
    };

    // --- Generic component constructor
    var componentConstructor = function (component, settings, options, events, isElementRequired, elementClass)
    {
        if (settings && options)
            extend(settings, options);

        //component.uniqueId = settings.uniqueId = Math.floor(Math.random() * 1000);

        assert(!settings.id || typeof settings.id == "string", "id must be a string.");

        if (isDOMElement(settings.element) && !settings.id)
        {
            if (settings.element.id && typeof settings.element.id == "string" && settings.element.id.length > 0)
                settings.id = settings.element.id;
        }
        else if (!settings.element && settings.id)
        {
            var el = document.getElementById(settings.id);

            if (isElementRequired)
                assert(isDOMElement(el), "id '" + settings.id + "' must resolve to a valid element.");

            if (el)
                settings.element = el;
        }
        else if (isElementRequired)
        {
            assert(isDOMElement(el), "element is required.");
        }

        if (settings.element && elementClass)
            addClass(settings.element, elementClass);

        if (settings)
        {
            addGetters(component, settings);

            if (settings.id)
            {
                var oldComponent = kw._components[settings.id];

                if (oldComponent && oldComponent._dispose)
                    oldComponent._dispose();

                kw._components[settings.id] = component;
            }
        }

        if (events)
        {
            addEventAdders(component, events);

            if (settings)
                connectEvents(events, settings);
        }
    };

    var extend = function (root)
    {
        for (var i = 0; i < arguments.length; i++)
        {
            var arg = arguments[i];

            if (arg)
            {
                for (var key in arg)
                    root[key] = arg[key];
            }
        }

        return root;
    };

    var addGetters = function (object, settings)
    {
        for (var key in settings)
        {
            var name = key.substr(0, 1).toUpperCase() + key.substr(1);

            // TODO: figure out a better way of doing this
            if (name != "Data")
            {
                (function (key, name)
                {
                    object["get_" + name] = function () { return settings[key] };
                })(key, name);
            }
            else
            {
                (function (key, name)
                {
                    object["get_" + name] = function (dataKey) { return settings[key][dataKey] };
                })(key, name);
            }
        }
    };

    var addEventAdders = function (object, events)
    {
        for (var key in events)
        {
            var name = key.substr(0, 1).toUpperCase() + key.substr(1, key.length - 9);

            (function (key, name)
            {
                object["add_On" + name] = function (handler, isInternal)
                {
                    handler.isInternal = isInternal;

                    if (isInternal)
                        events[key].splice(0, 0, handler);
                    else
                        events[key].push(handler);
                };
            })(key, name);
        }
    };

    var mergeComponent = function (dest, source, filter)
    {
        for (var key in source)
        {
            if (typeof source[key] == "function")
            {
                if (!dest[key] && filter && filter(key))
                    dest[key] = source[key];
            }
        }
    };

    var connectEvents = function (events, settings)
    {
        for (var key in settings)
        {
            if (key.length > 4)
            {
                var name = key.substr(2, 1).toLowerCase() + key.substr(3) + "Handlers";
                var event = events[name];

                if (event)
                {
                    var eventFunction = settings[key];

                    if (typeof eventFunction == "string")
                    {
                        if (window[eventFunction] && typeof window[eventFunction] == "function")
                        {
                            eventFunction = window[eventFunction];
                        }
                        else
                        {
                            // TODO: maybe throw exception instead?
                            eventFunction = (function (eventFunction)
                            {
                                return function (data) { return eval(eventFunction) };
                            })(eventFunction);
                        }
                    }

                    event.push(eventFunction);
                }
            }
        }
    };

    var resolveComponent = function (settings, name, type, typeName, isRequired)
    {
        var message = null;

        if (typeof settings[name] == "string")
        {
            var el = kw(settings[name]);

            if (el)
                settings[name] = el;
            else
                message = name + " '" + settings[name] + "' must resolve to a valid " + typeName + ".";
        }

        if (!message)
            message = name + " must be a " + typeName + ".";

        if (settings[name] || isRequired)
            assert(settings[name] instanceof type, message);
    };

    var resolveElement = function (settings, name, isRequired)
    {
        var message = null;

        if (typeof settings[name] == "string")
        {
            var el = document.getElementById(settings[name]);

            if (isDOMElement(el))
                settings[name] = el;
            else
                message = name + " '" + settings[name] + "' must resolve to an element.";
        }

        if (!message)
            message = name + " must be an element.";

        if (settings[name] || isRequired)
            assert(isDOMElement(settings[name]), message);
    };

    var getXmlReq = (function ()
    {
        if (window.XMLHttpRequest)
        {
            return function () { return new XMLHttpRequest(); };
        }
        else
        {
            var progIds = ["Msxml2.XMLHTTP.6.0", "MSXML2.XMLHTTP.3.0", "MSXML2.XMLHTTP", "Microsoft.XMLHTTP"];

            for (var i = 0; i < progIds.length; i++)
            {
                try
                {
                    var constructor = function () { return new ActiveXObject(progIds[i]); };

                    if (constructor())
                        return constructor;
                }
                catch (ex)
                {
                    // Eat it and try the next one
                }
            }

            return function () { return null; }
        }
    })();

    var existingBrands = [];
    var licenseData = null;

    // TODO: periodically check to ensure that brand still exists
    var handleLicense = function (data)
    {
        data = data || licenseData;

        for (var i = 0; i < existingBrands.length; i++)
        {
            var brand = existingBrands[i];

            if (brand.parentNode)
                brand.parentNode.removeChild(brand);
        }

        existingBrands = [];

        if (!data || !data.isLicensed)
        {
            licenseData = data;

            var brandLocation = data ? data.brandLocation : null;
            var version = data ? data.version : null;

            if (!version)
                version = "unknown";

            if (!brandLocation || brandLocation == "inline")
            {
                for (var key in kw._components)
                {
                    var component = kw._components[key];

                    if (component.constructor == kw.FileSelector)
                    {
                        var brand = document.createElement("div");

                        brand.style.cssText = "background-image:url('http://krystalware.com/brandping?version=" + version + "');clear:both;";

                        //brand.style.cssText = "background-image:url('http://krystalware.com/brandping?version=" + version + "');z-index:99999;background-color:#265ecf;border-top:1px solid #37b0e5;border-right:1px solid #37b0e5;position:absolute;position:fixed;right:0;bottom:0;margin:0;padding:.25em .5em .25em .5em;font-size:80%;font-family:Calibri,Verdana,Arial,sans-serif;filter:alpha(opacity=100);opacity:1;-moz-opacity:1";

                        var brandHTML =

						brandHTML = "<a href=\"http://krystalware.com/slickupload\" title=\"Powered By SlickUpload Community Edition\" style=\"text-decoration:none\" target=\"_top\" onmouseover=\"this.style.textDecoration='underline'\" onmouseout=\"this.style.textDecoration='none'\">";

                        if (data && data.brandUrl)
                            brandHTML += "<img src=\"" + data.brandUrl + "\" width=\"154\" height=\"28\" alt=\"Powered By SlickUpload Community Edition\" style=\"padding:1em;border:0\">";
                        else
                            brandHTML += "Powered By SlickUpload Community Edition";

                        brandHTML += "</a>";

                        brand.innerHTML = brandHTML;

                        var element;
                        
                        if (support.folderSelection)
                            element = component.get_FolderElement();

                        if (!element || !element.parentNode)
                            element = component.get_Element();

                        var parent = element.parentNode;

                        if (parent)
                        {
                            if (parent.lastChild == element)
                                parent.appendChild(element);
                            else
                                parent.insertBefore(brand, element.nextSibling);

                            existingBrands.push(brand);
                        }
                        /*while (element && !element.nextSibling)
                        element = element.parentNode;

                        if (element && element.nextSibling)
                        element.parentNode.insertBefore(brand, element.nextSibling);
                        else
                        document.body.appendChild(brand);*/
                    }
                }
            }
            else if (brandLocation == "bottom-right")
            {
                var brand = document.createElement("div");

                brand.style.cssText = "background-image:url('http://krystalware.com/brandping?version=" + version + "');z-index:99999;background-color:#265ecf;border-top:1px solid #37b0e5;border-right:1px solid #37b0e5;position:absolute;position:fixed;right:0;bottom:0;margin:0;padding:.25em .5em .25em .5em;font-size:80%;font-family:Calibri,Verdana,Arial,sans-serif;filter:alpha(opacity=100);opacity:1;-moz-opacity:1";

                brand.innerHTML = "<a href=\"http://krystalware.com/slickupload\" style=\"color: rgb(255, 255, 255);text-decoration:none\" target=\"_top\" onmouseover=\"this.style.textDecoration='underline'\" onmouseout=\"this.style.textDecoration='none'\">Powered By SlickUpload Community Edition</a>";

                document.body.appendChild(brand);

                existingBrands.push(brand);
            }
        }
    };

    // https://github.com/kangax/iseventsupported/blob/master/isEventSupported.js
    var isMouseEventSupported = function (eventName)
    {
        var el = document.createElement('div');
        eventName = 'on' + eventName;
        var isSupported = (eventName in el);
        if (!isSupported)
        {
            el.setAttribute(eventName, 'return;');
            isSupported = typeof el[eventName] == 'function';
        }
        el = null;
        return isSupported;
    };

    var support = (function ()
    {
        var ua = window.navigator.userAgent.toLowerCase();

        // From $.browser
        var browserMatch = /(webkit)[ \/]([\w.]+)/.exec(ua) ||
			/(opera)(?:.*version)?[ \/]([\w.]+)/.exec(ua) ||
			/(msie) ([\w.]+)/.exec(ua) ||
			!/compatible/.test(ua) && /(mozilla)(?:.*? rv:([\w.]+))?/.exec(ua) ||
			[];

        var browser = browserMatch[1] || "",
			browserVersion = parseFloat(browserMatch[2]) || 0,
			browserMinorVersion = 0;

        var parts = browserMatch[2] ? browserMatch[2].split(".") : [];

        if (parts.length > 2)
            browserMinorVersion = parseFloat(parts.slice(2).join(".")) || 0;

        var androidMatch = /(android)[ ]([\w.]+)/.exec(ua) || [];
        var androidVersion = androidMatch[1] ? parseFloat(androidMatch[2]) : null;

        var xhr = getXmlReq();

        // http://leaverou.me/2009/03/regarding-native-single-input-multiple-file-uploads/
        var fileInput = document.createElement("input");

        fileInput.type = 'file';

        // TODO: create iframe and see if we can target it
        var isSupportedBrowser = ("src" in document.createElement("iframe") && !fileInput.disabled && !(browser == "webkit" && browserVersion < 523) && (!androidVersion || androidVersion >= 2.2));

        var isSkinnable = (
			(browser == "msie" && browserVersion > 5) ||
			(browser == "opera" && browserVersion >= 9) ||
			(browser == "mozilla" && browserVersion > 1.7) ||
			(browser == "webkit" && browserVersion > 523)
		);

        if (browser == "msie" && browserVersion < 9)
        {
            // Check to make sure that "binary and script behaviors" is enabled in IE. That is required for opacity filter to work.
            try
            {
                fileInput.style.display = "none";

                document.appendChild(fileInput);

                var test = fileInput.filters;

                document.removeChild(fileInput);
            }
            catch (ex)
            {
                isSkinnable = false;
            }
        }

        // TODO: support dnd in safari
        var webkitNotChrome = (browser == "webkit" && ua.indexOf("chrome") == -1);

        var supportMatrix =
		{
		    browser: browser,
		    browserVersion: browserVersion,
		    browserMinorVersion: browserMinorVersion,
		    //dragDrop: ("draggable" in document.createElement("span")) && ("multiple" in fileInput) && ("files" in fileInput),
		    isSupportedBrowser: isSupportedBrowser,
		    isSkinnable: isSkinnable,
		    // TODO: figure out why multiple is broken in Safari 5.1
		    fileInputMultiple: ("multiple" in fileInput) && !(browser == "webkit" && window.navigator.platform.toLowerCase().indexOf("win") != -1 && browserVersion == 534.5),
		    folderSelection: ("webkitdirectory" in fileInput),
		    fileInputTabbable: ((browser == "mozilla" && browserVersion > 1.8) || (browser == "webkit" && browserVersion > 523)),
		    mouseEnterLeave: (isMouseEventSupported("mouseenter") && isMouseEventSupported("mouseleave")),
		    // TODO: implement full iframe progress for this
		    xmlHttpRequest: (xhr != null),
		    // TODO: see if newer versions work
		    xmlHttpRequestInSubmit: (browser != "webkit"),
		    html5Upload: ((window.FormData || (browser == "mozilla" && browserVersion >= 1.9 && browserMinorVersion >= 2)) ? true : false),
            cors: (xhr != null && "withCredentials" in xhr) || typeof XDomainRequest !== "undefined"
		};

        supportMatrix.dragDrop = supportMatrix.fileInputMultiple && ("draggable" in document.createElement("span")) && !webkitNotChrome;

        return supportMatrix;
    })();

    // TODO: is this the best way?
    kw.support = kw.support || {};

    extend(kw.support, support);

    // --- Helper functions
    // debug
    var log = function ()
    {
        if (kw.debug && window.console && console.log)
        {
            if (console.dir && arguments.length > 1)
            {
                console.log(arguments);
            }
            else
            {
                for (var i = 0; i < arguments.length; i++)
                    console.log(arguments[i]);
            }
        }

        if (kw.createLog)
        {
            var argArray = [];

            for (var i = 0; i < arguments.length; i++)
                argArray.push(arguments[i]);


            argArray.push(new Date().toString());

            kw.log.push(argArray);
        }
    };

    var assert = function (test, message)
    {
        if (!test)
            throw new Error(message);
    };

    // event binding
    var bind = function (source, event, ctx, handler, captureEvents, forceDirectWire)
    {
        var wrapper = function (e) { var x = handler.call(ctx, e, source); return x; };

        if (!forceDirectWire)
        {
            try
            {
                if (source.addEventListener)
                {
                    source.addEventListener(event, wrapper, captureEvents);

                    return wrapper;
                }
                else if (source.attachEvent)
                {
                    source.attachEvent("on" + event, wrapper);

                    return wrapper;
                }
            }
            catch (ex)
			{ }
        }

        var original;

        try
        {
            original = source["on" + event];
        }
        catch (ex)
		{ }

        // TODO: handle returns
        // TODO: handle unwiring
        if (original && !forceDirectWire)
            wrapper = function (e) { original(e); wrapper(e, source); };

        source["on" + event] = wrapper;
    };

    var unbind = function (source, event, handler, forceDirectWire)
    {
        try
        {
            if (forceDirectWire)
                source["on" + event] = null;
            if (source.removeEventListener)
                source.removeEventListener(event, handler, false);
            else if (source.detachEvent)
                source.detachEvent("on" + event, handler);

            return true;
            // TODO: handle old browsers wired directly
        }
        catch (e)
        {
            return false;
        }
    };

    var getComputedStyle = function (el, style)
    {
        if (!el || !el.style)
            return null;

        if (el.style[style])
            return el.style[style];
        if (el.currentStyle)
            return el.currentStyle[style];
        else if (document.defaultView && document.defaultView.getComputedStyle)
        {
            var styles = document.defaultView.getComputedStyle(el, null);

            if (styles)
                return styles[style];
        }
        else
            return null;
    };

    var showElement = function (el)
    {
        if (el && el.style)
        {
            var currentDisplay = getComputedStyle(el, "display");

            if (!currentDisplay || currentDisplay == "none")
                el.style.display = "block";
        }
    };


    // Checks if an event happened on an element within another element
    // Used in jQuery.event.special.mouseenter and mouseleave handlers
    // http://ecmascript.stchur.com/2007/03/15/mouseenter-and-mouseleave-events-for-firefox-and-other-non-ie-browsers/
    var withinElement = function (parent, event, relatedTarget)
    {
        // Check if mouse(over|out) are still within the same parent element
        if (!relatedTarget)
            relatedTarget = event.relatedTarget;

        // Firefox sometimes assigns relatedTarget a XUL element
        // which we cannot access the parentNode property of
        try
        {
            if (parent === relatedTarget) { return false; }

            while (relatedTarget && relatedTarget !== parent)
            {
                relatedTarget = relatedTarget.parentNode;
            }

            return relatedTarget === parent;

            // assuming we've left the element since we most likely mousedover a xul element
        } catch (e) { }

        return false;
    };

    // http://www.geekdaily.net/2007/07/04/javascript-cross-browser-window-size-and-centering/
    var windowSize = function ()
    {
        var sizeWindow = kw.dropZoneWindow;

        var w = 0;
        var h = 0;

        //IE
        if (!sizeWindow.innerWidth)
        {
            var sizeDoc = sizeWindow.document;

            //strict mode
            if (!(sizeDoc.documentElement.clientWidth == 0))
            {
                w = sizeDoc.documentElement.clientWidth;
                h = sizeDoc.documentElement.clientHeight;
            }
            //quirks mode
            else
            {
                w = sizeDoc.body.clientWidth;
                h = sizeDoc.body.clientHeight;
            }
        }
        //w3c
        else
        {
            w = sizeWindow.innerWidth;
            h = sizeWindow.innerHeight;
        }

        return { width: w, height: h };
    };

    var objectSize = function (obj)
    {
        var size = 0;
        var key;

        for (key in obj)
        {
            //if (obj.hasOwnProperty(key))
            size++;
        }

        return size;
    };

    var isDOMElement = function (el)
    {
        return el && el.nodeType == 1 && el.tagName;
    };

    var splitClasses = function (classes)
    {
        if (classes)
            return classes.split(/\s+/);
        else
            return [];
    };

    var addClass = function (el, className)
    {
        var classes = splitClasses(el.className);

        if (indexOf(classes, className) == -1)
            classes.push(className);

        el.className = classes.join(" ");
    };

    var removeClass = function (el, className)
    {
        var classes = splitClasses(el.className);

        for (var i = classes.length; i--; i >= 0)
        {
            if (classes[i] == className)
                classes.splice(i, 1);
        }

        el.className = classes.join(" ");
    };

    var indexOf = function (arr, item)
    {
        for (var i = 0; i < arr.length; i++)
            if (arr[i] == item)
                return i;

        return -1;
    };

    var trim = function (value)
    {
        if (value)
            return value.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
        else
            return null;
    };

    var queryStringSerialize = function (o)
    {
        var s = [];

        for (var k in o)
            s.push(k + "=" + encodeURIComponent(o[k]));

        return s.join("&");
    };

    var hexChars = "0123456789ABCDEF".split("");

    var generateGuid = function ()
    {
        var uuid = [];
        // rfc4122, version 4 form
        var r;

        // rfc4122 requires these characters
        uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
        uuid[14] = '4';

        // Fill in random data.  At i==19 set the high bits of clock sequence as
        // per rfc4122, sec. 4.1.5
        for (i = 0; i < 36; i++)
        {
            if (!uuid[i])
            {
                r = 0 | Math.random() * 16;
                uuid[i] = hexChars[(i == 19) ? (r & 0x3) | 0x8 : r];
            }
        }
        return uuid.join('');
    }

    var filter = function (list, filterFunction)
    {
        var filteredList = [];

        if (list && list.length > 0)
            for (var i = 0; i < list.length; i++)
                if (filterFunction(list[i]))
                    filteredList.push(list[i]);

        return filteredList;
    };

    var callHandlers = function (list)
    {
        if (list && list.length > 0)
        {
            var args = [];

            for (var i = 1; i < arguments.length; i++)
                args.push(arguments[i]);

            // TODO: use call so we set this
            for (var i = 0; i < list.length; i++)
            {
                if (list[i].apply(null, args) == false)
                    return false;
            }
        }

        return true;
    };

    var addIFrame = function (id, src)
    {
        var iframe = null;

        if (!src)
            src = "javascript:false;";
            //src = "javascript:\"\";";
            //src = getUrl("blank");

        // TODO: if couldn't create either, fall back to non-javascript progress mode

        var addIFrameInternal = function (id, el)
        {
            var iframe = document.createElement(el);

            iframe.name = iframe.id = id;
            iframe.src = src;
            // TODO: fix this if it doesn't work crossbrowser (old browsers may need 1x1px or 0x0px)
            iframe.style.display = "none";

            document.body.appendChild(iframe);

            return iframe;
        };

        // try the IE way first
        if (support.browser == "msie")
        {
            try
            {
                iframe = addIFrameInternal(id, "<iframe name=\"" + id + "\" />");

                if (!iframe || iframe.tagName.toLowerCase() != "iframe" || iframe.name != id || window.frames[id] == undefined || window.frames[id].name != id)
                {
                    if (iframe)
                        document.body.removeChild(iframe);

                    iframe = null;
                }
            }
            catch (ex)
            {
                iframe = null;
            }
        }

        // standard cross browser way
        if (iframe == null)
            iframe = addIFrameInternal(id, "iframe");

        //alert(iframe);
        //alert(iframe.src);
        //alert(window.frames[id].name);

        return iframe;
    };

    var isIFrameError = function (id, urlRoot)
    {
        var isError = false;
        var frameLocation = null;

        try
        {
            frameLocation = window.frames[id].location.pathname + window.frames[id].location.search;

            if (frameLocation != null && frameLocation.length > 0 && frameLocation != "false;")
                isError = (frameLocation.length < urlRoot.length || frameLocation.substr(0, urlRoot.length) != urlRoot);
        }
        catch (e)
        {
            isError = true;
        }

        if (!frameLocation)
            isError = true;

        //alert("error:" + isError);
        //alert(frameLocation);

        return isError;
    };

    var getIsLoaded = function (iframe)
    {
        var isLoaded = false;
        var frameLocation = window.frames[iframe.name].location.pathname + window.frames[iframe.name].location.search;

        if (frameLocation != null && frameLocation.length > 0 && frameLocation != "false;")
        {
            var doc = iframe.contentWindow || iframe.contentDocument;

            if (doc && doc.document)
                doc = doc.document;

            if (doc)
            {
                if (doc.readyState == "complete")
                    isLoaded = true;
                else if (doc.body && (doc.body.childNodes.length > 0 || (doc.body.innerHTML && doc.body.innerHTML.length > 0)))
                    isLoaded = true;
            }
        }

        // alert("loaded:" + isLoaded);

        return isLoaded;
    };

    // --- Templating
    var connectTemplate = function (element, templateElements, eachElementFunction)
    {
        var all = element.getElementsByTagName("*");

        for (var i = 0; i < all.length; i++)
        {
            var childEl = all[i];
            var classes = splitClasses(childEl.className);
            var templateSource = null;

            if (eachElementFunction)
                eachElementFunction(childEl);

            for (var j = 0; j < classes.length; j++)
            {
                if (classes[j].length > 5 && classes[j].substr(0, 3) == "su-")
                {
                    templateSource = classes[j].substr(3).toLowerCase().replace(/-/g, "");

                    break;
                }
            }

            if (templateSource != null)
            {
                childEl.kw_TemplateSource = templateSource;

                templateElements.push(childEl);
            }
        }
    };

    var updateTemplate = function (templateElements, templateValueFunction)
    {
        for (var i = 0; i < templateElements.length; i++)
        {
            var childEl = templateElements[i];
            var value = templateValueFunction(childEl, childEl.kw_TemplateSource ? childEl.kw_TemplateSource.toLowerCase() : null);

            // TODO: figure out how not to remove spaces in IE6
            if (value)
                childEl.innerHTML = value;
        }
    };

    // --- Components
    kw.File = function (options)
    {
        // fields
        var 
        //self = this,
			events = {
			    fileUpdatedHandlers: []
			},
			settings = {
			    id: generateGuid(),
			    fileSelector: null,
			    fileObject: null,
			    name: null,
			    extension: null,
			    size: null,
			    lastModified: null,
			    //type: null,
			    isErrored: false,
			    isCancelled: false,
			    isExtensionValid: true,
			    isSizeValid: true,
			    isValid: true,
			    status: null
			};

        this.getElementById = function (id)
        {
            return document.getElementById(settings.fileSelector.get_Id() + "_" + settings.id + "_" + id);
        };

        this._set_Validation = function (isExtensionValid, isSizeValid)
        {
            settings.isExtensionValid = isExtensionValid;
            settings.isSizeValid = isSizeValid;
            settings.isValid = settings.isExtensionValid && settings.isSizeValid;

            callHandlers(events.fileUpdatedHandlers, this);
        };

        this._set_Status = function (value, isLastCalculateSize)
        {
            settings.status = value;

            if ((settings.size == null || settings.size == -1) && settings.status)
            {
                if (settings.status.contentLength || isLastCalculateSize)
                {
                    settings.size = settings.status.contentLength || -1;

                    settings.fileSelector._validateFile(this);

                    log(settings.fileSelector.id + " - File size calculated.", this);
                }
            }
            else
            {
                callHandlers(events.fileUpdatedHandlers, this);
            }
        };

        this._set_IsCancelled = function (value)
        {
            settings.isCancelled = value;

            callHandlers(events.fileUpdatedHandlers, this);
        };

        var getFileName = function (path)
        {
            var pos = path.lastIndexOf("\\");

            if (pos == -1)
                pos = path.lastIndexOf("/");

            return path.substr(pos + 1);
        };

        var getExtension = function (fileName)
        {
            var pos = fileName.lastIndexOf(".");

            if (pos == -1)
                return null;
            else
                return fileName.substr(pos + 1);
        };

        /*var getFileType = function (fileName)
        {
        // TODO: parse filename and look up
        return null;
        };*/

        // constructor
        settings.name = getFileName(options.fileObject.fileName || options.fileObject.value || options.fileObject.name);
        settings.extension = getExtension(settings.name);

        if (!options.fileObject.tagName)
        {
            settings.size = options.fileObject.fileSize || options.fileObject.size;

            settings.lastModified = options.fileObject.lastModifiedDate;
            //settings.type = options.fileObject.type || getFileType(settings.name);
        }
        else if (options.fileObject.files && options.fileObject.files.length == 1)
        {
            // moz < 3.6 doesn't support xhr uploads, but we can still get size
            var file = options.fileObject.files[0];

            if ((file.name || file.fileName) == options.fileObject.value)
            {
                settings.size = file.fileSize || file.size;
                settings.lastModified = file.lastModifiedDate;
            }
        }

        if (!settings.size && settings.size != 0)
            settings.size = null;

        //if (!settings.type)
        //    settings.type = getFileType(settings.name);

        componentConstructor(this, settings, options, events);
    };

    kw.FileSelector = function (options)
    {
        // fields
        var 
			self = this,
			fileSelectorImpl = null,
			selectedFiles = [],
			uploadBoxIdCounter = 0,
			events = {
			    fileAddingHandlers: [],
			    fileAddedHandlers: [],
			    fileValidatedHandlers: [],
			    fileRemovedHandlers: []
			},
			settings = {
			    id: null,
			    element: null,
			    unskinnedElement: null,
			    unsupportedElement: null,
			    folderElement: null,
			    uploadConnector: null,
			    dropZone: this,
			    maxFiles: 100,
			    maxFileSize: 2097140,
			    allowZeroLengthFiles: false,
			    validExtensions: null,
			    isSkinned: true,
			    showDropZoneOnDocumentDragOver: false
			};

        var htmlFileSelector = function ()
        {
            // fields
            var 
				currentInput = null;

            var onChange = function (e, source)
            {
                // if this is opera and someone is typing in the box, ignore
                if (source.validity && !source.validity.valid)
                    return;

                log(settings.id + " - Files selected.");

                if (source.value && source.value.length > 0)
                {
                    currentInput = null;

                    if (source.parentNode)
                        source.parentNode.removeChild(source);

                    unbind(source, "change", source.onchangehandler);
                    source.onchangehandler = null;

                    //processFiles(source.files || source);
                    processFiles(support.html5Upload ? source.files : source);
                    //window.setTimeout(function () { processFiles(support.html5Upload ? source.files : source); }, 5000);
                }

                updateHover(false);

                removeClass(settings.element, "su-focus");
            };

            var processFiles = function (files)
            {
                var sourceIdSet = false;

                var loopTest = function (i, files)
                {
                    // If we hit maxfiles, we're done
                    if (settings.maxFiles && selectedFiles.length >= settings.maxFiles)
                        return false;
                    // If this is a single file selector, hit the loop once
                    else if (typeof files.length == "undefined")
                        return (i == 0);
                    // This is a multi file selector, loop through the files
                    else
                        return (i < files.length);
                };

                for (var i = 0; loopTest(i, files); i++)
                {
                    if (i == 0 || ((files[i].name || files[i].fileName) != (files[0].name || files[0].fileName)))
                    {
                        var fileObject = support.html5Upload ? files[i] : files;

                        var size = (fileObject.fileSize || fileObject.size);

                        // filter out directory file on folder selection
                        if (size == null || size > 0 || settings.allowZeroLengthFiles)
                        {
                            executeIfIsFile(fileObject, function (fileObject)
                            {
                                // TODO: filter out zero length files?
                                //if (fileObject.fileSize != 0 && fileObject.size != 0)
                                //{
                                var file = new kw.File({ fileObject: fileObject, fileSelector: self });

                                if (files.tagName && !sourceIdSet)
                                {
                                    // TODO: genericise the name
                                    files.id = files.name = file.get_FileSelector().get_Id() + "_" + file.get_Id();

                                    sourceIdSet = true;
                                }

                                add_File(file);
                                //}
                            });
                        }
                    }
                }

                ensureAddBoxIfLessThanMaxFiles();
            };

            var executeIfIsFile = function (fileObject, codeBlock)
            {
                var executeNow = true;

                if ((fileObject.name || fileObject.fileName) == ".")
                {
                    executeNow = false;
                }
                /*else
                {
                    var size = (fileObject.fileSize || fileObject.size);

                    if (size != null && size >= 0 && size < 1024 * 1024 && window.FileReader)
                    {
                        executeNow = false;

                        var blob = null;

                        try
                        {
                            if (fileObject.webkitSlice)
                                blob = fileObject.webkitSlice(0, 1);
                            else if (fileObject.mozSlice)
                                blob = fileObject.mozSlice(0, 1);
                            else if (fileObject.slice)
                                blob = fileObject.slice(0, 1);

                            var reader = new FileReader();

                            reader.onload = function ()
                            {
                                codeBlock(fileObject);

                                try
                                {
                                    reader.abort();
                                }
                                catch (ex)
                                { }
                            };

                            if (blob)
                                reader.readAsBinaryString(blob);
                            else
                                reader.readAsBinaryString(fileObject);
                        }
                        catch (ex)
                        {
                            // Do nothing. Errored while reading the "file", so it's probably not valid
                        }
                    }
                }*/

                if (executeNow)
                    codeBlock(fileObject);
            };

            this._onDrop = function (files)
            {
                log(settings.id + " - Files dropped.");

                processFiles(files);
            };

            this._onFileRemoved = function (file)
            {
                ensureAddBoxIfLessThanMaxFiles();
            };

            var ensureAddBoxIfLessThanMaxFiles = function ()
            {
                if (currentInput == null)
                {
                    currentInput = document.createElement("input");

                    setupFileInput(currentInput);

                    settings.element.appendChild(currentInput);
                }

                if (!settings.maxFiles || selectedFiles.length < settings.maxFiles)
                {
                    if (settings.isSkinned)
                        settings.element.style.display = "block";
                    else
                        showElement(settings.element);

                    if (settings.folderElement && kw.support.folderSelection)
                        showElement(settings.folderElement);
                }
                else
                {
                    settings.element.style.display = "none";

                    if (settings.folderElement && kw.support.folderSelection)
                        settings.folderElement.style.display = "none";
                }
            };

            var setupFileInput = function (input)
            {
                input.type = "file";
                input.name = input.id = settings.id + "_html_file" + uploadBoxIdCounter++;
                //input.style.cssText = "position:absolute;padding:0;margin:0;font-size:20px;z-index:100000;visibility:hidden;opacity:0;filter:alpha(opacity=0);-moz-opacity:0;outline:none;";
                input.hideFocus = true;

                if (support.isSkinnable && settings.isSkinned)
                {
                    // zoom:1 required to force positioning on IE7-8 as per http://joseph.randomnetworks.com/2006/08/16/css-opacity-in-internet-explorer-ie/
                    // -ms-filter: "alpha(opacity=80)"; required on ie8 as per http://snook.ca/archives/html_and_css/ie-position-fixed-opacity-filter
                    input.style.cssText = "position:absolute;right:0;top:0;padding:0;margin:0;height:auto;width:auto;font-family:Arial;font-size:215px;z-index:100000;zoom:1;-ms-filter:\"progid:DXImageTransform.Microsoft.Alpha(opacity=0)\";filter:\"progid:DXImageTransform.Microsoft.Alpha(opacity=0)\";filter:alpha(opacity=0);opacity:0;outline:none;";
                    //input.originalcss = "position:absolute;right:0;top:0;padding:0;margin:0;font-family:Arial;font-size:215px;z-index:100000;zoom:1;-ms-filter:\"progid:DXImageTransform.Microsoft.Alpha(opacity=0)\";filter:\"progid:DXImageTransform.Microsoft.Alpha(opacity=0)\";filter:alpha(opacity=0);opacity:0";

                    if (support.browser == "msie" && support.browserVersion < 9 && settings.element.offsetWidth > 0)
                    {
                        // Add width to fix opacity issue
                        input.style.width = settings.element.offsetWidth * 2 + "px";
                        input.style.height = settings.element.offsetHeight * 2 + "px";
                    }

                    if (!support.fileInputTabbable)
                        input.tabIndex = "-1";
                }

                // TODO: revisit this if maxfiles becomes settable
                if (support.html5Upload && support.fileInputMultiple && settings.maxFiles != 1)
                    input.multiple = true;

                input.onchangehandler = bind(input, "change", this, onChange);

                // TODO: handle the case when the mouse ends up on the selector after the box is closed (mousemove to readd hover class?)
                bind(input, "click", this, function () { updateHover(false); });
                bind(input, "keydown", this, function (e) { return e.keyCode == 9 || e.keyCode == 32 || e.keyCode == 13; });
                bind(input, "keypress", this, function (e)
                {
                    var key = e.charCode || e.keyCode;

                    return key == 9 || key == 13;
                });
                bind(input, "paste", this, function () { return false; });

                bind(input, "focus", this, function () { addClass(settings.element, "su-focus"); });
                bind(input, "blur", this, function () { removeClass(settings.element, "su-focus"); });
            };

            var onMouseOut = function (event)
            {
                if (support.mouseEnterLeave || !withinElement(settings.element, event))
                    updateHover(false);
            };

            var onMouseOver = function (event)
            {
                if (support.mouseEnterLeave || !withinElement(settings.element, event))
                    updateHover(true);
            };

            var updateHover = function (isOverSelector)
            {
                (isOverSelector ? addClass : removeClass)(settings.element, "su-hover");
            };

            this.selectFolder = function ()
            {
                if (currentInput && support.folderSelection)
                {
                    var input = currentInput;

                    input.webkitdirectory = true;

                    input.click();

                    if (input)
                        input.removeAttribute("webkitdirectory");
                }
            };

            this._dispose = function ()
            {
                if (currentInput && currentInput.onchangehandler)
                {
                    unbind(currentInput, "change", currentInput.onchangehandler);
                    currentInput.onchangehandler = null;
                }
            };

            // constructor
            var inputs = settings.element.getElementsByTagName("input");

            // TODO: support multiple elements in unskinned by turning off auto add
            for (var i = 0; i < inputs.length; i++)
            {
                if (inputs[i].type == "file")
                {
                    if (currentInput == null)
                    {
                        currentInput = inputs[i];

                        setupFileInput(currentInput);
                    }
                    else
                    {
                        inputs[i].parentNode.removeChild(inputs[i]);
                    }
                }
            }

            ensureAddBoxIfLessThanMaxFiles();

            if (support.mouseEnterLeave)
            {
                bind(settings.element, "mouseenter", this, onMouseOver);
                bind(settings.element, "mouseleave", this, onMouseOut);
            }
            else
            {
                bind(settings.element, "mouseover", this, onMouseOver);
                bind(settings.element, "mouseout", this, onMouseOut);
            }
        };

        var add_File = function (file)
        {
            self._validateFile(file, true);

            log(file);

            if (callHandlers(events.fileAddingHandlers, file) != false)
            {
                selectedFiles.push(file);

                var internalAddedHandlers = filter(events.fileAddedHandlers, function (handler) { return handler.isInternal == true; });
                var externalAddedHandlers = filter(events.fileAddedHandlers, function (handler) { return !handler.isInternal; });

                callHandlers(internalAddedHandlers, file);

                settings.uploadConnector._add_File(file);

                callHandlers(externalAddedHandlers, file);
                callHandlers(events.fileValidatedHandlers, file);
            }
        };

        this.get_Files = function ()
        {
            return selectedFiles.slice();
        };

        this.clear = function ()
        {
            for (var i = selectedFiles.length - 1; i >= 0; i--)
                this.remove_File(selectedFiles[i]);
        };

        this.remove_File = function (file)
        {
            for (var i = 0; i < selectedFiles.length; i++)
            {
                if (selectedFiles[i].get_Id() == file.get_Id())
                {
                    selectedFiles.splice(i, 1);

                    callHandlers(events.fileRemovedHandlers, file);

                    break;
                }
            }

            fileSelectorImpl._onFileRemoved(file);

            settings.uploadConnector._remove_File(file);

            log(settings.id + " - File removed.", file);
        };

        var updateValidationAll = function ()
        {
            for (var i = 0; i < selectedFiles.length; i++)
                self._validateFile(selectedFiles[i]);
        };

        this._validateFile = function (file, skipValidationEvent)
        {
            var isExtensionValid = true;
            var isSizeValid = true;
            var size = file.get_Size();

            if (settings.maxFileSize && size && size != -1)
                isSizeValid = size / 1024 < settings.maxFileSize;

            if (settings.validExtensions && settings.validExtensions.length > 0)
            {
                isExtensionValid = false;

                var name = file.get_Name();

                for (var i = 0; i < settings.validExtensions.length; i++)
                {
                    var ext = settings.validExtensions[i];

                    if (name.length > ext.length && name.substr(name.length - ext.length).toLowerCase() == ext.toLowerCase())
                    {
                        isExtensionValid = true;

                        break;
                    }
                }
            }

            file._set_Validation(isExtensionValid, isSizeValid);

            if (!skipValidationEvent)
                callHandlers(events.fileValidatedHandlers, file);
        };

        var onBeforeSessionEnd = function (data)
        {
            // TODO: should we be forcing this?
            settings.element.style.display = "none";

            if (settings.folderElement)
                settings.folderElement.style.display = "none";
        };

        this._dispose = function ()
        {
            if (fileSelectorImpl && fileSelectorImpl._dispose)
                fileSelectorImpl._dispose();
        };
        
        // constructor
        componentConstructor(this, settings, options, events, false, "su-fileselector");

        resolveElement(settings, "unskinnedElement", false);

        assert(settings.element || settings.unskinnedElement, "Either element or unskinnedElement is required.");

        if (settings.element == settings.unskinnedElement)
            settings.element = null;

        try
        {
            resolveElement(settings, "folderElement", false);
        }
        catch (ex)
        {
            settings.folderElement = null;

            log("Error resolving folderElement, ignoring:" + ex);
        }

        if (!settings.element)
            settings.isSkinned = false;

        if (settings.dropZone == this)
            settings.dropZone = settings.element;
        else
            resolveElement(settings, "dropZone", false);

        if (settings.maxFiles && settings.maxFiles < 0)
            settings.maxFiles = null;

        if (settings.maxFileSize && settings.maxFileSize < 0)
            settings.maxFileSize = null;

        if (typeof (settings.validExtensions) == "string")
            settings.validExtensions = settings.validExtensions.split(",");

        if (settings.validExtensions)
        {
            for (var i = 0; i < settings.validExtensions.length; i++)
                settings.validExtensions[i] = trim(settings.validExtensions[i]);
        }

        if (support.isSupportedBrowser)
        {
            resolveComponent(settings, "uploadConnector", kw.UploadConnector, "UploadConnector", true);

            if (support.isSkinnable && settings.isSkinned)
            {
                if (!settings.element.style.position || settings.element.style.position.toLowerCase() != "absolute")
                    settings.element.style.position = "relative";

                settings.element.style.display = "block";
                settings.element.style.overflow = "hidden";
                settings.element.style.direction = "ltr";

                if (settings.unskinnedElement && settings.unskinnedElement.parentNode)
                    settings.unskinnedElement.parentNode.removeChild(settings.unskinnedElement);
            }
            else
            {
                if (!settings.unskinnedElement)
                {
                    settings.unskinnedElement = document.createElement("div");

                    settings.element.parentNode.insertBefore(settings.unskinnedElement, settings.element);
                }

                addClass(settings.unskinnedElement, "su-fileselector");
                addClass(settings.unskinnedElement, "su-unskinned");

                if (settings.element && settings.element.parentNode)
                    settings.element.parentNode.replaceChild(settings.unskinnedElement, settings.element)

                settings.element = settings.unskinnedElement;

                showElement(settings.element);
            }

            fileSelectorImpl = new htmlFileSelector();

            if (settings.folderElement && support.folderSelection)
            {
                addClass(settings.folderElement, "su-folderselector");

                bind(settings.folderElement, "click", this, function () { fileSelectorImpl.selectFolder(); });

                showElement(settings.folderElement);
            }
            else if (settings.folderElement && settings.folderElement.parentNode)
            {
                settings.folderElement.parentNode.removeChild(settings.folderElement);
            }

            if (settings.dropZone)
            {
                if (support.dragDrop)
                {
                    kw.DropZoneManager.registerDropZone(settings.dropZone, fileSelectorImpl, settings.showDropZoneOnDocumentDragOver);

                    if (!settings.showDropZoneOnDocumentDragOver)
                        showElement(settings.dropZone);
                }
                else
                {
                    // We can't do this in case the drop zone has other controls inside it
                    //if (settings.dropZone.parentNode && settings.dropZone != settings.element)
                    //    settings.dropZone.parentNode.removeChild(settings.dropZone);

                    settings.dropZone = null;
                }
            }

            settings.uploadConnector.add_OnBeforeSessionEnd(onBeforeSessionEnd, true);
        }
        else
        {
            resolveElement(settings, "unsupportedElement", false);

            if (!settings.unsupportedElement)
            {
                settings.unsupportedElement = document.createElement("div");

                settings.unsupportedElement.appendChild(document.createTextNode("Your browser doesn't support uploading. Please upgrade to a recent browser."));
            }

            addClass(settings.unsupportedElement, "su-fileselector");
            addClass(settings.unsupportedElement, "su-unsupported");

            settings.element.parentNode.replaceChild(settings.unsupportedElement, settings.element);

            showElement(settings.unsupportedElement);

            if (settings.dropZone)
            {
                addClass(settings.dropZone, "su-unsupported");
                // TODO: maybe redo this? it breaks if the unsupportedelement or anything else is inside the drop zone
                //settings.dropZone.parentNode.removeChild(settings.dropZone);
                //settings.dropZone = null;
            }
        }

        if (licenseData)
            handleLicense();

        log("FileSelector constructed", settings.id, "SlickUpload " + (support.isSupportedBrowser ? "supported" : "NOT supported. Disabling..."), settings, this);
    };

    kw.FileList = function (options)
    {
        // fields
        var 
			self = this,
			fileElements = [],
			settings = {
			    id: null,
			    element: null,
			    templateElement: null,
			    fileSelector: null,
			    invalidFileSizeMessage: "File is too large.",
			    invalidExtensionMessage: "Invalid file extension.",
			    fileSizeFormatter: kw.defaultFileSizeFormatter,
			    fileValidationMessageFormatter: kw.defaultFileValidationMessageFormatter,
			    percentFormatter: kw.defaultPercentFormatter
			};

        var updateItem = function (el, file)
        {
            if (el && el.kw_TemplateElements)
            {
                var status = file.get_Status();

                var uploadState = settings.fileSelector.get_UploadConnector().get_Status().state;
                var isRemovable = (uploadState == kw.UploadState.Initializing || uploadState == kw.UploadState.Uploading) && !file.get_IsCancelled();

                updateTemplate(el.kw_TemplateElements, function (childEl, templateSource)
                {
                    var value = null;

                    switch (templateSource)
                    {
                        case "filename":
                            return file.get_Name();
                        case "filesize":
                            if (settings.fileSizeFormatter)
                                return settings.fileSizeFormatter(file.get_Size());
                            else
                                break;
                            /*
                            case "fileType":
                            if (settings.fileTypeFormatter)
                            return settings.fileTypeFormatter(file.get_Type());

                            break;
                            */
                        case "validationmessage":
                            if (settings.fileValidationMessageFormatter)
                                value = settings.fileValidationMessageFormatter(file, self);

                            childEl.style.display = (value ? "" : "none");

                            return value;
                        case "error":
                            if (status && status.errorType && status.errorType != kw.UploadErrorType.None && !file.get_IsCancelled())
                                addClass(el, "su-error");

                            // TODO: figure out a sane way to display error messages
                            /*value = status.errorMessage || status.errorType;

                            childEl.style.display = (value ? "" : "none");*/

                            return value;
                        case "removecommand":
                            if (!isRemovable)
                                childEl.style.display = "none";

                            break;
                        case "percentcompletetext":
                            if (settings.percentFormatter && status && status.contentLength && status.position)
                            {
                                var parsedPosition = parseFloat(status.position);
                                var parsedContentLength = parseFloat(status.contentLength);

                                if (isFinite(parsedPosition) && isFinite(parsedContentLength))
                                {
                                    var percentComplete = parsedPosition / parsedContentLength;

                                    if (isFinite(percentComplete))
                                        return settings.percentFormatter(percentComplete * 100);
                                }
                            }

                            break;
                        case "progressbar":
                            if (status && status.contentLength && status.position)
                            {
                                var parsedPosition = parseFloat(status.position);
                                var parsedContentLength = parseFloat(status.contentLength);

                                if (isFinite(parsedPosition) && isFinite(parsedContentLength))
                                {
                                    var percentComplete = parsedPosition / parsedContentLength;

                                    if (isFinite(percentComplete))
                                    {
                                        showElement(childEl);
                                        showElement(childEl.parentNode);

                                        childEl.style.width = Math.min(percentComplete * 100, 100).toFixed(2) + "%";
                                    }
                                }
                            }

                            break;
                    }
                });
            }
        };

        var onFileAdded = function (file)
        {
            var el = settings.templateElement.cloneNode(true);

            var id = file.get_Id();
            var idPrefix = file.get_FileSelector().get_Id() + "_" + id;

            el.id = null;
            el.file = file;
            el.style.display = "";
            el.kw_TemplateElements = [];
            removeClass(el, "su-filelisttemplate");
            addClass(el, "su-filelistitem");

            var ext = file.get_Extension();

            if (ext)
                addClass(el, "su-ext-" + ext.toLowerCase().replace(/\./g, "-"));

            connectTemplate(el, el.kw_TemplateElements, function (childEl)
            {
                var className = childEl.className.replace(/-/g, "");

                // hook remove command
                if (className.indexOf("suremovecommand") != -1)
                    bind(childEl, "click", childEl, function () { settings.fileSelector.remove_File(file); });

                // prefix ids
                if (childEl.id)
                    childEl.id = idPrefix + "_" + childEl.id;
                if (childEl.name)
                    childEl.name = idPrefix + "_" + childEl.name;
            });

            fileElements[id] = el;

            settings.element.appendChild(el);

            file.add_OnFileUpdated(onFileUpdated);

            updateItem(el, file);
        };

        var onFileUpdated = function (file)
        {
            var el = fileElements[file.get_Id()];

            updateItem(el, file);
        };

        var onFileRemoved = function (file)
        {
            var el = fileElements[file.get_Id()];

            delete fileElements[file.get_Id()];

            el.parentNode.removeChild(el);
        };

        var onBeforeSessionEnd = function (data)
        {
            for (var key in fileElements)
            {
                var el = fileElements[key];

                updateItem(el, el.file);
            }
        };

        this.getItemElementById = function (id)
        {
            return fileElements[id];
        };

        // constructor
        if (support.isSupportedBrowser)
        {
            componentConstructor(this, settings, options, null, true, "su-filelist");

            resolveElement(settings, "templateElement", true);
            resolveComponent(settings, "fileSelector", kw.FileSelector, "FileSelector", true);

            addClass(settings.templateElement, "su-filelisttemplate");

            settings.fileSelector.add_OnFileAdded(onFileAdded, true);
            settings.fileSelector.add_OnFileRemoved(onFileRemoved, true);
            settings.fileSelector.get_UploadConnector().add_OnBeforeSessionEnd(onBeforeSessionEnd, true);

            log("FileList constructed", settings.id, settings, this);
        }
        else
            log("FileList NOT constructed -- unsupported browser", settings.id, settings, this);
    };

    kw.UploadProgressDisplay = function (options)
    {
        // fields
        var 
			templateElements = [],
			settings = {
			    id: null,
			    element: null,
			    uploadConnector: null,
			    fileSizeFormatter: kw.defaultFileSizeFormatter,
			    percentFormatter: kw.defaultPercentFormatter,
			    timeFormatter: kw.defaultTimeFormatter,
			    showDuringUpload: true,
			    hideAfterUpload: true
			};

        var updateDisplay = function (status)
        {
            updateTemplate(templateElements, function (childEl, templateSource)
            {
                switch (templateSource)
                {
                    case "currentfilename":
                        return status.currentFileName;
                    case "currentfileindex":
                        return status.currentFileIndex;
                    case "filecount":
                        return status.fileCount;
                    case "contentlengthtext":
                        if (settings.fileSizeFormatter && status.contentLength)
                            return settings.fileSizeFormatter(status.contentLength);
                        else
                            break;
                    case "percentcompletetext":
                        if (settings.percentFormatter)
                            return settings.percentFormatter(status.percentComplete);
                        else
                            break;
                    case "speedtext":
                        var speedText = null;

                        if (status.totalSpeed)
                            speedText = settings.fileSizeFormatter(status.totalSpeed);

                        if (speedText)
                            return speedText + "/s";

                        break;
                    case "timeremainingtext":
                        if (settings.timeFormatter)
                            return settings.timeFormatter(status.timeRemaining);
                        else
                            break;
                    case "progressbar":
                        if (status.percentComplete != null)
                        {
                            showElement(childEl);
                            showElement(childEl.parentNode);

                            var parsedPercentComplete = parseFloat(status.percentComplete);

                            if (isFinite(parsedPercentComplete))
                                childEl.style.width = parsedPercentComplete.toFixed(2) + "%";
                        }

                        break;
                }
            });
        };

        var onSessionStarted = function (status)
        {
            if (settings.showDuringUpload)
                showElement(settings.element);

            updateDisplay(status);
        };

        var onSessionProgress = function (status)
        {
            updateDisplay(status);
        };

        var onSessionEnded = function (status)
        {
            updateDisplay(status);

            if (settings.hideAfterUpload)
                settings.element.style.display = "none";
        };

        // constructor
        if (support.isSupportedBrowser)
        {
            componentConstructor(this, settings, options, null, true, "su-uploadprogressdisplay");

            resolveComponent(settings, "uploadConnector", kw.UploadConnector, "UploadConnector", true);

            settings.uploadConnector.add_OnUploadSessionStarted(onSessionStarted, true);
            settings.uploadConnector.add_OnUploadSessionProgress(onSessionProgress, true);
            settings.uploadConnector.add_OnUploadSessionEnded(onSessionEnded, true);

            connectTemplate(settings.element, templateElements);

            log("UploadProgressDisplay constructed", settings.id, settings, this);
        }
        else
            log("UploadProgressDisplay NOT constructed -- unsupported browser", settings.id, settings, this);
    };

    kw.UploadConnector = function (options)
    {
        // fields
        var 
			self = this,
			uploadConnectorImpl = null,
			files = [],
			uploadThreadTimeout = null,
			originalOnSubmit = null,
			submittedElement = null,
			uploadSessionIdElement = null,
			uploadErrorTypeElement = null,
			sourceConnectorIdElement = null,
			failedRequestsElement = null,
            cancelledRequests = [],
			cancelledRequestsElement = null,
            completionIframeElement = null,
			uploadDataElement = null,
			isCompleting = false,
			isManuallyStartedUpload = false,
			settings = {
			    id: null,
			    allowPartialError: false,
			    uploadHandlerUrl: null,
			    completeHandlerUrl: null,
                documentDomain: null,
			    uploadSessionId: generateGuid(),
			    uploadForm: null,
			    autoUploadOnSubmit: false,
			    autoUploadTriggerIdList: null,
			    autoCompleteAfterLastFile: true,
			    // includeFormData: false, // TODO: implement
			    confirmNavigateDuringUploadMessage: null,
			    uploadProfile: null,
			    completionMethod: "POST",
			    completionBody: null,
			    completionContentType: "application/x-www-form-urlencoded",
			    pollInterval: 1000,
			    pollTimeout: 6500,
			    uploadTimeout: 15000,
			    timeoutDoubleOnResponse: true,
			    hasDoubledTimeout: false,
			    postbackFunction: null,
			    concurrentMaxFiles: 1,
			    data: {},
			    status: {
			        state: kw.UploadState.Initializing,
			        errorType: kw.UploadErrorType.None
			    }
			},
			events = {
			    beforeSessionStartHandlers: [],
			    uploadSessionStartedHandlers: [],
			    uploadFileStartedHandlers: [],
			    uploadFileEndedHandlers: [],
			    uploadSessionProgressHandlers: [],
			    beforeSessionEndHandlers: [],
			    uploadSessionEndedHandlers: []
			};

        this.set_CompletionBody = function (value) { settings.completionBody = value; }
        this.set_Data = function (key, value) { settings.data[key] = value; }

        var html4UploadConnector = function ()
        {
            this.uploadFile = function (file, uploadUrl)
            {
                var id = file.get_Id();
                var sourceId = id + "-source";
                var targetId = id + "-target";

                file.uploadFrame = addIFrame(targetId);
                file.form = addForm(sourceId, targetId, uploadUrl);
                file.uploadUrl = uploadUrl;
                file.pollProgress = true;

                if (objectSize(settings.data) > 0)
                {
                    var dataElement = document.createElement("input");

                    dataElement.type = "hidden";
                    dataElement.name = "kw_uploadData";
                    dataElement.value = queryStringSerialize(settings.data);

                    file.form.appendChild(dataElement);
                }

                file.form.appendChild(file.get_FileObject());

                // setTimeout required for webkit
                window.setTimeout(function ()
                {
                    if (file.form)
                    {
                        file.form.submit();

                        file.state = "uploading";
                    }
                }, 1);


                //file.form.submit();
            };

            this.heartbeatCheck = function ()
            {
                for (var i = files.length - 1; i >= 0; i--)
                {
                    var file = files[i];

                    if (file.state == "uploading")
                    {
                        if (getIsUploadError(file) || (file.action == "calculatesize" && getIsLoaded(file.uploadFrame) && !getIsUploadError(file, file.uploadUrl)))
                            file.state = "checking";
                    }

                    /*if (!file.findingError && file.isStarted)
                    {
                    var isErrored = false;

                    if (file.mode == "calculatesize" && (getIsUploadError(file) || (getIsLoaded(file.uploadFrame) && !getIsUploadError(file, file.uploadUrl))))
                    isErrored = true;
                    else if (file.mode == "upload" && getIsUploadError(file))
                    isErrored = true;

                    if (isErrored)
                    onRequestProgress(file, null);
                    }*/
                }
            };

            this.disposeFile = function (file)
            {
                if (file.uploadFrame)
                {
                    file.uploadFrame.src = "javascript:false;";
                    //window.frames[file.uploadFrame.id].location = "javascript:false;";

                    file.uploadFrame.parentNode.removeChild(file.uploadFrame);
                }
                if (file.progressFrame)
                    file.progressFrame.parentNode.removeChild(file.progressFrame);
                if (file.form)
                    file.form.parentNode.removeChild(file.form);

                delete file.uploadFrame;
                delete file.progressFrame;
                delete file.form;
                delete file.uploadUrl;
            };

            // private methods
            var addForm = function (id, targetId, url)
            {
                var form = document.createElement("form");

                form.name = form.id = id;
                form.action = url;

                form.enctype = form.encoding = "multipart/form-data";
                form.method = "POST";

                form.style.display = "none";

                document.body.appendChild(form);

                form.target = targetId;

                return form;
            };

            var getIsUploadError = function (file, uploadHandlerUrl)
            {
                if (!uploadHandlerUrl)
                    uploadHandlerUrl = settings.uploadHandlerUrl;

                return isIFrameError(file.get_Id() + "-target", uploadHandlerUrl);
            };
        };

        var html5UploadConnector = function ()
        {
            this.uploadFile = function (file, uploadUrl)
            {
                var fileData = file.get_FileObject();

                var xhr = getXmlReq();

                file.uploadXhr = xhr;

                //var eventSource = ;

                //xhr.upload.addEventListener("progress", onUploadProgress, false);
                //bind(eventSource, "load", file, onUploadComplete);
                bind(xhr, "error", file, onUploadError, null, true);
                //bind(xhr, "abort", file, onUploadError, null, true);
                bind(xhr.upload || xhr, "progress", file, onUploadProgress);
                bind(xhr, "readystatechange", file, onReadyStateChange, null, true);

                xhr.open("POST", uploadUrl, true);

                xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                xhr.setRequestHeader("X-File-Size", fileData.fileSize);

                //xhr.setRequestHeader("Content-Length", fileData.fileSize);
                // TODO: pass along form vars
                if (window.FormData)
                {
                    var formData = new FormData();

                    if (objectSize(settings.data) > 0)
                        formData.append("X-SlickUpload-Data", queryStringSerialize(settings.data));

                    // TODO: genericise the name
                    formData.append(file.get_FileSelector().get_Id() + "_" + file.get_Id(), fileData);

                    xhr.send(formData);
                }
                else
                {
                    xhr.setRequestHeader("X-File-Name", fileData.fileName);
                    xhr.setRequestHeader("X-File-Content-Type", fileData.type);
                    xhr.setRequestHeader("X-File-Source-Element", file.get_FileSelector().get_Id() + "_" + file.get_Id());
                    xhr.setRequestHeader("Content-Type", "application/octet-stream");

                    if (objectSize(settings.data) > 0)
                        xhr.setRequestHeader("X-SlickUpload-Data", queryStringSerialize(settings.data));

                    /*if (xhr.sendAsBinary && fileData.getAsBinary)
                    xhr.sendAsBinary(fileData.getAsBinary());
                    else*/
                    xhr.send(fileData);
                }

                file.state = "uploading";
            };

            this.disposeFile = function (file)
            {
                if (file.status != "complete" && file.uploadXhr && file.uploadXhr.abort)
                    file.uploadXhr.abort();

                delete file.uploadXhr;
            };

            function onUploadProgress(e)
            {
                var position = e.position || e.loaded;
                var total = e.totalSize || e.total;

                if (position == 0 || position == total || !this.lastProgressReceiveDate || (new Date().getTime() - this.lastProgressReceiveDate.getTime() > 100))
                {
                    // Don't update if we got to the end before we received the initial ping
                    // TODO: only up
                    if (this.lastProgressReceiveDate || position < total)
                    {
                        var data = { position: position };

                        // Only pass contentLength if we haven't already gotten it from the initial ping
                        if (!this.lastProgressReceiveDate)
                            data.contentLength = total;
                        else
                            // Make sure that date is updated so we don't time out if we do a manual progress request later
                            this.lastProgressReceiveDate = new Date();

                        onRequestProgressInternal(this, data);
                    }
                }

                var status = this.get_Status();

                if (status && status.state == kw.UploadState.Error)
                    disposeFile(this);
                // TODO: check for error
            }

            function onUploadError(e)
            {
                //  TODO: fix
                this.state = "checking";
                //onRequestProgress(this, null);
            }

            var onReadyStateChange = function (e, source)
            {
                if (source.readyState == 4)
                {
                    var data = null;

                    if (source.status == 200 && source.responseText && source.responseText.length > 0)
                    {
                        try
                        {
                            data = eval("(" + source.responseText + ")");
                        }
                        catch (ex)
                        {
                            data = null;
                        }
                    }

                    if (kw.verboseLog && data == null)
                        log("Received HTML5 completion with null data.", "status=" + source.status, source.responseText);

                    onRequestProgressInternal(this, data);

                    unbind(source, "readystatechange", null, true);
                }
            };
        };

        var uploadThread = function ()
        {
            if (uploadThreadTimeout)
            {
                clearTimeout(uploadThreadTimeout);

                uploadThreadTimeout = null;
            }

            if (uploadConnectorImpl.heartbeatCheck)
                uploadConnectorImpl.heartbeatCheck();

            var hasWork = false;

            var uploadingFileCount = 0;

            for (var i = 0; i < files.length; i++)
            {
                var file = files[i];

                if (file.state == "uploading" || file.state == "checking")
                    uploadingFileCount++;

                if (file.action == "calculatesize" || file.state == "checking" || (settings.status.state == kw.UploadState.Uploading && (file.state == "uploading" || file.state == "ready")))
                    hasWork = true;

                if (settings.status.state == kw.UploadState.Uploading && uploadingFileCount < settings.concurrentMaxFiles && file.state == "ready")
                {
                    uploadFile(file);

                    uploadingFileCount++;
                }

                if (file.action == "calculatesize" || (file.state == "uploading" && ((!file.lastProgressSendDate || !file.lastProgressReceiveDate) || file.pollProgress || !file.get_Status() || file.get_Status().errorType == "ServerUnavailable")) || file.state == "checking")
                {
                    var now = new Date();
                    var lastActionDate = file.lastProgressReceiveDate || file.firstProgressSendDate;

                    if (lastActionDate && now - lastActionDate > settings.uploadTimeout)
                    {
                        if (kw.verboseLog)
                            log("cancel-lastReceive:" + (file.lastProgressReceiveDate ? file.lastProgressReceiveDate : "null") + ";firstSend:" + (file.firstProgressSendDate ? file.firstProgressSendDate : "null") + ";actionTimeout:" + (now - lastActionDate));

                        cancelProgressRequest(file);

                        onRequestProgressInternal(file, null, true);
                        // TODO: error
                    }
                    else if (!file.lastProgressSendDate ||
							 now - file.lastProgressSendDate >= settings.pollTimeout ||
							 (file.lastProgressReceiveDate && file.lastProgressReceiveDate > file.lastProgressSendDate && now - file.lastProgressReceiveDate >= settings.pollInterval))
                    {
                        if (now - file.startDate >= settings.pollInterval)
                        {
                            if (kw.verboseLog)
                                log("execute-lastSend:" + (!file.lastProgressSendDate ? "null" : (file.lastProgressSendDate + ";lastSendInterval:" + (now - file.lastProgressSendDate) + ";lastReceiveInterval:" + (!file.lastProgressReceiveDate ? "null" : (now - file.lastProgressReceiveDate)))));

                            var isLastRetry = lastActionDate && now - lastActionDate + settings.pollTimeout + settings.pollInterval > settings.uploadTimeout;

                            executeProgressRequest(file, getUrl("progress", file, isLastRetry), onRequestProgressInternal);
                        }
                    }
                }

                // Exit early if we have nothing left to do
                if (uploadingFileCount >= settings.concurrentMaxFiles && hasWork)
                    break;
            }

            if (!hasWork && settings.autoCompleteAfterLastFile && (settings.status.state == kw.UploadState.Uploading || settings.status.state == kw.UploadState.Error))
                self.complete(true);

            if ((hasWork || settings.status.state == kw.UploadState.Completing) && settings.status.state != kw.UploadState.Error)
                uploadThreadTimeout = window.setTimeout(function () { uploadThread.call(this); }, settings.pollInterval / 2);

            if (settings.status.state == kw.UploadState.Completing)
            {
                var now = new Date();
                var lastActionDate = self.lastProgressReceiveDate || self.firstProgressSendDate;

                if (lastActionDate && now - lastActionDate > settings.uploadTimeout)
                {
                    //cancelProgressRequest(file);

                    //onRequestProgressInternal(file, null, true);
                    // TODO: error
                }
                else if (!self.lastProgressSendDate ||
						 now - self.lastProgressSendDate >= settings.pollTimeout ||
						 (self.lastProgressReceiveDate && self.lastProgressReceiveDate > self.lastProgressSendDate && now - self.lastProgressReceiveDate >= settings.pollInterval))
                {
                    //if (now - self.startDate >= settings.pollInterval)
                    executeProgressRequest(self, getUrl("progress"), onSessionProgressInternal);
                }
            }
        };

        var uploadFile = function (file, isCalculateSize)
        {
            var uploadUrl = getUrl((isCalculateSize ? "calculatesize" : "upload"), file);

            file.action = (isCalculateSize ? "calculatesize" : "upload");
            file.state = "uploading";
            file.startDate = new Date();

            file.lastProgressSendDate = null;
            file.firstProgressSendDate = null;
            file.lastProgressReceiveDate = null;

            uploadConnectorImpl.uploadFile(file, uploadUrl);

            if (isCalculateSize)
            {
                uploadThread();
            }
            else
            {
                callHandlers(events.uploadFileStartedHandlers, file);
            }

            log(settings.id + " - starting " + (isCalculateSize ? "size calculation" : "upload") + " - " + file.get_Name(), file);
        };

        var getUrl = function (handlerType, file, isLastRetry)
        {
            var url = settings.uploadHandlerUrl + (settings.uploadHandlerUrl.indexOf("?") > 0 ? "&" : "?") + "handlerType=" + handlerType;

            url += "&uploadSessionId=" + settings.uploadSessionId;

            if (file)
            {
                //url += "&name=" + encodeURIComponent(file.get_Name());
                //url += "&length=" + file.get_Size();

                url += "&uploadRequestId=" + file.get_Id();
         

                if (settings.uploadProfile)
                    url += "&uploadProfile=" + settings.uploadProfile;
            }

            if (isLastRetry)
                url += "&isLastRetry=true";

            return url;
        };

        var onRequestProgressInternal = function (file, data, isFinalRequest)
        {
            if (kw.verboseLog)
            {
                if (data)
                    log("Received file progress. Status: " + data.status, file, data.position, data.contentLength, "isFinalRequest=" + isFinalRequest);
                else
                    log("Received file progress with null data.", file, "isFinalRequest=" + isFinalRequest);
            }

            var currentStatus = file.get_Status();

            var newProgressException = null;

            if (data && data.progressException)
                newProgressException = data.progressException;

            if (!data || data.progressException)
            {
                data = { status: kw.UploadState.Error, errorType: "ServerUnavailable" };

                if (newProgressException)
                    data.progressException = newProgressException;
            }
            else if (data.status)
            {
                if (currentStatus && data.position < currentStatus.position) //|| data.position != data.contentLength) && !file.pollProgress && !file.lastProgressReceiveDate)
                {
                    // If we got to the end of the file before receiving the initial ping, switch to progress polling
                    if (!file.lastProgressReceiveDate && currentStatus.position == currentStatus.contentLength)
                        file.pollProgress = true;
                    else
                    // This is probably the test ping request for an HTML5 upload. Throw away position in case it is out of date.
                        delete data.position;
                }

                file.lastProgressReceiveDate = new Date();

                if (settings.timeoutDoubleOnResponse && !settings.hasDoubledTimeout)
                {
                    settings.pollTimeout *= 2;
                    settings.uploadTimeout *= 2;

                    settings.hasDoubledTimeout = true;

                    log("First response received and timeoutDoubleOnResponse=true. Doubling timeouts.", settings);
                }
            }

            if (data && currentStatus)
                data = extend(currentStatus, data);

            if (data.errorType == "ServerUnavailable" && isFinalRequest)
            {
                var message;

                if (data.progressException)
                    message = "DEBUG MESSAGE: Request progress handler (" + settings.uploadHandlerUrl + ") exception: " + data.progressException.replace(/\\r\\n/g, "\n");
                else if (file.action != "calculatesize")
                    message = "DEBUG MESSAGE: Could not contact SlickUpload request progress handler at \"" + settings.uploadHandlerUrl + "\".";

                log(message, file, data);

                if (kw.debug)
                    alert(message);
            }

            if (file.action == "calculatesize" && (isFinalRequest || data.errorType != "ServerUnavailable"))
            {
                file.action = "upload";
                file.state = "ready";

                uploadConnectorImpl.disposeFile(file);
            }
            else if (file.action == "upload")
            {
                // TODO: complete and errored modes?
                if (data.status == kw.UploadState.Complete || (data.status == kw.UploadState.Error && (isFinalRequest || data.errorType != "ServerUnavailable")) && file.state != "complete")
                {
                    file.state = "complete";

                    data.isErrored = (data.errorType != "None");

                    uploadConnectorImpl.disposeFile(file);

                    //file.pollProgress = false;

                    // Ensure status is set before we call other handlers. It's possible we complete before we get a progress event to fire. This is a known issue on FF 3.6 with tiny files.
                    file._set_Status(data, isFinalRequest);

                    callHandlers(events.uploadFileEndedHandlers, file);

                    var message = settings.id + " - file upload complete - " + file.get_Name() + ".";

                    if (data.errorType != "None")
                        message += " Error: " + data.errorType + ".";

                    if (data.errorMessage)
                        message += " Message: " + data.errorMessage;

                    log(message, file, data);
                }
            }

            // TODO: check for file.state = "complete" and don't do this, if it was done above?
            file._set_Status(data, isFinalRequest);

            if (settings.status.state == kw.UploadState.Uploading)
                onSessionStatusUpdate();
        };

        var onSessionProgressInternal = function (uploadConnector, data, isFinalRequest)
        {
            if (kw.verboseLog)
                log("Received session progress.", uploadConnector, data, "isFinalRequest=" + isFinalRequest);

            /*if (uploadConnector.progressTimeout)
            {
            window.clearTimeout(uploadConnector.progressTimeout);

            uploadConnector.progressTimeout = null;
            }*/

            var newProgressException = null;

            if (data && data.progressException)
                newProgressException = data.progressException;

            if (!data || data.progressException)
            {
                data = { status: kw.UploadState.Error, errorType: "ServerUnavailable" };

                if (newProgressException)
                    data.progressException = newProgressException;
            }
            else if (data.state)
                uploadConnector.lastProgressReceiveDate = new Date();

            if (data.errorType == "ServerUnavailable" && isFinalRequest)
            {
                var message;

                if (data.progressException)
                    message = "DEBUG MESSAGE: Session progress handler (" + settings.uploadHandlerUrl + ") exception: " + data.progressException.replace(/\\r\\n/g, "\n");
                else if (file.action != "calculatesize")
                    message = "DEBUG MESSAGE: Could not contact SlickUpload session progress handler at \"" + settings.uploadHandlerUrl + "\".";

                log(message, settings.status, data);

                if (kw.debug)
                    alert(message);
            }

            if (settings.status && settings.status.state == "Completing" && data)
                data.state = "Completing";

            extend(settings.status, data);

            /*if (settings.status.errorType == "ServerUnavailable")
            uploadConnector.progressRetryCount = (uploadConnector.progressRetryCount || 0) + 1;

            if (settings.status.state == kw.UploadState.Completing && (uploadConnector.progressRetryCount || 0) < 5)
            window.setTimeout(function ()
            {
            executeProgressRequest(uploadConnector, getUrl("progress"), onSessionProgressInternal);
            }, 1000);
            else if ((uploadConnector.progressRetryCount || 0) == 5)
            data.status = kw.UploadState.Error;*/

            sanitizeStatus();

            callHandlers(events.uploadSessionProgressHandlers, settings.status);
        };

        var onSessionStatusUpdate = function ()
        {
            var position = 0;
            var contentLength = 0;
            var fileCompleteCount = 0;
            var currentFileName = null;
            var currentFileIndex = 0;

            for (var i = 0; i < files.length; i++)
            {
                var file = files[i];
                var status = file.get_Status();
                var size = file.get_Size();

                if (status && status.position)
                    position += status.position;
                if (size)// && (status == null || status.status != kw.UploadState.Error))
                    contentLength += size;

                // Find either currently uploading file or the last completed
                if (file.action == "upload" && (file.state == "uploading" || file.state == "complete"))
                {
                    currentFileName = file.get_Name();
                    currentFileIndex++;
                }

                /*if (file.action == "upload" && file.startDate)
                {
                var elapsed = (new Date().getTime() - file.startDate.getTime()) / 1000;

                if (status && status.position)
                totalSpeed += status.position / elapsed;
                }*/
            }

            if (currentFileIndex == 0)
                currentFileIndex = 1;

            settings.status.position = position;
            settings.status.contentLength = contentLength;
            settings.status.percentComplete = (position / contentLength * 100);
            settings.status.currentFileName = currentFileName;
            settings.status.currentFileIndex = currentFileIndex;
            settings.status.fileCount = files.length;

            var elapsed = (new Date().getTime() - settings.status.startDate.getTime()) / 1000;

            if (elapsed)
                settings.status.totalSpeed = position / elapsed;

            if (settings.status.totalSpeed)
                settings.status.timeRemaining = (contentLength - position) / settings.status.totalSpeed;

            sanitizeStatus();

            callHandlers(events.uploadSessionProgressHandlers, settings.status);
        };

        var sanitizeStatus = function ()
        {
            if (settings.status.percentComplete)
            {
                var parsedPercentComplete = parseFloat(settings.status.percentComplete);

                if (isFinite(parsedPercentComplete))
                    settings.status.percentComplete = Math.min(parsedPercentComplete, 100);
                else
                    settings.status.percentComplete = 0;
            }

            if (settings.status.timeRemaining)
            {
                var parsedTimeRemaining = parseFloat(settings.status.timeRemaining);

                if (isFinite(parsedTimeRemaining))
                    settings.status.timeRemaining = Math.max(parsedTimeRemaining, 0);
                else
                    settings.status.timeRemaining = 0;
            }

            // TODO: sanitize other fields
        };

        var cancelProgressRequest = function (object)
        {
            if (object.progressFrame)
                object.progressFrame.contentWindow.location = "javascript:false;";
            else if (object.progressXhr)
            {
                unbind(object.progressXhr, "readystatechange", null, true);

                var isBusy = false;

                try
                {
                    isBusy = object.progressXhr.readyState && object.progressXhr.readyState != 4;
                }
                catch (ex)
				{ }

                if (object.progressXhr.abort && isBusy)
                {
                    // log("cancelling existing request");

                    object.progressXhr.abort();
                }
            }
        };

        var executeProgressRequest = function (object, url, onProgressRecieved)
        {
            if (kw.verboseLog)
                log("Executing progress request.", object, url);

            // TODO: do we need this?
            //cancelProgressRequest(object);

            object.lastProgressSendDate = new Date();

            if (!object.firstProgressSendDate)
                object.firstProgressSendDate = object.lastProgressSendDate;

            if ((support.xmlHttpRequestInSubmit || settings.status.state != kw.UploadState.Completing) &&
                (!settings.documentDomain || support.cors))
            {
                if (object.progressXhr)
                {
                    if (kw.verboseLog)
                        log("Cancelling previous progress request.", object);

                    cancelProgressRequest(object);
                }
                else
                {
                    object.progressXhr = getXmlReq();
                }

                object.progressXhr.open("GET", url, true);

                object.progressXhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");

                bind(object.progressXhr, "readystatechange", object.progressXhr, function ()
                {
                    if (object.progressXhr.readyState == 4)
                    {
                        var data = null;

                        try
                        {
                            data = eval("(" + object.progressXhr.responseText + ")");
                        }
                        catch (ex)
                        {
                            data = null;
                        }

                        if (objectSize(data) == 0)
                            data = null;

                        onProgressRecieved(object, data);

                        unbind(object.progressXhr, "readystatechange", null, true);
                    }
                }, null, true);

                object.progressXhr.send("");
            }
            else
            {
                if (!object.progressFrame)
                {
                    object.progressFrame = document.createElement("iframe");
                    object.progressFrame.src = "javascript:false;"
                    object.progressFrame.style.display = "none";

                    document.body.appendChild(object.progressFrame);
                }

                object.progressFrame.contentWindow.location = url;
            }
        };

        var onFormSubmitted = function (event)
        {
            if (settings.updatePanelId && window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
            {
                try
                {
                    // Check to see if this came from our panel
                    var triggerPanelId = Sys.WebForms.PageRequestManager.getInstance()._postBackSettings.panelID;

                    if (triggerPanelId && triggerPanelId.split("|")[0] != settings.updatePanelId)
                        return true;
                }
                catch (ex)
				{ }
            }

            // TODO: see if cancelling interferes with anything else...
            if (settings.status.state == kw.UploadState.Uploading)
            {
                // We cancel this submit, but .complete() triggers a new one
                window.setTimeout(self.complete, 1);

                return false;
            }
            else if (settings.status.state != kw.UploadState.Initializing)
                return true;

            var ret;

            /*if (originalOnSubmit)
            {
            var isPreventDefaultCalled = false;

            if (event == null)
            event = window.event;

            if (event != null && event.preventDefault)
            {
            var oldEvent = event;
            var eventAdapter = new Object();

            eventAdapter.prototype = event.prototype;

            for (key in event)
            eventAdapter[key] = event[key];

            eventAdapter.preventDefault = function ()
            {
            isPreventDefaultCalled = true;

            if (oldEvent.preventDefault)
            oldEvent.preventDefault();
            };

            event = eventAdapter;

            try
            {
            window.event = event;
            }
            catch (ex)
            { }
            }

            ret = originalOnSubmit.call(this);

            if (ret == false || isPreventDefaultCalled || (event != null && (event.returnValue == false || event.defaultPrevented == true)))
            ret = false;
            }*/

            //if (ret != false)
            //{
            if (files.length > 0 || !settings.autoCompleteAfterLastFile)
            {
                window.setTimeout(function () { self.start.call(self, true); }, 1);

                ret = false;
            }
            else
            {
                if (settings.autoUploadOnSubmit && originalOnSubmit)
                {
                    if (settings.uploadForm.validationCallbacks)
                        settings.uploadForm.validationCallbacks.splice(indexOf(settings.uploadForm.validationCallbacks, onFormSubmitted), 1);

                    ret = originalOnSubmit(event);
                }
                else
                    ret = true;
            }

            if (ret == false)
            {
                event = event || window.event;

                if (event)
                {
                    if (event.preventDefault)
                        event.preventDefault();
                    else if ("returnValue" in event)
                        event.returnValue = ret;
                }
            }
            //}

            return ret;
        };

        var wireAutoUploadOnSubmit = function ()
        {
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
                Array.add(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, onFormSubmitted);
            else if (window.Sys && Sys.Mvc && Sys.Mvc.AsyncForm)
            {
                settings.uploadForm.validationCallbacks = settings.uploadForm.validationCallbacks || [];
                settings.uploadForm.validationCallbacks.push(onFormSubmitted);
            }
            //}
            //else
            //{
            originalOnSubmit = settings.uploadForm.onsubmit;
            settings.uploadForm.onsubmit = onFormSubmitted;
            //}

            // hook submit elements so we can determine which element submitted
            // TODO: ensure we're not getting extra elements here
            var elements = settings.uploadForm.getElementsByTagName("*");

            for (var i = 0; i < elements.length; i++)
            {
                var el = elements[i];

                if (!settings.autoUploadTriggerIdList || indexOf(settings.autoUploadTriggerIdList, el.id) != -1)
                {
                    var isWireable = false;

                    if (el.type)
                    {
                        switch (el.type.toLowerCase())
                        {
                            case "submit":
                            case "image":
                                isWireable = true;

                                break;
                        }
                    }
                    else if (el.tagName == "A")
                        isWireable = true;

                    if (isWireable)
                        bind(el, "click", this, onElementSubmitted);
                }
            }

            // rewrite __doPostBack so we get the proper postback events
            // if ASP.NET AJAX, leave __doPostBack as is
            if (window.__doPostBack)// && !(window.Sys && Sys.Application))
            {
                var originalDoPostBack = __doPostBack;

                __doPostBack = function (eventTarget, eventArgument)
                {
                    /*theForm.__EVENTTARGET.value = eventTarget;
                    theForm.__EVENTARGUMENT.value = eventArgument;*/

                    submittedElement = document.getElementById(eventTarget);

                    if (!submittedElement && eventTarget)
                    {
                        var id = eventTarget.replace(/\$/g, "_");

                        while (id.indexOf("_") != -1 && ((submittedElement = document.getElementById(id)) == null))
                        {
                            id = id.substr(id.indexOf("_") + 1);
                        }
                    }

                    if (!theForm.onsubmit || (theForm.onsubmit() != false))
                        originalDoPostBack(eventTarget, eventArgument);
                    //if (!theForm.onsubmit || (theForm.onsubmit() != false))
                    //    theForm.submit();
                };
            }
        };

        var onElementSubmitted = function (e, source)
        {
            if (settings.status.state == kw.UploadState.Initializing || !submittedElement)
            {
                if (source.tagName != "A" || (source.href && source.href.toLowerCase().indexOf("dopostback") != -1))
                    submittedElement = source;
            }
        };

        this._add_File = function (file)
        {
            //file.mode = "ready";
            file.state = "ready";

            files.push(file);

            if (file.get_Size() == null)
                uploadFile(file, true);
            else if (settings.status.state == kw.UploadState.Uploading)
                uploadThread();
        };

        this._remove_File = function (file)
        {
            for (var i = 0; i < files.length; i++)
            {
                if (files[i].get_Id() == file.get_Id())
                {
                    files.splice(i, 1);

                    file._set_IsCancelled(true);

                    if ((file.action == "calculatesize" || file.action == "upload") && file.state == "uploading")
                        uploadConnectorImpl.disposeFile(file);

                    cancelledRequests.push(file.get_Id());

                    break;
                }
            }
        };

        this.start = function (invokedByAutoUploadOnSubmit)
        {
            if (settings.status.state != kw.UploadState.Initializing)
            {
                log(settings.id + " - session has already been started - " + settings.uploadSessionId + " - ignoring start request", settings.status);

                return;
            }

            // TODO: exception?
            //if (settings.status.state != kw.UploadState.Initializing)
            //    return false;

            // skip if this is an async postback
            /*if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
            {
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            if (prm.get_isInAsyncPostBack() || (prm._postBackSettings && prm._postBackSettings.async))
            return true;
            }*/

            if (settings.autoUploadOnSubmit)
            {
                if (window.Page_ClientValidate && Page_ClientValidate() == false)
                    return false;

                isManuallyStartedUpload = (invokedByAutoUploadOnSubmit != true);
            }

            if (callHandlers(events.beforeSessionStartHandlers, settings.status) == false)
                return false;

            settings.status.state = kw.UploadState.Uploading;
            settings.status.fileCount = files.length;
            settings.status.startDate = new Date();

            if (files.length > 0)
            {
                settings.status.currentFileIndex = 1;
                settings.status.currentFileName = files[0].get_Name();
            }

            log(settings.id + " - starting session - " + settings.uploadSessionId, settings.status, "files=" + files.length);

            callHandlers(events.uploadSessionStartedHandlers, settings.status);

            uploadThread();
        };

        this.cancel = function ()
        {
            if (settings.status.state == kw.UploadState.Uploading)
            {
                log(settings.id + " - cancelling session - " + settings.uploadSessionId, settings.status);

                for (var i = 0; i < files.length; i++)
                {
                    files[i]._set_IsCancelled(true);

                    uploadConnectorImpl.disposeFile(files[i]);
                }

                settings.status.state = kw.UploadState.Error;
                settings.status.errorType = kw.UploadErrorType.Cancelled;

                // Ensure cancel complete gets executed
                //uploadThread();
            }
            else
            {
                log(settings.id + " - skipping session cancellation - " + settings.uploadSessionId + ". Cancellation can only be performed when UploadState=Uploading.", settings.status);
            }
        };

        this.complete = function (isAutoComplete)
        {
            if (isCompleting)
            {
                log(settings.id + " - session already completing - " + settings.uploadSessionId + " - ignoring complete request", settings.status);

                return;
            }
            else if (settings.status.state == kw.UploadState.Uploading)
            {
                var hasWork = false;

                // TODO: combine this and the check in uploadThread
                // TODO: fix
                for (var i = 0; i < files.length; i++)
                {
                    var file = files[i];

                    if (file.action == "calculatesize" || file.state == "checking" || (settings.status.state == kw.UploadState.Uploading && (file.state == "uploading" || file.state == "ready")))
                    {
                        hasWork = true;

                        break;
                    }
                }

                if (hasWork)
                {
                    log(settings.id + " - session still has uploads in process - " + settings.uploadSessionId + " - setting autoCompleteAfterLastFile = true", settings.status);

                    settings.autoCompleteAfterLastFile = true;

                    return;
                }
                else
                {
                    isCompleting = true;
                }
            }

            if (files.length == 0 && settings.status.state != kw.UploadState.Error)
            {
                settings.status.state = kw.UploadState.Error;
                settings.status.errorType = kw.UploadErrorType.Cancelled;
            }
            else
            {
                var hasSuccessfulFiles = false;

                for (var i = 0; i < files.length; i++)
                {
                    var status = files[i].get_Status();

                    if (status && !status.isErrored)
                    {
                        hasSuccessfulFiles = true;

                        break;
                    }
                }

                if (!hasSuccessfulFiles)
                    settings.status.errorType = kw.UploadErrorType.Other;

                settings.status.state = (settings.status.errorType == kw.UploadErrorType.None) ? kw.UploadState.Completing : kw.UploadState.Error;
            }

            log(settings.id + " - session starting completion - " + settings.uploadSessionId, settings.status);

            callHandlers(events.beforeSessionEndHandlers, settings.status);

            if (settings.uploadForm && settings.uploadForm.validationCallbacks)
                settings.uploadForm.validationCallbacks.splice(indexOf(settings.uploadForm.validationCallbacks, onFormSubmitted), 1);

            if (settings.completeHandlerUrl)
                executeUploadCompleteHandler();
            else if (settings.uploadForm)
                executeUploadCompletePostBack();

            if (!isAutoComplete)
            // Ensure post processing occurs
                uploadThread();

            if (settings.uploadForm || settings.completeHandlerUrl)
            {
                log(settings.id + " - session completing - " + settings.uploadSessionId, settings.status);

                onSessionProgressInternal(this, {});
            }
            else
            {
                log(settings.id + " - session finished with no completion handler, and will be rolled back - " + settings.uploadSessionId, settings.status);
            }
        };

        var executeUploadCompletePostBack = function ()
        {
            //alert("completing");

            // pass back upload execution information
            if (!uploadSessionIdElement)
            {
                uploadSessionIdElement = document.createElement("input");

                uploadSessionIdElement.type = "hidden";
                uploadSessionIdElement.name = "kw_uploadSessionId";

                settings.uploadForm.appendChild(uploadSessionIdElement);
            }

            if (!uploadErrorTypeElement)
            {
                uploadErrorTypeElement = document.createElement("input");

                uploadErrorTypeElement.type = "hidden";
                uploadErrorTypeElement.name = "kw_uploadErrorType";

                settings.uploadForm.appendChild(uploadErrorTypeElement);
            }

            if (!sourceConnectorIdElement)
            {
                sourceConnectorIdElement = document.createElement("input");

                sourceConnectorIdElement.type = "hidden";
                sourceConnectorIdElement.name = "kw_sourceConnectorId";

                settings.uploadForm.appendChild(sourceConnectorIdElement);
            }

            if (!failedRequestsElement)
            {
                failedRequestsElement = document.createElement("input");

                failedRequestsElement.type = "hidden";
                failedRequestsElement.name = "kw_failedRequests";

                settings.uploadForm.appendChild(failedRequestsElement);
            }

            if (!cancelledRequestsElement)
            {
                cancelledRequestsElement = document.createElement("input");

                cancelledRequestsElement.type = "hidden";
                cancelledRequestsElement.name = "kw_cancelledRequests";

                settings.uploadForm.appendChild(cancelledRequestsElement);
            }

            if (!uploadDataElement && objectSize(settings.data) > 0)
            {
                uploadDataElement = document.createElement("input");

                uploadDataElement.type = "hidden";
                uploadDataElement.name = "kw_uploadData";

                settings.uploadForm.appendChild(uploadDataElement);
            }

            uploadSessionIdElement.value = settings.uploadSessionId;
            uploadErrorTypeElement.value = settings.status.errorType;
            sourceConnectorIdElement.value = settings.id;

            var failedRequests = [];

            for (var i = 0; i < files.length; i++)
            {
                var status = files[i].get_Status();

                if (status && status.isErrored)
                    failedRequests.push(files[i].get_Id());
            }

            if (failedRequests.length)
                failedRequestsElement.value = failedRequests.join(",");
            if (cancelledRequests.length)
                cancelledRequestsElement.value = cancelledRequests.join(",");

            if (objectSize(settings.data) > 0)
                uploadDataElement.value = queryStringSerialize(settings.data);

            //settings.uploadForm.enctype = form.encoding = "application/x-www-form-urlencoded";
            // clear our handler if it exists
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
                Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, onFormSubmitted);
            if (settings.autoUploadOnSubmit)
                settings.uploadForm.onsubmit = originalOnSubmit;

            if (submittedElement == null || isManuallyStartedUpload)
            {
                if (settings.postbackFunction)
                {
                    if (typeof settings.postbackFunction == "function")
                        settings.postbackFunction();
                    else
                        eval(settings.postbackFunction);
                }
                else
                {
                    // settings.uploadForm.submit();
                    // Instead, we create a dummy submit button and click it. This makes uploadForm.onsubmit handlers be called,
                    // which makes Ajax.BeginForm work, among other things.
                    var submitButton = document.createElement("input");

                    submitButton.type = "submit";
                    submitButton.style.display = "none";

                    settings.uploadForm.appendChild(submitButton);

                    submitButton.click();

                    settings.uploadForm.removeChild(submitButton);
                }
            }
            else
            {
                var submitFunction = function (wasDisabled)
                {
                    if (submittedElement.nodeName == "A" && submittedElement.href && submittedElement.href.toLowerCase().match("^javascript:"))
                    {
                        if (!submittedElement.onclick || submittedElement.onclick() != false)
                        {
                            // TODO: maybe try both ways?
                            eval(decodeURI(submittedElement.href));
                        }
                        else
                        {
                            // TODO: error?
                        }
                    }
                    else if (submittedElement.click)
                    {
                        submittedElement.click();
                    }
                    else
                    {
                        // TODO: error?
                    }

                    if (wasDisabled)
                        submittedElement.disabled = true;
                };

                if (submittedElement.disabled)
                {
                    submittedElement.disabled = false;

                    window.setTimeout(function () { submitFunction(true); }, 1);
                }
                else
                {
                    submitFunction();
                }
            }

            //callHandlers(events.uploadSessionEndedHandlers, settings.status);

            isCompleting = false;
            //alert("completed");
        };

        var executeUploadCompleteHandler = function ()
        {
            if (settings.completeHandlerUrl)
            {
                var url = settings.completeHandlerUrl + (settings.completeHandlerUrl.indexOf("?") > 0 ? "&" : "?");

                url += "uploadSessionId=" + settings.uploadSessionId;

                if (settings.uploadProfile)
                    url += "&uploadProfile=" + settings.uploadProfile;

                if (settings.status.errorType)
                    url += "&uploadErrorType=" + settings.status.errorType;

                var failedRequests = [];

                for (var i = 0; i < files.length; i++)
                {
                    var status = files[i].get_Status();

                    if (status && status.isErrored)
                        failedRequests.push(files[i].get_Id());
                }

                if (failedRequests.length)
                    url += "&failedRequests=" + failedRequests.join(",");
                if (cancelledRequests.length)
                    url += "&cancelledRequests=" + cancelledRequests.join(",");

                if (settings.id)
                    url += "&sourceConnectorId=" + settings.id;

                if (settings.completionContentType == "application/x-www-form-urlencoded" && !settings.completionBody)
                {
                    var completionData = {};
                    var fileListList = [];

                    for (var key in kw._components)
                    {
                        var component = kw._components[key];

                        if (component.constructor == kw.FileList)
                            fileListList.push(component);
                    }

                    for (var i = 0; i < fileListList.length; i++)
                    {
                        var elements = fileListList[i].get_Element().getElementsByTagName("*");

                        for (var j = 0; j < elements.length; j++)
                        {
                            var el = elements[j];

                            if (el.name && el.value)
                                completionData[el.name] = el.value;
                        }
                    }

                    settings.completionBody = queryStringSerialize(completionData);
                }

                var executeCompleteAction = function()
                {
                    var xhr = getXmlReq();

                    xhr.open(settings.completionMethod || "POST", url, true);

                    bind(xhr, "readystatechange", this, onUploadCompleteHandlerReadyStateChange, null, true);

                    if (settings.completionContentType)
                        xhr.setRequestHeader("Content-Type", settings.completionContentType);

                    if (objectSize(settings.data) > 0)
                        xhr.setRequestHeader("X-SlickUpload-Data", queryStringSerialize(settings.data));

                    xhr.send(settings.completionBody || "");
                };

                /*if (!settings.documentDomain || support.cors)
                    executeCompleteAction();
                else
                {*/
                    kw._completionActions.push(executeCompleteAction);

                    if (completionIframeElement && completionIframeElement.parentNode)
                        completionIframeElement.parentNode.removeChild(completionIframeElement);

                    completionIframeElement = executeCompleteAction.iframe = addIFrame(null, getUrl("complete"));
                //}
            }
        };

        var onUploadCompleteHandlerReadyStateChange = function (e, source)
        {
            if (source.readyState == 4)
            {
                settings.status.state = (source.status == 200 && settings.status.errorType == kw.UploadErrorType.None) ? kw.UploadState.Complete : kw.UploadState.Error;

                callHandlers(events.uploadSessionEndedHandlers, settings.status, source.responseText);

                log(settings.id + " - session completed - " + settings.uploadSessionId, settings.status);

                unbind(source, "readystatechange", null, true);

                isCompleting = false;

                if (completionIframeElement && completionIframeElement.parentNode)
                    completionIframeElement.parentNode.removeChild(completionIframeElement);
            }
        };

        var onBeforeUnload = function (e)
        {
            if (settings.status.state == kw.UploadState.Uploading)
            {
                e.returnValue = settings.confirmNavigateDuringUploadMessage;

                return settings.confirmNavigateDuringUploadMessage;
            }
        };

        this._dispose = function ()
        {
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
            {
                Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, onFormSubmitted);
            }

            if (uploadSessionIdElement && uploadSessionIdElement.parentNode)
                uploadSessionIdElement.parentNode.removeChild(uploadSessionIdElement);
            if (uploadErrorTypeElement && uploadErrorTypeElement.parentNode)
                uploadErrorTypeElement.parentNode.removeChild(uploadErrorTypeElement);
            if (sourceConnectorIdElement && sourceConnectorIdElement.parentNode)
                sourceConnectorIdElement.parentNode.removeChild(sourceConnectorIdElement);
            if (failedRequestsElement && failedRequestsElement.parentNode)
                failedRequestsElement.parentNode.removeChild(failedRequestsElement);
            if (cancelledRequestsElement && cancelledRequestsElement.parentNode)
                cancelledRequestsElement.parentNode.removeChild(cancelledRequestsElement);
            if (uploadDataElement && uploadDataElement.parentNode)
                uploadDataElement.parentNode.removeChild(uploadDataElement);
        }

        // constructor
        if (support.isSupportedBrowser)
        {
            if (options && options.state)
                delete options.state;

            componentConstructor(this, settings, options, events, false, "su-uploadconnector");

            if (settings.documentDomain)
                document.domain = settings.documentDomain;

            assert(settings.uploadHandlerUrl, "uploadHandlerUrl is required.");

            resolveElement(settings, "uploadForm", false);

            assert(settings.uploadForm || !settings.autoUploadOnSubmit, "uploadForm must be specified if autoUploadOnSubmit is true.");

            // ensure data isn't null
            settings.data = settings.data || {};

            if (settings.autoUploadOnSubmit || (settings.uploadForm && settings.autoUploadOnSubmit != false))
            {
                if (settings.autoUploadTriggerIdList)
                {
                    if (typeof (settings.autoUploadTriggerIdList) == "string")
                        settings.autoUploadTriggerIdList = settings.autoUploadTriggerIdList.split(",");

                    if (settings.autoUploadTriggerIdList)
                    {
                        for (var i = 0; i < settings.autoUploadTriggerIdList.length; i++)
                            settings.autoUploadTriggerIdList[i] = trim(settings.autoUploadTriggerIdList[i]);
                    }
                }

                wireAutoUploadOnSubmit();
            }

            if (settings.confirmNavigateDuringUploadMessage)
                bind(window, "beforeunload", this, onBeforeUnload);

            if (support.html5Upload)
                uploadConnectorImpl = new html5UploadConnector();
            else
                uploadConnectorImpl = new html4UploadConnector();

            kw._frameLoadedHandlers.push(function (data)
            {
                if (data.uploadSessionId == settings.uploadSessionId)
                {
                    if (data.uploadRequestId)
                    {
                        for (var i = 0; i < files.length; i++)
                        {
                            var file = files[i];

                            if (file.get_Id() == data.uploadRequestId)
                            {
                                onRequestProgressInternal(file, data);

                                break;
                            }
                        }
                    }
                    else
                        onSessionProgressInternal(this, data);
                }
            });

            log("UploadConnector constructed", settings.id, "Html" + (support.html5Upload ? "5" : "4") + " mode", settings, this);
        }
        else
            log("UploadConnector NOT constructed -- unsupported browser", settings.id, settings, this);

        kw._licenseHandlerUrl = getUrl("license");
    };

    kw.SlickUpload = function (options)
    {
        // fields
        var 
			settings = {
			    id: null,
			    element: null,
			    fileSelector: null,
			    fileList: null,
			    uploadProgressDisplay: null,
			    uploadConnector: null
			};

        var mergeFilter = function (key)
        {
            var underScorePos = key.indexOf("_");

            if (underScorePos == 0)
                return false;
            else if (underScorePos > 0)
                key = key.substr(underScorePos + 1);

            switch (key.toLowerCase())
            {
                case "templateelement":
                case "unskinnedelement":
                case "unsupportedelement":
                case "fileselector":
                case "uploadconnector":
                    return false;
                default:
                    return true;
            }
        };

        // TODO: implement props/methods

        // constructor
        componentConstructor(this, settings, options, null, true, "su-slickupload");

        delete this["get_FileSelector"];
        delete this["get_FileList"];
        delete this["get_UploadProgressDisplay"];
        delete this["get_UploadConnector"];

        resolveComponent(settings, "fileSelector", kw.FileSelector, "FileSelector", true);
        resolveComponent(settings, "fileList", kw.FileList, "FileList", false);
        resolveComponent(settings, "uploadProgressDisplay", kw.UploadProgressDisplay, "UploadProgressDisplay", false);
        resolveComponent(settings, "uploadConnector", kw.UploadConnector, "UploadConnector", true);

        mergeComponent(this, settings.fileSelector, mergeFilter);
        if (settings.fileList)
            mergeComponent(this, settings.fileList, mergeFilter);
        if (settings.uploadProgressDisplay)
            mergeComponent(this, settings.uploadProgressDisplay, mergeFilter);
        mergeComponent(this, settings.uploadConnector, mergeFilter);

        log("SlickUpload constructed", settings.id, settings, this);
    };

    kw.DropZoneManager = new (function ()
    {
        var zoneElements = [];
        var leaveTimeout = null;
        var areZonesVisible = false;

        var isValidDrag = function (dt)
        {
            // TODO: see if this is cross browser
            //if (dt && dt.files && dt.types && dt.types.length > 0 && dt.types[0] == "Files")
            return dt && ((dt.files && dt.files.length > 0) || (dt.types && indexOf(dt.types, "Files") != -1));
        };

        var onDocumentDragEnter = function (e)
        {
            var dt = e.dataTransfer;

            if (isValidDrag(dt))
                setZonesVisible(true);
        };

        var onDocumentDragOver = function (e)
        {
            var dt = e.dataTransfer;

            if (dt)
                dt.dropEffect = "none";

            if (!areZonesVisible && isValidDrag(dt))
                setZonesVisible(true);

            e.stopPropagation();
            e.preventDefault();

            resetLeaveTimeout();
        };

        var onDocumentDragLeave = function (e)
        {
            var relatedTarget = kw.dropZoneWindow.document.elementFromPoint(e.clientX, e.clientY);

            var isOutsideBody = false;

            if (!relatedTarget || relatedTarget.nodeName == "HTML" || (e.clientX == 0 && e.clientY == 0))
            {
                isOutsideBody = true;
            }
            else if (relatedTarget.nodeName == "BODY")
            {
                var size = windowSize();

                // TODO: test if 10px margin is enough or too much
                isOutsideBody = ((e.clientX <= 10 && e.clientY <= 10) || (e.clientX >= size.width - 10 && e.clientY >= size.height - 10));
            }

            if (e.dataTransfer == null || isOutsideBody)
                setZonesVisible(false);
        };

        var onZoneDragOver = function (e)
        {
            var dt = e.dataTransfer;

            if (dt)
            {
                if (dt.effectAllowed == "move" || dt.effectAllowed == "linkMove")
                    dt.dropEffect = "move";
                else
                    dt.dropEffect = "copy";
            }

            e.stopPropagation();
            e.preventDefault();

            resetLeaveTimeout();
        };

        var onZoneDragEnter = function (e)
        {
            var relatedTarget = kw.dropZoneWindow.document.elementFromPoint(e.clientX, e.clientY);

            if (relatedTarget == this || withinElement(this, e, relatedTarget))
                addClass(this, "kw-dragover");
        };

        var onZoneDragLeave = function (e)
        {
            var relatedTarget = kw.dropZoneWindow.document.elementFromPoint(e.clientX, e.clientY);

            if (relatedTarget != this && !withinElement(this, e, relatedTarget))
                removeClass(this, "kw-dragover");
        };

        var onZoneDrop = function (e)
        {
            e.stopPropagation();
            e.preventDefault();

            var dt = e.dataTransfer;

            log("DropZoneManager - File drop detected.");

            this._onDrop(dt.files);

            setZonesVisible(false);
        };

        var resetLeaveTimeout = function ()
        {
            // TODO: browser sniff (only should be needed on firefox)
            if (leaveTimeout)
                clearTimeout(leaveTimeout);

            leaveTimeout = setTimeout(function () { setZonesVisible(false); }, 100);
        };

        var setZonesVisible = function (isVisible)
        {
            for (var i = 0; i < zoneElements.length; i++)
            {
                var el = zoneElements[i];

                if (isVisible)
                {
                    addClass(el, "kw-dragging");

                    if (el._showDropZoneOnDocumentDragOver)
                        showElement(el);
                }
                else
                {
                    removeClass(el, "kw-dragging");

                    if (el._showDropZoneOnDocumentDragOver)
                        el.style.display = "none";
                }
            }

            areZonesVisible = isVisible;
        };

        this.registerDropZone = function (el, dropTarget, showDropZoneOnDocumentDragOver)
        {
            if (zoneElements.length == 0)
            {
                var zoneDocument = kw.dropZoneWindow.document;

                bind(zoneDocument, "dragover", zoneDocument, onDocumentDragOver);
                bind(zoneDocument, "dragenter", zoneDocument, onDocumentDragEnter);
                bind(zoneDocument, "dragleave", zoneDocument, onDocumentDragLeave);
            }

            addClass(el, "su-dropzone");

            el.style.zIndex = "1000";

            bind(el, "dragenter", el, onZoneDragEnter, true);
            bind(el, "dragleave", el, onZoneDragLeave, true);
            bind(el, "dragover", el, onZoneDragOver);
            bind(el, "drop", dropTarget, onZoneDrop);

            el._showDropZoneOnDocumentDragOver = showDropZoneOnDocumentDragOver;

            zoneElements.push(el);
        };

        // static constructor
        if (support.browser == "msie" && support.browserVersion <= 9)
        {
            kw(function ()
            {
                var cancelDropFunction = function (e) { e.returnValue = false; };

                var zoneBody = kw.dropZoneWindow.document.body;

                bind(zoneBody, "dragenter", null, cancelDropFunction);
                bind(zoneBody, "dragover", null, cancelDropFunction);
                bind(zoneBody, "drop", null, cancelDropFunction);
            });
        }
    })();

    var init = function ()
    {
        if (!kw._hasWindowLoaded)
        {
            log("Feature support", support);

            if (window._kwInit)
            {
                for (var i = 0; i < _kwInit.length; i++)
                {
                    var initFunction = _kwInit[i];

                    // TODO: error if not
                    //if (typeof initFunction == "function")
                        kw(initFunction);
                }
            }
            else
            {
                window._kwInit = [];
            }

            _kwInit.push = kw;

            for (var i = 0; i < kw._initFunctionList.length; i++)
                kw._initFunctionList[i]();

            kw._initFunctionList = [];

            kw._hasWindowLoaded = true;

            // Check licensing
            /*if (kw._licenseHandlerUrl)
            {
                var req = getXmlReq();

                req.open("GET", kw._licenseHandlerUrl, true);

                var licenseTimeout = window.setTimeout(function () { handleLicense(null); }, 10000);

                bind(req, "readystatechange", req, function ()
                {
                    if (req.readyState == 4)
                    {
                        var data = null;

                        try
                        {
                            data = eval("(" + req.responseText + ")");
                        }
                        catch (ex)
                        {
                            data = null;
                        }

                        if (objectSize(data) == 0)
                            data = null;

                        handleLicense(data);

                        window.clearTimeout(licenseTimeout);
                    }
                }, null, true);

                req.send("");
            }
            else
            {
                handleLicense(null);
            }*/
        }
    };

    if (document.readyState && document.readyState == "complete")
    {
        init();
    }
    else
    {
        bind(window, "load", this, init);
        bind(window, "DOMContentLoaded", this, init);

        if (document.getElementsByTagName && (document.getElementsByTagName("body")[0] != null || document.body != null))
        {
            var lastCount = null;

            var interval = window.setInterval(function ()
            {
                var count = document.getElementsByTagName("*").length;

                if (count == lastCount || kw._hasWindowLoaded)
                {
                    window.clearInterval(interval);

                    if (!kw._hasWindowLoaded)
                        init();
                }
                else
                {
                    lastCount = count;
                }
            }, 200);
        }
    }

    try
    {
        if (window.Sys)
        {
            // Apply hack to fix webkit browsers on older ASP.NET AJAX versions, as per http://forums.asp.net/t/1252014.aspx/1
            // TODO: test with v4 to make sure we're not breaking anything
            if (Sys.Browser && !Sys.Browser.WebKit)
            {
                Sys.Browser.WebKit = {}; // Safari 3 is considered WebKit

                if (navigator.userAgent.indexOf('WebKit/') > -1)
                {
                    Sys.Browser.agent = Sys.Browser.WebKit;
                    Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
                    Sys.Browser.name = 'WebKit';
                }
            }

            if (Sys.Application)
            {
                Sys.Application.notifyScriptLoaded();
            }
        }
    }
    catch (ex)
	{ }

    window.kw = kw;
})();