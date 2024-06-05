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
import { ConfirmComponent } from '../confirm/confirm.component';
import { de } from 'date-fns/locale';
import { Membro } from '../../interfaces/membro';
import { Leader } from '../../interfaces/leader';

@Component({
    selector: 'app-team-editor',
    templateUrl: './team-editor.component.html',
    styleUrls: ['./team-editor.component.scss']
})
export class TeamEditorModalComponent implements OnInit {

    @BlockUI() public blockUI: NgBlockUI;
    @BlockUI("TeamEditorModal") blockUITeamEditorModal: NgBlockUI;

    public emEdicao: boolean;
    public TeamEditorForm: FormGroup;
    public Team: Equipa;
    public Teams: Equipa[];
    public Users: Utilizador[];

    get teamEditorControls() { return this.TeamEditorForm.controls; }

    constructor(
        private formBuilder: FormBuilder,
        private baseService: BaseService,
        private acessosService: AcessosService,
        private modalService: NgbModal,
        private sessionService: SessionService,
        private toastr: ToastrService,
        public modal: NgbActiveModal,
        private apiErrorService: ApiErrorResponseHandlerService,
        private menuSettingsService: MenuSettingsService
    ) {}

    ngOnInit(): void {

        this.TeamEditorForm = this.formBuilder.group({
            Codigo: ['', Validators.required],   
            Descricao: ['', Validators.required],
            Email: ['', Validators.required],
            Activa: [undefined],
            Membros:[undefined],
            Leaders:[undefined],
        });

        this.loadUsers();
        this.loadTeams();

        if(this.Team)
            this.loadTeam()
    }


    loadTeam(){

        if (!this.Team) {
            this.toastr.error('A Equipa não existe.', '', { timeOut: 3000 });
            return;
        }

        this.emEdicao = true;

        this.teamEditorControls.Codigo.setValue(this.Team.Codigo);
        this.teamEditorControls.Descricao.setValue(this.Team.Descricao);
        this.teamEditorControls.Activa.setValue(this.Team.Activa); 
        this.teamEditorControls.Email.setValue(this.Team.Email);           

        let lstLeaders =  this.Team.Leaders.map(x => x.Utilizador)
        let lstMembers = this.Team.Membros.map(x => x.Utilizador)

        this.teamEditorControls.Leaders.setValue(lstLeaders); 
        this.teamEditorControls.Membros.setValue(lstMembers); 
    }

    deleteEquipa() {

        if (!this.Team) {
            this.toastr.error('Equipa não definida.', '', { timeOut: 4000 });
            return;
        }
    
        const modalRef = this.modalService.open(ConfirmComponent, { windowClass: 'animated fadeInDown', centered: true });
    
        modalRef.componentInstance.modalTitle = "Eliminar Equipa?";
        modalRef.componentInstance.modalMessage = "Tem a certeza que deseja eliminar a equipa?";
    
        modalRef.result
            .then((value: any) => {
    
              this.blockUI.start('A eliminar documento...');
    
                this.baseService.deleteEquipa(this.Team.Codigo).subscribe({
                  next: (res: void) => {

                    this.blockUI.stop();

                    this.resetForm();
        
                    this.modal.close('gravar');

                      this.toastr.success('Equipa eliminada!', '', { timeOut: 2500 });                                
                  },
                  error: (error: any) => {
                      this.blockUI.stop();
          
                      this.apiErrorService.handleError(error, "Erro a eliminar equipa");
                  }
              });
            })
            .catch((reason: any) => {
    
            });
      }

      mapMembro(codUtilizador: string, codEquipa: string) {
    
        let obj: Membro = {
            CodEquipa: codEquipa,
            Utilizador: codUtilizador
        };

        return obj;
      }

