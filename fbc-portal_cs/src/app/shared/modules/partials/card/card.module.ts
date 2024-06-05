import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatchHeightModule } from '../match-height/match-height.module';
import { CardComponent } from './card.component';
import { CardDirective } from './card.directive';

@NgModule({
    imports: [
        CommonModule,
        MatchHeightModule
    ],
    declarations: [CardComponent, CardDirective],
    exports: [CardComponent]
})
export class CardModule { }
