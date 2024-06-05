import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { InternosService } from 'src/app/shared/services/internos.service';
import { DocumentoInterno } from 'src/app/shared/interfaces/documento-interno';
import { DocumentoCompra } from 'src/app/shared/interfaces/documento-compra';
import { ComprasService } from 'src/app/shared/services/compras.service';

@Component({
  selector: 'app-approval',
  templateUrl: './approval.component.html',
  styleUrls: ['./approval.component.scss']
})
export class ApprovalComponent implements OnInit {

  @BlockUI() blockUI: NgBlockUI;
  @BlockUI("approvalTable") blockUIApprovalTable: NgBlockUI;

  public pesquisaDespesas = '';
  public despesas: DocumentoInterno[];
  public despesasFiltradas: DocumentoInterno[];

  public pesquisaEncomendas = '';
  public encomendas: DocumentoCompra[];
  public encomendasFiltradas: DocumentoCompra[];

  constructor(
    private comprasService : ComprasService,
    private internosService : InternosService,
    private toastr: ToastrService,
    private router: Router,
    private apiErrorService: ApiErrorResponseHandlerService,
    private modalService: NgbModal,
    private datePipe: DatePipe,
  ) { }

  ngOnInit(): void {

    this.loadExpenses()
    this.loadOrders();
  }

  loadExpenses(){

    this.blockUIApprovalTable.start('A carregar...');

    this.internosService.getRascunhosAprovacao().subscribe({
        next: (despesas: DocumentoInterno[]) => {
            this.blockUIApprovalTable.stop();

            this.despesas = despesas;

            this.filtrarDespesas();
        },
        error: (error: any) => {
            this.blockUIApprovalTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar despesas");
        },
    });
  }

  filtrarDespesas() {
    if (!this.pesquisaDespesas)
        this.despesasFiltradas = this.despesas;
    else {
        const termo = this.pesquisaDespesas.toLowerCase();

        this.despesasFiltradas = this.despesas?.filter(d =>
            d.NomeEntidade?.toLowerCase().includes(termo) === true
            || d.Entidade?.toLowerCase().includes(termo) === true
            || d.Documento?.toLowerCase().includes(termo) === true
            || d.Utilizador?.toLowerCase().includes(termo) === true
            //|| this.estadoDocPipe.transform(d.Estado)?.toLowerCase().includes(termo) === true
            || this.datePipe.transform(d.DataDoc, "dd/MM/yyyy")?.toLowerCase().includes(termo) === true
        );
    }
  }

  pesquisaDespesasAlterada(pesquisa: string) {
    this.pesquisaDespesas = pesquisa;

    this.filtrarDespesas();
  }

  
  loadOrders(){

    this.blockUIApprovalTable.start('A carregar...');

    this.comprasService.getRascunhosAprovacao().subscribe({
        next: (encomendas: DocumentoCompra[]) => {
            this.blockUIApprovalTable.stop();

            this.encomendas = encomendas;

            this.filtrarEncomendas();
        },
        error: (error: any) => {
            this.blockUIApprovalTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar encomendas");
        },
    });
  }

  pesquisaEncomendasAlterada(pesquisa: string) {
    this.pesquisaEncomendas = pesquisa;

    this.filtrarEncomendas();
  }

  filtrarEncomendas() {
    if (!this.pesquisaEncomendas)
        this.encomendasFiltradas = this.encomendas;
    else {
        const termo = this.pesquisaEncomendas.toLowerCase();

        this.encomendasFiltradas = this.encomendas?.filter(d =>
            d.NomeEntidade?.toLowerCase().includes(termo) === true
            || d.Entidade?.toLowerCase().includes(termo) === true
            || d.Documento?.toLowerCase().includes(termo) === true
            || this.datePipe.transform(d.DataDoc, "dd/MM/yyyy")?.toLowerCase().includes(termo) === true
        );
    }
  }

  editDespesa( id: string) {

    if (!id) {
        this.toastr.error('Identificação de documento não definido.', '', { timeOut: 4000 });
        return;
    }

    this.router.navigate(["expenses/approval", id]);
  }

  editEncomenda( id: string) {

    if (!id) {
        this.toastr.error('Identificação de documento não definido.', '', { timeOut: 4000 });
        return;
    }

    this.router.navigate(["orders/approval", id]);
  }

}
