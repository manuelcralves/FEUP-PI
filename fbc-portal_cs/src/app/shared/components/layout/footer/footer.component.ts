import { Component, OnInit, Renderer2 } from '@angular/core';
import { ThemeSettingsService } from 'src/app/shared/modules/settings/theme/theme-settings.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, NavigationEnd, NavigationStart, Event } from '@angular/router';
import { AppConstants } from 'src/app/shared/helpers/app.constants';

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
    public showFooter: boolean;
    public fixedFooter: boolean;
    public darkFooter: boolean;
    public hideMadeWithLove: boolean;

    private _unsubscribeAll: Subject<any>;
    private _themeSettingsConfig: any;

    constructor(private renderer: Renderer2,
        private _renderer: Renderer2,
        private router: Router,
        private _themeSettingsService: ThemeSettingsService) {

        this._unsubscribeAll = new Subject();

        this.router.events.subscribe((event: Event) => {
            const footerElement = document.getElementsByClassName('footer');

            if (event instanceof NavigationStart) {
                // Show loading indicator
            }

            if (event instanceof NavigationEnd) {
                if (footerElement.item(0)) {
                    this._renderer.removeClass(footerElement.item(0), 'fixed-bottom');
                    this.renderer.addClass(footerElement.item(0), 'footer-static');
                }
            }
        });
    }

    ngOnInit() {
        const isMobile = window.innerWidth < AppConstants.MOBILE_RESPONSIVE_WIDTH;

        this.showFooter = true;
        this.darkFooter = false;
        this.fixedFooter = false;

        if (isMobile) {
            this.hideMadeWithLove = true;
        }

        // Subscribe to config changes
        this._themeSettingsService.config
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((config) => {
                this._themeSettingsConfig = config;
            });
    }
}
