moment.locale('ru');

$('#birthdate').daterangepicker({
    autoUpdateInput: false,
    autoApply: true,
    moment: moment,
    locale: {
        format: "DD.MM.YYYY",
        separator: " - "
    }
});

$('#birthdate').on('apply.daterangepicker', function (event, picker) {
    $(this).val(picker.startDate.format("DD.MM.YYYY") + ' - ' + picker.endDate.format('DD.MM.YYYY'));
});

$('#birthdate').on('cancel.daterangepicker', function (event, picker) {
    $(this).val('');
});