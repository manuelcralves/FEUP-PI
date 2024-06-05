import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { BlockUIModule } from "ng-block-ui";
import { BlockTemplateComponent } from "src/app/shared/components/blockui/block-template.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { LoginModalComponent } from "./login-modal.component";
import { ReactiveFormsModule } from "@angular/forms";

@NgModule({
    imports: [CommonModule, NgbModule, ReactiveFormsModule, BlockUIModule.forRoot({ template: BlockTemplateComponent })],
    declarations: [LoginModalComponent],
    providers: [],
    exports: []
})
export class LoginModalModule { }
