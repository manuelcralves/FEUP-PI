import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { MenuSettingsService } from 'src/app/shared/modules/settings/menu/menu-settings.service';
import { SessionService } from 'src/app/shared/services/session.service';
import { ApiErrorResponseHandlerService } from '../../services/api-error-response-handler.service';
import { BaseService } from '../../services/base.service';
import { Equipa } from '../../interfaces/equipa';
import { Utilizador } from '../../interfaces/utilizador';
import { AcessosService } from '../../services/acessos.service';
import { ToastrService } from 'ngx-toastr';
import { Membro } from '../../interfaces/membro';

@Component({
    selector: 'app-team-user',
    templateUrl: './team-user.component.html',
    styleUrls: ['./team-user.component.scss']
})
export class TeamUserModalComponent implements OnInit {

    @BlockUI() public blockUI: NgBlockUI;
    @BlockUI("TeamUserModal") blockUITeamUserModal: NgBlockUI;

    public teamUserForm: FormGroup;
    public Teams: Equipa[];
    public Users: Utilizador[];
    public contextTeam: string;

    get teamUserControls() { return this.teamUserForm.controls; }

    constructor(

        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private baseService: BaseService,
        private acessosService: AcessosService,
        private sessionService: SessionService,
        public modal: NgbActiveModal,
        private apiErrorService: ApiErrorResponseHandlerService,
        private menuSettingsService: MenuSettingsService
    ) {}

    ngOnInit(): void {
        this.teamUserForm = this.formBuilder.group({
            Utilizador: ['', Validators.required],      
            Nome: ['', Validators.required],
            Email: ['', Validators.required],
            Equipa: [undefined]
        });

        this.loadUsers();
        this.loadTeams();

        if(this.contextTeam)
            this.teamUserControls.Equipa.setValue(this.contextTeam);

    }

    onSubmit(form: FormGroup) {

        this.teamUserForm.markAllAsTouched();
    
        if (this.teamUserControls.invalid) {
            this.toastr.error('Corriga os valores inválidos identificados no formulário.', '', { timeOut: 4000 });
            return;
        }     
    
        this.addTeamMember();            
    }

    addTeamMember(){

        let membro: Membro = {
            CodEquipa: this.teamUserControls.Equipa.value,
            Utilizador: this.teamUserControls.Utilizador.value,
            Nome: this.teamUserControls.Nome.value,
            Email: this.teamUserControls.Email.value,
        }
        
        this.blockUI.start('A adicionar membro...');

        this.baseService.addMembroEquipa(membro).subscribe({
            next: (any: void) => {

                this.blockUI.stop();

                this.resetForm();
    
                this.modal.close('gravar');
    
                this.toastr.success('Membro adicionado!', '', { timeOut: 2500 });
      
            },
            error: (error: any) => {

                this.blockUI.stop();
    
                this.apiErrorService.handleError(error, "Erro a adicionar membro");
            }
        });
    }

    loadUsers()
    {
        this.blockUITeamUserModal.start('A carregar...');
    
        this.acessosService.getUtilizadores().subscribe({
            next: (utilizadores: Utilizador[]) => {
                this.blockUITeamUserModal.stop();
    
                this.Users = utilizadores;
            },
            error: (error: any) => {
                this.blockUITeamUserModal.stop();
    
                this.apiErrorService.handleError(error, "Erro a carregar Utilizadores");
            },
        });
    }

    loadTeams(){

        this.blockUITeamUserModal.start('A carregar...');
    
        this.baseService.getEquipas().subscribe({
            next: (equipas: Equipa[]) => {
                this.blockUITeamUserModal.stop();
                
                this.Teams = equipas;

                if(this.contextTeam)
                    this.Teams = equipas.filter(t => t.Codigo == this.contextTeam);
    
            },
            error: (error: any) => {
                this.blockUITeamUserModal.stop();
    
                this.apiErrorService.handleError(error, "Erro a carregar equipas");
            },
        });
      }

      
  loadUserInfo(codUtilizador: string) {

        if (!codUtilizador) {
            this.resetUtilizador()
            return;
        }

        this.blockUI.start('A carregar utilizador...');

        this.acessosService.getUtilizador(codUtilizador).subscribe({
            next: (utilizador: Utilizador) => {
                this.blockUI.stop();

                if (!utilizador)
                this.toastr.error(`Utilizador '${codUtilizador}' não existe.`, '', { timeOut: 3000 });

                this.setUtilizadorInfo(utilizador);
            },
            error: (error: any) => {
                this.blockUI.stop();

                this.apiErrorService.handleError(error, "Erro a carregar utilizador");
            }
        });
    }

    setUtilizadorInfo(utilizador: Utilizador) {

        if (utilizador) {
            this.teamUserForm.patchValue({
                Codigo: utilizador.Codigo,
                Nome: utilizador.Nome,
                Email: utilizador.Email,
            });
        }
        else {
            this.resetUtilizador();
        }
      }

      resetUtilizador() {
        this.teamUserForm.patchValue({
            Codigo: null,
            Nome: null,
            Email: null,
        });
      }

      resetForm() {
        this.teamUserForm.reset();
      }


}
