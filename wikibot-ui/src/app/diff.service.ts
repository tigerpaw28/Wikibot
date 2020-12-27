import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DiffService {

  constructor(private http: HttpClient) { }

  getDiff(filename:string, folder:string) : Observable<string>
  {
    var diff: string;
    return this.http.get(environment.assetFolderPath+"/"+folder+"/"+filename, {responseType:'text'}
    );
  }
}
