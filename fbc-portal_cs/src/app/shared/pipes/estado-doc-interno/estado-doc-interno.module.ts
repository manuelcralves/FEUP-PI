import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EstadoDocInternoPipe } from './estado-doc-interno.pipe';


@NgModule({
 
  imports: [
    CommonModule
  ],
   declarations: [EstadoDocInternoPipe],
   exports: [EstadoDocInternoPipe]
})
export class EstadoDocInternoModule { }
