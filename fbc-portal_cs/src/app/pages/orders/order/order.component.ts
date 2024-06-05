import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, ChangeDetectorRef, TemplateRef, ViewChild, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NgbDateStruct, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DatatableComponent } from '@siemens/ngx-datatable';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, fromEvent, Subscription } from 'rxjs';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';


import { AppConstants } from 'src/app/shared/helpers/app.constants';
import { DecimalPrecision } from 'src/app/shared/helpers/decimal-precision';
import { DateTime } from 'luxon';
import { LinhaDocumentoVenda } from 'src/app/shared/interfaces/linha-documento-venda';
import { Anexo } from 'src/app/shared/interfaces/anexo';
import { SerieVendas } from 'src/app/shared/interfaces/serie-vendas';
import { TipoDocumentosVendas } from 'src/app/shared/interfaces/tipo-documentos-vendas';
import { SelecionarArtigoComponent } from 'src/app/shared/modals/selecionar-artigo/selecionar-artigo.component';
import { Artigo } from 'src/app/shared/interfaces/artigo';
import { SelecionarClienteComponent } from 'src/app/shared/modals/selecionar-cliente/selecionar-cliente.component';
import { Cliente } from 'src/app/shared/interfaces/cliente';
import { SelecionarUnidadeComponent } from 'src/app/shared/modals/selecionar-unidade/selecionar-unidade.component';
import { Unidade } from 'src/app/shared/interfaces/unidade';
import { SelecionarFornecedorComponent } from 'src/app/shared/modals/selecionar-fornecedor/selecionar-fornecedor.component';
import { Fornecedor } from 'src/app/shared/interfaces/fornecedor';
import { ComprasService } from 'src/app/shared/services/compras.service';
import { SelecionarObraComponent } from 'src/app/shared/modals/selecionar-obra/selecionar-obra.component';
import { Obra } from 'src/app/shared/interfaces/obra';
import { DocumentoCompra } from 'src/app/shared/interfaces/documento-compra';
import { BaseService } from 'src/app/shared/services/base.service';
import { ConfirmComponent } from 'src/app/shared/modals/confirm/confirm.component';
import { Utilizador } from 'src/app/shared/interfaces/utilizador';
import { SessionService } from 'src/app/shared/services/session.service';


@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss']
})
export class OrderComponent implements OnInit{

    public titulo = 'Encomenda';
    public documentoId = "";
    public sketchId = "";
    private obraId = "";
    public estado = "";
    private sessionUser: Utilizador;
    public AprovacaoDocumento: FormGroup;
    public EditorDocumento: FormGroup;
    public EditorLinha: FormGroup;
    public dropZoneFiles: File[] = [];
    public numericMask = { mask: Number, scale: 4, thousandsSeparator: ' ', padFractionalZeros: true, signed: false, radix: ',', mapToRadix: ['.'], min: 0, max: 9999999999.9999 };
    public nrViasMask = { mask: Number, scale: 0, thousandsSeparator: ' ', signed: false, min: 1, max: 6 };
    public formDesativado = false;
    public windowWidth = 0;
    public workaroundTabela1 = false;
    public workaroundTabela2 = false;
    public approvalMode = false;


    private sketch: DocumentoCompra;
    public resizeSubscription: Subscription;
    public tiposDocumento: TipoDocumentosVendas[];
    public series: SerieVendas[];
    public lstLinhas: LinhaDocumentoVenda[] = [];
    public lstAnexos: Anexo[] = [];
    public linhaEditar: LinhaDocumentoVenda;
    public pedidoSeries: Promise<any>;
    private mainRoute: string = "/orders"


  @BlockUI() blockUI: NgBlockUI;
  @BlockUI('formOrder') blockUIFormOrder: NgBlockUI;

  @ViewChild("LinhaModalContent") linhaModalContent: TemplateRef<any>;
  @ViewChild("tabelaLinhas") tabelaLinhas: DatatableComponent;


  public tamanhosEsconderColunas = {
    "Quantidade": 1600,
    "Preco": 1750,
    "Total": 1900,
};

    // convenience getter for easy access to form fields
    get approvalControls() { return this.AprovacaoDocumento.controls; }
    get documentoControls() { return this.EditorDocumento.controls; }
    get linhaControls() { return this.EditorLinha.controls; }

