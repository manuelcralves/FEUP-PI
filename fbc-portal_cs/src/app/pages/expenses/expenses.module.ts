import { NgModule } from '@angular/core';
import { CommonModule,DatePipe } from '@angular/common';

import { ExpensesComponent } from './expenses.component';
import { ExpensesRoutingModule } from './expenses-routing.module';

import { EstadoDocInternoPipe } from 'src/app/shared/pipes/estado-doc-interno/estado-doc-interno.pipe';
import { EstadoDocInternoModule } from 'src/app/shared/pipes/estado-doc-interno/estado-doc-interno.module';

import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BlockUIModule } from "ng-block-ui";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { NgxDatatableModule } from "@siemens/ngx-datatable";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { BlockTemplateComponent } from 'src/app/shared/components/blockui/block-template.component';


@NgModule({

  imports: [
    CommonModule,
    ExpensesRoutingModule,
    PerfectScrollbarModule,
    NgxDatatableModule,
    NgbModule,
    ReactiveFormsModule,
    EstadoDocInternoModule,
    FormsModule,
    BlockUIModule.forRoot({ template: BlockTemplateComponent })
  ],
  declarations: [ExpensesComponent],
  providers: [DatePipe, EstadoDocInternoPipe],
  exports: []
})
export class ExpensesModule { }



