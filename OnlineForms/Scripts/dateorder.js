////const { param } = require("jquery");

$(function () {

    jQuery.validator.addMethod('dateorder', function (value, element, params) {
        var valDateorderPropertytested = $(element).data('valDateorderPropertytested');
        var comparedate = $("#" + valDateorderPropertytested);

        if (comparedate.val() > value) {
            return false;
        }
        return true;
    }, '');

    jQuery.validator.unobtrusive.adapters.add('dateorder', {}, function (options) {
        options.rules['dateorder'] = true;
        options.messages['dateorder'] = options.message;
    });

    jQuery.validator.addMethod('beforetoday', function (value, element, params) {
        var todayD = new Date();
        value = new Date(value);
        var formattedtodayD = todayD.getFullYear() + '-' + todayD.getMonth() + '-' + todayD.getDate();
        console.log(value.toUTCString());
        console.log(todayD.toUTCString());
        console.log(formattedtodayD);
        if (value < todayD) {
            return false;
        }
        return true;
    }, '');

    jQuery.validator.unobtrusive.adapters.add('beforetoday', {}, function (options) {
        options.rules['beforetoday'] = true;
        options.messages['beforetoday'] = options.message;
    });

}(jQuery));