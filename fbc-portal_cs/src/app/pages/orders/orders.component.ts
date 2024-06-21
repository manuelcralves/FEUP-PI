import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { ComprasService } from 'src/app/shared/services/compras.service';
import { DocumentoCompra } from 'src/app/shared/interfaces/documento-compra';
import { ConfirmComponent } from 'src/app/shared/modals/confirm/confirm.component';
import { EstadoDocInternoPipe } from 'src/app/shared/pipes/estado-doc-interno/estado-doc-interno.pipe';


@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;
  @BlockUI("ordersTable") blockUIOrdersTable: NgBlockUI;

  public pesquisa = '';
  public documentos: any[];
  public documentosFiltrados: any[];

  public pesquisaRascunhos = '';
  public rascunhos: any[];
    public rascunhosFiltrados: any[];

    public totalEncomendasPorAprovar: number = 0;

  constructor(
    private comprasService: ComprasService,
    private toastr: ToastrService,
    private router: Router,
    private apiErrorService: ApiErrorResponseHandlerService,
    private modalService: NgbModal,
    private datePipe: DatePipe,
    private estadoDocPipe: EstadoDocInternoPipe
  ) { }

  ngOnInit(): void {

    this.loadOrders()
    this.loadSketchs()
  }


  editRascunho( id: string) {

    if (!id) {
        this.toastr.error('Identificação de documento não definido.', '', { timeOut: 4000 });
        return;
    }

    this.router.navigate(["orders/sketch", id]);
  }

  deleteRascunho(rascunhoId: string) {

    if (!rascunhoId) {
        this.toastr.error('Id não definido.', '', { timeOut: 4000 });
        return;
    }

    const modalRef = this.modalService.open(ConfirmComponent, { windowClass: 'animated fadeInDown', centered: true });

    modalRef.componentInstance.modalTitle = "Eliminar documento?";
    modalRef.componentInstance.modalMessage = "Tem a certeza que deseja eliminar o documento?";

    modalRef.result
        .then((value: any) => {

          this.blockUI.start('A emilinar documento...');

            this.comprasService.deleteRascunho(rascunhoId).subscribe({
              next: (res: void) => {
                  this.blockUI.stop();
      
                  this.toastr.success('Documento eliminado!', '', { timeOut: 2500 });
      
                  this.loadSketchs();
              },
              error: (error: any) => {
                  this.blockUI.stop();
      
                  this.apiErrorService.handleError(error, "Erro a eliminar documento");
              }
          });
        })
        .catch((reason: any) => {

        });
  }

  loadSketchs(){

    this.blockUIOrdersTable.start('A carregar...');

    this.comprasService.getRascunhosUtilizador().subscribe({
        next: (rascunhos: DocumentoCompra[]) => {
            this.blockUIOrdersTable.stop();

            this.rascunhos = rascunhos;

            this.filtrarRascunhos();
        },
        error: (error: any) => {
            this.blockUIOrdersTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar rascunhos");
        },
    });
  }

  pesquisaRascunhosAlterada(pesquisa: string) {
    this.pesquisaRascunhos = pesquisa;

    this.filtrarRascunhos();
  }

  filtrarRascunhos() {
    if (!this.pesquisaRascunhos)
        this.rascunhosFiltrados = this.rascunhos;
    else {
        const termo = this.pesquisaRascunhos.toLowerCase();

        this.rascunhosFiltrados = this.rascunhos?.filter(d =>
            d.NomeEntidade?.toLowerCase().includes(termo) === true
            || d.Entidade?.toLowerCase().includes(termo) === true
            || d.Documento?.toLowerCase().includes(termo) === true
            || this.estadoDocPipe.transform(d.Estado)?.toLowerCase().includes(termo) === true
            || this.datePipe.transform(d.DataDoc, "dd/MM/yyyy")?.toLowerCase().includes(termo) === true
        );
    }
  }




  NewOrder(){
    this.router.navigate(["orders/new"]);
  }

  loadOrders() {

    this.blockUIOrdersTable.start('A carregar...');

    this.comprasService.getEncomendas().subscribe({
        next: (documentos: DocumentoCompra[]) => {
            this.blockUIOrdersTable.stop();

            this.documentos = documentos;

            this.filtrarDocumentos();

            this.comprasService.atualizarTotalEncomendas(this.documentos.length);

        },
        error: (error: any) => {
            this.blockUIOrdersTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar encomendas");
        },
    });
  } 

  pesquisaAlterada(pesquisa: string) {
    this.pesquisa = pesquisa;

    this.filtrarDocumentos();
  }

  filtrarDocumentos() {
    if (!this.pesquisa)
        this.documentosFiltrados = this.documentos;
    else {
        const termo = this.pesquisa.toLowerCase();

        this.documentosFiltrados = this.documentos?.filter(d =>
            d.NomeEntidade?.toLowerCase().includes(termo) === true
            || d.Entidade?.toLowerCase().includes(termo) === true
            || d.Documento?.toLowerCase().includes(termo) === true
            || this.datePipe.transform(d.Data, "dd/MM/yyyy")?.toLowerCase().includes(termo) === true
        );
    }
  }

}
