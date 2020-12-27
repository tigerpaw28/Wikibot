import { Component, OnInit } from '@angular/core';
import { RequestService } from '../request.service';
import { Observable } from 'rxjs/internal/Observable';

@Component({
  selector: 'app-results-table',
  templateUrl: './results-table.component.html',
  styleUrls: ['./results-table.component.css']
})
export class ResultsTableComponent implements OnInit {
  requests$: Observable<Request[]>;
  constructor(private requestService: RequestService) { 
    this.requestService = requestService;
  }

  ngOnInit(): void {
    this.requests$ = this.requestService.getRequests();
  }

}
