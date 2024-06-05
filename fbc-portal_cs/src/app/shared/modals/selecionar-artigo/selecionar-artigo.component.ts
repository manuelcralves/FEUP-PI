import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Artigo } from 'src/app/shared/interfaces/artigo';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BaseService } from 'src/app/shared/services/base.service';

@Component({
    selector: 'app-selecionar-artigo',
    templateUrl: './selecionar-artigo.component.html',
    styleUrls: ['./selecionar-artigo.component.scss']
})
export class SelecionarArtigoComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    public pageSize = 10;
    public page = 1;
    public artigo = "";
    public descricao = "";

    public codigoArtigo = "Codigo";

    public codEmpresa = "";
    public artigos: Artigo[];

    constructor(
        private baseService: BaseService,
        private apiErrorService: ApiErrorResponseHandlerService,
        public modal: NgbActiveModal
    ) { }

    ngOnInit(): void {

    }

    loadArtigos() {
        this.blockUI.start('A carregar...');

        this.baseService.getArtigos().subscribe({
            next: (artigos: Artigo[]) => {

                this.blockUI.stop();
                this.artigos = artigos;  

            },
            error: (error: any) => {

                this.blockUI.stop();
                this.apiErrorService.handleError(error, "Erro a carregar artigos");
            },
        });
    }


    artigosFiltrados(): Artigo[] {
        let listaFiltrada = this.artigos;

        if (this.artigo)
            listaFiltrada = listaFiltrada?.filter(
                d => d.Codigo?.toUpperCase().startsWith(this.artigo.toUpperCase()) === true);

        if(this.descricao)
            listaFiltrada = listaFiltrada?.filter(
                d => d.Descricao?.toUpperCase().includes(this.descricao.toUpperCase()) === true);

        return listaFiltrada;
    }
}
