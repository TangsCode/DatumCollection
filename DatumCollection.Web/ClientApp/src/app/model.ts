import { SpiderScheduleSetting,SpiderItem } from "./interface";


export class SpiderItemModel {
  constructor(item: SpiderItem) {
    if (item) {
      this.id = item.id;
      this.FK_Channel_ID = item.channel.id;
      this.contentType = item.contentType;
      this.encoding = item.encoding;
      this.method = item.method;
      this.url = item.url;
      this.skuName = item.skuName;
    } else {
      this.id = '';
      this.FK_Channel_ID = '';
      this.skuName = '';
      this.contentType = '';
      this.url = '';
      this.method = '';
      this.encoding = '';
    }    
  }
  id: string;
  contentType: string;
  encoding: string;
  method: string;
  url: string;
  skuName: string;
  FK_Channel_ID: string;
}

function toTimeString(value) {
  return (new Date(value)).toLocaleTimeString('it-IT');
}

export class SpiderScheduleSettingModel {
  constructor(item: SpiderScheduleSetting) {
    this.id = item.id;
    this.interval = item.interval;
    this.spiderFrequency = item.spiderFrequency
    this.scheduleDayOfWeek = item.scheduleDayOfWeek;
    this.scheduleMonthOfYear = item.scheduleMonthOfYear;
    this.startTime = toTimeString(item.startTime);
    this.endTime = toTimeString(item.endTime)
    this.startDate = item.startDate;
    this.endDate = item.endDate;
    this.isEnabled = item.isEnabled;
    this.spiderItems = [];
    debugger
    if (item.spiderScheduleItems != null && item.spiderScheduleItems.length > 0) {
      for (var i = 0; i < item.spiderScheduleItems.length; i++) {
        this.spiderItems.push(item.spiderScheduleItems[i].fK_SpiderItem_ID);
      }
    }
  }
  id: string;
  interval: string;
  spiderFrequency: number;
  scheduleDayOfWeek: number;
  scheduleMonthOfYear: number;
  startTime: string;
  endTime: string;
  startDate: Date;
  endDate: Date;
  isEnabled: boolean;
  spiderItems: string[];  
}

export class WebExecutionResult{
  success: boolean;
  errorMsg: string;
}

export enum SpiderFrequency {
  Once,
  Second,
  Minute,
  Day,
  Week,
  Month,
  Season
}

export enum Operation {
  View,
  Add,
  Edit
}

export type Option = {
  key: string,
  value: string
}


