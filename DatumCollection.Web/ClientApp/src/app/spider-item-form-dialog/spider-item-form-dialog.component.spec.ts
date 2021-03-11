import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpiderItemFormDialogComponent } from './spider-item-form-dialog.component';

describe('SpiderItemFormDialogComponent', () => {
  let component: SpiderItemFormDialogComponent;
  let fixture: ComponentFixture<SpiderItemFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpiderItemFormDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SpiderItemFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
