
$.fn.clearValidation = function () {
    $(this).find(".field-validation-error").html("");
};

jQuery.validator.addMethod("moreThanInput", function (value, element, inputName) {

    var min = $(element).closest("form").find("input[name=" + inputName + "]");

    return value > min.val();

}, "must be more than {0}");