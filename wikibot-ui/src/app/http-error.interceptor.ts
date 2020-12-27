import { Overlay } from '@angular/cdk/overlay';
import {

    HttpEvent,
   
    HttpInterceptor,
   
    HttpHandler,
   
    HttpRequest,
   
    HttpResponse,
   
    HttpErrorResponse
   
   } from '@angular/common/http';
import { Injectable } from '@angular/core';
   
   import { Observable, throwError } from 'rxjs';
   
   import { retry, catchError, finalize } from 'rxjs/operators';
   import { SpinnerOverlayService } from './spinner-overlay.service';
   
   @Injectable({
    providedIn: 'root',
  })
   
   export class HttpErrorInterceptor implements HttpInterceptor {
    spinnerOverlayService : SpinnerOverlayService;
    constructor(spinnerOverlayService: SpinnerOverlayService){
      this.spinnerOverlayService = spinnerOverlayService;
    }
   
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
   
      this.spinnerOverlayService.show();
    
      return next.handle(request)
   
        .pipe(
   
          retry(1),
   
          catchError((error: HttpErrorResponse) => {
   
            let errorMessage = '';
   
            if (error.error instanceof ErrorEvent) {
   
              // client-side error
   
              errorMessage = `Error: ${error.error.message}`;
   
            } else {
   
              // server-side error
   
              errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
   
            }
   
            window.alert(errorMessage);
   
            return throwError(errorMessage);
   
          }),
          finalize(()=> {
            this.spinnerOverlayService.hide();
          })
        )
    }
   
   }