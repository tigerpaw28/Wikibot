import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { MockRequestService } from './mock-request-service';

import { RequestService } from './request.service';

describe('RequestService', () => {
  let service: MockRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(MockRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
