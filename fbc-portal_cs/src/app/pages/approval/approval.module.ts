import { NgModule } from '@angular/core';
import { CommonModule,DatePipe } from '@angular/common';

import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BlockUIModule } from "ng-block-ui";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { NgxDatatableModule } from "@siemens/ngx-datatable";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { BlockTemplateComponent } from 'src/app/shared/components/blockui/block-template.component';

import { ApprovalComponent } from './approval.component';
import { ApprovalRoutingModule } from './approval-routing.module';


@NgModule({
  imports: [
    CommonModule,
    ApprovalRoutingModule,
    PerfectScrollbarModule,
    NgxDatatableModule,
    NgbModule,
    ReactiveFormsModule,
    FormsModule,
    BlockUIModule.forRoot({ template: BlockTemplateComponent })
  ],
  declarations: [ApprovalComponent],
  providers: [DatePipe],
  exports: []
})
export class ApprovalModule { }
