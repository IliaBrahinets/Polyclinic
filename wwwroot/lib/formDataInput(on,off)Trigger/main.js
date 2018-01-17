//this func uses == for equaling corresponding elements in Arrays not === 
Array.prototype.equals = function (arr) {
    if (Array.isArray(arr)) {

        if (this.length !== arr.length)
            return false;

        for (var i = 0; i < this.length; i++)
            if (this[i] != arr[i])
                return false;

        return true;
    }

    return false;
}

$(".triggered").each(function (index, element) {
    var elem = $(element);

    //disabled by default
    var hideAttr = elem.data("trigger-hideAttr") || "disabled";

    //can be an enable trigger or an disable trigger
    var triggerType = elem.data("trigger-type");

    var form = elem.closest("form");

    var toggle = form.find("[name=" + elem.data("trigger-elem") + "]");
    var toggleValues = elem.data("trigger-val");

    if (!(toggleValues instanceof Array)) {
        toggleValues = [toggleValues];
    }
    //if equal => true
    var valueCheckers = {

        "input": function (elem, values) {

            elem = $(elem);
      
            if (elem.attr("type") == "checkbox" || elem.attr("type") == "radio")
                if (elem.is(":checked"))
                    return true;
                else
                    return false;

                
            var elemVal = elem.val();
            
            for (var i = 0; i < values.length; i++)
                if (elemVal == values[i]) return true;

            return false;
        },

        //for select we must check both the value and the innerText of the selected option
        "select": function (elem, values) {
            elem = $(elem);

            if (elem[0].hasAttribute("multiple")) {

                var elemValues = elem.val() || [];

                var elemTexts = [];

                elem.find(":selected").each(function (index, elem) {
                    elemTexts.push($(elem).text());
                });
                console.log(elemValues, elemTexts)

                if (values.equals(elemValues) || values.equals(elemTexts))
                    return true;

                return false;
            } else {

                var elemVal = elem.val();
                var elemText = elem.find(":selected").text();

                for (var i = 0; i < values.length; i++)
                    if (elemVal == values[i] || elemText == values[i]) return true;

                return false;
            }


        }

    };
    valueCheckers["textarea"] = valueCheckers["input"];

    var valueChecker = valueCheckers[toggle.prop("tagName").toLowerCase()];

    toggle.on("change", function listener(event) {
        var isFound = valueChecker(event.target, toggleValues);

        if (triggerType == "disable")
            isFound = !isFound;

        if (isFound) {
            elem.removeAttr(hideAttr);
        } else {
            elem.attr(hideAttr, "");
        }
    });

    toggle.trigger("change");
});