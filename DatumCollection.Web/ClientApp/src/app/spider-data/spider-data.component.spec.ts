import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpiderDataComponent } from './spider-data.component';

describe('SpiderDataComponent', () => {
  let component: SpiderDataComponent;
  let fixture: ComponentFixture<SpiderDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpiderDataComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SpiderDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
