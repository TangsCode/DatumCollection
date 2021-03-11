"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var control_1 = require("./control");
var Textbox = /** @class */ (function (_super) {
    __extends(Textbox, _super);
    function Textbox() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.controlType = 'textbox';
        return _this;
    }
    return Textbox;
}(control_1.Control));
exports.Textbox = Textbox;
//# sourceMappingURL=control.textbox.js.map