  constructor(
    private formBuilder: FormBuilder,
    private sessionService: SessionService,
    private activatedRoute: ActivatedRoute,
    private modalService: NgbModal,
    private cd: ChangeDetectorRef,
    private router: Router,
    private toastr: ToastrService,
    private baseService: BaseService,
    private comprasService: ComprasService,
    private apiErrorService: ApiErrorResponseHandlerService,
  ) { }

  ngOnInit(): void {

    this.refreshWindowWidth();

    // atualizar propriedade com tamanho de janela quando se faz resize, mas com um "debounceTime" de 200ms para evitar eventos que disparam várias vezes seguidos (causava problemas de performance)
    this.resizeSubscription = fromEvent(window, 'resize').pipe(debounceTime(200)).subscribe(evt => { this.refreshWindowWidth(); });

    this.sessionUser = this.sessionService.getUtilizadorFromToken()

    this.EditorLinha = this.formBuilder.group({
      Artigo: [undefined, [Validators.required, Validators.maxLength(48)]],
      Descricao: [undefined, Validators.maxLength(512)],
      DataEntrega: [DateTime.now(), Validators.required],
      Quantidade: [undefined, [Validators.required, Validators.min(0), Validators.max(9999999999.9999)]],
      Preco: [undefined, [Validators.required, Validators.min(0), Validators.max(9999999999.9999)]],
      Unidade: [undefined],
      Total: [{ value: undefined, disabled: true }],
      Observacoes: [undefined, Validators.maxLength(4000)],
  });

    this.EditorDocumento = this.formBuilder.group({
      TipoDoc: [undefined, Validators.required],
      Serie: [undefined, Validators.required],
      NumDoc: [{ value: undefined, disabled: true }],
      Data: [DateTime.now(), Validators.required],
      DataVenc: [DateTime.now(), Validators.required],
      Estado: [undefined],
      Fornecedor: [undefined,[Validators.required,Validators.maxLength(12)]],
      NomeFornecedor: [{ value: undefined, disabled: true,}],
      Obra: [undefined, Validators.maxLength(12)],
      NomeObra: [{ value: undefined, disabled: true }],
      DescFornecedor:[undefined, [Validators.required, Validators.min(0), Validators.max(100.00)]],
      DescFinanceiro: [undefined, [Validators.required, Validators.min(0), Validators.max(100.00)]],
      Utilizador: [{ value: undefined, disabled: true }],
      Aprovador: [{ value: undefined, disabled: true }],
      DataAprovacao: [{ value: undefined, disabled: true }],
      MotivoRejeicao: [undefined],
     });

     this.loadTiposDocumento();

     this.documentoControls.DescFornecedor.setValue(0);
     this.documentoControls.DescFinanceiro.setValue(0);

     this.documentoControls.TipoDoc.valueChanges.subscribe(val => {
        this.loadSeries(val);
      });

  this.documentoControls.Serie.valueChanges.subscribe(val => {
      if (!this.documentoId)
         this.getProximoNumeroSerie();
  });

     this.linhaControls.Quantidade.valueChanges.subscribe(val => {
      this.linhaControls.Total.setValue(DecimalPrecision.round(DecimalPrecision.round(val, 4) * DecimalPrecision.round(this.linhaControls.Preco.value, 4), 4));
     
  });

    this.linhaControls.Preco.valueChanges.subscribe(val => {
      this.linhaControls.Total.setValue(DecimalPrecision.round(DecimalPrecision.round(this.linhaControls.Quantidade.value, 4) * DecimalPrecision.round(val, 4), 4));
  });

  this.blockUI.start('A carregar...');

            
  this.activatedRoute.params.subscribe({
      next: (params: Params) => {
          this.blockUI.stop();

          if(this.router.url.includes('/sketch/'))
          {
          
            if (params.id) {
                this.sketchId = params.id;
                this.loadSketch();
            }
          }


          if(this.router.url.includes('/approval/'))
          {
            this.approvalMode = true;

            if (params.id) {
                this.sketchId = params.id;
                this.loadSketch();
            }
          }

      },
      error: (error: any) => {
          this.blockUI.stop();
          console.error("error on ngOnInit activatedRoute.params.subscribe", error);
      }
  });
}

