import { Component, OnInit } from '@angular/core';
import { DateTime } from 'luxon';
import { timer } from 'rxjs';
import { SessionService } from 'src/app/shared/services/session.service';
import { InternosService } from 'src/app/shared/services/internos.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  DateTime: Date;
    currentUser: any;
    totalDespesas: number = 0;

  //Hour = DateTime.now().toFormat("t")
  //Day = DateTime.now().toFormat("DDD")

    constructor(private sessionService: SessionService, private internosService: InternosService) { 

  }

    ngOnInit(): void {
        this.internosService.obterTotalDespesas().subscribe(total => {
            this.totalDespesas = total;
        });

    this.currentUser = this.sessionService.getUtilizadorFromToken();

    timer(0, 1000).subscribe(()=>{
      this.DateTime =  new Date();
    })

  }

}
