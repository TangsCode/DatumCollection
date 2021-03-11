import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpiderScheduleComponent } from './spider-schedule.component';

describe('SpiderScheduleComponent', () => {
  let component: SpiderScheduleComponent;
  let fixture: ComponentFixture<SpiderScheduleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpiderScheduleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SpiderScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
