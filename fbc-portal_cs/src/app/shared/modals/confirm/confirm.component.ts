import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-confirm-modal',
    templateUrl: './confirm.component.html',
    styleUrls: ['./confirm.component.scss']
})
export class ConfirmComponent implements OnInit {
    public modalTitle = "Confirmar";
    public modalMessage = "";
    public defaultButton = "nao";

    constructor(public modal: NgbActiveModal) { this.defaultButton = "nao"; }

    ngOnInit(): void {

    }
}
