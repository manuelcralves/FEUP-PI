import { PermissaoPerfilPortal } from "./permissao-perfil-portal"

export interface PerfilPortal {
    Codigo: string
    Descricao?: string
    Ativo?: boolean
    Permissoes?: PermissaoPerfilPortal[]
}
