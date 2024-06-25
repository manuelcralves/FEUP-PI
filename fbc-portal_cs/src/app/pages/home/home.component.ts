import { Component, OnInit } from '@angular/core';
import { DateTime } from 'luxon';
import { timer } from 'rxjs';
import { SessionService } from 'src/app/shared/services/session.service';
import { InternosService } from 'src/app/shared/services/internos.service';
import { ComprasService } from 'src/app/shared/services/compras.service';
import { forkJoin } from 'rxjs';

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

        timer(0, 1000).subscribe(() => {
            this.DateTime = new Date();
        });

        forkJoin({
            totalDespesas: this.internosService.obterTotalDespesas(),
            totalEncomendas: this.comprasService.obterTotalEncomendas(),
            totalEncomendasPorAprovar: this.comprasService.obterTotalEncomendasPorAprovar(),
            totalDespesasPorAprovar: this.internosService.obterTotalDespesasPorAprovar(),
        }).subscribe(({ totalDespesas, totalEncomendas, totalEncomendasPorAprovar, totalDespesasPorAprovar }) => {
            this.totalDespesas = totalDespesas;
            this.totalEncomendas = totalEncomendas;
            this.totalEncomendasPorAprovar = totalEncomendasPorAprovar;
            this.totalDespesasPorAprovar = totalDespesasPorAprovar;

            google.charts.load('current', { 'packages': ['corechart'] });

            google.charts.setOnLoadCallback(() => {
                this.drawChartEncomendas();
                this.drawChartDespesas();
                this.drawLineChartEncomendas();
            });
        });
    }


    drawChartEncomendas() {
        const data = new google.visualization.DataTable();
        data.addColumn('string', 'Status');
        data.addColumn('number', 'Quantidade');
        data.addRows([
            ['Aprovadas', this.totalEncomendas],
            ['Por Aprovar', this.totalEncomendasPorAprovar]
        ]);

        const options = {
            pieHole: 0.6,
            legend: 'none', 
            pieSliceText: 'none', 
            slices: {
                0: { color: '#8C0EC1' }, 
                1: { color: '#9f5dd2' }
            },
            backgroundColor: 'transparent', 
            width: 400, 
            height: 300 
        };

        const chart = new google.visualization.PieChart(document.getElementById('donutChartEncomendas'));
        chart.draw(data, options);
    }

    drawLineChartEncomendas() {
        const data = google.visualization.arrayToDataTable([
            ['Trimestre', 'Encomendas Aprovadas', { 'type': 'string', 'role': 'style' }],
            ['Jan-Mar', 4, 'point { size: 5; shape-type: circle; fill-color: #18eadf; }'],
            ['Abr-Jun', 2, 'point { size: 5; shape-type: circle; fill-color: #18eadf; }'],
            ['Jul-Set', 8, 'point { size: 5; shape-type: circle; fill-color: #18eadf; }'],
            ['Out-Dez', 6, 'point { size: 5; shape-type: circle; fill-color: #18eadf; }']
        ]);

        const options = {
            curveType: 'none',
            legend: 'none',
            backgroundColor: 'transparent',
            width: 800,
            height: 340,
            hAxis: {
                gridlines: {
                    color: 'transparent'
                },
                textStyle: {
                    color: '#8C0EC1',
                    bold: true
                },
                baselineColor: 'transparent'
            },
            vAxis: {
                gridlines: {
                    color: 'transparent'
                },
                textStyle: {
                    color: '#8C0EC1',
                    bold: true
                },
                baselineColor: 'transparent'
            },
            series: {
                0: {
                    color: '#8C0EC1', 
                    pointsVisible: true,
                }
            }
        };

        const chart = new google.visualization.LineChart(document.getElementById('lineChartEncomendas'));
        chart.draw(data, options);
    }


    drawChartDespesas() {
        const data = new google.visualization.DataTable();
        data.addColumn('string', 'Status');
        data.addColumn('number', 'Quantidade');
        data.addRows([
            ['Aprovadas', 6], // placeholders pois não há valores das despesas
            ['Por Aprovar', 1]
        ]);

        const options = {
            pieHole: 0.6,
            legend: 'none', 
            pieSliceText: 'none', 
            slices: {
                0: { color: '#8C0EC1' },
                1: { color: '#9f5dd2' } 
            },
            backgroundColor: 'transparent', 
            width: 400, 
            height: 300 
        };

        const chartDespesas = new google.visualization.PieChart(document.getElementById('donutChartDespesas'));
        chartDespesas.draw(data, options);
    }

}
