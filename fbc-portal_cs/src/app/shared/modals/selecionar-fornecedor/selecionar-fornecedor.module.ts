import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { BlockUIModule } from "ng-block-ui";
import { BlockTemplateComponent } from "src/app/shared/components/blockui/block-template.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { FormsModule } from "@angular/forms";
import { SelecionarFornecedorComponent } from "./selecionar-fornecedor.component";

@NgModule({
    imports: [CommonModule, NgbModule, FormsModule, BlockUIModule.forRoot({ template: BlockTemplateComponent })],
    declarations: [SelecionarFornecedorComponent],
    providers: [],
    exports: []
})
export class SelecionarFornecedorModule { }
