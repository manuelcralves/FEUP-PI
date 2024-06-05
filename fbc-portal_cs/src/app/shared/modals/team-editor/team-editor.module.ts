import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { BlockUIModule } from "ng-block-ui";
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { BlockTemplateComponent } from "src/app/shared/components/blockui/block-template.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { TeamEditorModalComponent } from "./team-editor.component";
import { ReactiveFormsModule } from "@angular/forms";

@NgModule({
    imports: [
        CommonModule,
        NgbModule,
        NgSelectModule,
        FormsModule,
        ReactiveFormsModule,
        BlockUIModule.forRoot({ template: BlockTemplateComponent })
    ],
    declarations: [TeamEditorModalComponent],
    providers: [],
    exports: []
})
export class TeamEditorModalModule { }
