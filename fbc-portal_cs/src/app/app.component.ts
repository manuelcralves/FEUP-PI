﻿import { Component, Inject, OnInit , Injectable } from '@angular/core';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { NavigationStart, RouteConfigLoadStart, RouteConfigLoadEnd, NavigationEnd, NavigationCancel } from '@angular/router';
import { DeviceDetectorService } from 'ngx-device-detector';
import { DOCUMENT } from '@angular/common';
import { Router } from '@angular/router';
import { AppConstants } from './shared/helpers/app.constants';
import { MenuSettingsService } from 'src/app/shared/modules/settings/menu/menu-settings.service';
import { ThemeSettingsService } from 'src/app/shared/modules/settings/theme/theme-settings.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Title } from '@angular/platform-browser';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
  selector: 'app-main',
  templateUrl: 'app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  public _menuSettingsConfig: any;
  public _themeSettingsConfig: any;
  public _unsubscribeAll: Subject<any>;
  private _unsubscribeAllMenu: Subject<any>;
  public showContent = false;
  public title;

  constructor(
      @Inject(DOCUMENT) private document: Document,
      private router: Router,
      public loader: LoadingBarService,
      private deviceService: DeviceDetectorService,
      public _menuSettingsService: MenuSettingsService,
      public _themeSettingsService: ThemeSettingsService,
      private titleService: Title
  ) {

      this._unsubscribeAll = new Subject();
      this._unsubscribeAllMenu = new Subject();

      this.setTitle();
  }

  ngOnInit(): void {
    
      this._menuSettingsService.config
          .pipe(takeUntil(this._unsubscribeAllMenu))
          .subscribe((config) => {
              this._menuSettingsConfig = config;
          });

      this._themeSettingsService.config
          .pipe(takeUntil(this._unsubscribeAll))
          .subscribe((config) => {
              this._themeSettingsConfig = config;
          });

      // page progress bar percentage
      this.router.events.subscribe(event => {
          if (event instanceof NavigationStart) {
              // loading start
              this.blockUI.start('A carregar...');
          }

          if (event instanceof NavigationEnd || event instanceof NavigationCancel) {
              // loading stop
              this.blockUI.stop();

              this.showContent = true;

              // close menu for mobile view
              if (this.deviceService.isMobile() || window.innerWidth < AppConstants.MOBILE_RESPONSIVE_WIDTH) {
                  if (document.body.classList.contains('menu-open')) {
                      document.body.classList.remove('menu-open');
                      document.body.classList.add('menu-close');
                  }
              }

              if (this.title) {
                  this.titleService.setTitle(this.title + ' - ' + this._themeSettingsConfig.defaultTitleSuffix);
              } else {
                  this.titleService.setTitle(this._themeSettingsConfig.defaultTitleSuffix);
              }
          }
      });
  }

  setTitle() {
      this.router.events.subscribe(event => {
          if (event instanceof NavigationEnd) {
              if (this._themeSettingsConfig.layout.style === 'vertical') {
                  for (let i = 0; i < this._menuSettingsConfig.vertical_menu.items.length; i++) {
                      if (!this._menuSettingsConfig.vertical_menu.items[i].submenu && this._menuSettingsConfig.vertical_menu.items[i].page === this.router.url) {
                          this.title = this._menuSettingsConfig.vertical_menu.items[i].title;
                          break;
                      } else if (this._menuSettingsConfig.vertical_menu.items[i].submenu) {
                          // Level 1 menu
                          for (let j = 0; j < this._menuSettingsConfig.vertical_menu.items[i].submenu.items.length; j++) {
                              if (!this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].submenu && this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].page === this.router.url) {
                                  this.title = this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].title;
                                  break;
                              } else if (this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].submenu) {
                                  // Level 2 menu
                                  for (let k = 0; k < this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].submenu.items.length; k++) {
                                      if (this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].submenu.items[k].page === this.router.url) {
                                          this.title = this._menuSettingsConfig.vertical_menu.items[i].submenu.items[j].submenu.items[k].title;
                                      }
                                  }
                              }
                          }
                      }
                  }
              } else if (this._themeSettingsConfig.layout.style === 'horizontal') {
                  for (let i = 0; i < this._menuSettingsConfig.horizontal_menu.items.length; i++) {
                      if (!this._menuSettingsConfig.horizontal_menu.items[i].submenu && this._menuSettingsConfig.horizontal_menu.items[i].page === this.router.url) {
                          this.title = this._menuSettingsConfig.horizontal_menu.items[i].title;
                          break;
                      } else if (this._menuSettingsConfig.horizontal_menu.items[i].submenu) {
                          // Level 1 menu
                          for (let j = 0; j < this._menuSettingsConfig.horizontal_menu.items[i].submenu.items.length; j++) {
                              if (!this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu && this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].page === this.router.url) {
                                  this.title = this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].title;
                                  break;
                              } else if (this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu) {
                                  // Level 2 menu
                                  for (let k = 0; k < this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu.items.length; k++) {
                                      if (this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu.items[k].page === this.router.url) {
                                          this.title = this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu.items[k].title;
                                      }
                                  }
                              }
                          }
                      }
                  }
              }
          }
      });
  }
}
