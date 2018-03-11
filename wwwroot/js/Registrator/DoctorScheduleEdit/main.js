
$(document).ready(function () {
    //RequestData
    // Assuming you've got the appropriate language files,
    // clndr will respect whatever moment's language is set to.

    moment.locale("ru");


    // The order of the click handlers is predictable. Direct click action
    // callbacks come first: click, nextMonth, previousMonth, nextYear,
    // previousYear, nextInterval, previousInterval, or today. Then
    // onMonthChange (if the month changed), inIntervalChange if the interval
    // has changed, and finally onYearChange (if the year changed).

     var schedule = $('#schedule').clndr({

        moment: moment,

        numberOfRows: 5,

        dateParameter: 'Date',

        constraints: {
            startDate: moment.utc($.getJSON("GetCurrentDate")).add(-1, "day")
        },

        trackSelectedDate: true,

        ignoreInactiveDaysInSelection: true,

        clickEvents: {
            click: function (target) {

                RelieveSetted.DeleteAll();

                RelieveSetted.Add(target.events);

            },
            today: function () {
                console.log('Cal-1 today');
            },
            nextMonth: function () {

                schedule.removeEvents(function () {
                    return true;
                });

                schedule.getRelieves();

            },
            previousMonth: function () {

                schedule.removeEvents(function () {
                    return true;
                });

                schedule.getRelieves();

            },
            onMonthChange: function () {
                console.log('Cal-1 month changed');
            },
            nextYear: function () {
                console.log('Cal-1 next year');
            },
            previousYear: function () {
                console.log('Cal-1 previous year');
            },
            onYearChange: function () {
                console.log('Cal-1 year changed');
            },
            nextInterval: function () {
                console.log('Cal-1 next interval');
            },
            previousInterval: function () {
                console.log('Cal-1 previous interval');
            },
            onIntervalChange: function () {
                console.log('Cal-1 interval changed');
            }
        },
        doneRendering: function () {

        },
        showAdjacentMonths: true,
        adjacentDaysChangeMonth: false,
        template: $("#scheduletemplate").html()
    });

    schedule.getRelieves = function (callback) {

        var currDate = schedule.intervalStart;

        $.ajax({
            url: "/Registrator/Relieves",
            dataType: "json",
            cache: false,
            data: {
                DoctorId: RequestData.DoctorId,
                year: currDate.format("YYYY"),
                month: currDate.format("MM")
            },
            success: function (response) {
                schedule.addEvents(response);
                if (callback)
                    callback();
            }
        });

     };

    var RelieveUsed = new ListView({
        container: $("#relievesUsed"),
        template: $("#relievesUsedTemplate").html()
    });


    var RelieveSetted = new ListView({
        container: $("#relievesSetted"),
        template: $("#relievesSettedTemplate").html()
    });


    console.log(schedule);
    //modals manipulation

    //RelieveTimes
    InitDynamicModal("#RelieveTimesModal",
    //init
        {
            "CreateRelieveTime": function (modal, form, initiator) {

            },
            "EditRelieveTime": function (modal, form, initiator) {

                var data = initiator.closest(".ListItem").attr("id");

                data = RelieveUsed.getDataElement(data);

                for (var prop in data) {

                    var dataInput = form.find("[name=" + prop + "]");

                    dataInput.val(data[prop]);

                }

            },
            "DeleteRelieveTime": function (modal, form, initiator) {

                this.EditRelieveTime(modal, form, initiator);

                //:last because of the ReqToken
                var input = $(form).find("input:not([name=Id],[type=submit],:last)");
                var textarea = $(form).find("textarea");

                input.attr("disabled", "");
                textarea.attr("disabled", "");

                modal.on("hidden.bs.modal", function () {
                    console.log(1);
                    input.removeAttr("disabled");
                    textarea.removeAttr("disabled", "");
                });

                var submit = $(form).find("[type=submit]");

                var prev = submit.text();

                submit.text("Удалить");

                modal.on("hidden.bs.modal", function () {
                    submit.text(prev);
                });
            }

        },
        //succes
        {
            "CreateRelieveTime": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                data["Id"] = response;

                RelieveUsed.Add(data);

            },
            "EditRelieveTime": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                var id = initiator.closest(".ListItem").attr("id");

                RelieveUsed.Update(id, data);

            },
            "DeleteRelieveTime": function (form, initiator, response) {

                var id = initiator.closest(".ListItem").attr("id");

                RelieveUsed.Delete(id);
            }
        });

    //Relieves
    InitDynamicModal("#RelievesModal",
        //init
        {
            "CreateRelieve": function (modal, form, initiator) {
                //0-ISO
                data = {
                    DoctorId: RequestData.DoctorId,
                    //without offset
                    Date: moment(schedule.options.selectedDate).format('YYYY-MM-DDTHH:mm:ss')
                };

                for (var prop in data) {

                    var dataInput = form.find("[name=" + prop + "]");
                    console.log(dataInput);
                    dataInput.val(data[prop]);

                }

            },
            "EditRelieve": function (modal, form, initiator) {

                var data = initiator.closest(".ListItem").attr("id");

                data = RelieveSetted.getDataElement(data);

                data.DoctorId = RequestData.DoctorId;

                for (var prop in data) {

                    var dataInput = form.find("[name=" + prop + "]");

                    dataInput.val(data[prop]);

                }

            },
            "DeleteRelieve": function (modal, form, initiator) {

                this.EditRelieve(modal, form, initiator);

                //:last because of the ReqToken
                var input = $(form).find("input:not([name=Id],[type=submit],:last)");

                input.attr("disabled", "");

                modal.on("hidden.bs.modal", function () {
                    input.removeAttr("disabled");
                });

                var submit = $(form).find("[type=submit]");

                var prev = submit.text();

                submit.text("Удалить");

                modal.on("hidden.bs.modal", function () {
                    submit.text(prev);
                });
            },
            "CreateRelieveFromUsed": function (modal, form, initiator) {

                this.CreateRelieve(modal, form, initiator);

                var data = initiator.closest(".ListItem").attr("id");

                data = RelieveUsed.getDataElement(data);

                var dataInput = form.find("[name=" + "StartTime" + "]");

                dataInput.val(data["StartTime"]);

                dataInput = form.find("[name=" + "EndTime" + "]");

                dataInput.val(data["EndTime"]);

            }

        },
        //succes
        {
            "CreateRelieve": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                data["Id"] = response;

                RelieveSetted.Add(data);

                schedule.addEvents([data]);

            },
            "EditRelieve": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                var id = initiator.closest(".ListItem").attr("id");

                RelieveSetted.Update(id, data);

                schedule.removeEvents(function (event) {
                    return (event.Date === data.Date) && (event.Id === data.Id);
                });

                schedule.addEvents([data]);

            },
            "DeleteRelieve": function (form, initiator, response) {

                var id = initiator.closest(".ListItem").attr("id");

                var data = RelieveSetted.getDataElement(id);

                schedule.removeEvents(function (event) {
                    return (event.Date === data.Date) && (event.Id === data.Id);
                });

                RelieveSetted.Delete(id);
            },
            "CreateRelieveFromUsed": function (form, initiator, response) {
                this.CreateRelieve(form, initiator, response);
            }
        });

    function init() {

        $.ajax({
            url: "/Registrator/RelieveTimes",
            dataType: "json",
            success: function (response) {
                RelieveUsed.Add(response);
            }
        });

        //with a callback
        schedule.getRelieves(function () { $("#schedule .today").trigger("click"); });

      

    }

    init();

    init();
});