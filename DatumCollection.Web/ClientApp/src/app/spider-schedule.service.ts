import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Control } from './form/control';
import { Textbox } from './form/textbox';
import { of } from 'rxjs';
import { Dropdown } from './form/dropdown';
import { Hidden } from './form/hidden';
import { SpiderScheduleSetting } from './interface';
import { DatePicker } from './form/datepicker';
import { Chips} from './form/chips';
import { FormatterService } from './services/formatter.service';
import { IService } from './services/interface';
import { WebExecutionResult } from './model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Select } from './form/select';

@Injectable({
  providedIn: 'root'
})
export class SpiderScheduleService implements IService {

  items$: BehaviorSubject<SpiderScheduleSetting[]>;
  items: Array<SpiderScheduleSetting>;

  options = [];

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private _formmater: FormatterService,
    private _snackBar: MatSnackBar,
  ) {
    this.items$ = new BehaviorSubject([]);
  }

  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }

  async getAll() {
    await this.query();
    this.items$.next(this.items);
  }

  formatter(fmt: Function, input: any, args: []) {
    return fmt(input, args);
  }

  async query() {
    var data = await this.http.get<SpiderScheduleSetting[]>(this.baseUrl + 'api/spiderschedule/query').toPromise();
    this.items = [];
    this.items = data;
    this.items$.next(this.items);
  }

  async add(item: SpiderScheduleSetting) {
    this.http.post(this.baseUrl + 'api/spiderschedule/add', item).subscribe(async result => {      
      if (result) { }
      await this.query();
    }, error => { console.log(error) });
  }

  async get(id: string) {
    var data = await this.http.get<SpiderScheduleSetting>(this.baseUrl + "api/spiderschedule/get/" + id).toPromise();    
    return data;
  }

  async edit(item) {
    this.http.post<WebExecutionResult>(this.baseUrl + 'api/spiderschedule/edit', item).subscribe(result => {
      debugger
      if (result.success) {
        this.openSnackBar('更新成功', '');
      } else {
        this.openSnackBar('更新失败<br />' + result.errorMsg, '');
      }
    }, error => { console.log(error) });
  }

  async delete(item: SpiderScheduleSetting) {    
    this.http.post(this.baseUrl + 'api/spiderschedule/delete', item).subscribe(result => {
      debugger
      if (result) { }
    }, error => { console.log(error) });
  }

  getFormControls() {    
    const controls: Control<Object>[] = [
      new Hidden({
        key: 'id',
        order: 0
      }),
      new DatePicker({
        key: 'startDate',
        label: '开始日期',
        type: 'date',
        required: true,
        order: 1
      }),
      new DatePicker({
        key: 'endDate',
        label: '结束日期',
        type: 'date',
        required: false,
        order: 1
      }),
      new Textbox({
        key: 'startTime',
        label: '开始时间',
        type: 'time',
        required: true,
        order: 1
      }),
      new Textbox({
        key: 'endTime',
        label: '结束时间',
        type: 'time',
        required: false,
        order: 1
      }),
      new Textbox({
        key: 'interval',
        label: '间隔',
        type: 'number',
        required: true,
        order: 1
      }),
      new Dropdown({
        key: 'spiderFrequency',
        label: '频率',
        options: [
          { key: 1 , value: 'Second' },
          { key: 2 , value: 'Minute' },
          { key: 3 , value: 'Day' },
          { key: 4 , value: 'Week' },
          { key: 5 , value: 'Month' },
          { key: 6 , value: 'Season' },
        ],
        order: 2
      }),
      new Dropdown({
        key: 'scheduleDayOfWeek',
        label: 'Day of week',
        options: [
          { key: 0 , value: 'Sunday' },
          { key: 1 , value: 'Monday' },
          { key: 2 , value: 'Tuesday' },
          { key: 3 , value: 'Wednesday' },
          { key: 4 , value: 'Thursday' },
          { key: 5 , value: 'Friday' },
          { key: 6 , value: 'Saturday' },
        ],
        order: 2
      }),
      new Dropdown({
        key: 'scheduleMonthOfYear',
        label: 'Month of year',
        options: [
          { key: 0, value: 'January' },
          { key: 1, value: 'February' },
          { key: 2, value: 'March' },
          { key: 3, value: 'April' },
          { key: 4, value: 'May' },
          { key: 5, value: 'June' },
          { key: 6, value: 'July' },
          { key: 7, value: 'August' },
          { key: 8, value: 'September' },
          { key: 9, value: 'October' },
          { key: 10, value: 'November' },
          { key: 11, value: 'December' },
        ],
        order: 2
      }),
      new Dropdown({
        key: 'isEnabled',
        label: '是否启用',
        options: [
          { key: true, value: '是' },
          { key: false, value: '否' },
        ],
        order: 2
      }),
      new Select({
        key: 'spiderItems',
        label: '所选商品',
        dataSourceUrl: 'api/SpiderItem/getSelectOption',
        loadDataUrl:'api/SpiderSchedule/loadSpiderItems',
        order: 2
      })
    ];
    return of(controls.sort((a, b) => a.order - b.order));
  }

  getFormTitle() {
    return "Add Spider Schedule";
  }
}

