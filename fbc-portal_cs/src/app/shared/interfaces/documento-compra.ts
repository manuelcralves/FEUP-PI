import { Anexo } from "./anexo"
import { LinhaDocumentoCompra } from "./linha-documento-compra"

export interface DocumentoCompra {
    Id?: string
    Documento?: string
    TipoDoc: string
    Serie: string
    NumDoc?: number
    DataDoc?: Date
    DataVenc?: Date
    Entidade?: string
    TipoEntidade?: string
    NomeEntidade?: string
    DescFinanceiro?: number
    DescFornecedor?: number
    Estado?: string  
    ObraId?: string
    Obra?: string
    NomeObra?: string
    TotalDocumento?: number  
    Observacoes?: string
    Aprovador?: string
    DataAprovacao?: Date
    MotivoRejeicao?: string
    Linhas: LinhaDocumentoCompra[]
    Utilizador?: string
    Equipa?:string
    Anexos?: Anexo[]
}
