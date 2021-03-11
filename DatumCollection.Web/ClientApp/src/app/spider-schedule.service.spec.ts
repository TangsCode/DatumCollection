import { TestBed } from '@angular/core/testing';

import { SpiderScheduleService } from './spider-schedule.service';

describe('SpiderScheduleService', () => {
  let service: SpiderScheduleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SpiderScheduleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
