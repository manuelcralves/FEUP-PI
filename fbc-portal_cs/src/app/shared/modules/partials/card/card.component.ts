/* eslint-disable @angular-eslint/component-selector */
import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, Input, OnInit, ViewChild, EventEmitter, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { AppConstants } from 'src/app/shared/helpers/app.constants';

@Component({
    selector: 'm-card',
    templateUrl: './card.component.html',
    styleUrls: ['./card.component.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CardComponent implements AfterViewInit {
    @Input() loading$: Observable<boolean>;
    @Input() options: any;
    @Output() reloadFunction: EventEmitter<any> = new EventEmitter();

    @ViewChild('mCard', { static: true }) elCard: ElementRef;
    @ViewChild('mCardHeader', { static: true }) elHeader: ElementRef;
    @ViewChild('mCardHeaderTitle', { static: true }) elHeaderTitle: ElementRef;
    @ViewChild('mCardContent', { static: true }) elContent: ElementRef;
    @ViewChild('mCardBody', { static: true }) elBody: ElementRef;
    @ViewChild('mCardFooter', { static: true }) elFooter: ElementRef;
    @ViewChild('mCardHeaderTools', { static: true }) elHeaderTools: ElementRef;

    constructor() {
    }

    ngAfterViewInit(): void {

        if (this.elHeader && this.elHeader.nativeElement.children.length === 0) {
            this.elHeader.nativeElement.style.display = 'none';
        } else if (this.options && this.options['headerClass']) {
            this.options['headerClass'].forEach(element => {
                this.elHeader.nativeElement.classList.add(element);
            });
        }

        if (this.elHeaderTitle && (this.elHeaderTitle.nativeElement.children.length === 0 && this.elHeaderTitle.nativeElement.innerText.trim().length === 0)) {
            this.elHeader.nativeElement.style.display = 'none';
        }

        if (this.elFooter && this.elFooter.nativeElement.children.length === 0) {
            this.elFooter.nativeElement.style.display = 'none';
        }

        if (this.elHeaderTools && this.elHeaderTools.nativeElement.children.length === 0) {
            this.elFooter.nativeElement.style.display = 'none';
        }

        if (this.elContent && this.elContent.nativeElement.children.length === 0) {
            this.elContent.nativeElement.style.display = 'none';
        } else if (this.options && this.options['contentClass']) {
            this.options['contentClass'].forEach(element => {
                this.elContent.nativeElement.classList.add(element);
            });
        }

        if (this.elBody && this.elBody.nativeElement.children.length === 0) {
            this.elBody.nativeElement.style.display = 'none';
        } else if (this.options && this.options['bodyClass']) {
            this.options['bodyClass'].forEach(element => {
                this.elBody.nativeElement.classList.add(element);
            });
        }

        if (this.options && this.options['cardClass']) {
            this.options['cardClass'].forEach(element => {
                this.elCard.nativeElement.classList.add(element);
            });
        }
    }

    toggleCollpase(event) {
        let target = event.target || event.srcElement || event.currentTarget;

        if (this.elContent.nativeElement.classList.contains('show')) {
            this.elContent.nativeElement.classList.add('collapse');
            this.elContent.nativeElement.classList.remove('show');

            if (!target.classList.contains('ft-plus') && !target.classList.contains('ft-minus')) {
                target = event.target.querySelector('i');
            }

            target.classList.remove('ft-minus');
            target.classList.add('ft-plus');
        } else {
            this.elContent.nativeElement.classList.add('show');

            if (!target.classList.contains('ft-plus') && !target.classList.contains('ft-minus')) {
                target = event.target.querySelector('i');
            }

            this.elContent.nativeElement.classList.remove('collapse');
            target.classList.remove('ft-plus');
            target.classList.add('ft-minus');
        }

        this.toggleMobileMenu();
    }

    reload() {
        this.reloadFunction.emit(this.options);
    }

    close() {
        this.elCard.nativeElement.classList.add('hidden');
        this.elCard.nativeElement.classList.remove('card-fullscreen');

        this.toggleMobileMenu();
    }

    toggleExpand(event) {
        let target = event.target || event.srcElement || event.currentTarget;

        if (this.elCard.nativeElement.classList.contains('card-fullscreen')) {
            this.elCard.nativeElement.classList.remove('card-fullscreen');

            if (!target.classList.contains('ft-maximize') && !target.classList.contains('ft-minimize')) {
                target = event.target.querySelector('i');
            }

            target.classList.add('ft-maximize');
            target.classList.remove('ft-minimize');
        } else {
            this.elCard.nativeElement.classList.add('card-fullscreen');

            if (!target.classList.contains('ft-maximize') && !target.classList.contains('ft-minimize')) {
                target = event.target.querySelector('i');
            }

            target.classList.remove('ft-maximize');
            target.classList.add('ft-minimize');
        }

        this.toggleMobileMenu();
    }

    toggleMobileMenu() {
        if (this.elHeaderTools.nativeElement.classList.contains('visible')) {
            this.elHeaderTools.nativeElement.classList.remove('visible');
        } else {
            this.elHeaderTools.nativeElement.classList.add('visible');
        }

        // fire resize event for graphs
        setTimeout(() => { AppConstants.fireRefreshEventOnWindow(); }, 300);
    }
}
