import { Leader } from "./leader";
import { Membro } from "./membro";


export interface Equipa {
    Codigo: string;
    Descricao?: string;
    Email?: string;
    Activa?: boolean;
    Membros?: Membro[];
    Leaders?: Leader[]
}
