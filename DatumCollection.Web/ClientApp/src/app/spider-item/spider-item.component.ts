import { OnInit, AfterViewInit, Component, ViewChild, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { SpideritemService } from '../spideritem.service';
import { Subscription } from 'rxjs';
import { SpiderItemFormDialogComponent } from '../spider-item-form-dialog/spider-item-form-dialog.component';
import { SpiderItemModel } from '../model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SpiderItem } from '../interface';

@Component({
  selector: 'app-spider-item',
  templateUrl: './spider-item.component.html',
  styleUrls: ['./spider-item.component.css']
})
export class SpiderItemComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = ['channel', 'skuName', 'contentType', 'encoding', 'method', 'createTime', 'actions'];
  dataSource: MatTableDataSource<SpiderItem>;
  selection: SelectionModel<SpiderItem>;
  private serviceSubscribe: Subscription;
  

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    public dialog: MatDialog,
    private spiderItemService: SpideritemService,
    private _snackBar: MatSnackBar
  ) {
    const initialSelection = [];
    const allowMultiSelect = true;
    this.selection = new SelectionModel<SpiderItem>(allowMultiSelect, initialSelection);
  }

  async ngOnInit() {
    if (!this.dataSource) {
      await this.getDataSource();
    }
    this.dataSource.paginator = this.paginator;
    this.sort.active = 'createTime';
    this.sort.direction = 'desc';
    this.dataSource.sort = this.sort;
    this.serviceSubscribe = this.spiderItemService.items$.subscribe(res => {      
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
    await this.spiderItemService.getAll();
    this.dataSource = new MatTableDataSource(this.spiderItemService.items);    
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

/** operations*/
  add() {
    const dialogRef = this.dialog.open(SpiderItemFormDialogComponent, {
      width: '500px',
      data: new SpiderItemModel(null)
    });

    dialogRef.afterClosed().subscribe(async result => {
      if (result) {
        await this.spiderItemService.add(result);        
      }
    });
  }
  edit(data: SpiderItem) {
    const dialogRef = this.dialog.open(SpiderItemFormDialogComponent, {
      width: '500px',
      data: new SpiderItemModel(data)
    });

    dialogRef.afterClosed().subscribe(async result => {
      if (result) {
        await this.spiderItemService.edit(result);
        await this.getDataSource();
      }
    });
  }

  delete(data: SpiderItemModel) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent);

    dialogRef.afterClosed().subscribe(async result => {
      if (result) {
        await this.spiderItemService.delete(data);
        this.openSnackBar('item remove success', 'Close');
      }
    });
  }

  options = [
    { value: 'auto', label: 'auto' },
    { value: 'html', label: 'html' },
    { value: 'json', label: 'json' },
    { value: 'xml', label: 'xml' },
    { value: 'text', label: 'text' },
    { value: 'file', label:'file'}
  ];  
  

  
}
