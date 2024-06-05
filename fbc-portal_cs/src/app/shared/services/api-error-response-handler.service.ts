import { Injectable } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";
import { environment } from "src/environments/environment";
import { SessionService } from "./session.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { LoginModalComponent } from "../modals/login/login-modal.component";

@Injectable({
    providedIn: "root",
})
export class ApiErrorResponseHandlerService {
    public isLoginModalShown: boolean;

    constructor(
        private toastr: ToastrService,
        private sessionService: SessionService,
        private modalService: NgbModal
    ) {
        this.isLoginModalShown = false;
    }

    handleError(error: any, errorTitle?: string, suppressErrorMessages: boolean = false, suppressLogin: boolean = false): boolean {
        let errorHandled = false;

        errorTitle = errorTitle ?? "";

        if (typeof error == "object" && error instanceof HttpErrorResponse) {
            let httpErrorResponse: HttpErrorResponse = error;

            if (httpErrorResponse.url.startsWith(environment.MainUrl)) {
                if (httpErrorResponse.status === 401) {
                    this.sessionService.logout();

                    if (!suppressErrorMessages) {
                        this.toastr.error("Sessão inválida. Precisa de fazer login novamente.", errorTitle, { timeOut: 4000 + 35 * errorTitle.length });

                        errorHandled = true;
                    }

                    if (!suppressLogin) {
                        if (!this.isLoginModalShown) {
                            this.isLoginModalShown = true;

                            try {
                                const modalRef = this.modalService.open(LoginModalComponent, { windowClass: 'animated fadeInDown', centered: true });

                                modalRef
                                    .shown
                                    .subscribe({
                                        next: () => {
                                            modalRef
                                                .result
                                                .then((value: string) => {
                                                    this.isLoginModalShown = false;
                                                })
                                                .catch((reason: string) => {
                                                    this.isLoginModalShown = false;
                                                });
                                        },
                                        error: () => {
                                            this.isLoginModalShown = false;
                                        }
                                    });
                            }
                            catch (ex) {
                                this.isLoginModalShown = false;
                                console.error("erro a mostrar modal de login", ex);
                                throw ex;
                            }
                        }

                        errorHandled = true;
                    }
                } else if (httpErrorResponse.status === 404) {
                    if (!suppressErrorMessages) {
                        this.toastr.error("O recurso não foi encontrado no servidor. Se o problema continuar a acontecer contacte um técnico.", errorTitle, { timeOut: 6000 + 35 * errorTitle.length });

                        errorHandled = true;
                    }
                } else if (httpErrorResponse.status === 400) {
                    if (!suppressErrorMessages) {
                        if (typeof httpErrorResponse.error == "object" && "ErrorMessage" in httpErrorResponse.error) {
                            let errorMessage: string = httpErrorResponse.error.ErrorMessage;

                            if (errorMessage) {
                                this.toastr.error(errorMessage, errorTitle, { timeOut: 3500 + 35 * errorMessage.length + 35 * errorTitle.length });

                                errorHandled = true;
                            }
                        }

                        if (!errorHandled) {
                            this.toastr.error("O servidor respondeu que o pedido é inválido. Se o problema continuar a acontecer contacte um técnico.", errorTitle, { timeOut: 6000 + 35 * errorTitle.length });

                            errorHandled = true;
                        }
                    }
                } else if (httpErrorResponse.status === 500) {
                    if (!suppressErrorMessages) {
                        if (typeof httpErrorResponse.error == "object" && "ErrorMessage" in httpErrorResponse.error) {
                            let errorMessage: string = httpErrorResponse.error.ErrorMessage;

                            if (errorMessage) {
                                this.toastr.error(errorMessage, errorTitle, { timeOut: 3500 + 35 * errorMessage.length + 35 * errorTitle.length });

                                errorHandled = true;
                            }
                        }

                        if (!errorHandled) {
                            this.toastr.error("Aconteceu um erro desconhecido no servidor. Se o problema continuar a acontecer contacte um técnico.", errorTitle, { timeOut: 6000 + 35 * errorTitle.length });

                            errorHandled = true;
                        }
                    }
                } else if (httpErrorResponse.status === 0) {
                    if (!suppressErrorMessages) {
                        this.toastr.error("Não foi possível ligar ao servidor.", errorTitle, { timeOut: 4000 + 35 * errorTitle.length });

                        errorHandled = true;
                    }
                }
            }
        }

        if (!errorHandled && !suppressErrorMessages) {
            this.toastr.error("Aconteceu um erro desconhecido. Se o problema continuar a acontecer contacte um técnico.", errorTitle, { timeOut: 6000 + 35 * errorTitle.length });
        }

        return errorHandled;
    }
}
