$(".sendWithAddition").click(function () {

    var InputNames = JSON.parse($(this).attr("data-to-add"));

    var ReqNames;

    try {
        ReqNames = JSON.parse($(this).attr("data-to-add-reqname"));
    } catch (SyntaxError) {
        ReqNames = [];
    }

    //taking a data from inputs by the names

    var RequestStr = '';

    var data_group = $(this).closest(".data-group");

    for (var i = 0; i < InputNames.length; i++) {

        RequestStr += '&';

        var Input = data_group.find("input[name=" + InputNames[i] + "]");

        RequestStr += (ReqNames[i] || InputNames[i]) + "=" + encodeURIComponent(Input.val());

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

        Inputs.attr("readonly", "");

        next.removeClass("show");
        next.addClass("hide");

    }

});