//

var ListView = function (options) {

    this.container = $(options.container);

    this.itemTag = options.itemTag;

    if (this.itemTag === null || this.itemTag === undefined)
        this.itemTag = "div";

    this.template = _.template(options.template);

    this.dataObj = {};
    this.dataCount = 0;

    if (options.dataList)
        Add(options.datаList);

    var Add = function (data) {

        this.container.append('<' + this.itemTag + ' class = "ListItem"' + ' id = ' + this.dataCount + '></' + this.itemTag + '>');

        $(this.container).find("[id=" + this.dataCount + "]")
                         .append(this.template(data));

        this.dataObj[this.dataCount++] = data;
        
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

        var item = this.container.find(".ListItem[id=" + id + "]");

        item.html(this.template(data));

        this.dataObj[id] = data;

    }
    this.Update = Update;

    function Delete(id) {

        if ((this.dataObj[id] === null) || (this.dataObj[id] == undefined)) return;

        this.dataObj[id] = undefined;

        var del = this.container.find(".ListItem[id=" + id + "]");

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

        $(this.container).children(".ListItem").remove();

    }

    this.DeleteAll = DeleteAll;
}

//
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

        var funcName = initiator.data("method") || action;

        form.attr("action", action);

        form.trigger("reset");

        //
        initForActions[funcName](modal, form, initiator);
        //
        function listener(event) {

            event.preventDefault();

            if (form.validate) {
                var validator = form.validate();
                console.log("submit");

                if (!form.valid()) return false;
            }

            //disable form
            form.off("submit", listener);
            form.submit("submit", function () { return false; });

            $.ajax({

                type: form.attr("method"),

                url: form.attr("action"),

                data: form.serialize(),

                success: function (response) {

                    form.off("submit", listener);
                    //
                    successForActions[funcName](form, initiator, response);
                    //

                    modal.modal("hide");

                },
                error: function (xhr) {
                    //enableform
                    form.on("submit", listener);

                    var Error = ErrorAlert(xhr.responseText || "Произошла ошибка попробуйте еще раз!");

                    modal.find(".modal-body").append(Error);

                    modal.on("hidden.bs.modal", function () { Error.alert("close"); })
                },
                timeout: 500
            });

            return false;
        }

        form.on("submit", listener);

        modal.on("hide.bs.modal", function () {
            form.off("submit", listener);

            if (form.clearValidation)
                form.clearValidation();
        });

    });
}
//