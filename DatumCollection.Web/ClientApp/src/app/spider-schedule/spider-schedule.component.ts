import { OnInit, AfterViewInit, Component, ViewChild, Inject } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SpiderScheduleService } from '../spider-schedule.service';
import { ModalComponent } from '../modal/modal.component';
import { SpiderScheduleSetting } from '../interface';
import { SpiderFrequency, SpiderScheduleSettingModel, Operation } from '../model';
import { FormatterService } from '../services/formatter.service';
import { IService } from '../services/interface';

@Component({
  selector: 'app-spider-schedule',
  templateUrl: './spider-schedule.component.html',
  styleUrls: ['./spider-schedule.component.css']
})
export class SpiderScheduleComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [
    'select',
    'startDate',
    'endDate',
    'startTime',
    'endTime',
    'interval',
    'spiderFrequency',
    'isEnabled',
    'createTime',
    'actions'
  ];

  dataSource: MatTableDataSource<SpiderScheduleSetting>;
  selection: SelectionModel<SpiderScheduleSetting>;
  private serviceSubscribe: Subscription;
  _formService: IService;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  constructor(
    @Inject('BASE_URL') private baseUrl: string,
    public dialog: MatDialog,
    private _service: SpiderScheduleService,
    private _snackBar: MatSnackBar,
    private _formatter: FormatterService
  ) {
    const initialSelection = [];
    const allowMultiSelect = true;
    this.selection = new SelectionModel<SpiderScheduleSetting>(allowMultiSelect, initialSelection);
    this._formService = _service;
  }

  async ngOnInit() {
    if (!this.dataSource) {
      await this.getDataSource();
    }
    this.dataSource.paginator = this.paginator;
    this.sort.active = 'createTime';
    this.sort.direction = 'desc';
    this.dataSource.sort = this.sort;
    this.serviceSubscribe = this._service.items$.subscribe(res => {
      this.dataSource.data = res;
    })
  }

  ngOnDestroy(): void {
    this.serviceSubscribe.unsubscribe();
  }

  ngAfterViewInit() {

  }

  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }

  async getDataSource() {
    await this._service.getAll();
    this.dataSource = new MatTableDataSource(this._service.items);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    //if (this.dataSource.paginator) {
    //  this.dataSource.paginator.firstPage();
    //}
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected == numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  formatterFrequency(value) {
    var fre = SpiderFrequency[value];    
    return fre;
  }

  formatterIsEnabled(value) {
    return value ? "Yes" : "No";
  }

  formatterDate(value) {
    return this._formatter.dateFormat('yyyy-MM-dd', new Date(value));
  }

  formatterTime(value) {
    return this._formatter.dateFormat('HH:mm', new Date(value));
  }

  /** operations*/
  add() {
    const dialogRef = this.dialog.open(ModalComponent, {
      width: '600px',
      data: {
        operation: Operation.Add,
        formData: null,
      }
    });
    
    dialogRef.afterClosed().subscribe(async result => {
      if (result) {
        await this._service.add(result);
      }
    });
  }
  async edit(id: string) {
    var data = await this._service.get(id);
    debugger
    const dialogRef = this.dialog.open(ModalComponent, {
      width: '600px',
      data: {
        operation: Operation.Edit,
        formData: new SpiderScheduleSettingModel(data),
      }
    });

    dialogRef.afterClosed().subscribe(async result => {      
      if (result) {
        switch (result.operation) {
          case Operation.Edit:
            await this._service.edit(result.data);
            break;
          default:
        }
        await this.getDataSource();
      }
    });
  }
  delete(data: SpiderScheduleSetting) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent);

    dialogRef.afterClosed().subscribe(async result => {
      if (result) {
        await this._service.delete(data);
        this.openSnackBar('item remove success', 'Close');
      }
    });
  }

}
