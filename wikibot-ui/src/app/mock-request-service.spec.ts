import { HttpClient, HttpClientModule, HttpHandler } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { MockRequestService } from './mock-request-service';


describe('MockRequestService', () => {
  let service: MockRequestService;
  beforeEach(() => {
  TestBed.configureTestingModule({
    imports: [HttpClientModule]
  });
  service = TestBed.inject(MockRequestService);
  });
  it('should create an instance', () => {
    expect(service).toBeTruthy();
  });
});
