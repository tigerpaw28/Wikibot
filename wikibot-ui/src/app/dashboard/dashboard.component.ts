import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  loading: boolean;
  error: boolean;
  success: boolean;
  constructor(private route: ActivatedRoute) { }

  ngOnInit() : void{
    localStorage.setItem('auth_token',this.route.snapshot.queryParamMap.get('auth_token')); //['auth_token']);
    var test = 'test;'
    this.route.snapshot.queryParams
    ////.filter(params => params.order)
    //.subscribe(params => {
     // var auth_token = params['token'];
     // localStorage.setItem('auth_token', auth_token)
    //});
  }

}
