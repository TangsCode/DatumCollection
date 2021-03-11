import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { SpiderItemModel } from './model';
import { SpiderItem, ElectronicCommerceWebsite } from './interface';

@Injectable({
  providedIn: 'root'
})
export class SpideritemService {

  items$: BehaviorSubject<SpiderItem[]>;
  items: Array<SpiderItem>;

  selectOptions = [];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.items$ = new BehaviorSubject([]);  
  }

  async getAll() {
    await this.query();    
    this.items$.next(this.items);
  }

  async getItemPriceTrends(id: string): Promise<ElectronicCommerceWebsite[]> {
    var data = await this.http.get<ElectronicCommerceWebsite[]>(this.baseUrl + 'api/spideritem/queryItemsPrice', { params: {'id': id} }).toPromise();    
    return data;
  }

  async query() {
    var data = await this.http.get<SpiderItem[]>(this.baseUrl + 'api/spideritem/query').toPromise();
    this.items = data;
    this.items$.next(this.items);
  }

  async add(item: SpiderItem) {
    this.http.post(this.baseUrl + 'api/spideritem/add', item).subscribe(async result => {
      debugger
      if (result) { }
      await this.query();
    }, error => { console.log(error) });    
  }

  async edit(item: SpiderItemModel) {
    this.http.post(this.baseUrl + 'api/spideritem/edit', item).subscribe(result => {
      debugger
      if (result) { }
    }, error => { console.log(error) });
  }

  async delete(item: SpiderItemModel) {
    debugger
    this.http.post(this.baseUrl + 'api/spideritem/delete', item).subscribe(result => {
      debugger
      if (result) { }
    }, error => { console.log(error) });

    await this.getAll();
  }
}
