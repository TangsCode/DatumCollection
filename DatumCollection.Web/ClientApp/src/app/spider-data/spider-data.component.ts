import { OnInit, AfterViewInit, Component, ViewChild,Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { ElectronicCommerceWebsite } from '../interface';

@Component({
  selector: 'app-spider-data',
  templateUrl: './spider-data.component.html',
  styleUrls: ['./spider-data.component.css']
})
export class SpiderDataComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = ['select', 'skuName', 'price', 'screenshotPath', 'taxFee', 'createTime'];
  dataSource: MatTableDataSource<ElectronicCommerceWebsite>;
  selection: SelectionModel<ElectronicCommerceWebsite>;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
    

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    const initialSelection = [];
    const allowMultiSelect = true;    
    this.selection = new SelectionModel<ElectronicCommerceWebsite>(allowMultiSelect, initialSelection);
  }

  ngOnInit(): void {
    
  }

  async ngAfterViewInit() {
    if (!this.dataSource) {
      await this.getDataSource();
    }
    this.dataSource.paginator = this.paginator;
    this.sort.active = 'createTime';
    this.sort.direction = 'desc';
    this.dataSource.sort = this.sort;
  }

  async getDataSource() {
    //retriive data from database
    var data = await this.http.get<ElectronicCommerceWebsite[]>(this.baseUrl + 'api/goodsdata/getGoodsData').toPromise();
    // Assign the data to the data source for the table to render
    this.dataSource = new MatTableDataSource(data);
  }

  applyFilter(event: Event) {
    debugger
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

}

 
