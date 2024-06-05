import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DataTableBodyCellComponent } from '@siemens/ngx-datatable';
import { addISOWeekYears } from 'date-fns';
import { de } from 'date-fns/locale';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ToastrService } from 'ngx-toastr';
import { Equipa } from 'src/app/shared/interfaces/equipa';
import { Leader } from 'src/app/shared/interfaces/leader';
import { Membro } from 'src/app/shared/interfaces/membro';
import { Utilizador } from 'src/app/shared/interfaces/utilizador';
import { ConfirmComponent } from 'src/app/shared/modals/confirm/confirm.component';
import { TeamEditorModalComponent } from 'src/app/shared/modals/team-editor/team-editor.component';
import { TeamUserModalComponent } from 'src/app/shared/modals/team-user/team-user.component';
import { AcessosService } from 'src/app/shared/services/acessos.service';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BaseService } from 'src/app/shared/services/base.service';
import { SessionService } from 'src/app/shared/services/session.service';

@Component({
  selector: 'app-teams',
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.scss']
})
export class TeamsComponent implements OnInit {

  @BlockUI() blockUI: NgBlockUI;
  @BlockUI("TeamsTable") blockUITeamsTable: NgBlockUI;

  public utilizador: Utilizador;
  public currentUser: Utilizador;
  public Admin: boolean;
  public Teams: Equipa[];
  public selectedTeam: Equipa;
  public selectedMembros: Membro[];
  public selectedLeaders: Leader[];

  
  constructor(
    private baseService: BaseService,
    private modalService: NgbModal,
    private sessionService: SessionService,
    private acessosService: AcessosService,
    private apiErrorService: ApiErrorResponseHandlerService,
    private toastr: ToastrService,
    ) { }

  ngOnInit(): void {  

    this.UserLoader();
    this.TeamsLoader();
  }

  UserLoader(){

    this.currentUser = this.sessionService.getUtilizadorFromToken();

    if(this.currentUser.Administrador == "S")
      this.Admin = true;
  }

  resetSelectedTeam(){
    this.Teams = null;
    this.selectedTeam = null;
    this.selectedMembros = null;
    this.selectedLeaders = null;
  }

  TeamsLoader(){

    this.resetSelectedTeam();

    if(this.Admin)
      this.loadAllTeams();
    else
      this.loadTeamsUser();
  }

  loadAllTeams(){

    this.blockUITeamsTable.start('A carregar...');

    this.baseService.getEquipas().subscribe({
        next: (equipas: Equipa[]) => {
            this.blockUITeamsTable.stop();

            if(equipas.length < 1)
              return;

            this.Teams = equipas;

            this.loadTeam(equipas[0].Codigo);

        },
        error: (error: any) => {
            this.blockUITeamsTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar equipas");
        },
    });
  }

  loadTeamsUser(){

    this.blockUITeamsTable.start('A carregar...');

    this.baseService.getEquipasUtilizador().subscribe({
        next: (equipas: Equipa[]) => {
            this.blockUITeamsTable.stop();

            if (equipas.length < 1) {
              this.toastr.error('O utilizador não tem equipas.', '', { timeOut: 3000 });
              return;
            }
              
              this.Teams = equipas;

              this.loadTeam(equipas[0].Codigo);

        },
        error: (error: any) => {
            this.blockUITeamsTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar equipas utilizador");
        },
    });
  }

  selectTeam(CodEquipa: string ){  


    this.loadTeam(CodEquipa);
  }

  loadTeam(CodEquipa: string ){

    if (!CodEquipa) {
      this.toastr.error('Identificação da equipa não definida.', '', { timeOut: 4000 });
      return;
    }

    this.blockUITeamsTable.start('A carregar...');

    this.baseService.getEquipa(CodEquipa).subscribe({
        next: (equipa: Equipa) => {
            this.blockUITeamsTable.stop();

            this.selectedTeam = equipa;

            this.selectedMembros = equipa.Membros;
            this.selectedLeaders = equipa.Leaders;
            
        },
        error: (error: any) => {
            this.blockUITeamsTable.stop();

            this.apiErrorService.handleError(error, "Erro a carregar equipas");
        },
    });
  }
 

  editTeam() {

    this.editTeamModal(this.selectedTeam);
  }

  newTeam(){
   
    this.editTeamModal();
  }

  editMember(){
    const modalRef = this.modalService.open(TeamUserModalComponent, { windowClass: 'animated fadeInDown', size: 'md' });

    modalRef.componentInstance.contextCompany = this.selectedTeam.Codigo;

    modalRef
    .result
    .then((value: any) => {
         
      this.TeamsLoader();
    })
    .catch((reason) => {

    });
  }

  deleteMember(codUtilizador: string){

    if (!codUtilizador) {
      this.toastr.error('Membro não definido.', '', { timeOut: 4000 });
      return;
  }

  const modalRef = this.modalService.open(ConfirmComponent, { windowClass: 'animated fadeInDown', centered: true });

  modalRef.componentInstance.modalTitle = "Eliminar Membro?";
  modalRef.componentInstance.modalMessage = "Tem a certeza que deseja eliminar o membro?";

  modalRef.result
      .then((value: any) => {

        this.blockUI.start('A eliminar membro...');

        debugger

          this.baseService.deleteMembroEquipa(codUtilizador, this.selectedTeam.Codigo).subscribe({
            next: (res: void) => {

              this.blockUI.stop();

                this.toastr.success('Membro eliminado!', '', { timeOut: 2500 });           
                
                this.TeamsLoader();
            },
            error: (error: any) => {
                this.blockUI.stop();
    
                this.apiErrorService.handleError(error, "Erro a eliminar membro");
            }
        });
      })
      .catch((reason: any) => {

      });
  }

  addTeamUserModal() {
    
    const modalRef = this.modalService.open(TeamUserModalComponent, { windowClass: 'animated fadeInDown', size: 'md' });

    modalRef.componentInstance.contextTeam = this.selectedTeam.Codigo;

    modalRef
    .result
    .then((value: any) => {
         
      this.TeamsLoader();
    })
    .catch((reason) => {

    });
  }

  editTeamModal(team?: Equipa) {
    
    const modalRef = this.modalService.open(TeamEditorModalComponent, { windowClass: 'animated fadeInDown', size: 'md' });

    if(team)
      modalRef.componentInstance.Team = team;

    modalRef
    .result
    .then((value: any) => {

      this.TeamsLoader();
    })
    .catch((reason) => {

    });
  }

}
