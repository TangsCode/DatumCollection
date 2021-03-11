import { Component, OnInit, Input, Output, EventEmitter, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Select } from '../../select';
import { HttpClient } from '@angular/common/http';
import { Option } from '../../../model';

@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.css']
})
export class SelectComponent implements OnInit {
  @Input() formCtrl: FormControl;
  @Input() control: Select;
  @Input() formId: string;
  @Output() private outer = new EventEmitter();

  items: Option[];
  selected: Option[];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  ngOnInit(): void {
    this.http.get<Option[]>(this.baseUrl + this.control.dataSourceUrl).subscribe(options => {
      debugger
      this.items = options;
    });
  }

  controlValueChange(event) {    
    this.outer.emit(event);
  }
}
