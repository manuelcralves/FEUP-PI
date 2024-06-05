import { Injectable } from "@angular/core";
import { CanActivate, UrlTree, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from "@angular/router";
import { is } from "date-fns/locale";
import { ToastrService } from "ngx-toastr";
import { first, Observable } from "rxjs";
import { PermissaoPerfilPortal } from "../interfaces/permissao-perfil-portal";
import { MenuConfig } from "../modules/settings/menu/menu-config";
import { MenuItem } from "../modules/settings/menu/menu-item";
import { MenuSettingsService } from "../modules/settings/menu/menu-settings.service";
import { ThemeSettingsService } from "../modules/settings/theme/theme-settings.service";
import { AcessosService } from "../services/acessos.service";
import { ApiErrorResponseHandlerService } from "../services/api-error-response-handler.service";


@Injectable({
    providedIn: "root",
})
export class PermissionsGuard implements CanActivate {
    constructor(
        private acessosService: AcessosService,
        private apiErrorService: ApiErrorResponseHandlerService,
        private themeSettingsService: ThemeSettingsService,
        private menuSettingsService: MenuSettingsService,
        private toastr: ToastrService,
        private router: Router
        ) { }

        canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
            let menuAtual: Partial<MenuItem>;
            let erro = false;

            let canActivatePromise = new Promise<boolean>((resolve, reject) => {
                this.acessosService.getPermissoesAtivasUtilizadorAtual().subscribe({
                    next: (permissoesAtivas: PermissaoPerfilPortal[]) => {
                        this.themeSettingsService.config
                        .pipe(first())
                        .subscribe({
                            next: (layoutConfig: any) => {
                                let menuConfig = this.menuSettingsService.getConfigOriginal();

                                let menuItems = menuConfig.vertical_menu.items;
                                if (layoutConfig.layout.style == "horizontal")
                                //menuItems = menuConfig.horizontal_menu.items;

                                menuAtual = this.getMenuAtual(route, menuItems);

                                let isAllowed = false;
                                if (menuAtual) {
                                    if (!menuAtual.permissaoNecessaria)
                                    isAllowed = true;
                                    else {
                                        let permissao = permissoesAtivas.find(p => menuAtual.permissaoNecessaria.includes(p.Codigo?.toUpperCase()));

                                        if (permissao)
                                        isAllowed = true;
                                    }
                                }

                                resolve(isAllowed);
                            },
                            error: (error: any) => {
                                this.apiErrorService.handleError(error, "Erro a obter layoutConfig");

                                erro = true;
                                resolve(false);
                            }
                        });
                    },
                    error: (error: any) => {
                        this.apiErrorService.handleError(error, "Erro a obter permissões de utilizador");

                        erro = true;
                        resolve(false);
                    }
                });
            });

            canActivatePromise
            .then((isAllowed) => {
                if (!isAllowed) {
                    if (!erro) {
                        if (menuAtual)
                        this.toastr.error(`Não tem acesso a '${menuAtual.title}'.`, '', { timeOut: 3000 });
                        else {
                            let routeUrl = route.pathFromRoot
                            .filter(v => v.routeConfig)
                            .map(v => v.routeConfig!.path)
                            .join('/');

                            if (routeUrl)
                            this.toastr.error(`Não tem acesso a '${routeUrl}'.`, '', { timeOut: 3000 });
                            else
                            this.toastr.error(`Não tem acesso a esta opção.`, '', { timeOut: 3000 });
                        }
                    }

                    this.router.navigate(["/"]);
                }
            })
            .catch((reason: any) => {
                this.apiErrorService.handleError(reason, "Erro a validar permissões", true, true);
            });

            return canActivatePromise;
        }

        getMenuAtual(route: ActivatedRouteSnapshot, menuItems: Partial<MenuItem>[]): Partial<MenuItem> {
            let routeUrl = route.pathFromRoot
            .filter(v => v.routeConfig)
            .map(v => v.routeConfig!.path)
            .join('/');

            for (let i = 0; i < menuItems.length; i++) {
                const menu = menuItems[i];

                if (routeUrl.startsWith(menu.page))
                return menu;
                else if (menu.submenu)
                {
                    let subMenu = this.getMenuAtual(route, menu.submenu.items);
                    if(subMenu)
                    return subMenu;
                }
            }

            return null;
        }
    }
