import { Injectable } from "@angular/core";
import { CanActivate, UrlTree, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { SessionService } from "../services/session.service";

@Injectable({
    providedIn: "root",
})
export class AuthGuard implements CanActivate {
    constructor(private sessionService: SessionService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        let isLoggedIn = this.sessionService.isLoggedIn();

        if (!isLoggedIn) {
            this.router.navigate(["/login"], { queryParams: { returnUrl: state.url } });
        }

        return isLoggedIn;
    }
}
