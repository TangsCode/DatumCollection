import { TestBed } from '@angular/core/testing';

import { SpideritemService } from './spideritem.service';

describe('SpideritemService', () => {
  let service: SpideritemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SpideritemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
