import { Injectable } from "@angular/core";
import { HttpRequest, HttpHandler, HttpInterceptor, HttpEvent } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { AppConstants } from "../helpers/app.constants";
import { TokenObject } from "../interfaces/token-object";

@Injectable()
export class ApiAuthInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!request.url.startsWith(environment.MainUrl))
            return next.handle(request);

        let tokenObjStr: string = localStorage[AppConstants.SESSIONSTORAGEID_TOKEN_OBJECT];

        if (!tokenObjStr)
            return next.handle(request);

        let tokenObj: TokenObject;

        try {
            tokenObj = JSON.parse(tokenObjStr);
        }
        catch (ex) {
            console.error(ex);
        }

        if (!tokenObj)
            return next.handle(request);

        const authReq = request.clone({
            setHeaders: {
                Authorization: `Bearer ${tokenObj.access_token}`,
            },
        });

        return next.handle(authReq);
    }
}
