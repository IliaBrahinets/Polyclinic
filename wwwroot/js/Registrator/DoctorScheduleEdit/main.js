
//configuring clndr.js
$(document).ready(function () {
    //
    // Assuming you've got the appropriate language files,
    // clndr will respect whatever moment's language is set to.

    moment.locale("ru");

    // Here's some magic to make sure the dates are happening this month.
    var thisMonth = moment().format('DD-MM-YYYY');

    // The order of the click handlers is predictable. Direct click action
    // callbacks come first: click, nextMonth, previousMonth, nextYear,
    // previousYear, nextInterval, previousInterval, or today. Then
    // onMonthChange (if the month changed), inIntervalChange if the interval
    // has changed, and finally onYearChange (if the year changed).

    var shedule = $('#schedule').clndr({

        moment: moment,

        numberOfRows: 5,

        dateParameter: 'Date',

        clickEvents: {
            click: function (target) {
                console.log('Cal-1 clicked: ', target);

                RelieveSetted.DeleteAll();

                RelieveSetted.Add(target.events);

                if (this.selected)
                    $(this.selected.element).removeClass("selected");

                this.selected = target;
                $(target.element).addClass("selected");
            },
            today: function () {
                console.log('Cal-1 today');
            },
            nextMonth: function () {

                RelieveSetted.DeleteAll();

                shedule.removeEvents(function () {
                    return true;
                });

                shedule.getRelieves();

            },
            previousMonth: function () {

                RelieveSetted.DeleteAll();

                shedule.removeEvents(function () {
                    return true;
                });

                shedule.getRelieves();

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

    shedule.getRelieves = function () {

        var currDate = shedule.intervalStart;


        $.ajax({
            url: "/Registrator/Relieves",
            dataType: "json",
            data: {
                DoctorId: RequestData.DoctorId,
                year: currDate.format("yyyy"),
                month: currDate.format("MM")
            },
            success: function (response) {
                shedule.addEvents(response);
            }
        });

    };

    shedule.getRelieves();
    //

    var RelieveList = function (options) {

        this.container = $(options.container);

        this.template = _.template(options.template);

        this.dataObj = {};
        this.dataCount = 0;

        if (options.dataList)
            Add(options.datаList);

        var Add = function (data) {
            elem = $('<div class = "ListElem"' + ' id = ' + this.dataCount + '></div>');

            this.dataObj[this.dataCount++] = data;

            elem.html(this.template(data));

            this.container.append(elem);
        }.bind(this);

        this.Add = function (data) {
            if (data instanceof Array) {

                data.forEach(function (value, index, arr) {
                    Add(value);
                });

            } else {

                Add(data);

            }
        }


        function Update(id, data) {
            if ((this.dataObj[id] === null) || (this.dataObj[id] == undefined)) return;

            var elem = this.container.find(".ListElem[id=" + id + "]");

            elem.html(this.template(data));

            this.dataObj[id] = data;

        }
        this.Update = Update;

        function Delete(id) {

            if ((this.dataObj[id] === null) || (this.dataObj[id] == undefined)) return;

            this.dataObj[id] = undefined;

            var del = this.container.find(".ListElem[id=" + id + "]");

            del.remove();

        }

        this.Delete = Delete;

        function getDataElement(id) {
            return this.dataObj[id];
        }

        this.getDataElement = getDataElement;

        function DeleteAll() {

            this.dataObj = {};
            this.dataCount = 0;

            $(this.container).children(".ListElem").remove();

        }

        this.DeleteAll = DeleteAll;
    }

    var RelieveAvaliable = new RelieveList({
        container: $("#relievesAvaliable"),
        template: $("#relievesAvaliableTemplate").html()
    });


    var RelieveSetted = new RelieveList({
        container: $("#relievesSetted"),
        template: $("#relievesSettedTemplate").html()
    });



    //modals manipulation

    function ErrorAlert(message) {
        var htmlAlert = '<div class="alert alert-danger" role="alert">';

        var button = '<button type="button" class="close" data-dismiss="alert" aria-label="Close">'
            + '<span aria-hidden="true" >&times;</span>' +
            '</button>';

        htmlAlert += button;

        htmlAlert += message + "</div>"

        return $(htmlAlert);
    }

    function NameValueArrayToObj(arr) {

        var Obj = {};
        for (var i = 0; i < arr.length; i++)
            Obj[arr[i].name] = arr[i].value;

        return Obj

    }

    function InitDynamicModal(modalSelector, initForActions, successForActions) {


        $(modalSelector).on('show.bs.modal', function (event) {

            var initiator = $(event.relatedTarget);

            var modal = $(this);

            var form = modal.find("form");

            var action = initiator.data("action");

            form.attr("action", "/Registrator/" + action);

            form[0].reset();

            //
            initForActions[action](modal, form, initiator);
            //

            form.on("submit", function listener(event) {

                event.preventDefault();

                if (!form.valid()) return;

                //disable form
                form.off("submit", listener);
                form.submit("submit", function () { return false; });

                $.ajax({

                    type: form.attr("method"),

                    url: form.attr("action"),

                    data: form.serialize(),

                    success: function (response) {

                        //
                        successForActions[action](form, initiator, response);
                        //

                        modal.modal("hide");

                    },
                    error: function () {
                        //enableform
                        form.on("submit", listener);

                        var Error = ErrorAlert("Произошла ошибка попробуйте еще раз!");

                        modal.find(".modal-body").append(Error);

                        modal.on("hidden.bs.modal", function () { Error.alert("close"); })
                    }
                });

                return false;
            });
        });
    }

    //RelieveTimes
    InitDynamicModal("#RelieveTimesModal",
    //init
        {
            "CreateRelieveTime": function (modal, form, initiator) {

            },
            "EditRelieveTime": function (modal, form, initiator) {

                var data = initiator.closest(".ListElem").attr("id");

                data = RelieveAvaliable.getDataElement(data);

                for (var prop in data) {

                    var dataInput = form.find("[name=" + prop + "]");

                    dataInput.val(data[prop]);

                }

            },
            "DeleteRelieveTime": function (modal, form, initiator) {

                this.EditRelieveTime(modal, form, initiator);

                var input = $(form).find("input");
                var textarea = $(form).find("textarea");

                input.attr("readonly", "");
                textarea.attr("readonly", "");

                modal.on("hidden.bs.modal", function () {
                    input.removeAttr("readonly");
                    textarea.removeAttr("readonly");
                });

                var submit = $(form).find("[type=submit]");

                var prev = submit.text();

                submit.text("Удалить");
                submit.attr("readonly", "");

                modal.on("hidden.bs.modal", function () {
                    submit.text(prev);
                });
            },

        },
        //succes
        {
            "CreateRelieveTime": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                data["Id"] = response;

                RelieveAvaliable.Add(data);

            },
            "EditRelieveTime": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                var id = initiator.closest(".ListElem").attr("id");

                RelieveAvaliable.Update(id, data);

            },
            "DeleteRelieveTime": function (form, initiator, response) {

                var id = initiator.closest(".ListElem").attr("id");

                RelieveAvaliable.Delete(id);
            }
        });

    //Relieves
    InitDynamicModal("#RelievesModal",
        //init
        {
            "CreateRelieve": function (modal, form, initiator) {

                data = {
                    DoctorId: RequestData.DoctorId,
                    Date: shedule.selected.date.format()
                };
                
                for (var prop in data) {
                    
                    var dataInput = form.find("[name=" + prop + "]");
                    console.log(dataInput);
                    dataInput.val(data[prop]);

                }

            },
            "EditRelieve": function (modal, form, initiator) {

                var data = initiator.closest(".ListElem").attr("id");

                data = RelieveSetted.getDataElement(data);

                data.DoctorId = RequestData.DoctorId;

                for (var prop in data) {

                    var dataInput = form.find("[name=" + prop + "]");

                    dataInput.val(data[prop]);

                }

            },
            "DeleteRelieve": function (modal, form, initiator) {

                this.EditRelieve(modal, form, initiator);

                var input = $(form).find("input");

                input.attr("readonly", "");

                modal.on("hidden.bs.modal", function () {
                    input.removeAttr("readonly");
                });

                var submit = $(form).find("[type=submit]");

                var prev = submit.text();

                submit.text("Удалить");
                submit.attr("readonly", "");

                modal.on("hidden.bs.modal", function () {
                    submit.text(prev);
                });
            },

        },
        //succes
        {
            "CreateRelieve": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                data["Id"] = response;

                RelieveSetted.Add(data);

                shedule.addEvents(data);

            },
            "EditRelieve": function (form, initiator, response) {
                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                var id = initiator.closest(".ListElem").attr("id");

                RelieveSetted.Update(id, data);

                shedule.removeEvents(function (event) {
                    return (event.Date == data.Date) && (event.Id == data.Id);
                });

                shedule.addEvents(data);

            },
            "DeleteRelieve": function (form, initiator, response) {

                //[ {name:,value:} ] to {name:value} 
                var data = NameValueArrayToObj(form.serializeArray());

                var id = initiator.closest(".ListElem").attr("id");

                RelieveSetted.Delete(id);

                shedule.removeEvents(function (event) {
                    return (event.Date == data.Date) && (event.Id == data.Id);
                });

              
            }
        });

    function init() {

        $.ajax({
            url: "/Registrator/RelieveTimes",
            dataType: "json",
            success: function (response) {
                RelieveAvaliable.Add(response);
            }
        });

        $("#schedule .today").trigger("click");

    }

    init();
});