import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { BlockUIModule } from "ng-block-ui";
import { BlockTemplateComponent } from "src/app/shared/components/blockui/block-template.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { SelecionarClienteComponent } from "./selecionar-cliente.component";
import { FormsModule } from "@angular/forms";

@NgModule({
    imports: [CommonModule, NgbModule, FormsModule, BlockUIModule.forRoot({ template: BlockTemplateComponent })],
    declarations: [SelecionarClienteComponent],
    providers: [],
    exports: []
})
export class SelecionarClienteModule { }
