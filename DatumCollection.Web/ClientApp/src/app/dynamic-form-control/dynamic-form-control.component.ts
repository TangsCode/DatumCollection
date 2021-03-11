import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Control } from '../form/control';

@Component({
  selector: 'app-dynamic-form-control',
  templateUrl: './dynamic-form-control.component.html',
  styleUrls: ['./dynamic-form-control.component.css']
})
export class DynamicFormControlComponent implements OnInit {
  @Output() private outer = new EventEmitter();
  @Input() control: Control<string>;
  @Input() form: FormGroup;
  get isValid() { return this.form.controls[this.control.key].valid; }

  constructor() { }

  ngOnInit(): void {    
  }

  controlValueChange(event) {    
    this.outer.emit({ name: this.control.key, value: event });
  }
}
