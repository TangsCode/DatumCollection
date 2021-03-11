import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as Highcharts from 'highcharts';
import { SpideritemService } from '../spideritem.service';

@Component({
  selector: 'app-price-line-chart',
  templateUrl: './price-line-chart.component.html',
  styleUrls: ['./price-line-chart.component.css'],
})
export class PriceLineChartComponent implements OnInit {

  Highcharts: any; // 必填
  chartConstructor = 'chart'; // 可选 String，默认为 'chart'
  chartOptions = { };
  chartCallback = function (chart) {  } // 可选 Function，图表加载后的回调函数，默认为空
  updateFlag = true; // 可选 Boolean
  oneToOneFlag = true; // 可选 Boolean，默认为 false
  runOutsideAngular = false; // 可选 Boolean，默认为 false

  itemId: string;
  prices = [];
  seriesName: string;

  constructor(private _activatedroute: ActivatedRoute,
    private _service: SpideritemService
  ) { }

  ngOnInit() {
    this.itemId = this._activatedroute.snapshot.params['id'];    
    this._service.getItemPriceTrends(this.itemId).then(items => {
      items.forEach(item => {
        var _date = new Date(item.createTime);
        var _utc = Date.UTC(_date.getFullYear(), _date.getMonth(), _date.getDate(), _date.getHours(), _date.getMinutes(), _date.getSeconds());
        this.prices.push([_utc, item.price]);
      })

      this.Highcharts = Highcharts;
      this.chartOptions = {
        title: { text: 'item price trends' },
        subtitle: { text: 'in last 24 hours' },
        xAxis: {
          type: 'datetime',
          dateTimeLabelFormats: {
            millisecond: '%H:%M:%S.%L',
            second: '%H:%M:%S',
            minute: '%H:%M',
            hour: '%H:%M',
            day: '%m-%d',
            week: '%m-%d',
            month: '%Y-%m',
            year: '%Y'
          }
        },
        yAxis: {
          title: {
            text: '价格'
          },
          min: 0
        },
        series: [{
          name: items[0].spiderSource.skuName,
          data: this.prices
        }]
      };

      //this.Highcharts.chart({
      //  series: [
      //    {
      //      type: 'line',
      //      name: items[0].spiderSource.skuName,
      //      data: this.prices
      //    }
      //  ]
      //})
      
    });    
    
  }


}

