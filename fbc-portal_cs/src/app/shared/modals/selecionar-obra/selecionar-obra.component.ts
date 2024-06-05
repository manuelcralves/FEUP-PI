import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BaseService } from 'src/app/shared/services/base.service';
import { Obra } from '../../interfaces/obra';

@Component({
    selector: 'app-selecionar-obra',
    templateUrl: './selecionar-obra.component.html',
    styleUrls: ['./selecionar-obra.component.scss']
})
export class SelecionarObraComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    public pageSize = 10;
    public page = 1;
    public pesquisa = "";
    public obras: Obra[];

    constructor(
        private baseService: BaseService,
        private apiErrorService: ApiErrorResponseHandlerService,
        public modal: NgbActiveModal
    ) { }


    ngOnInit(): void {}

    loadObras() {

        this.blockUI.start('A carregar...');

        this.baseService.getObras().subscribe({
            next: (dados: Obra[]) => {
                this.blockUI.stop();

                this.obras = dados;
            },
            error: (error: any) => {
                this.blockUI.stop();

                this.apiErrorService.handleError(error, "Erro a carregar obras");
            },
        });
    }

    obrasFiltradas(): Obra[] {
        if (this.pesquisa)
            return this.obras?.filter(d => d.Descricao?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true || d.Codigo?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true);
        else
            return this.obras;
    }

    pesquisar(): void {

    }
}
