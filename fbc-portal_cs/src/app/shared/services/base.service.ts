import { MoradaFornecedor } from './../interfaces/moradaFornecedor';
import { Unidade } from './../interfaces/unidade';
import { MoradaArmazem } from './../interfaces/morada-armazem';
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { Empresa } from "../interfaces/empresa";
import { CondicaoPagamento } from "../interfaces/condicao-pagamento";
import { Cliente } from "../interfaces/cliente";
import { Artigo } from "../interfaces/artigo";
import { MoradaAlternativa } from "../interfaces/morada-alternativa";
import { Fornecedor } from "../interfaces/fornecedor";
import { Utilizador } from "../interfaces/utilizador";
import { Obra } from '../interfaces/obra';
import { Equipa } from '../interfaces/equipa';
import { Membro } from '../interfaces/membro';

@Injectable({
    providedIn: "root",
})
export class BaseService {
    constructor(private httpClient: HttpClient) { }


    getEmpresas(): Observable<Empresa[]> {
        return new Observable((observer) => {
            this.httpClient.get<Empresa[]>(`${environment.MainUrl}/api/Base/Empresas/`).subscribe({
                next: (response: Empresa[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getEmpresas", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getEquipasUtilizador(): Observable<Equipa[]> {
        return new Observable((observer) => {
            this.httpClient.get<Equipa[]>(`${environment.MainUrl}/api/Base/Equipas/Utilizador`).subscribe({
                next: (response: Equipa[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getEquipasUtilizador", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getEquipas(): Observable<Equipa[]> {
        return new Observable((observer) => {
            this.httpClient.get<Equipa[]>(`${environment.MainUrl}/api/Base/Equipas/`).subscribe({
                next: (response: Equipa[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getEquipas", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getEquipa(codEquipa: string): Observable<Equipa> {
        return new Observable((observer) => {
            this.httpClient.get<Equipa>(`${environment.MainUrl}/api/Base/Equipas/${encodeURIComponent(codEquipa)}/`).subscribe({
                next: (response: Equipa) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getEquipa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    criarEquipa(equipa: Equipa): Observable<void> {

        if (!equipa)
        throw new Error('equipa não definida');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Base/Equipa`, equipa)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.criarEquipa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    alterarEquipa(equipa: Equipa): Observable<void> {

        if (!equipa)
        throw new Error('equipa não definida');

        return new Observable((observer) => {
            this.httpClient.put<void>(`${environment.MainUrl}/api/Base/Equipa`, equipa)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.alterarEquipa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    deleteEquipa(id: string): Observable<void> {

        if (!id)
        throw new Error('id não definido');

        debugger

        return new Observable((observer) => {
            this.httpClient.delete<void>(`${environment.MainUrl}/api/Base/Equipas/${encodeURIComponent(id)}`)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.deleteEquipa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    addMembroEquipa(membro: Membro): Observable<void> {

        if (!membro)
        throw new Error('membro não definido');

        return new Observable((observer) => {
            this.httpClient.post<void>(`${environment.MainUrl}/api/Base/Equipa/Membro`, membro)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.addMembroEquipa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    deleteMembroEquipa(membro: string, equipa: string): Observable<void> {

        if (!membro)
        throw new Error('membro não definido');

        if (!equipa)
        throw new Error('equipa não definida');

        return new Observable((observer) => {
            this.httpClient.delete<void>(`${environment.MainUrl}/api/Base/Equipa/Membro/${encodeURIComponent(membro)}/${encodeURIComponent(equipa)}/`)
            .subscribe({
                next: (response: void) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.deleteMembroEquipa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }


    getMoradasAlternativas(cliente: string): Observable<MoradaAlternativa[]> {

        if (!cliente)
        throw new Error('cliente não definido');

        return new Observable((observer) => {
            this.httpClient.get<MoradaAlternativa[]>(`${environment.MainUrl}/api/Base/MoradasAlternativas/${encodeURIComponent(cliente)}/`)
            .subscribe({
                next: (response: MoradaAlternativa[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradasAlternativas", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getMoradasFornecedores(codFornecedor: string): Observable<MoradaFornecedor[]>  {


        if (!codFornecedor)
        throw new Error('codFornecedor não definido');

        return new Observable((observer) => {
            this.httpClient.get<MoradaFornecedor[]>(`${environment.MainUrl}/api/Base/MoradasFornecedores/${encodeURIComponent(codFornecedor)}/`)
            .subscribe({
                next: (response: MoradaFornecedor[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradasFornecedores", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getMoradasArmazens(): Observable<MoradaArmazem[]> {

        return new Observable((observer) => {
            this.httpClient.get<MoradaArmazem[]>(`${environment.MainUrl}/api/Base/MoradasArmazens`)
            .subscribe({
                next: (response: MoradaArmazem[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradasArmazens", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getArtigosVolumetria(volumetria: string): Observable<Artigo[]> {

        return new Observable((observer) => {
            this.httpClient.get<Artigo[]>(`${environment.MainUrl}/api/Base/ArtigosPorVolumetria/${encodeURIComponent(volumetria)}`)
            .subscribe({
                next: (response: Artigo[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradasArmazens", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getMoradaFornecedor(fornecedor: string, codMorada: string): Observable<MoradaFornecedor> {

        if (!fornecedor)
        throw new Error('fornecedor não definido');

        if (!codMorada)
        throw new Error('codMorada não definido');

        return new Observable((observer) => {
            this.httpClient.get<MoradaFornecedor>(`${environment.MainUrl}/api/Base/MoradasFornecedores/${encodeURIComponent(fornecedor)}/${encodeURIComponent(codMorada)}/`)
            .subscribe({
                next: (response: MoradaFornecedor) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradaFornecedor", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getMoradaAlternativa(cliente: string, codMorada: string): Observable<MoradaAlternativa> {

        if (!cliente)
        throw new Error('cliente não definido');

        if (!codMorada)
        throw new Error('codMorada não definido');

        return new Observable((observer) => {
            this.httpClient.get<MoradaAlternativa>(`${environment.MainUrl}/api/Base/MoradasAlternativas/${encodeURIComponent(cliente)}/${encodeURIComponent(codMorada)}/`)
            .subscribe({
                next: (response: MoradaAlternativa) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradaAlternativa", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getMoradaArmazem(codMorada: string): Observable<MoradaArmazem> {

        if (!codMorada)
        throw new Error('codMorada não definido');

        return new Observable((observer) => {
            this.httpClient.get<MoradaArmazem>(`${environment.MainUrl}/api/Base/MoradasArmazens/${encodeURIComponent(codMorada)}/`)
            .subscribe({
                next: (response: MoradaArmazem) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getMoradaArmazem", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getCondicoesPagamento(): Observable<CondicaoPagamento[]> {

        return new Observable((observer) => {
            this.httpClient.get<CondicaoPagamento[]>(`${environment.MainUrl}/api/Base/CondicoesPagamento/`)
            .subscribe({
                next: (response: CondicaoPagamento[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getCondicoesPagamento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getCondicaoPagamento(codCondPag: string): Observable<CondicaoPagamento> {

        if (!codCondPag)
        throw new Error('codCondPag não definido');

        return new Observable((observer) => {
            this.httpClient.get<CondicaoPagamento>(`${environment.MainUrl}/api/Base/CondicoesPagamento/${encodeURIComponent(codCondPag)}/`)
            .subscribe({
                next: (response: CondicaoPagamento) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getCondicaoPagamento", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getUnidades(): Observable<Unidade[]> {


        return new Observable((observer) => {
            this.httpClient.get<Unidade[]>(`${environment.MainUrl}/api/Base/Unidades/`)
            .subscribe({
                next: (response: Unidade[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getUnidades", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getUnidade( codUnidade: string): Observable<Unidade> {


        if (!codUnidade)
        throw new Error('codUnidade não definido');

        return new Observable((observer) => {
            this.httpClient.get<Unidade>(`${environment.MainUrl}/api/Base/Unidades/${encodeURIComponent(codUnidade)}/`)
            .subscribe({
                next: (response: Unidade) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getUnidade", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getClientes(): Observable<Cliente[]> {


        return new Observable((observer) => {
            this.httpClient.get<Cliente[]>(`${environment.MainUrl}/api/Base/Clientes/`)
            .subscribe({
                next: (response: Cliente[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getClientes", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getCliente(codCliente: string): Observable<Cliente> {


        if (!codCliente)
        throw new Error('codCliente não definido');

        return new Observable((observer) => {
            this.httpClient.get<Cliente>(`${environment.MainUrl}/api/Base/Clientes/${encodeURIComponent(codCliente)}/`)
            .subscribe({
                next: (response: Cliente) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getCliente", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getFornecedores(): Observable<Fornecedor[]> {


        return new Observable((observer) => {
            this.httpClient.get<Fornecedor[]>(`${environment.MainUrl}/api/Base/Fornecedores/`)
            .subscribe({
                next: (response: Fornecedor[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getFornecedores", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getFornecedor(codFornecedor: string): Observable<Fornecedor> {

        if (!codFornecedor)
        throw new Error('codFornecedor não definido');

        return new Observable((observer) => {
            this.httpClient.get<Fornecedor>(`${environment.MainUrl}/api/Base/Fornecedores/${encodeURIComponent(codFornecedor)}/`)
            .subscribe({
                next: (response: Fornecedor) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getFornecedor", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getArtigos(): Observable<Artigo[]> {


        return new Observable((observer) => {
            this.httpClient.get<Artigo[]>(`${environment.MainUrl}/api/Base/Artigos/`)
            .subscribe({
                next: (response: Artigo[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getArtigos", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getArtigo(codArtigo: string): Observable<Artigo> {


        if (!codArtigo)
        throw new Error('codArtigo não definido');

        return new Observable((observer) => {
            this.httpClient.get<Artigo>(`${environment.MainUrl}/api/Base/Artigos/${encodeURIComponent(codArtigo)}/`)
            .subscribe({
                next: (response: Artigo) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getArtigo", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getObra(obra: string): Observable<Obra> {

        if (!obra)
        throw new Error('Obra não definida');

        return new Observable((observer) => {
            this.httpClient.get<Obra>(`${environment.MainUrl}/api/Base/Obras/${encodeURIComponent(obra)}/`)
            .subscribe({
                next: (response: Obra) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getObra", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

    getObras(): Observable<Obra[]> {

        return new Observable((observer) => {
            this.httpClient.get<Obra[]>(`${environment.MainUrl}/api/Base/Obras/`)
            .subscribe({
                next: (response: Obra[]) => {
                    observer.next(response);
                },
                error: (error: any) => {
                    console.error("error on BaseService.getObras", error, error?.error);
                    observer.error(error);
                },
                complete: () => {
                    observer.complete();
                }
            });
        });
    }

}
