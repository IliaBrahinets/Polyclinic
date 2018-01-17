moment.locale('ru');

$('#datePicker').daterangepicker({
    autoUpdateInput: false,
    autoApply: true,
    moment: moment,
    locale: {
        format: "DD.MM.YYYY",
        separator: " - "
    }
});

$('#datePicker').on('apply.daterangepicker', function (event, picker) {
    $(this).val(picker.startDate.format("DD.MM.YYYY") + ' - ' + picker.endDate.format('DD.MM.YYYY'));
});

$('#datePicker').on('cancel.daterangepicker', function (event, picker) {
    $(this).val('');
});