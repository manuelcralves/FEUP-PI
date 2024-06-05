import { Anexo } from "./anexo";
import { LinhaDocumentoVenda } from "./linha-documento-venda";

export interface DocumentoVenda {
    Id?: string
    CodigoEmpresa?: string
    NomeEmpresa?: string
    Documento?: string
    TipoDoc: string
    Serie: string
    NumDoc?: number
    Data?: Date
    Entidade?: string
    TipoEntidade?: string
    NomeEntidade?: string
    DataInicio?: Date
    DataFim?: Date
    Vendedor?: string
    NomeVendedor?: string
    CondPag?: string
    NomeCondPag?: string
    LimiteCredito?: number
    Credito?: number
    Morada?: string
    CodPostal?: string
    LocalidadeCodPostal?: string
    OutrasInformacoes?: string
    CaracteristicasTecnicas?: string
    Organizacao?: string
    Estado?: string
    Anulado?: boolean
    Fechado?: boolean
    Linhas?: LinhaDocumentoVenda[]
    Anexos?: Anexo[]
    NumContribuinte?: string
    Rascunho?: boolean;
}
