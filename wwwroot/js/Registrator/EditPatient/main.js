moment.locale('ru');

$("#birthdate").daterangepicker({
    singleDatePicker: true,
    moment: moment,
    startDate: ($("#birthdate").val() || "30.06.1972"),
    autoApply:true
});

googleMapsAutoCompleteInit(document.getElementsByClassName("streetInput")[0]);


$(function SetCustomMessage() {
    $("[name=HouseNumber]").rules().remote.complete = function (xhr) {
        if (xhr.status == 200 && xhr.responseText === 'true') {

            var Id = xhr.getResponseHeader("Id");

            if ((Id == null) || (Id == "")) return;

            var RegionId = $(this).find("[name=RegionId]");

            RegionId.val(Id);
        }
    };
});

