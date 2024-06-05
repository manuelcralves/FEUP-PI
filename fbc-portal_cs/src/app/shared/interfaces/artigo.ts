import { Unidade } from './unidade';

export interface Artigo {
    Codigo: string;
    Descricao?: string;
    CodBarras?: string;
    Marca?: string;
    Modelo?: string;
    UnidadeVenda?: string;
    PVP1?: number;
}
