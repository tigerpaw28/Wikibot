import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RequestService implements OnInit{
  auth_token:string;
  private requestUrl = environment.requestServiceURL;
  httpOptions = {
      headers: new HttpHeaders({
        //'Content-Type':  'application/json',
        //'Authorization': 'Bearer '+ this.auth_token,
        'Access-Control-Allow-Origin': environment.requestServiceURL
      })
    };

  constructor(private http: HttpClient) {
    this.auth_token = localStorage.getItem('auth_token');
    //this.httpOptions.headers = this.httpOptions.headers.set('Authorization', 'Bearer '+ this.auth_token); 
   }

   ngOnInit() {

   }

  private setHeaders(){
    this.auth_token = localStorage.getItem('auth_token');
    this.httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json',
        'Authorization': 'Bearer '+ this.auth_token,
        'Access-Control-Allow-Origin': environment.requestServiceURL
      })
    };
  }

  getRequests():Observable<Request[]>{
    this.setHeaders();
    return this.http.get<Request[]>(this.requestUrl+"/api/Request/requests", this.httpOptions);
  }

  preApproveRequest(id: number){
    this.setHeaders();
    return this.http.post<void>(this.requestUrl+"/api/Request/preapprove?requestId="+id, id, this.httpOptions);
  }

  ApproveRequest(id: number){
    return this.http.post<void>(this.requestUrl+"/api/Request/approve?requestId="+id, id, this.httpOptions);
  }

  RejectRequest(id: number, commentText: string){
    return this.http.post<void>(this.requestUrl+"/api/Request/reject?requestId="+id, JSON.stringify(commentText), this.httpOptions);
  }
}