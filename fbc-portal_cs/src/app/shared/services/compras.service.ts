import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { catchError, map, Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { TipoDocumentosCompras } from "../interfaces/tipo-documentos-compras";
import { SerieCompras } from "../interfaces/serie-compras";
import { DocumentoCompra } from "../interfaces/documento-compra";

@Injectable({
    providedIn: "root",
})
export class ComprasService {
    constructor(private httpClient: HttpClient) { }

    alterarDocumento(id: string, documento: DocumentoCompra, ficheirosAnexos: File[]): Observable<void> {

        if (!id)
        throw new Error('id não definido');

        if (!documento)
        throw new Error('documento não definido');

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Compras/Documento/${encodeURIComponent(id)}/`, formData)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on ComprasService.alterarDocumento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    criarDocumento(documento: DocumentoCompra, ficheirosAnexos: File[]): Observable<string> {

        if (!documento)
        throw new Error('documento não definido');

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.post<string>(`${environment.MainUrl}/api/Compras/Documento`, formData)
            .subscribe({
                next: (response: string) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on ComprasService.createDocumento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }
    
    getEncomendas(): Observable<DocumentoCompra[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoCompra[]>(`${environment.MainUrl}/api/Compras/Encomendas`)
            .pipe(
                map((items: DocumentoCompra[]) => {
                    return items.map(
                        item => {
                            if (item.DataDoc)
                            item.DataDoc = new Date(item.DataDoc);
                            return item;
                        });
                    }),
                    catchError((err, caught) => {
                        throw err;
                    })
                    )
                    .subscribe({
                        next: (response: DocumentoCompra[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on ComprasService.getEncomendas", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
                });
    }

    getTiposDocumento(): Observable<TipoDocumentosCompras[]> {

        return new Observable((observer) => {
            this.httpClient.get<TipoDocumentosCompras[]>(`${environment.MainUrl}/api/Compras/TiposDocumento/`)
                .subscribe({
                    next: (response: TipoDocumentosCompras[]) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on ComprasService.TiposDocumentos", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    getSeries(tipoDocumento: string): Observable<SerieCompras[]> {


        if (!tipoDocumento)
            throw new Error('tipoDocumento não definido');

        return new Observable((observer) => {
            this.httpClient.get<SerieCompras[]>(`${environment.MainUrl}/api/Compras/Series/?tipoDocumento=${encodeURIComponent(tipoDocumento)}`)
                .subscribe({
                    next: (response: SerieCompras[]) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on ComprasService.getSeries", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    getProximoNumeroSerie( tipoDocumento: string, serie: string): Observable<number> {


        if (!tipoDocumento)
            throw new Error('tipoDocumento não definido');

        if (!serie)
            throw new Error('serie não definido');

        return new Observable((observer) => {
            this.httpClient.get<number>(`${environment.MainUrl}/api/Compras/ProximoNumeroSerie/?tipoDocumento=${encodeURIComponent(tipoDocumento)}&serie=${encodeURIComponent(serie)}`)
                .subscribe({
                    next: (response: number) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on ComprasService.getProximoNumeroSerie", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    getRascunho(Id: string): Observable<DocumentoCompra> {

        if (!Id)
        throw new Error('Id não definido');

        return new Observable((observer) => {
            this.httpClient.get<DocumentoCompra>(`${environment.MainUrl}/api/Compras/Rascunhos/${encodeURIComponent(Id)}`)
            .pipe(
                map((item: DocumentoCompra) => {
                    if (item) {
                        if (item.DataDoc)
                        item.DataDoc = new Date(item.DataDoc);
                        
                        if (item.DataVenc)
                        item.DataVenc = new Date(item.DataVenc);
                    }
                    return item;
                }),
                catchError((err, caught) => {
                    throw err;
                })
                )
                .subscribe({
                    next: (response: DocumentoCompra) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on ComprasService.getRascunho", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
            });
        }

    getRascunhosUtilizador(): Observable<DocumentoCompra[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoCompra[]>(`${environment.MainUrl}/api/Compras/Rascunhos/Utilizador`)
            .pipe( map((items: DocumentoCompra[]) => {
                    return items.map(
                        item => {
                            if (item.DataDoc)
                            item.DataDoc = new Date(item.DataDoc);
                            return item;
                        });
                    }), 
                    catchError((err, caught) => {
                        throw err;
                    })
                    )
                    .subscribe({
                        next: (response: DocumentoCompra[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on ComprasService.getRascunhos", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    getRascunhosEstado(estado: string): Observable<DocumentoCompra[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoCompra[]>(`${environment.MainUrl}/api/Compras/Rascunhos/${encodeURIComponent(estado)}`)
            .pipe( map((items: DocumentoCompra[]) => {
                    return items.map(
                        item => {
                            if (item.DataDoc)
                            item.DataDoc = new Date(item.DataDoc);
                            return item;
                        });
                    }), 
                    catchError((err, caught) => {
                        throw err;
                    })
                    )
                    .subscribe({
                        next: (response: DocumentoCompra[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on ComprasService.getRascunhosEstado", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    getRascunhos(): Observable<DocumentoCompra[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoCompra[]>(`${environment.MainUrl}/api/Compras/Rascunhos`)
            .pipe( map((items: DocumentoCompra[]) => {
                    return items.map(
                        item => {
                            if (item.DataDoc)
                            item.DataDoc = new Date(item.DataDoc);
                            return item;
                        });
                    }), 
                    catchError((err, caught) => {
                        throw err;
                    })
                    )
                    .subscribe({
                        next: (response: DocumentoCompra[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on ComprasService.getRascunhos", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    getRascunhosAprovacao(): Observable<DocumentoCompra[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoCompra[]>(`${environment.MainUrl}/api/Compras/Rascunhos/Aprovacao`)
            .pipe( map((items: DocumentoCompra[]) => {
                    return items.map(
                        item => {
                            if (item.DataDoc)
                            item.DataDoc = new Date(item.DataDoc);
                            return item;
                        });
                    }), 
                    catchError((err, caught) => {
                        throw err;
                    })
                    )
                    .subscribe({
                        next: (response: DocumentoCompra[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on ComprasService.getRascunhosAprovacao", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    alterarRascunho( id: string, documento: DocumentoCompra, ficheirosAnexos: File[]): Observable<void> {

        if (!id)
        throw new Error('id não definido');

        if (!documento)
        throw new Error('documento não definido');

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Compras/Rascunhos/${encodeURIComponent(id)}/`, formData)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on ComprasService.alterarRascunho", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    criarRascunho(documento: DocumentoCompra, ficheirosAnexos: File[]): Observable<string> {

        if (!documento)
        throw new Error('documento não definido');

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.post<string>(`${environment.MainUrl}/api/Compras/Rascunhos`, formData)
            .subscribe({
                next: (response: string) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on ComprasService.criarRascunho", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    deleteRascunho( id: string): Observable<void> {

        if (!id)
        throw new Error('id não definido');

        debugger

        return new Observable((observer) => {
            this.httpClient.delete<void>(`${environment.MainUrl}/api/Compras/Rascunhos/${encodeURIComponent(id)}`)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on ComprasService.anularRascunho", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

}