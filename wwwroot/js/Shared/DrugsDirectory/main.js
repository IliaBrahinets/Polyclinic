$(".sendWithAddition").click(function () {

    var DataNames = JSON.parse($(this).attr("data-to-add"));

    //taking a data from inputs by the names

    var RequestStr = '&';

    for (var i = 0; i < DataNames.length; i++) {

        var data =$(this).closest(".data-group").find("[data-name=" + DataNames[i] + "]");

        RequestStr += DataNames[i] + "=" + data.text() + "&";

    }

    var tmp = $(this).attr("href") || '';
    $(this).attr("href", tmp + RequestStr);
}); 

 
$(".clickShowHideNext").click(function () {

    var Data = $(this).closest(".data-group").find(".AdditionalData");

    var next = $(this).next();

    if (next.hasClass("hide")) {

        Data.addClass("form-control");

        Data.attr("contenteditable", "true");

        next.removeClass("hide");
        next.addClass("show");

    } else {

        Data.removeClass("form-control");

        Data.removeAttr("contenteditable");

        next.removeClass("show");
        next.addClass("hide");

    }

});