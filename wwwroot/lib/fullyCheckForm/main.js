
$(".fullyCheckForm").each(function (index, element) {

    element = $(element);

    var checkUrl = element.attr("data-check-url");
    var method = element.attr("method") || "get";

    var messageCont = element.find(element.attr("data-message-elem"));

    function emptyListener() {
        messageCont.text("");
    }

    element.on("change", emptyListener);

    element.on("submit",function listener() {
        event.preventDefault();

        if (!$(this).valid()) return false;

        $.ajax({
            url: checkUrl,
            data: element.serialize(),
            type: method,
            dataType:"json",
            success: function (data, status) {
                if (data === true) {

                    element.off("submit", listener);

                    messageCont.text("");
                    element.off("change", emptyListener);

                    element.submit();

                } else {
                    messageCont.text(data);
                }
            }
        });

        return false;
    });
});