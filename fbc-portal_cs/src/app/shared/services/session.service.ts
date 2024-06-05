import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { Router } from "@angular/router";
import { environment } from "src/environments/environment";
import { AppConstants } from "../helpers/app.constants";
import { TokenObject } from "../interfaces/token-object";
import { DateTime } from "luxon";
import { Utilizador } from "../interfaces/utilizador";

@Injectable({
    providedIn: "root",
})
export class SessionService {
    constructor(private httpClient: HttpClient, private router: Router) { }

    login(username: string, password: string): Observable<boolean> {
        localStorage.removeItem(AppConstants.SESSIONSTORAGEID_TOKEN_OBJECT);

        const httpOptions = {
            headers: new HttpHeaders({
                "Authorization": `Basic ${Buffer.from(`${environment.ApiClientId}:`).toString('base64')}`,
                "Content-Type": "application/x-www-form-urlencoded",
            }),
            //observe: "response" as "response",
        };

        const body = new HttpParams()
            .set('username', username)
            .set('password', password)
            .set('grant_type', 'password');

        return new Observable((observer) => {
            this.httpClient.post<TokenObject>(`${environment.MainUrl}/Token`, body, httpOptions)
                .subscribe({
                    next: (response: TokenObject) => {
                        localStorage[AppConstants.SESSIONSTORAGEID_TOKEN_OBJECT] = JSON.stringify(response);

                        observer.next(true);
                    },
                    error: (error: any) => {
                        console.error("error on login", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    logout(): void {
        localStorage.removeItem(AppConstants.SESSIONSTORAGEID_TOKEN_OBJECT);
    }

    isLoggedIn(): boolean {
        let tokenObjStr: string = localStorage[AppConstants.SESSIONSTORAGEID_TOKEN_OBJECT];

        if (!tokenObjStr)
            return false;

        let tokenObj: TokenObject;

        try {
            tokenObj = JSON.parse(tokenObjStr);
        }
        catch (ex) {
            console.error(ex);
        }

        if (!tokenObj)
            return false;

        let expiresStr = tokenObj[".expires"];

        if (!expiresStr)
            return true;

        let expiresDt = DateTime.fromHTTP(expiresStr);

        if (DateTime.now() >= expiresDt)
            return false;

        return true;
    }

    getUtilizadorFromToken(): Utilizador {
        let tokenObjStr: string = localStorage[AppConstants.SESSIONSTORAGEID_TOKEN_OBJECT];

        if (!tokenObjStr)
            return null;

        let tokenObj: TokenObject;

        try {
            tokenObj = JSON.parse(tokenObjStr);
        }
        catch (ex) {
            console.error(ex);
        }

        if (!tokenObj)
            return null;

        return { Codigo: tokenObj.codigoUtilizador, Nome: tokenObj.nomeUtilizador, Administrador: tokenObj.adminUtilizador};
    }
}