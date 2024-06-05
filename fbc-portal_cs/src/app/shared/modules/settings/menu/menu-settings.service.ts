import { Injectable, InjectionToken, Inject } from '@angular/core';
import { Router, RoutesRecognized } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { filter } from 'rxjs/operators';

import * as _ from 'lodash';
import { MenuConfig } from './menu-config';
import { AcessosService } from 'src/app/shared/services/acessos.service';
import { PermissaoPerfilPortal } from 'src/app/shared/interfaces/permissao-perfil-portal';
import { ApiErrorResponseHandlerService } from 'src/app/shared/services/api-error-response-handler.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { MenuItem } from './menu-item';

export const MENU_SETTINGS_CONFIG = new InjectionToken('menuCustomConfig');

@Injectable({
  providedIn: 'root'
})
export class MenuSettingsService {

  private _configSubject: BehaviorSubject<any>;
  private readonly _defaultConfig: any;
  @BlockUI() private blockUI: NgBlockUI;

  constructor(
    private _router: Router, @Inject(MENU_SETTINGS_CONFIG) private _config,
    private acessosService: AcessosService,
    private apiErrorService: ApiErrorResponseHandlerService
  ){
      
    this._defaultConfig = _config;
    // Initialize the service
    this._init();
  }

  private _init(): void {
    // Set the config from the default config
    this._configSubject = new BehaviorSubject(_.cloneDeep(this._defaultConfig));

    // Reload the default layout config on every RoutesRecognized event
    // if the current layout config is different from the default one
    this._router.events
      .pipe(filter(event => event instanceof RoutesRecognized))
      .subscribe(() => {
        if (!_.isEqual(this._configSubject.getValue().layout, this._defaultConfig.layout)) {
          // Clone the current config
          const config = _.cloneDeep(this._configSubject.getValue());

          // Set the config
          this._configSubject.next(config);
        }
      });    

      this.processarPermissoes();
  }

  getConfigOriginal(): MenuConfig {
    let config: MenuConfig = this._defaultConfig;

    return config;
  }


  processarPermissoes() {
    this.blockUI.start('A carregar...');
    this.acessosService.getPermissoesUtilizadorAtual().subscribe({
      next: (permissoesAtivas: string[]) => {
        this.blockUI.stop();

        let config: MenuConfig = _.cloneDeep(this._defaultConfig);

        this.processarPermissoesMenuItems(permissoesAtivas, config.vertical_menu.items);
        this.processarPermissoesMenuItems(permissoesAtivas, config.horizontal_menu.items);

        this._configSubject.next(config);
      },
      error: (error: any) => {
        this.blockUI.stop();

        let config: MenuConfig = _.cloneDeep(this._defaultConfig);

        config.vertical_menu.items = config.vertical_menu.items.filter(i => !i.permissaoNecessaria);
        config.horizontal_menu.items = config.horizontal_menu.items.filter(i => !i.permissaoNecessaria);

        this._configSubject.next(config);

        this.apiErrorService.handleError(error, "Erro a obter permissões de utilizador", window.location?.pathname?.toLowerCase().indexOf("login") > -1, true);
      }
    });
  }

  processarPermissoesMenuItems(permissoesAtivas: string[], menuItems: Partial<MenuItem>[]) {
    
    for (let i = menuItems.length - 1; i >= 0; i--) {
      const menu: Partial<MenuItem> = menuItems[i];

      if (menu.submenu)
        this.processarPermissoesMenuItems(permissoesAtivas, menu.submenu.items);

      if (menu.permissaoNecessaria) {
        let permissao = permissoesAtivas.find(p => menu.permissaoNecessaria.includes(p.toUpperCase()));

        if (!permissao)
          menuItems.splice(i, 1);
      }
    }
  }
  

  set config(value) {
    // Get the value from the behavior subject
    let config = this._configSubject.getValue();

    // Merge the new config
    config = _.merge({}, config, value);

    // Notify the observers
    this._configSubject.next(config);
  }

  get config(): any | Observable<any> {
    return this._configSubject.asObservable();
  }

}
