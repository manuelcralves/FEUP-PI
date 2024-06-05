import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { InternosService } from 'src/app/shared/services/internos.service';
import { DocumentoInterno } from 'src/app/shared/interfaces/documento-interno';
import { EstadoDocInternoPipe } from 'src/app/shared/pipes/estado-doc-interno/estado-doc-interno.pipe';
import { ConfirmComponent } from 'src/app/shared/modals/confirm/confirm.component';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.scss']
})
export class ExpensesComponent implements OnInit { 
  

  @BlockUI() blockUI: NgBlockUI;
  @BlockUI("expensesTable") blockUIExpensesTable: NgBlockUI;

  public pesquisaDocumentos = '';
  public documentos: DocumentoInterno[];
  public documentosFiltrados: DocumentoInterno[];

  public pesquisaRascunhos = '';
  public rascunhos: DocumentoInterno[];
  public rascunhosFiltrados: DocumentoInterno[];

  constructor(
    private internosService : InternosService,
    private toastr: ToastrService,
    private router: Router,
    private apiErrorService: ApiErrorResponseHandlerService,
    private modalService: NgbModal,
    private datePipe: DatePipe,
    private estadoDocPipe: EstadoDocInternoPipe
  ) { }

  ngOnInit(): void {
  
    this.loadExpenses();
    this.loadSketchs();
  }

  NewExpense(){
    this.router.navigate(["expenses/new"]);
  }

  editRascunho( id: string) {

    if (!id) {
        this.toastr.error('Identificação de documento não definido.', '', { timeOut: 4000 });
        return;
    }

    this.router.navigate(["expenses/sketch", id]);
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

            this.internosService.deleteRascunho(rascunhoId).subscribe({
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

    this.blockUIExpensesTable.start('A carregar...');

    this.internosService.getRascunhosUtilizador().subscribe({
        next: (rascunhos: DocumentoInterno[]) => {
            this.blockUIExpensesTable.stop();

            this.rascunhos = rascunhos;

            this.filtrarRascunhos();
        },
        error: (error: any) => {
            this.blockUIExpensesTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar rascunhos");
        },
    });
  }

  loadExpenses() {

    this.blockUIExpensesTable.start('A carregar...');

    this.internosService.getDespesas().subscribe({
        next: (documentos: DocumentoInterno[]) => {
            this.blockUIExpensesTable.stop();

            this.documentos = documentos;

            this.filtrarDocumentos();
        },
        error: (error: any) => {
            this.blockUIExpensesTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar despesas");
        },
    });
  } 

  pesquisaRascunhosAlterada(pesquisa: string) {
    this.pesquisaDocumentos = pesquisa;

    this.filtrarRascunhos();
  }

  pesquisaDocumentosAlterada(pesquisa: string) {
    this.pesquisaDocumentos = pesquisa;

    this.filtrarDocumentos();
  }

  filtrarDocumentos() {
    if (!this.pesquisaDocumentos)
        this.documentosFiltrados = this.documentos;
    else {
        const termo = this.pesquisaDocumentos.toLowerCase();

        this.documentosFiltrados = this.documentos?.filter(d =>
            d.NomeEntidade?.toLowerCase().includes(termo) === true
            || d.Entidade?.toLowerCase().includes(termo) === true
            || d.Documento?.toLowerCase().includes(termo) === true
            || this.estadoDocPipe.transform(d.Estado)?.toLowerCase().includes(termo) === true
            || this.datePipe.transform(d.DataDoc, "dd/MM/yyyy")?.toLowerCase().includes(termo) === true
        );
    }
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
}



