import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Unidade } from 'src/app/shared/interfaces/unidade';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BaseService } from 'src/app/shared/services/base.service';

@Component({
    selector: 'app-selecionar-unidade',
    templateUrl: './selecionar-unidade.component.html',
    styleUrls: ['./selecionar-unidade.component.css']
})
export class SelecionarUnidadeComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    public pageSize = 10;
    public page = 1;
    public pesquisa = "";
    public codEmpresa = "";
    public unidades: Unidade[];

    constructor(
        private baseService: BaseService,
        private apiErrorService: ApiErrorResponseHandlerService,
        public modal: NgbActiveModal
    ) { }


    ngOnInit(): void {}

    loadUnidades() {

        this.blockUI.start('A carregar...');

        this.baseService.getUnidades().subscribe({
            next: (dados: Unidade[]) => {
                this.blockUI.stop();

                this.unidades = dados;
            },
            error: (error: any) => {
                this.blockUI.stop();

                this.apiErrorService.handleError(error, "Erro a carregar unidades");
            },
        });
    }

    unidadesFiltradas(): Unidade[] {
        if (this.pesquisa)
            return this.unidades?.filter(d => d.Descricao?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true || d.Codigo?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true);
        else
            return this.unidades;
    }

    pesquisar(): void {

    }
}
