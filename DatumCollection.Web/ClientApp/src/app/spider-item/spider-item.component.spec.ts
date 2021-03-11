import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpiderItemComponent } from './spider-item.component';

import { FormsModule } from '@angular/forms';

describe('SpiderItemComponent', () => {
  let component: SpiderItemComponent;
  let fixture: ComponentFixture<SpiderItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SpiderItemComponent],
      imports: [FormsModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpiderItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
