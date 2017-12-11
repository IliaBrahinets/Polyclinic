$(".sendWithAddition").click(function () {

    var InputNames = JSON.parse($(this).attr("data-to-add"));

    //taking a data from inputs by the names

    var RequestStr = '&';

    for (var i = 0; i < InputNames.length; i++) {

        var Input = $(this).closest(".data-group").find("input[name=" + InputNames[i] + "]");

        RequestStr +=  InputNames[i] + "=" + Input.val() + "&";

    }

    var tmp = $(this).attr("href") || '';
    $(this).attr("href", tmp + RequestStr);
});

 
$(".clickShowHideNext").click(function () {

    var Inputs = $(this).closest(".data-group").find("input");

    var next = $(this).next();

    if (next.hasClass("hide")) {

        Inputs.removeClass("form-control-plaintext");
        Inputs.addClass("form-control");

        Inputs.removeAttr("readonly");

        next.removeClass("hide");
        next.addClass("show");

    } else {

        Inputs.removeClass("form-control");
        Inputs.addClass("form-control-plaintext");

        Inputs.attr("readonly","");

        next.removeClass("show");
        next.addClass("hide");

    }

});