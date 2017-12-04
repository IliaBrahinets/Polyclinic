//configuring clndr.js
$(document).ready(function() {

    // Assuming you've got the appropriate language files,
    // clndr will respect whatever moment's language is set to.

    moment.locale("ru");

    // Here's some magic to make sure the dates are happening this month.
    var thisMonth = moment().format('DD-MM-YYYY');
    console.log(thisMonth);
    // Events to load into calendar
    var eventArray = [{
        date: '2017-11-23',
        relief: '8:00-19:00',
        count: '8/10'
    }, {
        date: '2017-11-23',
        relief: '8:00-19:00',
        count: '8/10'
    }];

    // The order of the click handlers is predictable. Direct click action
    // callbacks come first: click, nextMonth, previousMonth, nextYear,
    // previousYear, nextInterval, previousInterval, or today. Then
    // onMonthChange (if the month changed), inIntervalChange if the interval
    // has changed, and finally onYearChange (if the year changed).
    var sh = $('#schedule').clndr({

        //daysOfTheWeek: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Вс'],
        moment: moment,

        numberOfRows: 5,

        events: eventArray,
        clickEvents: {
            click: function(target) {
                console.log('Cal-1 clicked: ', target);
            },
            today: function() {
                console.log('Cal-1 today');
            },
            nextMonth: function() {
                console.log('Cal-1 next month');
            },
            previousMonth: function() {
                console.log('Cal-1 previous month');
            },
            onMonthChange: function() {
                console.log('Cal-1 month changed');
            },
            nextYear: function() {
                console.log('Cal-1 next year');
            },
            previousYear: function() {
                console.log('Cal-1 previous year');
            },
            onYearChange: function() {
                console.log('Cal-1 year changed');
            },
            nextInterval: function() {
                console.log('Cal-1 next interval');
            },
            previousInterval: function() {
                console.log('Cal-1 previous interval');
            },
            onIntervalChange: function() {
                console.log('Cal-1 interval changed');
            }
        },
        doneRendering: function() {

        },
        showAdjacentMonths: true,
        adjacentDaysChangeMonth: false,
        template: $("#scheduletemplate").html()
    });


});