      mapLeader(codUtilizador: string, codEquipa: string) {
    
        let obj: Leader = {
            CodEquipa: codEquipa,
            Utilizador: codUtilizador
        };

        return obj;
      }

      
    createTeam(){

        let equipa: Equipa = {
            Codigo: this.teamEditorControls.Codigo.value,
            Descricao: this.teamEditorControls.Descricao.value,
            Email: this.teamEditorControls.Email.value,
            Activa: this.teamEditorControls.Activa.value,
        };

        equipa.Leaders = [];   
        equipa.Membros = [];

        if(this.teamEditorControls.Membros.value) {
        this.teamEditorControls.Membros.value.forEach( codUtilizador => {
            
            let member = this.mapMembro(codUtilizador, equipa.Codigo);

            equipa.Membros.push(member);      
        });}

        if(this.teamEditorControls.Leaders.value){
        this.teamEditorControls.Leaders.value.forEach( codUtilizador => {

            let leader = this.mapLeader(codUtilizador, equipa.Codigo);

            equipa.Leaders.push(leader);      
        });}

        this.blockUI.start('A gravar equipa...');

        this.baseService.criarEquipa(equipa).subscribe({
            next: (any: void) => {

                this.blockUI.stop();

                this.resetForm();
    
                this.modal.close('gravar');
    
                this.toastr.success('Equipa criada!', '', { timeOut: 2500 });
      
            },
            error: (error: any) => {

                this.blockUI.stop();
    
                this.apiErrorService.handleError(error, "Erro a criar equipa");
            }
        });
    }

    editTeam(){

        let equipa: Equipa = {
            Codigo: this.teamEditorControls.Codigo.value,
            Descricao: this.teamEditorControls.Descricao.value,
            Email: this.teamEditorControls.Email.value,
            Activa: this.teamEditorControls.Activa.value,
        };

        equipa.Leaders = [];   
        equipa.Membros = [];

        this.teamEditorControls.Membros.value.forEach( codUtilizador => {
            
            let member = this.mapMembro(codUtilizador, equipa.Codigo);

            equipa.Membros.push(member);      
        });

        this.teamEditorControls.Leaders.value.forEach( codUtilizador => {

            let leader = this.mapLeader(codUtilizador, equipa.Codigo);

            equipa.Leaders.push(leader);      
        });

        this.blockUI.start('A gravar equipa...');

        this.baseService.alterarEquipa(equipa).subscribe({
            next: (any: void) => {

                this.blockUI.stop();

                this.resetForm();
    
                this.modal.close('gravar');
    
                this.toastr.success('Equipa alterada!', '', { timeOut: 2500 });
      
            },
            error: (error: any) => {

                this.blockUI.stop();
    
                this.apiErrorService.handleError(error, "Erro a alterar equipa");
            }
        });
    }


    onSubmit(form: FormGroup) {

        this.TeamEditorForm.markAllAsTouched();
    
        if (this.TeamEditorForm.invalid) {
            this.toastr.error('Corriga os valores inválidos identificados no formulário.', '', { timeOut: 4000 });
            return;
        }
    
        if(this.emEdicao)
            this.editTeam();
        else
            this.createTeam();   
            
    }

    loadUsers()
    {
        this.blockUITeamEditorModal.start('A carregar...');
    
        this.acessosService.getUtilizadores().subscribe({
            next: (utilizadores: Utilizador[]) => {
                this.blockUITeamEditorModal.stop();
    
                this.Users = utilizadores;
            },
            error: (error: any) => {
                this.blockUITeamEditorModal.stop();
    
                this.apiErrorService.handleError(error, "Erro a carregar Utilizadores");
            },
        });
    }

    loadTeams(){

        this.blockUITeamEditorModal.start('A carregar...');
    
        this.baseService.getEquipas().subscribe({
            next: (equipas: Equipa[]) => {
                this.blockUITeamEditorModal.stop();
    
                this.Teams = equipas;
            },
            error: (error: any) => {
                this.blockUITeamEditorModal.stop();
    
                this.apiErrorService.handleError(error, "Erro a carregar equipas");
            },
        });
      }
      
      resetForm() {
        this.TeamEditorForm.reset();
      }

}
