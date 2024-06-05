import { Component, AfterViewInit, OnInit, Inject, Renderer2, HostListener } from '@angular/core';
import { trigger, state, transition, style, animate } from '@angular/animations';
import { NavbarService } from 'src/app/shared/services/navbar.service';
import { DOCUMENT } from '@angular/common';
import { ThemeSettingsService } from 'src/app/shared/modules/settings/theme/theme-settings.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MenuSettingsService } from 'src/app/shared/modules/settings/menu/menu-settings.service';
// import { isArray } from 'util';
import { Router } from '@angular/router';
import { AppConstants } from 'src/app/shared/helpers/app.constants';


@Component({
  selector: 'app-horizontalnav',
  templateUrl: './horizontalnav.component.html',
  styleUrls: ['./horizontalnav.component.css'],
  animations: [
    trigger('fade',
      [
        state('void', style({ opacity: 0 })),
        transition(':enter', [animate(300)]),
        transition(':leave', [animate(500)]),
      ]
    )]
})
export class HorizontalnavComponent implements OnInit, AfterViewInit {

  insideTm: any;
  private _themeSettingsConfig: any;
  private _unsubscribeAll: Subject<any>;
  private _unsubscribeAllMenu: Subject<any>;
  public _menuSettingsConfig: any;

  constructor(@Inject(DOCUMENT) private document: Document,
    private navbarService: NavbarService,
    private _themeSettingsService: ThemeSettingsService,
    public _menuSettingsService: MenuSettingsService,
    private _renderer: Renderer2,
    private router: Router) {
    this._unsubscribeAll = new Subject();
    this._unsubscribeAllMenu = new Subject();
  }
  ngAfterViewInit(): void {
    this.refreshView();
  }

  ngOnInit() {
    // Subscribe to config changes
    this._themeSettingsService.config
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe((config) => {
        this._themeSettingsConfig = config;
        this.refreshView();
      });
    this._menuSettingsService.config
      .pipe(takeUntil(this._unsubscribeAllMenu))
      .subscribe((config) => {
        this._menuSettingsConfig = config;
      });
    this.setActiveRouteInNavbar();
  }
  setTheme(theme) {
    this._themeSettingsService.config = {
      colorTheme: theme, // semi-light, semi-dark
    };
  }

  setLayout(layout) {
    this._themeSettingsService.config = {
      layout: {
        pattern: layout
      }
    };
  }

  fixComponent(component, value) {
    if (component === 'header') {
      this._themeSettingsService.config = {
        header: value
      };
    } else if (component === 'footer') {
      this._themeSettingsService.config = {
        footer: value
      };
    } else {
      this._themeSettingsService.config = {
        header: value,
        footer: value
      };
    }
  }

  callFunction(event) {
    const methodName = event.methodName;
    if (this[methodName]) {
      // method exists on the component
      const param = event.methodParam;
      if (!Array.isArray(param)) {
        this[methodName](param); // call it
      } else {
        this[methodName](param[0], param[1]); // call it
      }

    }
  }


