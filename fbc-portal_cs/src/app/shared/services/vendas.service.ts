import { Artigo } from './../interfaces/artigo';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { catchError, map, Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { TipoDocumentosVendas } from "../interfaces/tipo-documentos-vendas";
import { SerieVendas } from "../interfaces/serie-vendas";
import { DocumentoVenda } from "../interfaces/documento-venda";
import { format } from "date-fns";
import { Anexo } from "../interfaces/anexo";
import { LinhaDocumentoVenda } from '../interfaces/linha-documento-venda';
import { EstornoDocumento } from '../interfaces/estorno-documento';

@Injectable({
    providedIn: "root",
})
export class VendasService {
    constructor(private httpClient: HttpClient) { }

    getTiposDocumento( tipoTipoDocument?: number): Observable<TipoDocumentosVendas[]> {

        return new Observable((observer) => {
            this.httpClient.get<TipoDocumentosVendas[]>(`${environment.MainUrl}/api/Vendas/TiposDocumento/?tipoTipoDocumento=${tipoTipoDocument ?? ""}`)
            .subscribe({
                next: (response: TipoDocumentosVendas[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.getTiposDocumentoProposta", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getSeries( tipoDocumento: string): Observable<SerieVendas[]> {

        if (!tipoDocumento)
        throw new Error('tipoDocumento não definido');

        return new Observable((observer) => {
            this.httpClient.get<SerieVendas[]>(`${environment.MainUrl}/api/Vendas/Series/?tipoDocumento=${encodeURIComponent(tipoDocumento)}`)
            .subscribe({
                next: (response: SerieVendas[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.getSeriesPropostas", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getTiposDocumentoEncomendaCliente(codEmpresa: string): Observable<TipoDocumentosVendas[]> {


        return new Observable((observer) => {
            this.httpClient.get<TipoDocumentosVendas[]>(`${environment.MainUrl}/api/Vendas/TiposDocumentoEncomendaCliente/`)
            .subscribe({
                next: (response: TipoDocumentosVendas[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.getTiposDocumentoEncomendaCliente", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }


    getPropostasAbertas(menu: string): Observable<DocumentoVenda[]> {
        return new Observable((observer) => {
            this.httpClient.get<DocumentoVenda[]>(`${environment.MainUrl}/api/Vendas/PropostasAbertas/${menu}`)
            .pipe(
                map((items: DocumentoVenda[]) => {
                    return items.map(
                        item => {
                            if (item.Data)
                            item.Data = new Date(item.Data);
                            return item;
                        });
                    }),
                    catchError((err, caught) => {
                        throw err;
                    })
                    )
                    .subscribe({
                        next: (response: DocumentoVenda[]) => {
                            observer.next(response);
                        },
                        error: (error: any) => {
                            console.error("error on VendasService.getDocumentos", error, error?.error);
                            observer.error(error);
                        },
                        complete: () => {
                            observer.complete();
                        }
                    });
                });
    }

    getPropostaAberta( propostaId: string): Observable<DocumentoVenda> {


        if (!propostaId)
        throw new Error('propostaId não definido');

        return new Observable((observer) => {
            this.httpClient.get<DocumentoVenda>(`${environment.MainUrl}/api/Vendas/PropostasAbertas/${encodeURIComponent(propostaId)}`)
            .pipe(
                map((item: DocumentoVenda) => {
                    if (item) {
                        if (item.Data)
                        item.Data = new Date(item.Data);
                        if (item.DataInicio)
                        item.DataInicio = new Date(item.DataInicio);
                        if (item.DataFim)
                        item.DataFim = new Date(item.DataFim);      
                    }
                    return item;
                }),
                catchError((err, caught) => {
                    throw err;
                })
                )
                .subscribe({
                    next: (response: DocumentoVenda) => {
                        observer.next(response);
                    },
                    error: (error: any) => {
                        console.error("error on VendasService.getDocumento", error, error?.error);
                        observer.error(error);
                    },
                    complete: () => {
                        observer.complete();
                    }
                });
            });
     }

    anularPropostaAberta( id: string, dadosEstorno: EstornoDocumento): Observable<void> {


        if (!id)
        throw new Error('id não definido');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Vendas/AnularPropostaAberta/${encodeURIComponent(id)}`, dadosEstorno)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.anularPropostaAberta", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    alterarPropostaAberta( id: string, documento: DocumentoVenda, ficheirosAnexos: File[]): Observable<void> {


        if (!id)
        throw new Error('id não definido');

        if (!documento)
        throw new Error('documento não definido');

        // função replacer para formatar datas sem hora, zona horária ou alterações de zona horaria ao formatar em JSON
        let jsonReplacer = function (key: string, value: any) {
            if (this[key] instanceof Date)
            return format(this[key], "yyyy-MM-dd");

            return value;
        }

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento, jsonReplacer));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Vendas/PropostasAbertas/${encodeURIComponent(id)}/`, formData)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.alterarPropostaAberta", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    criarPropostaAberta( documento: DocumentoVenda, ficheirosAnexos: File[]): Observable<string> {


        if (!documento)
        throw new Error('documento não definido');

        // função replacer para formatar datas sem hora, zona horária ou alterações de zona horaria ao formatar em JSON
        let jsonReplacer = function (key: string, value: any) {
            if (this[key] instanceof Date)
            return format(this[key], "yyyy-MM-dd");

            return value;
        }

        const formData = new FormData();
        formData.append('documento', JSON.stringify(documento, jsonReplacer));

        ficheirosAnexos.forEach(ficheiro => {
            formData.append('ficheirosAnexos', ficheiro);
        });

        return new Observable((observer) => {
            this.httpClient.post<string>(`${environment.MainUrl}/api/Vendas/PropostasAbertas/`, formData)
            .subscribe({
                next: (response: string) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.createDocumento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getTokenDownloadAnexoProposta( idAnexo: string): Observable<string> {


        if (!idAnexo)
        throw new Error('idAnexo não definido');

        return new Observable((observer) => {
            this.httpClient.get<string>(`${environment.MainUrl}/api/Vendas/GerarTokenDownloadAnexoProposta/${encodeURIComponent(idAnexo)}/`)
            .subscribe({
                next: (response: string) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.getTokenDownloadAnexoProposta", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getTokenDownloadPdfProposta( idDocumento: string, report: string, nrVias: number): Observable<string[]> {
        return new Observable((observer) => {

            if (!idDocumento) {
                console.error("error on VendasService.getTokenDownloadPdfProposta", "idDocumento não definido");
                observer.error("idDocumento não definido");
            }

            if (!nrVias) {
                console.error("error on VendasService.getTokenDownloadPdfProposta", "via não definido");
                observer.error("via não definido");
            }

            this.httpClient.get<string[]>(`${environment.MainUrl}/api/Vendas/GerarTokenDownloadPdfProposta/${encodeURIComponent(idDocumento)}?report=${encodeURIComponent(report)}&nrVias=${encodeURIComponent(nrVias)}`)
            .subscribe({
                next: (response: string[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.getTokenDownloadPdfProposta", error, error?.error);
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
            this.httpClient.get<number>(`${environment.MainUrl}/api/Vendas/ProximoNumeroSerie/?tipoDocumento=${encodeURIComponent(tipoDocumento)}&serie=${encodeURIComponent(serie)}`)
            .subscribe({
                next: (response: number) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on VendasService.getProximoNumeroSerie", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

}
