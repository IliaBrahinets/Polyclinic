moment.locale('ru');
$('#datePicker').daterangepicker({
    //singleDatePicker: true,
    moment: moment,
    autoApply: true
});