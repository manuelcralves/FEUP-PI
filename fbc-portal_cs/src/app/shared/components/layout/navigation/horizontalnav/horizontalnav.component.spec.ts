import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { HorizontalnavComponent } from './horizontalnav.component';

describe('HorizontalnavComponent', () => {
  let component: HorizontalnavComponent;
  let fixture: ComponentFixture<HorizontalnavComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ HorizontalnavComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HorizontalnavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
