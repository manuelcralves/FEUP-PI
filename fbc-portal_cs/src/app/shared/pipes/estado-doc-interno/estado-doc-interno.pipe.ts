import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "estadoDocInterno",
})
export class EstadoDocInternoPipe implements PipeTransform {
    transform(estado: string): string {
        if (estado === "P")
            return "Pendente";
        else if (estado === "B")
            return "Aberto";
        else if (estado === "A")
            return "Aprovado";
        else if (estado === "C")
            return "Copiado";
        else if (estado === "N")
            return "Anulado";
        else if (estado === "F")
            return "Fechado";
        else if (estado === "E")
            return "Em Aprovação";
        else if (estado === "R")
            return "Rejeitado";

        return estado;
    }
}
