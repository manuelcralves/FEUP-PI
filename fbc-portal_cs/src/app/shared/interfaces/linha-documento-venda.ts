import { DateTime } from 'luxon';
import { Anexo } from './anexo';

export interface LinhaDocumentoVenda {
    Id?: string
    NumLinha?: number
    Artigo?: string
    Descricao?: string
    Quantidade?: number
    DataEntrega?: Date
    Preco?: number
    Desconto?: number;
    Total?: number;
    Unidade?: string
    Observacoes?: string;
    ObraId?:string;
}
