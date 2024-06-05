import { ErrorHandler, Inject, Injectable, Injector } from "@angular/core";
import { ToastrService } from "ngx-toastr";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    constructor(@Inject(Injector) private readonly injector: Injector) { }

    private get toastr() {
        return this.injector.get(ToastrService);
    }

    handleError(error: any) {
        this.toastr.error("Ocorreu um erro inesperado. Pode precisar de refrescar a página.", "", { disableTimeOut: true });

        console.error("Erro não tratado, capturado em GlobalErrorHandler:", error);
    }
}