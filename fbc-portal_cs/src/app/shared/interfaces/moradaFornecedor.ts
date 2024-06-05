import { BaseMorada } from "./base-morada"

export interface MoradaFornecedor extends BaseMorada  {
    CodMoradaAlternativa: string
    CodigoAPA?: string
}
