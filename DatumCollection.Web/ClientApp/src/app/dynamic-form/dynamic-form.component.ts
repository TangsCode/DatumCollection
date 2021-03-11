import { Component, OnInit, Input, Inject } from '@angular/core';
import { Control } from '../form/control';
import { FormGroup } from '@angular/forms';
import { FormControlService } from '../services/form-control.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SpiderScheduleService } from '../spider-schedule.service';
import { Operation } from '../model';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dynamic-form',
  templateUrl: './dynamic-form.component.html',
  styleUrls: ['./dynamic-form.component.css']
})
export class DynamicFormComponent implements OnInit {
  @Input() controls: Control<Object>[] = [];
  form: FormGroup;
  operation: Operation
  origin = '';
  changes = '';
  changedModel = {};

  constructor(
    private _service: FormControlService,
    private _scheduleService: SpiderScheduleService,
    public dialogRef: MatDialogRef<DynamicFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) { 
  }

  ngOnInit() {
    this._scheduleService.getFormControls().subscribe($controls => {
      this.controls = $controls;      
    });
    
    this.form = this._service.toFormGroup(this.controls);
    this.operation = this.data.operation;
    if (this.data.formData != null) {
      debugger
      this.form.patchValue(this.data.formData);
      this.changedModel['id'] = this.data.formData.id;
    }
    this.origin = JSON.stringify(this.form.getRawValue());    
  }

  onSubmit() {
    this.dialogRef.close({
      operation: this.operation,
      data: this.changedModel
    });
  }
  
  controlValueChange(model) {
    debugger
    this.changedModel[model.name] = model.value;
    this.changes = JSON.stringify(this.changedModel);
  }
  
}
