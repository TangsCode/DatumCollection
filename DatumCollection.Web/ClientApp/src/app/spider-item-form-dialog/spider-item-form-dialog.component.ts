import { Component, OnInit, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SpiderItemModel } from '../model';
import { HttpClient } from '@angular/common/http';
import { Channel } from '../interface';

@Component({
  selector: 'app-spider-item-form-dialog',
  templateUrl: './spider-item-form-dialog.component.html',
  styleUrls: ['./spider-item-form-dialog.component.css']
})
export class SpiderItemFormDialogComponent implements OnInit {

  options = [
    { value: 'auto', text: 'auto' },
    { value: 'html', text: 'html' },
    { value: 'json', text: 'json' },
    { value: 'xml', text: 'xml' },
    { value: 'text', text: 'text' },
    { value: 'file', text: 'file' }
  ];

  channels: Channel[];
  selectedValue: string;

  formInstance: FormGroup;
  constructor(public dialogRef: MatDialogRef<SpiderItemFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SpiderItemModel,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {
    this.getChannels();    
    this.formInstance = new FormGroup({
      "id": new FormControl(''),
      "FK_Channel_ID": new FormControl(''),
      "skuName": new FormControl('', Validators.required),
      "contentType": new FormControl('', Validators.required),
      "url": new FormControl('', Validators.required),
      "method": new FormControl(''),
      "encoding": new FormControl('')
    });

    this.formInstance.setValue(data);
  }

  ngOnInit(): void {
    //this.http.get<Channel[]>(this.baseUrl + 'api/spideritem/getChannels').subscribe(data => {      
    //  this.channels = data;
    //})
  }

  async getChannels() {    
    var data = await this.http.get<Channel[]>(this.baseUrl + 'api/spideritem/getChannels').toPromise();
    this.channels = data;
  }

  save(): void {
    debugger
    this.dialogRef.close(Object.assign({}, this.formInstance.value));
  }

}
