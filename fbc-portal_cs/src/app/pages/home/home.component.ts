import { Component, OnInit } from '@angular/core';
import { DateTime } from 'luxon';
import { timer } from 'rxjs';
import { SessionService } from 'src/app/shared/services/session.service';
import { InternosService } from 'src/app/shared/services/internos.service';
import { ComprasService } from 'src/app/shared/services/compras.service';

declare var google: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  DateTime: Date;
    currentUser: any;
    totalDespesas: number = 0;
    totalEncomendas: number = 0; 
    totalEncomendasPorAprovar: number = 0;
    totalDespesasPorAprovar: number = 0;

  //Hour = DateTime.now().toFormat("t")
  //Day = DateTime.now().toFormat("DDD")

    constructor(private sessionService: SessionService, private internosService: InternosService, private comprasService: ComprasService) { 

  }

    ngOnInit(): void {


    this.currentUser = this.sessionService.getUtilizadorFromToken();

    timer(0, 1000).subscribe(()=>{
      this.DateTime =  new Date();
    })
        this.internosService.obterTotalDespesas().subscribe(total => {
            this.totalDespesas = total;
        });

        this.comprasService.obterTotalEncomendas().subscribe(total => {
            this.totalEncomendas = total;
        });

        this.comprasService.obterTotalEncomendasPorAprovar().subscribe(total => {
            this.totalEncomendasPorAprovar = total;
        });

        this.internosService.obterTotalDespesasPorAprovar().subscribe(total => {
            this.totalDespesasPorAprovar = total;
        });

        google.charts.load('current', { 'packages': ['corechart'] });
        google.charts.setOnLoadCallback(this.drawChart.bind(this));
    }

    drawChart() {
        const data = new google.visualization.DataTable();
        data.addColumn('string', 'Status');
        data.addColumn('number', 'Quantidade');
        data.addRows([
            ['Aprovadas', this.totalEncomendas],
            ['Por Aprovar', this.totalEncomendasPorAprovar]
        ]);

        const options = {
            pieHole: 0.6,
            legend: 'none', // Mantém as legendas ocultas
            pieSliceText: 'none', // Exibe as porcentagens nas fatias
            slices: {
                0: { color: '#8C0EC1' }, // Define a cor da primeira fatia para verde
                1: { color: '#9f5dd2' } // Define a cor da segunda fatia para azul
            },
            backgroundColor: 'transparent', // Define o fundo do gráfico como transparente
            width: 400, // Define a largura do gráfico
            height: 300 // Define a altura do gráfico
        };

        const chart = new google.visualization.PieChart(document.getElementById('donutChart'));
        chart.draw(data, options);
    }

}
