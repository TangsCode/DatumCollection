import { Component, OnInit, ViewChild, ElementRef, Input, Inject, Output, EventEmitter } from '@angular/core';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Observable } from 'rxjs';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { map, startWith } from 'rxjs/operators';
import { MatChipInputEvent } from '@angular/material/chips';
import { FormControl } from '@angular/forms';
import { Chips } from '../../chips';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-chips',
  templateUrl: './chips.component.html',
  styleUrls: ['./chips.component.css']
})
export class ChipsComponent implements OnInit {
  @Input() formCtrl: FormControl;
  @Input() control: Chips;
  @Output() private outer = new EventEmitter();

  visible = true;
  selectable = true;
  removable = true;
  separatorKeysCodes: number[] = [ENTER, COMMA];
  filteredChips: Observable<string[]>;
  chips: string[] = [];
  allChips: string[] = [];

  @ViewChild('chipsInput') chipInput: ElementRef<HTMLInputElement>;
  @ViewChild('auto') matAutocomplete: MatAutocomplete;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    
  }

  ngOnInit(): void {
    this.http.get<{ key: string, value: string }[]>(this.baseUrl + this.control.dataSourceUrl).toPromise().then(options => {      
      options.forEach(opt => {
        this.allChips.push(opt.value);
      });      
      //this.allChips = value;
    })
    this.filteredChips = this.formCtrl.valueChanges.pipe(
      startWith(null),
      map((chip: string | null) => chip ? this._filter(chip) : this.allChips.slice()));
    this.formCtrl.valueChanges.subscribe(value => {
      debugger
    });
  }

  controlValueChange(event) {    
    this.outer.emit(event);
  }

  add(event: MatChipInputEvent): void {
    debugger
    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || '').trim()) {
      this.chips.push(value.trim());
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }

    this.formCtrl.setValue(null);
  }

  remove(fruit: string): void {
    debugger
    const index = this.chips.indexOf(fruit);

    if (index >= 0) {
      this.chips.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    debugger
    this.chips.push(event.option.viewValue);
    this.chipInput.nativeElement.value = '';
    this.formCtrl.setValue(null);
  }

  private _filter(value: string): string[] {
    debugger
    const filterValue = value.toLowerCase();
    return this.allChips.filter(chip => chip.toLowerCase().indexOf(filterValue) === 0);
  }
}
