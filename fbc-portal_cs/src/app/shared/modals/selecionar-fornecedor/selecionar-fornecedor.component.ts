import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Fornecedor } from 'src/app/shared/interfaces/fornecedor';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BaseService } from 'src/app/shared/services/base.service';

@Component({
    selector: 'app-selecionar-fornecedor',
    templateUrl: './selecionar-fornecedor.component.html',
    styleUrls: ['./selecionar-fornecedor.component.scss']
})
export class SelecionarFornecedorComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    public pageSize = 10;
    public page = 1;
    public pesquisa = "";
    public codEmpresa = "";
    public fornecedores: Fornecedor[];

    constructor(
        private baseService: BaseService,
        private apiErrorService: ApiErrorResponseHandlerService,
        public modal: NgbActiveModal
    ) { }

    ngOnInit(): void {

    }

    loadFornecedores() {

        this.blockUI.start('A carregar...');

        this.baseService.getFornecedores().subscribe({
            next: (fornecedores: Fornecedor[]) => {
                this.blockUI.stop();

                this.fornecedores = fornecedores;
            },
            error: (error: any) => {
                this.blockUI.stop();

                this.apiErrorService.handleError(error, "Erro a carregar fornecedores");
            },
        });
    }

    fornecedoresFiltrados(): Fornecedor[] {
        if (this.pesquisa)
            return this.fornecedores?.filter(d => d.Nome?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true || d.Codigo?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true);
        else
            return this.fornecedores;
    }
}
