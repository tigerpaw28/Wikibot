import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestReviewPanelComponent } from './request-review-panel.component';

describe('RequestReviewPanelComponent', () => {
  let component: RequestReviewPanelComponent;
  let fixture: ComponentFixture<RequestReviewPanelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestReviewPanelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestReviewPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
