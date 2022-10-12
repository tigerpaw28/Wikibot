import { OverlayModule } from '@angular/cdk/overlay';
import { TestBed } from '@angular/core/testing';

import { SpinnerOverlayService } from './spinner-overlay.service';

describe('SpinnerOverlayService', () => {
  let service: SpinnerOverlayService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [OverlayModule]
    });
    service = TestBed.inject(SpinnerOverlayService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
