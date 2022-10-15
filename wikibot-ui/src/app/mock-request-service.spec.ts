import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { MockRequestService } from './mock-request-service';


describe('MockRequestService', () => {
  let service: MockRequestService;
  beforeEach(() => {
  TestBed.configureTestingModule({
    imports: [HttpClientTestingModule]
  });
  service = TestBed.inject(MockRequestService);
  });
  it('should create an instance', () => {
    expect(service).toBeTruthy();
  });
});
