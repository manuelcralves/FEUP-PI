export interface TokenObject {
    access_token: string;
    token_type: string;
    expires_in: number;
    nomeUtilizador: string;
    codigoUtilizador?: string;
    adminUtilizador?: string;
    ".issued"?: string;
    ".expires"?: string;
}
