import { NgModule } from '@angular/core';
import { CommonModule, DatePipe} from '@angular/common';

import { ExpenseComponent } from './expense.component';
import { ExpenseRoutingModule } from './expense-routing.module';

import { BlockUIModule } from "ng-block-ui";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { NgxDatatableModule } from "@siemens/ngx-datatable";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { NgSelectModule } from '@ng-select/ng-select';
import { CardModule } from "src/app/shared/modules/partials/card/card.module";
import { BlockTemplateComponent } from 'src/app/shared/components/blockui/block-template.component';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { DropzoneCustomFilePreviewModule } from "src/app/shared/components/dropzone-custom-file-preview/dropzone-custom-file-preview.module";

import { SelecionarClienteModule } from 'src/app/shared/modals/selecionar-cliente/selecionar-cliente.module';
import { SelecionarArtigoModule } from 'src/app/shared/modals/selecionar-artigo/selecionar-artigo.module';
import { SelecionarUnidadeModule } from 'src/app/shared/modals/selecionar-unidade/selecionar-unidade.module';
import { SelecionarFornecedorModule } from 'src/app/shared/modals/selecionar-fornecedor/selecionar-fornecedor.module';
import { SelecionarObraModule } from 'src/app/shared/modals/selecionar-obra/selecionar-obra.module';


@NgModule({ 
  imports: [
   CommonModule,
   ExpenseRoutingModule,
   PerfectScrollbarModule,
   NgxDatatableModule,
   NgbModule,
   NgxDropzoneModule,
   DropzoneCustomFilePreviewModule,
   BlockUIModule.forRoot({ template: BlockTemplateComponent }),
   ReactiveFormsModule,
   FormsModule,
   NgSelectModule,
   CardModule,
   SelecionarClienteModule,
   SelecionarArtigoModule,
   SelecionarUnidadeModule,
   SelecionarFornecedorModule,
   SelecionarObraModule
 ],
 declarations: [ExpenseComponent],
 providers: [DatePipe],
 exports: []
})
export class ExpenseModule { }
