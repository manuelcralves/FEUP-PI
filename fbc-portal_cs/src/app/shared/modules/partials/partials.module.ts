import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CardModule } from './card/card.module';


@NgModule({
    declarations: [

    ],
    exports: [
        CardModule
    ],
    imports: [
        CommonModule,
        RouterModule,
        NgbModule
    ]
})
export class PartialsModule { }
