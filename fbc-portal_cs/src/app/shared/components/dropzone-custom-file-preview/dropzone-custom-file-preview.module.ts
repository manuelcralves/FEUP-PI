import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { DropZoneCustomFilePreviewComponent } from './dropzone-custom-file-preview.component';

@NgModule({

  imports: [
    CommonModule,
    NgxDropzoneModule
  ],
   declarations: [DropZoneCustomFilePreviewComponent],
   exports: [DropZoneCustomFilePreviewComponent]
})
export class DropzoneCustomFilePreviewModule { }
