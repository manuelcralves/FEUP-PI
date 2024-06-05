import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { MenuSettingsService } from 'src/app/shared/modules/settings/menu/menu-settings.service';
import { SessionService } from 'src/app/shared/services/session.service';

@Component({
    selector: 'app-login',
    templateUrl: './login-modal.component.html',
    styleUrls: ['./login-modal.component.scss']
})
export class LoginModalComponent implements OnInit {
    @BlockUI() public blockUI: NgBlockUI;
    public loginForm: FormGroup;
    public submitted = false;
    public sendingRequest = false;
    public errorText: string;

    get loginControls() { return this.loginForm.controls; }

    constructor(
        private formBuilder: FormBuilder,
        private sessionService: SessionService,
        public modal: NgbActiveModal,
        private menuSettingsService: MenuSettingsService
    ) {

    }

    ngOnInit(): void {
        this.loginForm = this.formBuilder.group({
            Username: ['', Validators.required],
            Password: ['', Validators.required],
        });

        if (navigator.credentials && typeof PasswordCredential != "undefined" && localStorage["PreventAutoLogin"] !== "1") {
            navigator.credentials
                .get({
                    password: true,
                    mediation: "silent",
                })
                .then((passwordCred: PasswordCredential) => {
                    if (passwordCred) {
                        this.loginControls.Username.setValue(passwordCred.id);
                        this.loginControls.Password.setValue(passwordCred.password);

                        this.tryLogin();
                    }
                })
                .catch((error) => {
                    console.error("credentials.get error", error);
                });
        }
    }

    tryLogin() {
        this.submitted = true;
        this.sendingRequest = true;
        this.errorText = "";

        this.loginForm.markAllAsTouched();

        // stop here if form is invalid
        if (this.loginForm.invalid) {
            this.sendingRequest = false;
            return;
        }

        this.blockUI.start('A carregar...');
        this.sessionService.login(this.loginControls.Username.value, this.loginControls.Password.value).subscribe({
            next: (value: boolean) => {
                this.sendingRequest = false;
                this.blockUI.stop();

                if (value) {
                    localStorage.removeItem("PreventAutoLogin");

                    if (navigator.credentials && typeof PasswordCredential != "undefined") {
                        // guardar/memorizar credenciais em browser
                        navigator.credentials
                            .create({
                                password: {
                                    id: this.loginControls.Username.value,
                                    password: this.loginControls.Password.value,
                                    name: "Portal",
                                },
                            })
                            .then((cred: CredentialType) => {
                                if (cred) {
                                    navigator.credentials.store(cred).catch((error) => {
                                        console.error("store error", error);
                                    });
                                }
                            })
                            .catch((error) => {
                                console.error("create credential error", error);
                            });
                    }

                    this.submitted = false;
                    this.loginForm.reset();

                    this.menuSettingsService.processarPermissoes();

                    this.modal.close("login");
                } else {
                    console.error("unexpected login result");
                }
            },
            error: (error: any) => {
                this.sendingRequest = false;
                this.blockUI.stop();

                if (error?.status === 400) {
                    if (error instanceof HttpErrorResponse) {
                        let httpErrorResponse: HttpErrorResponse = error;

                        if (httpErrorResponse.error) {
                            if (httpErrorResponse.error.error === "invalid_grant") {
                                this.errorText = "Utilizador ou password incorretos.";
                            }
                            else if (httpErrorResponse.error.error === "access_denied") {
                                this.errorText = "Não tem permissões de acesso.";
                            }
                            else if (httpErrorResponse.error.error === "invalid_request") {
                                this.errorText = "Pedido inválido, ClientId não identificado. Se o problema continuar a acontecer contacte um técnico.";
                            }
                            else if (httpErrorResponse.error.error === "unauthorized_client") {
                                this.errorText = "Pedido inválido, ClientId não autorizado. Se o problema continuar a acontecer contacte um técnico.";
                            }
                            else if (httpErrorResponse.error.error === "server_error") {
                                this.errorText = "Ocorreu um erro inesperado no servidor. Se o problema continuar a acontecer contacte um técnico.";
                            }
                            else if (httpErrorResponse.error.error === "unsupported_grant_type") {
                                this.errorText = "Pedido inválido, tipo de pedido não suportado. Se o problema continuar a acontecer contacte um técnico.";
                            }
                            else {
                                console.error("servidor respondeu que pedido de login é inválido, mas 'httpErrorResponse.error.error' não é conhecido", error);
                                this.errorText = "Pedido inválido. Se o problema continuar a acontecer contacte um técnico.";
                            }
                        }
                        else {
                            console.error("servidor respondeu que pedido de login é inválido, mas 'httpErrorResponse.error' está vazio", error);
                            this.errorText = "Pedido inválido. Se o problema continuar a acontecer contacte um técnico.";
                        }
                    }
                    else {
                        console.error("servidor respondeu que pedido de login é inválido, mas tipo de resposta é desconhecida", error);
                        this.errorText = "Pedido inválido. Se o problema continuar a acontecer contacte um técnico.";
                    }
                }
                else if (error?.status === 0) {
                    this.errorText = "Não foi possível ligar ao servidor.";
                }
                else {
                    this.errorText = "Tente novamente. Se o problema continuar a acontecer contacte um técnico.";
                }
            }
        });
    }
}
