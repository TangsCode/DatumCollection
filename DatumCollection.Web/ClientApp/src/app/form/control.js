"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Control = /** @class */ (function () {
    function Control(options) {
        if (options === void 0) { options = {}; }
        this.value = options.value;
        this.key = options.key || '';
        this.label = options.label || '';
        this.required = !!options.required;
        this.order = options.order === undefined ? 1 : options.order;
        this.controlType = options.controlType || '';
        this.type = options.type || '';
        this.options = options.options || [];
        this.dataSourceUrl = options.dataSourceUrl || '';
        this.loadDataUrl = options.loadDataUrl || '';
    }
    return Control;
}());
exports.Control = Control;
//# sourceMappingURL=control.js.map