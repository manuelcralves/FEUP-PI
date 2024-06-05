import { Component, OnInit } from '@angular/core';
import { DateTime } from 'luxon';
import { timer } from 'rxjs';
import { SessionService } from 'src/app/shared/services/session.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  DateTime: Date;
  currentUser: any;

  //Hour = DateTime.now().toFormat("t")
  //Day = DateTime.now().toFormat("DDD")

  constructor( private sessionService: SessionService) { 

  }

  ngOnInit(): void {

    this.currentUser = this.sessionService.getUtilizadorFromToken();

    timer(0, 1000).subscribe(()=>{
      this.DateTime =  new Date();
    })

  }

}
