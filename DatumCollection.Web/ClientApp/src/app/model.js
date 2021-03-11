"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var SpiderItemModel = /** @class */ (function () {
    function SpiderItemModel(item) {
        if (item) {
            this.id = item.id;
            this.FK_Channel_ID = item.channel.id;
            this.contentType = item.contentType;
            this.encoding = item.encoding;
            this.method = item.method;
            this.url = item.url;
            this.skuName = item.skuName;
        }
        else {
            this.id = '';
            this.FK_Channel_ID = '';
            this.skuName = '';
            this.contentType = '';
            this.url = '';
            this.method = '';
            this.encoding = '';
        }
    }
    return SpiderItemModel;
}());
exports.SpiderItemModel = SpiderItemModel;
function toTimeString(value) {
    return (new Date(value)).toLocaleTimeString('it-IT');
}
var SpiderScheduleSettingModel = /** @class */ (function () {
    function SpiderScheduleSettingModel(item) {
        this.id = item.id;
        this.interval = item.interval;
        this.spiderFrequency = item.spiderFrequency;
        this.scheduleDayOfWeek = item.scheduleDayOfWeek;
        this.scheduleMonthOfYear = item.scheduleMonthOfYear;
        this.startTime = toTimeString(item.startTime);
        this.endTime = toTimeString(item.endTime);
        this.startDate = item.startDate;
        this.endDate = item.endDate;
        this.isEnabled = item.isEnabled;
        this.spiderItems = [];
        debugger;
        if (item.spiderScheduleItems != null && item.spiderScheduleItems.length > 0) {
            for (var i = 0; i < item.spiderScheduleItems.length; i++) {
                this.spiderItems.push(item.spiderScheduleItems[i].fK_SpiderItem_ID);
            }
        }
    }
    return SpiderScheduleSettingModel;
}());
exports.SpiderScheduleSettingModel = SpiderScheduleSettingModel;
var WebExecutionResult = /** @class */ (function () {
    function WebExecutionResult() {
    }
    return WebExecutionResult;
}());
exports.WebExecutionResult = WebExecutionResult;
var SpiderFrequency;
(function (SpiderFrequency) {
    SpiderFrequency[SpiderFrequency["Once"] = 0] = "Once";
    SpiderFrequency[SpiderFrequency["Second"] = 1] = "Second";
    SpiderFrequency[SpiderFrequency["Minute"] = 2] = "Minute";
    SpiderFrequency[SpiderFrequency["Day"] = 3] = "Day";
    SpiderFrequency[SpiderFrequency["Week"] = 4] = "Week";
    SpiderFrequency[SpiderFrequency["Month"] = 5] = "Month";
    SpiderFrequency[SpiderFrequency["Season"] = 6] = "Season";
})(SpiderFrequency = exports.SpiderFrequency || (exports.SpiderFrequency = {}));
var Operation;
(function (Operation) {
    Operation[Operation["View"] = 0] = "View";
    Operation[Operation["Add"] = 1] = "Add";
    Operation[Operation["Edit"] = 2] = "Edit";
})(Operation = exports.Operation || (exports.Operation = {}));
//# sourceMappingURL=model.js.map