  refreshView() {
    const menuHeaderElement = document.getElementsByClassName('menu-header');
    // Theme
    if (menuHeaderElement && menuHeaderElement.length > 0) {
      if (this._themeSettingsConfig.colorTheme === 'light') {
        this._renderer.removeClass(menuHeaderElement.item(0), 'navbar-dark');
        this._renderer.addClass(menuHeaderElement.item(0), 'navbar-light');
      } else if (this._themeSettingsConfig.colorTheme === 'dark') {
        this._renderer.addClass(menuHeaderElement.item(0), 'navbar-dark');
        this._renderer.removeClass(menuHeaderElement.item(0), 'navbar-light');
      }
      if (this._themeSettingsConfig.layout.pattern === 'static') {
        this._renderer.addClass(menuHeaderElement.item(0), 'menu-static');
      } else if (this._themeSettingsConfig.layout.pattern === 'fixed') {
        this._renderer.removeClass(menuHeaderElement.item(0), 'menu-static');
      }
    }

  }
  setActiveRouteInNavbar() {
    for (let i = 0; i < this._menuSettingsConfig.horizontal_menu.items.length; i++) {
      if (!this._menuSettingsConfig.horizontal_menu.items[i].submenu &&
        this._menuSettingsConfig.horizontal_menu.items[i].page === this.router.url) {
        this._menuSettingsConfig.horizontal_menu.items[i]['isSelected'] = true;
        break;
      } else if (this._menuSettingsConfig.horizontal_menu.items[i].submenu) {
        // Level 1 menu
        for (let j = 0; j < this._menuSettingsConfig.horizontal_menu.items[i].submenu.items.length; j++) {
          if (!this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu &&
            this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].page === this.router.url) {
            this._menuSettingsConfig.horizontal_menu.items[i]['isSelected'] = true;
            this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j]['isSelected'] = true;
            this._menuSettingsConfig.horizontal_menu.items[i].isOpen = true;
            break;
          } else if (this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu) {
            // Level 2 menu
            for (let k = 0; k < this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu.items.length; k++) {
              if (this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu.items[k].page === this.router.url) {
                this._menuSettingsConfig.horizontal_menu.items[i]['isSelected'] = true;
                this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j]['isSelected'] = true;
                this._menuSettingsConfig.horizontal_menu.items[i].isOpen = true;

                this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j]['isSelected'] = true;
                this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].submenu.items[k]['isSelected'] = true;
                this._menuSettingsConfig.horizontal_menu.items[i].submenu.items[j].isOpen = true;
              }
            }
          }
        }
      }
    }
  }
  toggleMenu(event, child, isSubmenuOfSubmenu) {
    const toggle = document.getElementById('sidenav-overlay');
    const target = event.target || event.srcElement || event.currentTarget;
    this.resetOtherActiveMenu(child, isSubmenuOfSubmenu);
    child['isSelected'] = true;

    if (this.router.url !== '') {
      this._renderer.addClass(toggle, 'd-none');
      this._renderer.removeClass(toggle, 'd-block');
    }
  }

  resetOtherActiveMenu(selectedChild, isSubmenuOfSubmenu) {
    for (let i = 0; i < this._menuSettingsConfig.horizontal_menu.items.length; i++) {
      if (selectedChild.page !== 'null') {
        this._menuSettingsConfig.horizontal_menu.items[i]['isSelected'] = false;
      }
      this.handleSubmenuItems(this._menuSettingsConfig.horizontal_menu.items[i], selectedChild, isSubmenuOfSubmenu);
    }
  }

  handleSubmenuItems(parentItem, selectedChild, isSubmenuOfSubmenu) {
    let isSelectedChildExist = false;
    if (selectedChild['title'] === 'Horizontal') {
      localStorage.setItem('currentLayoutStyle', AppConstants.LAYOUT_STYLE_HORIZONTAL);
      window.location.reload();
    } else if (selectedChild['title'] === 'Vertical') {
      localStorage.setItem('currentLayoutStyle', AppConstants.LAYOUT_STYLE_VERTICAL);
      window.location.reload();
    } else if (parentItem['submenu'] &&
      parentItem['submenu']['items'] &&
      parentItem['submenu']['items'].length > 0) {
      for (let j = 0; j < parentItem['submenu']['items'].length; j++) {
        if (selectedChild.title === parentItem['submenu']['items'][j].title) {
          isSelectedChildExist = true;
        }
        this.handleSubmenuItems(parentItem['submenu']['items'][j], selectedChild, isSubmenuOfSubmenu);

        if (parentItem['submenu']['items'][j]['isSelected'] === true) {
          isSelectedChildExist = true;
        }
      }
      if (!isSelectedChildExist) {
        parentItem['isSelected'] = false;
      } else {
        parentItem['isSelected'] = true;
      }
    } else if (parentItem.title !== selectedChild.title && !isSubmenuOfSubmenu) {
      parentItem['isSelected'] = false;
    }
  }


  @HostListener('window:scroll', ['$event'])
  onWindowScroll(e) {
    const element = document.getElementById('sticky-wrapper');
    if (this._themeSettingsConfig.layout.style === 'horizontal') {
      if (this._themeSettingsConfig.layout.pattern === 'fixed' ||
        this._themeSettingsConfig.layout.pattern === '') {
        if (window.pageYOffset > 70) { // 70 is the height of the horizontal header
          element.classList.add('is-sticky');
          element.classList.remove('container');
        } else {
          element.classList.remove('is-sticky');
        }
      } else if (this._themeSettingsConfig.layout.pattern === 'boxed') {
        if (window.pageYOffset > 70) { // 70 is the height of the horizontal header
          element.classList.add('is-sticky');
          element.classList.add('container');
        } else {
          element.classList.remove('is-sticky');
          element.classList.remove('container');
        }
      } else if (this._themeSettingsConfig.layout.pattern === 'static') {
        element.classList.remove('is-sticky');
        element.classList.remove('container');
        element.classList.add('is-static');
      }
    }
  }


  mouseEnter(e) {
    const _event = e;
    _event.srcElement.classList.add('show');
  }

  /**
     * Use for fixed left aside menu, to show menu on mouseenter event.
     * @param e Event
     */
  mouseLeave(event) {
    const _event = event;
    _event.srcElement.classList.remove('show');
  }
}
