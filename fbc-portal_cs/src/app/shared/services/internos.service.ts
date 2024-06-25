import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { catchError, map, Observable, BehaviorSubject } from "rxjs";
import { environment } from "src/environments/environment";
import { TipoDocumentosInternos } from "../interfaces/tipo-documentos-internos";
import { SerieInternos } from "../interfaces/serie-internos";
import { DocumentoInterno } from "../interfaces/documento-interno";


@Injectable({
    providedIn: "root",
})
export class InternosService {
    private totalDespesasSource = new BehaviorSubject<number>(0);

    constructor(private httpClient: HttpClient) { }

    atualizarTotalDespesas(total: number) {
        this.totalDespesasSource.next(total);
    }

    obterTotalDespesas(): Observable<number> {
        return this.getDespesas().pipe(
            map(encomendas => encomendas.length)
        );
    }

    obterTotalDespesasPorAprovar(): Observable<number> {
        return this.getRascunhos().pipe(
            map(encomendas => encomendas.length)
        );
    }

    obterDespesasPorTrimestre(ano: number): Observable<number[]> {
        return this.getDespesas().pipe(
            map(despesas => {
                const trimestres = [0, 0, 0, 0];
                despesas.forEach(despesa => {
                    if (despesa.DataDoc) {
                        const data = new Date(despesa.DataDoc);
                        if (data.getFullYear() === ano) {
                            const trimestre = Math.floor(data.getMonth() / 3);
                            trimestres[trimestre]++;
                        }
                    }
                });
                return trimestres;
            })
        );
    }
    
    alterarDocumento(id: string, documento: DocumentoInterno, ficheirosAnexos: File[]): Observable<void> {

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
            this.httpClient.put<void>(`${environment.MainUrl}/api/Internos/Documento/${encodeURIComponent(id)}/`, formData)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on InternosService.alterarDocumento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    criarDocumento(documento: DocumentoInterno, ficheirosAnexos: File[]): Observable<string> {

        if (!documento)
        throw new Error('documento não definido');

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.post<string>(`${environment.MainUrl}/api/Internos/Documento`, formData)
            .subscribe({
                next: (response: string) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on InternosService.createDocumento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }


    getDespesas(): Observable<DocumentoInterno[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoInterno[]>(`${environment.MainUrl}/api/Internos/Despesas`)
            .pipe(
                map((items: DocumentoInterno[]) => {
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
                        next: (response: DocumentoInterno[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on InternosService.getDepesas", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
                });
            }

    getRascunho(Id: string): Observable<DocumentoInterno> {

        if (!Id)
        throw new Error('Id não definido');

        return new Observable((observer) => {
            this.httpClient.get<DocumentoInterno>(`${environment.MainUrl}/api/Internos/Rascunhos/${encodeURIComponent(Id)}`)
            .pipe(
                map((item: DocumentoInterno) => {
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
                    next: (response: DocumentoInterno) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on InternosService.getRascunho", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
            });
        }

    getRascunhosUtilizador(): Observable<DocumentoInterno[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoInterno[]>(`${environment.MainUrl}/api/Internos/Rascunhos/Utilizador`)
            .pipe( map((items: DocumentoInterno[]) => {
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
                        next: (response: DocumentoInterno[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on InternosService.getRascunhos", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    getRascunhosEstado(estado: string): Observable<DocumentoInterno[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoInterno[]>(`${environment.MainUrl}/api/Internos/Rascunhos/${encodeURIComponent(estado)}`)
            .pipe( map((items: DocumentoInterno[]) => {
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
                        next: (response: DocumentoInterno[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on InternosService.getRascunhos", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    getRascunhosAprovacao(): Observable<DocumentoInterno[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoInterno[]>(`${environment.MainUrl}/api/Internos/Rascunhos/Aprovacao`)
            .pipe( map((items: DocumentoInterno[]) => {
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
                        next: (response: DocumentoInterno[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on InternosService.getRascunhosAprovacao", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    getRascunhos(): Observable<DocumentoInterno[]> {

        return new Observable((observer) => {
            this.httpClient.get<DocumentoInterno[]>(`${environment.MainUrl}/api/Internos/Rascunhos`)
            .pipe( map((items: DocumentoInterno[]) => {
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
                        next: (response: DocumentoInterno[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on InternosService.getRascunhos", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
        });
    }

    alterarRascunho( id: string, documento: DocumentoInterno, ficheirosAnexos: File[]): Observable<void> {

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
            this.httpClient.put<void>(`${environment.MainUrl}/api/Internos/Rascunhos/${encodeURIComponent(id)}/`, formData)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on InternosService.alterarRascunho", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    criarRascunho(documento: DocumentoInterno, ficheirosAnexos: File[]): Observable<string> {

        if (!documento)
        throw new Error('documento não definido');

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.post<string>(`${environment.MainUrl}/api/Internos/Rascunhos`, formData)
            .subscribe({
                next: (response: string) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on InternosService.criarRascunho", error, error?.error);
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
            this.httpClient.delete<void>(`${environment.MainUrl}/api/Internos/Rascunhos/${encodeURIComponent(id)}`)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on InternosService.anularRascunho", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getTiposDocumento(): Observable<TipoDocumentosInternos[]> {

        return new Observable((observer) => {
            this.httpClient.get<TipoDocumentosInternos[]>(`${environment.MainUrl}/api/Internos/TiposDocumento/`)
                .subscribe({
                    next: (response: TipoDocumentosInternos[]) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on InternosService.TiposDocumentos", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }

    getSeries(tipoDocumento: string): Observable<SerieInternos[]> {

        if (!tipoDocumento)
            throw new Error('tipoDocumento não definido');

        return new Observable((observer) => {
            this.httpClient.get<SerieInternos[]>(`${environment.MainUrl}/api/Internos/Series/?tipoDocumento=${encodeURIComponent(tipoDocumento)}`)
                .subscribe({
                    next: (response: SerieInternos[]) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on InternosService.getSeries", error, error?.error);
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
            this.httpClient.get<number>(`${environment.MainUrl}/api/Internos/ProximoNumeroSerie/?tipoDocumento=${encodeURIComponent(tipoDocumento)}&serie=${encodeURIComponent(serie)}`)
                .subscribe({
                    next: (response: number) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on InternosService.getProximoNumeroSerie", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
        });
    }
}