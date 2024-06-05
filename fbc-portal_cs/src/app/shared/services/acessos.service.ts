import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { PerfilPortal } from "../interfaces/perfil-portal";
import { Utilizador } from "../interfaces/utilizador";
import { PermissaoPerfilPortal } from "../interfaces/permissao-perfil-portal";
import { PermissaoPortal } from "../interfaces/permissao-portal";
import { EmpresaUtilizador } from "../interfaces/empresa-utilizador";

@Injectable({
    providedIn: "root",
})
export class AcessosService {
    constructor(private httpClient: HttpClient) { }


    checkEmpresasUtilizador(codigoUtil: string ): Observable<void> {
        if (!codigoUtil)
            throw new Error('codigoUtilizador não definido');

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Acessos/VerificaEmpresasUtilizador/${encodeURIComponent(codigoUtil)}/`, "")
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.checkEmpresasUtilizador", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    checkPermissoesPerfil(codPerfil: string ): Observable<void> {
        if (!codPerfil)
            throw new Error('codigoPerfil não definido');

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Acessos/VerificaPermissoesPerfil/${encodeURIComponent(codPerfil)}/`, "")
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.checkPermissoesPerfil", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }


    getPermissoesUtilizadorAtual(): Observable<string[]> {
        return new Observable((observer) => {
            this.httpClient.get<string[]>(`${environment.MainUrl}/api/Acessos/PermissoesUtilizadorAtual/`).subscribe({
                next: (response: string[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on AcessosService.getPermissoesUtilizadorAtual", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }
    
    getEmpresasUtilizador(codPerfil: string): Observable<EmpresaUtilizador[]>{
        if (!codPerfil)
        throw new Error('codPerfil não definido');

        return new Observable((observer) => {
            this.httpClient.get<EmpresaUtilizador[]>(`${environment.MainUrl}/api/Acessos/EmpresasUtilizador/${encodeURIComponent(codPerfil)}`).subscribe({
                next: (response: EmpresaUtilizador[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on AcessosService.getEmpresasUtilizador", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getPermissoesPortal(): Observable<PermissaoPortal[]> {
        return new Observable((observer) => {
            this.httpClient.get<PermissaoPortal[]>(`${environment.MainUrl}/api/Acessos/PermissoesPortal/`).subscribe({
                next: (response: PermissaoPortal[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on AcessosService.getPermissoesPortal", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getPerfisPortal(): Observable<PerfilPortal[]> {
        return new Observable((observer) => {
            this.httpClient.get<PerfilPortal[]>(`${environment.MainUrl}/api/Acessos/PerfisPortal/`).subscribe({
                next: (response: PerfilPortal[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on AcessosService.getPerfisPortal", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getPerfilPortal(codPerfil: string, semPermissoes: boolean = false): Observable<PerfilPortal> {
        if (!codPerfil)
            throw new Error('codPerfil não definido');

        return new Observable((observer) => {
            this.httpClient.get<PerfilPortal>(`${environment.MainUrl}/api/Acessos/PerfisPortal/${encodeURIComponent(codPerfil)}/?semPermissoes=${encodeURIComponent(semPermissoes)}`)
                .subscribe({
                    next: (response: PerfilPortal) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.getPerfilPortal", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    criarPerfilPortal(perfil: PerfilPortal): Observable<void> {
        if (!perfil)
            throw new Error('perfil não definido');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Acessos/PerfisPortal/`, perfil)
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.criarPerfilPortal", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    alterarPerfilPortal(codigoPerfil: string, perfil: PerfilPortal): Observable<void> {
        if (!codigoPerfil)
            throw new Error('codigoPerfil não definido');

        if (!perfil)
            throw new Error('perfil não definido');

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Acessos/PerfisPortal/${encodeURIComponent(codigoPerfil)}/`, perfil)
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.alterarPerfilPortal", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    desativarPerfilPortal(codigoPerfil: string): Observable<void> {
        if (!codigoPerfil)
            throw new Error('codigoPerfil não definido');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Acessos/DesativarPerfilPortal/${encodeURIComponent(codigoPerfil)}/`, "")
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.desativarPerfilPortal", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    getUtilizadores(): Observable<Utilizador[]> {
        return new Observable((observer) => {
            this.httpClient.get<Utilizador[]>(`${environment.MainUrl}/api/Acessos/Utilizadores/`).subscribe({
                next: (response: Utilizador[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on AcessosService.getUtilizadores", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getUtilizador(codUtilizador: string): Observable<Utilizador> {
        
        if (!codUtilizador)
            throw new Error('codUtilizador não definido');

        return new Observable((observer) => {
            this.httpClient.get<Utilizador>(`${environment.MainUrl}/api/Acessos/Utilizadores/${encodeURIComponent(codUtilizador)}/`)
                .subscribe({
                    next: (response: Utilizador) => {

                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.getUtilizador", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    alterarUtilizador(codigoUtilizador: string, utilizador: Utilizador): Observable<void> {
        if (!codigoUtilizador)
            throw new Error('codigoUtilizador não definido');

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Acessos/Utilizadores/${encodeURIComponent(codigoUtilizador)}/`, utilizador)
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.alterarUtilizador", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    desativarAcessoPortal(codigoUtilizador: string): Observable<void> {
        if (!codigoUtilizador)
            throw new Error('codigoUtilizador não definido');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Acessos/DesativarAcessoPortal/${encodeURIComponent(codigoUtilizador)}/`, "")
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.desativarAcessoPortal", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    gerarEnviarAcessoPortal(codigoUtilizador: string): Observable<void> {
        if (!codigoUtilizador)
            throw new Error('codigoUtilizador não definido');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Acessos/GerarEnviarAcessoPortal/${encodeURIComponent(codigoUtilizador)}/`, "")
                .subscribe({
                    next: (response: void) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on AcessosService.gerarEnviarAcessoPortal", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }
}