import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

import { OrdersComponent } from './orders.component';
import { OrdersRoutingModule } from './orders-routing.module';

import { EstadoDocInternoPipe } from 'src/app/shared/pipes/estado-doc-interno/estado-doc-interno.pipe';
import { EstadoDocInternoModule } from 'src/app/shared/pipes/estado-doc-interno/estado-doc-interno.module';

import { BlockUIModule } from "ng-block-ui";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { NgxDatatableModule } from "@siemens/ngx-datatable";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { BlockTemplateComponent } from 'src/app/shared/components/blockui/block-template.component';


@NgModule({
  
  imports: [
    OrdersRoutingModule,
    CommonModule,
    PerfectScrollbarModule,
    EstadoDocInternoModule,
    NgxDatatableModule,
    NgbModule,
    BlockUIModule.forRoot({ template: BlockTemplateComponent })
  ],
    declarations: [OrdersComponent],
    providers: [DatePipe,EstadoDocInternoPipe],
    exports: []
})
export class OrdersModule { }
