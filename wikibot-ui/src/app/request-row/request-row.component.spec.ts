import { Overlay, OverlayModule } from '@angular/cdk/overlay';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { Request } from '../request';
import { RequestService } from '../request.service';
import { RequestRowComponent } from './request-row.component';

describe('RequestRowComponent', () => {
  let component: RequestRowComponent;
  let fixture: ComponentFixture<RequestRowComponent>;

  const testRequest = {
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
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestRowComponent ],
      imports: [OverlayModule],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestRowComponent);
    component = fixture.componentInstance;
    component.request = testRequest;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
