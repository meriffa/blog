var ByteZoo = ByteZoo || {};
ByteZoo.Blog = ByteZoo.Blog || {};
ByteZoo.Blog.Web = ByteZoo.Blog.Web || {};
(function (namespace, $, undefined) {

    // Display error
    namespace.displayError = function (jqXHR, textStatus, errorThrown) {
        alert("ERROR: " + errorThrown);
    }

    // Log message
    namespace.logMessage = function (text) {
        console.log(`[${namespace.formatDateTimeMilliseconds(new Date())}] ${text}`);
    }

}(ByteZoo.Blog.Web.Site = ByteZoo.Blog.Web.Site || {}, jQuery));