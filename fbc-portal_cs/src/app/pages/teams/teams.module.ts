import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from "@siemens/ngx-datatable";
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';

import { TeamsRoutingModule } from './teams-routing.module';
import { TeamsComponent } from './teams.component';

import { TeamUserModalModule } from 'src/app/shared/modals/team-user/team-user.module';
import { TeamEditorModalModule } from 'src/app/shared/modals/team-editor/team-editor.module';


@NgModule({  
  imports: [
     CommonModule, 
     TeamsRoutingModule,
     NgxDatatableModule,
     FormsModule,
     NgbModule,
     PerfectScrollbarModule,
     TeamUserModalModule,
     TeamEditorModalModule
    ],
  declarations: [ TeamsComponent]
})

export class TeamsModule { }