  onFileSelect(event) {
    this.dropZoneFiles.push(...event.addedFiles);
}

onFileRemove(event) {
    const index = this.dropZoneFiles.indexOf(event);
    if (index >= 0)
    this.dropZoneFiles.splice(index, 1);
}

  refreshWindowWidth() {
    this.windowWidth = window.innerWidth;
  }

  onSubmit(form: FormGroup) {

    this.EditorDocumento.markAllAsTouched();

    if (this.EditorDocumento.invalid) {
        this.toastr.error('Corriga os valores inválidos identificados no formulário.', '', { timeOut: 4000 });
        return;
    }

   this.gravaRascunho(form, "E");
    
}

loadApprovalData(documento: DocumentoCompra){

    if (!documento) {
        this.toastr.error('O Documento não existe.', '', { timeOut: 3000 });
        return;
    }

    this.documentoControls.Utilizador.setValue(documento.Utilizador);  
}

loadRejectData(documento: DocumentoCompra){

    if (!documento) {
        this.toastr.error('O Documento não existe.', '', { timeOut: 3000 });
        return;
    }

    this.documentoControls.Aprovador.setValue(documento.Aprovador);
    this.documentoControls.MotivoRejeicao.setValue(documento.MotivoRejeicao);

    if (documento.DataAprovacao)
    this.documentoControls.DataAprovacao.setValue(documento.DataAprovacao);
    else
    this.documentoControls.DataAprovacao.setValue(undefined);

    this.documentoControls.MotivoRejeicao.disable();

}

deleteRascunho() {

    if (!this.sketchId) {
        this.toastr.error('Id não definido.', '', { timeOut: 4000 });
        return;
    }

    const modalRef = this.modalService.open(ConfirmComponent, { windowClass: 'animated fadeInDown', centered: true });

    modalRef.componentInstance.modalTitle = "Eliminar documento?";
    modalRef.componentInstance.modalMessage = "Tem a certeza que deseja eliminar o documento?";

    modalRef.result
        .then((value: any) => {

          this.blockUI.start('A eliminar documento...');

            this.comprasService.deleteRascunho(this.sketchId).subscribe({
              next: (res: void) => {
                  this.blockUI.stop();
      
                  this.router.navigate([this.mainRoute]);

                  this.toastr.success('Documento eliminado!', '', { timeOut: 2500 });
                            
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


gravarDocumento(form: any) {

  let documento: DocumentoCompra = { 
      TipoEntidade: "F",
      TipoDoc: this.documentoControls.TipoDoc.value,
      Serie: this.documentoControls.Serie.value,
      NumDoc: this.documentoControls.NumDoc.value,
      Estado: this.documentoControls.Estado.value,
      Entidade: this.documentoControls.Fornecedor.value,
      NomeEntidade: this.documentoControls.NomeFornecedor.value,
      DescFinanceiro: this.documentoControls.DescFinanceiro.value,
      DescFornecedor: this.documentoControls.DescFornecedor.value,
      Linhas: this.lstLinhas,
      Anexos: this.lstAnexos,
  }

 if(this.obraId){

    documento.ObraId = this.obraId;
    documento.Obra = this.documentoControls.Obra.value
    documento.NomeObra = this.documentoControls.NomeObra.value
 }

  if(!documento.Estado)
  documento.Estado = 'G';

  if (form.Data)
  documento.DataDoc = new Date(form.Data.year, form.Data.month - 1, form.Data.day +1);

  if (form.DataVenc)
  documento.DataVenc = new Date(form.DataVenc.year, form.DataVenc.month - 1, form.DataVenc.day +1);


  this.blockUI.start('A gravar documento...');

  if (this.documentoId) {
      this.comprasService.alterarDocumento(this.documentoId, documento, this.dropZoneFiles).subscribe({
          next: res => {
              this.blockUI.stop();

              this.terminadoGravacao(this.EditorDocumento, false, true);
          },
          error: (error: any) => {
              this.blockUI.stop();

              if (this.dropZoneFiles.length > 0 && typeof error == "object" && error instanceof HttpErrorResponse && error.status === 0)
              this.toastr.error("Não foi possível ligar ao servidor ou não foi possivel enviar alguns dos anexos selecionados. Confirme se tem acesso aos ficheiros que selecionou.", "Erro a gravar documento", { timeOut: 9000 });
              else
              this.apiErrorService.handleError(error, "Erro a gravar documento");
          }
      });
  }
  else {
      this.comprasService.criarDocumento(documento, this.dropZoneFiles).subscribe({
          next: (documentoId: string) => {
              this.blockUI.stop();

              this.documentoId = documentoId;

              this.gravaRascunho(form, "A");

              this.terminadoGravacao(this.EditorDocumento, true, false);
          },
          error: (error: any) => {
              this.blockUI.stop();

              if (this.dropZoneFiles.length > 0 && typeof error == "object" && error instanceof HttpErrorResponse && error.status === 0)
              this.toastr.error("Não foi possível ligar ao servidor ou não foi possivel enviar alguns dos anexos selecionados. Confirme se tem acesso aos ficheiros que selecionou.", "Erro a criar documento", { timeOut: 9000 });
              else
              this.apiErrorService.handleError(error, "Erro a criar documento");
          }
      });
  }
}


ApproveDocument(form: FormGroup){

    this.EditorDocumento.markAllAsTouched();

    if (this.EditorDocumento.invalid) {
        this.toastr.error('Corriga os valores inválidos identificados no formulário.', '', { timeOut: 4000 });
        return;
    }

    try{

        this.blockUI.start('A aprovar documento...');

        this.gravarDocumento(form);

        this.blockUI.stop();
    }
    catch (error){

        this.blockUI.stop();

        this.apiErrorService.handleError(error, "Erro a aprovar documento");
    }
   
}

RejectDocument(form: FormGroup){

    this.EditorDocumento.markAllAsTouched();

    if (this.EditorDocumento.invalid) {
        this.toastr.error('Corriga os valores inválidos identificados no Formulário do Documento.', '', { timeOut: 4000 });
        return;
    }

    if (!this.documentoControls.MotivoRejeicao.value) {
        this.toastr.error('Motivo de rejeição obrigatório.', '', { timeOut: 4000 });
        return;
    }


    this.gravaRascunho(form, "R");
}


loadSketch() {

  if (!this.sketchId) {
      console.warn("Id não definido");
      return;
  }

  this.blockUI.start('A carregar documento...');

  this.comprasService.getRascunho(this.sketchId).subscribe({
      next: (documento: DocumentoCompra) => {

          this.blockUI.stop();

          if (!documento) {
              this.toastr.error('O Documento não existe.', '', { timeOut: 3000 });
              return;
          }

          this.documentoControls.Serie.setValue(documento.Serie);
          this.documentoControls.TipoDoc.setValue(documento.TipoDoc);
          this.documentoControls.NumDoc.setValue(documento.NumDoc);                
          this.documentoControls.Fornecedor.setValue(documento.Entidade);
          this.documentoControls.NomeFornecedor.setValue(documento.NomeEntidade);
          this.documentoControls.Obra.setValue(documento.Obra);
          this.documentoControls.NomeObra.setValue(documento.NomeObra);
          this.documentoControls.DescFinanceiro.setValue(documento.DescFinanceiro);
          this.documentoControls.DescFornecedor.setValue(documento.DescFornecedor);


          if (documento.DataDoc)
          this.documentoControls.Data.setValue({ year: documento.DataDoc.getFullYear(), month: documento.DataDoc.getMonth() + 1, day: documento.DataDoc.getDate()});
          else
          this.documentoControls.Data.setValue(undefined);

          if (documento.DataVenc)
          this.documentoControls.DataVenc.setValue({ year: documento.DataVenc.getFullYear(), month: documento.DataVenc.getMonth() + 1, day: documento.DataVenc.getDate()});
          else
          this.documentoControls.DataVenc.setValue(undefined);

          this.obraId = documento.ObraId;
          this.estado = documento.Estado;
          this.sketch = documento;
        
          this.lstLinhas = documento.Linhas;
          this.lstAnexos = documento.Anexos;

          if(this.approvalMode)
            this.loadApprovalData(documento);

          if(documento.Estado == 'R')
            this.loadRejectData(documento);

          if(documento.Estado == 'A'){
            this.formDesativado = true;
            this.EditorDocumento.disable();
            this.EditorLinha.disable();
        }

          // workaround a bug em calculo de tamanho de colunas/tabela de componente ngx-datatable
          setTimeout(() => {
              this.workaroundTabela1 = !this.workaroundTabela1;

              setTimeout(() => {
                  AppConstants.fireRefreshEventOnWindow();
              }, 1);
          }, 10);

      },
      error: (error: any) => {
          this.blockUI.stop();

          this.apiErrorService.handleError(error, "Erro a carregar documento");
      }
  });
}

gravaRascunho(form: any, estado: string){

let documento: DocumentoCompra = { 
  TipoDoc: this.documentoControls.TipoDoc.value,
  Serie: this.documentoControls.Serie.value,
  NumDoc: this.documentoControls.NumDoc.value,
  Estado: this.documentoControls.Estado.value,
  TipoEntidade: "F",
  Entidade: this.documentoControls.Fornecedor.value,
  NomeEntidade: this.documentoControls.NomeFornecedor.value,
  DescFinanceiro: this.documentoControls.DescFinanceiro.value,
  DescFornecedor: this.documentoControls.DescFornecedor.value,
  Linhas: this.lstLinhas,
  Anexos: this.lstAnexos,
}

if(this.obraId){
    documento.ObraId = this.obraId;
    documento.Obra = this.documentoControls.Obra.value
    documento.NomeObra = this.documentoControls.NomeObra.value
}

if(!documento.Estado)
documento.Estado = estado;

if (form.Data)
documento.DataDoc = new Date(form.Data.year, form.Data.month -1, form.Data.day + 1);

if (form.DataVenc)
documento.DataVenc = new Date(form.DataVenc.year, form.DataVenc.month -1, form.DataVenc.day + 1);

if(this.approvalMode){
    documento.Aprovador = this.sessionUser.Codigo,
    documento.MotivoRejeicao = this.documentoControls.MotivoRejeicao.value;
    documento.DataAprovacao = new Date(DateTime.now().year, DateTime.now().month -1, DateTime.now().day +1);
}

this.blockUI.start('A gravar documento...');

if (this.sketchId) {
  this.comprasService.alterarRascunho(this.sketchId, documento, this.dropZoneFiles).subscribe({
      next: res => {
          this.blockUI.stop();

          this.terminadoGravacao(this.EditorDocumento, false, true);
      },
      error: (error: any) => {
          this.blockUI.stop();

          if (this.dropZoneFiles.length > 0 && typeof error == "object" && error instanceof HttpErrorResponse && error.status === 0)
          this.toastr.error("Não foi possível ligar ao servidor ou não foi possivel enviar alguns dos anexos selecionados. Confirme se tem acesso aos ficheiros que selecionou.", "Erro a gravar documento", { timeOut: 9000 });
          else
          this.apiErrorService.handleError(error, "Erro a gravar documento");
      }
  });
}
else {
  this.comprasService.criarRascunho(documento, this.dropZoneFiles).subscribe({
      next: (sketchId: string) => {
          this.blockUI.stop();

          this.sketchId = sketchId;

          this.terminadoGravacao(this.EditorDocumento, true, false);
      },
      error: (error: any) => {
          this.blockUI.stop();

          if (this.dropZoneFiles.length > 0 && typeof error == "object" && error instanceof HttpErrorResponse && error.status === 0)
          this.toastr.error("Não foi possível ligar ao servidor ou não foi possivel enviar alguns dos anexos selecionados. Confirme se tem acesso aos ficheiros que selecionou.", "Erro a criar documento", { timeOut: 9000 });
          else
          this.apiErrorService.handleError(error, "Erro a criar documento");
      }
  });
}
}


terminadoGravacao(form: FormGroup, criado: boolean, alterado: boolean) {
  
  this.resetVars();

  if (criado)
  this.toastr.success('Documento criado!', '', { timeOut: 2500 });
  else if (alterado)
  this.toastr.success('Documento gravado!', '', { timeOut: 2500 });

  this.router.navigate([this.mainRoute]);
}

resetVars() {
  this.documentoId = undefined;
  this.sketchId = undefined;
  this.lstLinhas = [];
  this.lstAnexos = [];
  this.linhaEditar = undefined;
  this.dropZoneFiles = [];
}


  verificarAlteracaoTipoDoc() {
    this.documentoControls.Serie.setValue("");
   
    if (this.pedidoSeries) {
        this.pedidoSeries.finally(() => {
            this.selecionarSeriePorDefeito();
        });
    }
    else {
        this.selecionarSeriePorDefeito();
    }
  }

  selecionarSeriePorDefeito() {
    let seriesPorDefeito = this.series.filter(s => s.SeriePorDefeito == true);

    if (seriesPorDefeito.length > 0)
    this.documentoControls.Serie.setValue(seriesPorDefeito[0].Codigo);
  }

  resetForm(form: FormGroup) {
    if (form != null) {
        form.reset();
    }
  }

  loadArtigoInfo(codArtigo: string) {

    if (!codArtigo) {
        this.resetArtigo();
        return;
    }

    this.blockUI.start('A carregar artigo...');

    this.baseService.getArtigo(codArtigo).subscribe({
        next: (artigo: Artigo) => {
            this.blockUI.stop();

            if (!artigo)
            this.toastr.error(`Artigo '${codArtigo}' não existe.`, '', { timeOut: 3000 });

            this.setArtigoInfo(artigo);
        },
        error: (error: any) => {
            this.blockUI.stop();

            this.apiErrorService.handleError(error, "Erro a carregar artigo");
        }
    });
}


  loadObraInfo(codObra: string) {
    if (!codObra) {
        this.resetObra()
        return;
    }
    this.blockUI.start('A carregar obra...');

    this.baseService.getObra(codObra).subscribe({
        next: (obra: Obra) => {
            this.blockUI.stop();

            if (!obra)
            this.toastr.error(`Obra '${codObra}' não existe.`, '', { timeOut: 3000 });

            this.setObraInfo(obra);
        },
        error: (error: any) => {
            this.blockUI.stop();

            this.apiErrorService.handleError(error, "Erro a carregar obra");
        }
    });
}

  loadFornecedorInfo(codFornecedor: string) {
    if (!codFornecedor) {
        this.resetFornecedor()
        return;
    }

    this.blockUI.start('A carregar fornecedor...');

    this.baseService.getFornecedor(codFornecedor).subscribe({
        next: (fornecedor: Fornecedor) => {
            this.blockUI.stop();

            if (!fornecedor)
            this.toastr.error(`Fornecedor '${codFornecedor}' não existe.`, '', { timeOut: 3000 });

            this.setFornecedorInfo(fornecedor);
        },
        error: (error: any) => {
            this.blockUI.stop();

            this.apiErrorService.handleError(error, "Erro a carregar fornecedor");
        }
    });
}

  loadTiposDocumento() {
    this.tiposDocumento = [];

    this.blockUI.start('A carregar tipos de documento...');

    this.comprasService.getTiposDocumento().subscribe({
        next: (documentos: TipoDocumentosVendas[]) => {
            this.blockUI.stop();

            this.tiposDocumento = documentos;

            if (this.documentoControls.TipoDoc.value) {
                if (!this.tiposDocumento.find(t => t.Codigo === this.documentoControls.TipoDoc.value))
                this.documentoControls.TipoDoc.setValue("");

                if (this.documentoControls.TipoDoc.value)
                this.loadSeries(this.documentoControls.TipoDoc.value);
            }
        },
        error: (error: any) => {
            this.blockUI.stop();

            this.apiErrorService.handleError(error, "Erro a carregar tipos de documento");
        },
        complete: () => {
        }
    });
}

loadSeries(tipodoc: string) {
  this.series = [];

  if (!tipodoc) {
    console.warn("Tipo documento não definido");
    return;
  }

  this.blockUI.start('A carregar séries...');

  this.pedidoSeries = new Promise<void>((resolve, reject) => {
      this.comprasService.getSeries(tipodoc).subscribe({
          next: (series: SerieVendas[]) => {
              this.blockUI.stop();

              this.series = series;

              if (this.documentoControls.Serie.value) {
                  if (!this.series.find(t => t.Codigo === this.documentoControls.Serie.value))
                  this.documentoControls.Serie.setValue("");
              }

              resolve();
          },
          error: (error: any) => {
              this.blockUI.stop();

              this.apiErrorService.handleError(error, "Erro a carregar séries");

              reject();
          },
          complete: () => {
              this.pedidoSeries = null;
          }
      });
  });
}

getProximoNumeroSerie() {

  if (!this.documentoControls.Serie.value || !this.documentoControls.TipoDoc.value) {
      console.warn("serie e/ou tipodoc não definidos");
      this.documentoControls.NumDoc.setValue("");
      return;
  }

  this.blockUI.start('A carregar próximo número...');

  this.comprasService.getProximoNumeroSerie(this.documentoControls.TipoDoc.value, this.documentoControls.Serie.value).subscribe({
      next: (proximoNumero: number) => {
          this.blockUI.stop();

          this.documentoControls.NumDoc.setValue(proximoNumero);
      },
      error: (error: any) => {
          this.blockUI.stop();

          this.apiErrorService.handleError(error, "Erro a carregar próximo número");
      }
  });
}

LstObrasModal() {

  const modalRef = this.modalService.open(SelecionarObraComponent, { windowClass: 'animated fadeInDown', size: 'lg' });

  modalRef.componentInstance.loadObras();

  modalRef
  .result
  .then((value: Obra) => {
      this.setObraInfo(value);
  })
  .catch((reason) => {

  });
}

  LstArtigosModal() {

    const modalRef = this.modalService.open(SelecionarArtigoComponent, { windowClass: 'animated fadeInDown', size: 'lg' });

    modalRef.componentInstance.loadArtigos();

    modalRef
    .result
    .then((value: Artigo) => {
        this.setArtigoInfo(value);
    })
    .catch((reason) => {

    });
  }

  LstClientesModal() {
    
    const modalRef = this.modalService.open(SelecionarClienteComponent, { windowClass: 'animated fadeInDown', size: 'lg' });

    modalRef.componentInstance.loadClientes();

    modalRef
    .result
    .then((value: Cliente) => {
        this.setClienteInfo(value);
    })
    .catch((reason) => {

    });
  }

  LstUnidades() {

    const modalRef = this.modalService.open(SelecionarUnidadeComponent, { windowClass: 'animated fadeInDown', size: 'lg' });

    modalRef.componentInstance.loadUnidades();

    modalRef
    .result
    .then((value: Unidade) => {
        this.setUnidade(value)
    })
    .catch((reason) => {

    });
  }

  LstFornecedores() {

    const modalRef = this.modalService.open(SelecionarFornecedorComponent, { windowClass: 'animated fadeInDown', size: 'lg' });

    modalRef.componentInstance.loadFornecedores();

    modalRef
    .result
    .then((value: Fornecedor) => {
        this.setFornecedorInfo(value);
    })
    .catch((reason) => {

    });
  }

  MostrarLinhaModal(){

      this.linhaControls.Quantidade.setValue(1);
      this.linhaControls.Preco.setValue(0);

      this.modalService.open(this.linhaModalContent, { windowClass: 'animated fadeInDown', size: 'lg' })
      .result
      .then((value: any) => {

      })
      .catch((reason: any) => {

      })
      .finally(() => {
          this.resetForm(this.EditorLinha);
          this.linhaEditar = null;
      });
    }

  
    setClienteInfo(cliente: Cliente) {

      if (cliente != null) {
          this.EditorDocumento.patchValue({
              Entidade: cliente.Codigo,
              NomeEntidade: cliente.Nome,
          });

      }
      else {
          this.EditorDocumento.patchValue({
              NomeEntidade: null,
          });
      }
  }


  setObraInfo(obra: Obra) {
    
    if (obra) {
         this.obraId = obra.Id;
                    
        this.EditorDocumento.patchValue({
            Obra: obra.Codigo,
            NomeObra: obra.Descricao,
        });
    }
    else {
        this.resetObra();
    }
  }

  setFornecedorInfo(fornecedor: Fornecedor) {
    if (fornecedor) {
        this.EditorDocumento.patchValue({
            Fornecedor: fornecedor.Codigo,
            NomeFornecedor: fornecedor.Nome,
        });
    }
    else {
        this.resetFornecedor();
    }
  }

  resetArtigo() {
    this.EditorLinha.patchValue({
        Artigo: null,
        Descricao: null,
        Unidade: null,
        Preco: 0,
    });
  }

  resetObra() {
    this.obraId = null;
    this.EditorDocumento.patchValue({
        NomeObra: null,
    });
  }

  resetFornecedor() {
    this.EditorDocumento.patchValue({
      NomeFornecedor: null,
    });
  }
  
  setUnidade(unidade: Unidade) {
    if (unidade) {
        this.EditorLinha.patchValue({ Unidade: unidade.Codigo });
    }
    else {
        this.EditorLinha.patchValue({ Unidade: null });
    }
  }

  resetUnidade() {
    this.EditorLinha.patchValue({
        Unidade: null,
    });
  }

  setArtigoInfo(artigo: Artigo) {

    if (artigo) {
        this.EditorLinha.patchValue({ Artigo: artigo.Codigo });
        this.EditorLinha.patchValue({ Descricao: artigo.Descricao });
        this.EditorLinha.patchValue({ Unidade: artigo.UnidadeVenda });
        this.EditorLinha.patchValue({ Preco: artigo.PVP1 });
    }
  }

  expandirRecolherLinha(linha: LinhaDocumentoVenda) {
      this.tabelaLinhas.rowDetail.toggleExpandRow(linha);
  }

  deleteLinha(linha: LinhaDocumentoVenda) {
    const index = this.lstLinhas.indexOf(linha);
    if (index >= 0) {
        this.lstLinhas.splice(index, 1);
        this.lstLinhas = [...this.lstLinhas];
    }
  }

  editLinha(linha: LinhaDocumentoVenda) {
      this.MostrarLinhaModal();

      this.linhaEditar = linha;

      this.linhaControls.Artigo.setValue(linha.Artigo);
      this.linhaControls.Descricao.setValue(linha.Descricao);
      this.linhaControls.Quantidade.setValue(linha.Quantidade);
      this.linhaControls.Preco.setValue(linha.Preco);
      this.linhaControls.Unidade.setValue(linha.Unidade);
      this.linhaControls.Observacoes.setValue(linha.Observacoes);

      if(linha.DataEntrega)
        this.linhaControls.DataEntrega.setValue({ year: linha.DataEntrega.getFullYear(),  month: linha.DataEntrega.getMonth() + 1, day: linha.DataEntrega.getDate() });
      
  }

  gravarLinha(data): void {
      this.EditorLinha.markAllAsTouched();

      if (this.EditorLinha.invalid) {
          this.toastr.error('Corrija os valores inválidos identificados no formulário.', '', { timeOut: 4000 });
          return;
      }

      this.adicionaLinha(data);
  }

  fornecedoresModal() {

    const modalRef = this.modalService.open(SelecionarFornecedorComponent, { windowClass: 'animated fadeInDown', size: 'lg' });

    modalRef.componentInstance.loadFornecedores(this.documentoControls.Empresa.value);

    modalRef
    .result
    .then((value: Fornecedor) => {
        this.setFornecedorInfo(value);
    })
    .catch((reason) => {

    });
}

  adicionaLinha(data){

    if (this.linhaEditar) {
        this.linhaEditar.Artigo = data.Artigo;
        this.linhaEditar.Descricao = data.Descricao;
        this.linhaEditar.Quantidade = DecimalPrecision.round(data.Quantidade, 4);
        this.linhaEditar.Preco = DecimalPrecision.round(data.Preco, 4);
        this.linhaEditar.Unidade = data.Unidade;
        this.linhaEditar.Observacoes = data.Observacoes;
        this.linhaEditar.DataEntrega = new Date( data.DataEntrega.year, data.DataEntrega.month -1, data.DataEntrega.day +1);
    }
    else {
        this.lstLinhas.push({
            Id: "",
            NumLinha: 0,
            Artigo: data.Artigo,
            Descricao: data.Descricao,
            DataEntrega: new Date( data.DataEntrega.year, data.DataEntrega.month -1, data.DataEntrega.day +1),
            Quantidade: DecimalPrecision.round(data.Quantidade, 4),
            Preco: DecimalPrecision.round(data.Preco, 4),
            Unidade: data.Unidade,
            Observacoes: data.Observacoes,
        });

        this.lstLinhas = [...this.lstLinhas];
    }

    this.modalService.dismissAll();

    // workaround a bug em calculo de tamanho de colunas/tabela de componente ngx-datatable
    setTimeout(() => {
        this.workaroundTabela2 = !this.workaroundTabela2;

        setTimeout(() => {
            AppConstants.fireRefreshEventOnWindow();
        }, 1);
    }, 10);
  }


}
