import { DateTime } from 'luxon';
export interface Anexo {
    Id: string
    FicheiroOrig: string
    TamanhoBytes: number
    Descricao: string
    DocumentoMotorista: boolean
    NumDocumentoMotorista: string
    DescricaoDocumento: string
    Data: Date
    Assinatura: File
    FicheiroAssinatura: string;
}
