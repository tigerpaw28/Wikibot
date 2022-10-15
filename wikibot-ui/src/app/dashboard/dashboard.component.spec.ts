import { HttpClient, HttpHandler } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppRoutingModule } from '../app-routing.module';
import { MockRequestService } from '../mock-request-service';
import { RequestService } from '../request.service';
import { ResultsTableComponent } from '../results-table/results-table.component';

import { DashboardComponent } from './dashboard.component';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardComponent, ResultsTableComponent ],
      imports: [ AppRoutingModule],
      providers: [ResultsTableComponent, HttpHandler, HttpClientTestingModule, HttpClient,
        { provides: RequestService, useClass: MockRequestService },
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    TestBed.inject(MockRequestService);
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
