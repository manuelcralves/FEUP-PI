import { Component, Input, OnInit } from '@angular/core';
import { NgxDropzonePreviewComponent } from 'ngx-dropzone';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-dropzone-custom-file-preview',
    templateUrl: './dropzone-custom-file-preview.component.html',
    styleUrls: ['./dropzone-custom-file-preview.component.scss']
})
export class DropZoneCustomFilePreviewComponent extends NgxDropzonePreviewComponent implements OnInit {

    @Input()
    get labelText(): string {
        return this._labelText;
    }
    set labelText(value: string) {
        this._labelText = value ?? "";
    }
    protected _labelText = "";

    constructor(sanitizer: DomSanitizer) {
        super(sanitizer);
    }

    ngOnInit() {

    }

    remover(event) {
        event.stopPropagation();
        this.remove();
    }

    fileSize(): string {
        let size = 0;
        let unit = "B";

        if (this.file)
            size = this.file.size;

        if (size >= 1073741824) {
            size /= 1073741824;
            unit = "GB";
        }
        else if (size >= 1048576) {
            size /= 1048576;
            unit = "MB";
        }
        else if (size >= 1024) {
            size /= 1024;
            unit = "KB";
        }

        return `${size.toFixed(2)} ${unit}`;
    }
}
