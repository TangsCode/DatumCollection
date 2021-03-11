
export interface SpiderItem {
  id: string;
  channel: Channel;
  contentType: string;
  encoding: string;
  method: string;
  url: string;
  skuName: string;
  createTime: Date;
}

export interface Channel {
  id: string;
  channelCode: string;
  channelEnum: number;
  channelName: string;
  closeXPath: string;
  couponXPath: string;
  imageUrlXPath: string;
  postageXPath: string;
  preferentialXPath: string;
  priceXPath: string;
  screenshotXPath: string;
  createTime: Date;
}

export interface ElectronicCommerceWebsite {
  spiderSource: SpiderItem;
  price: number;
  screenshotPath: string;
  taxFee: number;
  postage: string;
  preferential: string;
  coupon: string;
  imageText: string;
  createTime: Date;
}

export interface SpiderScheduleSetting {
  id: string;
  interval: string;
  spiderFrequency: number;
  scheduleDayOfWeek: number;
  scheduleMonthOfYear: number;
  startTime: Date;
  endTime: Date;
  startDate: Date;
  endDate: Date;
  isEnabled: boolean;
  spiderScheduleItems: SpiderScheduleItems[];
  createTime: Date;
}

export interface SpiderScheduleItems {
  fK_SpiderSchedule_ID: string;
  fK_SpiderItem_ID: string;
  spiderItem: SpiderItem;
}


