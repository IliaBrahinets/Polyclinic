moment.locale('ru');

$("#birthdate").daterangepicker({
    singleDatePicker: true,
    moment: moment,
    autoApply: true
});