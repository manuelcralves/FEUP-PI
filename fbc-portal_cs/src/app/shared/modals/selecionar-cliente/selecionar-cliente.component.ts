import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Cliente } from 'src/app/shared/interfaces/cliente';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BaseService } from 'src/app/shared/services/base.service';

@Component({
    selector: 'app-selecionar-cliente',
    templateUrl: './selecionar-cliente.component.html',
    styleUrls: ['./selecionar-cliente.component.scss']
})
export class SelecionarClienteComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    public pageSize = 10;
    public page = 1;
    public pesquisa = "";
    public codEmpresa = "";
    public clientes: Cliente[];

    constructor(
        private baseService: BaseService,
        private apiErrorService: ApiErrorResponseHandlerService,
        public modal: NgbActiveModal
    ) { }

    ngOnInit(): void {
    }

    
    loadClientes() {

        this.blockUI.start('A carregar...');

        this.baseService.getClientes().subscribe({
            next: (clientes: Cliente[]) => {
                this.blockUI.stop();

                this.clientes = clientes;
            },
            error: (error: any) => {
                this.blockUI.stop();

                this.apiErrorService.handleError(error, "Erro a carregar clientes");
            },
        });
    }

    clientesFiltrados(): Cliente[] {
        if (this.pesquisa)
            return this.clientes?.filter(d => d.Nome?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true || d.Codigo?.toUpperCase().includes(this.pesquisa.toUpperCase()) === true);
        else
            return this.clientes;
    }
}
