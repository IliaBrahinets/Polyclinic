
$("#addStreetField").click(function () {

    var streetField = $("#CreateRegionForm .streetField").first();

    var newstreetField = $(streetField).clone();

    var nextNumber = $(".streetField").parent().children().length;


    //change names for example: [0].name to [1].name
    newstreetField.find("input").each(function (index, elem) {

        elem = $(elem);

        var tmp = elem.attr("name");
      
        tmp = tmp.replace("[" + (nextNumber - 1) + "]", "[" + nextNumber + "]");
       
        elem.attr("name", tmp);


    });

    newstreetField.find("span").each(function (index, elem) {

        elem = $(elem);

        if (elem[0].hasAttribute("data-valmsg-for")) {

            var tmp = elem.attr("data-valmsg-for");

            tmp = tmp.replace("[" + (nextNumber - 1) + "]", "[" + nextNumber + "]");

            elem.attr("data-valmsg-for", tmp);

        }

    });

    googleMapsAutoCompleteInit(newstreetField.find(".streetInput")[0]);

    $(newstreetField).appendTo(".streetFieldsContainer");

    //for validation working
    $('#CreateRegionForm').removeData('validator');
    $('#CreateRegionForm').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('#CreateRegionForm');

    //to work the delete button of the new streetField
    SetDeleteListener();
});

function SetDeleteListener() {

    $(".deleteStreetField").click(function () {

        var StreetField = $(this).closest(".streetField");

        if (StreetField.parent().children().length <= 1) return;

        StreetField.remove();
    });

}

SetDeleteListener();
googleMapsAutoCompleteInit(document.getElementsByClassName("streetInput")[0]);

//to pass a collection of streets
/*
$("#CreateRegionForm").submit(function () {
    event.preventDefault();

    if ( ! $(this).valid() ) return;

    var data = $(this).serializeArray();

    var SendObj = {};
    //-1 because of a RequestToken
    for (var i = 0; i < data.length - 1; i += 2) {

        var SendObjNumb = i / 2;

        SendObj["[" + SendObjNumb + "]." + data[i].name] = data[i].value;
        SendObj["[" + SendObjNumb + "]." + data[i + 1].name] = data[i + 1].value;
    }

    SendObj[data[data.length - 1].name] = data[data.length - 1].value;

    $.ajax("/Registrator/CreateRegion",
        {
            method: "POST", 
            url: "/Registrator/CreateRegion",
            data: SendObj,
            success: function () {
                location.href = "/Registrator/Regions"
            }
        });

});
*/

