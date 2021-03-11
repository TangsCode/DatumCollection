export class Control<T> {
  value: T;
  key: string;
  label: string;
  required: boolean;
  order: number;
  controlType: string;
  type: string;
  options: { key: string | number | boolean, value: string }[];
  dataSourceUrl: string;
  loadDataUrl: string;
  constructor(options: {
    value?: T;
    key?: string;
    label?: string;
    required?: boolean;
    order?: number;
    controlType?: string;
    type?: string;
    options?: { key: string | number | boolean, value: string }[];
    dataSourceUrl?: string;
    loadDataUrl?: string;
  } = {}) {
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
}

