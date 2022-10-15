import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { Request } from "./request";

@Injectable({
    providedIn: 'root'
  })
  
export class MockRequestService {

    testRequest:Request = {
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
        constructor(private http: HttpClient) {
        }
        
        getRequests():Observable<Request[]>{
            return of([this.testRequest]);
        }

}
