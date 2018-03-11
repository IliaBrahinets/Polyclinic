
$(".fullyCheckForm").each(function (index, form) {

    form = $(form);

    var checkUrl = form.attr("data-check-url");
    var method = form.attr("method") || "get";

    var messageCont = form.find(form.attr("data-message-elem"));

    function emptyListener() {
        messageCont.text("");
    }

    form.on("change", emptyListener);

    form.on("submit",function listener() {
        event.preventDefault();

        $(this).validate();

        if (!$(this).valid()) return false;

        $.ajax({
            url: checkUrl,
            data: form.serialize(),
            type: method,
            dataType:"json",
            success: function (data, status) {
                if (data === true) {


                    form.off("submit", listener);

                    messageCont.text("");

                    form.submit();

                    element.off("submit", listener);

                    messageCont.text("");
            }
        }});

        return false;
    });
});