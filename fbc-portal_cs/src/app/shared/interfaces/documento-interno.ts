import { Anexo } from "./anexo";
import { LinhaDocumentoInterno } from "./linha-documento-interno";

export interface DocumentoInterno {
    Id?: string
    Documento?: string
    TipoDoc: string
    Serie: string
    NumDoc?: number
    Entidade?: string
    TipoEntidade?: string
    NomeEntidade?: string
    Estado?: string
    DataDoc?: Date
    DataVenc?: Date
    DescFinanceiro?: number
    DescFornecedor?: number
    ObraId?: string
    Obra?: string
    NomeObra?: string
    TotalDocumento?: number
    Aprovador?: string
    DataAprovacao?: Date
    MotivoRejeicao?: string
    Observacoes?: string
    Linhas: LinhaDocumentoInterno[]
    Equipa?:string
    Utilizador?: string
    Anexos?: Anexo[]
}
