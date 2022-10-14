import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed, inject, getTestBed } from '@angular/core/testing';
import { Request as WikiRequest } from '../request';
import { MockRequestService } from '../mock-request-service';

import { CONTAINER_DATA, RequestReviewPanelComponent } from './request-review-panel.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RequestService } from '../request.service';

describe('RequestReviewPanelComponent', () => {
  let component: RequestReviewPanelComponent;
  let fixture: ComponentFixture<RequestReviewPanelComponent>;
  const testRequest:WikiRequest = {
    id: 1, 
    statusName: 'Approved',
    comment: 'Someone messed up',
    requestingUsername: 'Tigerpaw28',
    submittedDateUTC:  new Date('09/28/2020'),
    timePreStartedUTC: null,
    timeStartedUTC: null,
    timePreFinishedUTC: null,
    timeFinishedUTC: null,
    rawRequest: 'Convert a=>b',
    notes: 'Nada',
    diffs: []
  }
  const injectorTokens = new WeakMap();
  injectorTokens.set(CONTAINER_DATA, testRequest);

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestReviewPanelComponent],
      imports: [HttpClientModule, FontAwesomeModule],
      providers: [{ provide: CONTAINER_DATA, useValue:testRequest },
        { provides: RequestService, useClass: MockRequestService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestReviewPanelComponent);
    TestBed.inject(CONTAINER_DATA);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
