var ByteZoo = ByteZoo || {};
ByteZoo.Blog = ByteZoo.Blog || {};
ByteZoo.Blog.Web = ByteZoo.Blog.Web || {};
(function (namespace, $, undefined) {

    // Send get request
    namespace.getRequest = function (contentId, requestUrl) {
        $(contentId).show();
        $.ajax({
            method: "GET",
            contentType: "application/json",
            url: requestUrl,
            success: function () {
                namespace.logMessage(`Request '${requestUrl}' completed.`);
                $(contentId).hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                namespace.displayError(jqXHR, textStatus, errorThrown);
                $(contentId).hide();
            }
        });
        namespace.logMessage(`Request '${requestUrl}' submitted.`);
    }

    // Display error
    namespace.displayError = function (jqXHR, textStatus, errorThrown) {
        alert(`ERROR: ${jqXHR.status} - ${jqXHR.statusText} (${jqXHR.responseText})`);
    }

    // Convert UTC to local date & time
    function convertToLocalDateTime(value) {
        return new Date(value.getTime() - value.getTimezoneOffset() * 60 * 1000);
    }

    // Format date & time
    namespace.formatDateTimeMilliseconds = function (value) {
        return value != null ? convertToLocalDateTime(new Date(value)).toISOString().replace("T", " ").replace("Z", "") : null;
    }

    // Log message
    namespace.logMessage = function (text) {
        console.log(`[${namespace.formatDateTimeMilliseconds(new Date())}] ${text}`);
    }

    // Sleep (duration [ms])
    namespace.sleep = function (duration) {
        return new Promise(resolve => setTimeout(resolve, duration));
    }

}(ByteZoo.Blog.Web.Site = ByteZoo.Blog.Web.Site || {}, jQuery));