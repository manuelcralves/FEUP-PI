import { ErrorHandler, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule, HAMMER_GESTURE_CONFIG, HammerGestureConfig } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingBarRouterModule } from '@ngx-loading-bar/router';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { RouterModule } from '@angular/router';
import { BlockUIModule } from 'ng-block-ui';
import { ToastrModule } from 'ngx-toastr';

import { registerLocaleData } from "@angular/common";
import localePt from "@angular/common/locales/pt-PT";

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.routing';
import { BlockTemplateComponent } from './shared/components/blockui/block-template.component';
import { SettingsModule } from 'src/app/shared/modules/settings/settings.module';
import { ThemeSettingsConfig } from 'src/app/shared/modules/settings/theme/theme-settings.config';
import { MenuSettingsConfig } from 'src/app/shared/modules/settings/menu/menu-settings.config';
import { AlertComponent } from './shared/components/alert/alert.component';
import { AlertService } from './shared/components/alert/alert.service';
import { NavbarService } from './shared/services/navbar.service';
import { FooterComponent } from './shared/components/layout/footer/footer.component';
import { NavigationComponent } from './shared/components/layout/navigation/navigation.component';
import { HeaderComponent } from './shared/components/layout/header/header.component';
import { VerticalnavComponent } from './shared/components/layout/navigation/verticalnav/verticalnav.component';
import { HorizontalnavComponent as HorizontalNavComponent } from './shared/components/layout/navigation/horizontalnav/horizontalnav.component';
import { PrivateLayoutComponent } from './shared/components/layout/private-layout/private-layout.component';
import { PublicLayoutComponent } from './shared/components/layout/public-layout/public-layout.component';
import { VerticalComponent } from './shared/components/layout/header/vertical/vertical.component';
import { HorizontalComponent } from './shared/components/layout/header/horizontal/horizontal.component';
import { PartialsModule } from './shared/modules/partials/partials.module';
import { MatchHeightModule } from './shared/modules/partials/match-height/match-height.module';
import { ApiAuthInterceptor } from './shared/interceptors/api-auth.interceptor';
import { LoginModalModule } from './shared/modals/login/login-modal.module';
import { UiSwitchModule } from 'ngx-ui-switch';



import { GlobalErrorHandler } from './shared/services/global-error-handler';



registerLocaleData(localePt, "pt-PT");

@NgModule({
    declarations: [
        AppComponent,
        PublicLayoutComponent,
        PrivateLayoutComponent,
        HeaderComponent,
        HorizontalComponent,
        VerticalComponent,
        FooterComponent,
        NavigationComponent,
        AlertComponent,
        VerticalnavComponent,
        HorizontalNavComponent,
        BlockTemplateComponent,
    ],
    entryComponents: [],
    imports: [
        BrowserModule,
        UiSwitchModule,
        AppRoutingModule,
        PartialsModule,
        MatchHeightModule,
        HttpClientModule,
        BrowserAnimationsModule,
        NgbModule,
        FormsModule,
        LoginModalModule,
        ToastrModule.forRoot({ maxOpened: 10 }),
        SettingsModule.forRoot(ThemeSettingsConfig, MenuSettingsConfig),
        PerfectScrollbarModule,
        NgxSpinnerModule,
        LoadingBarRouterModule,
        LoadingBarHttpClientModule,
        BlockUIModule.forRoot({
            template: BlockTemplateComponent
        })
    ],
    providers: [
        {
            provide: HAMMER_GESTURE_CONFIG,
            useClass: HammerGestureConfig
        },
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandler,
        },       
        {
            provide: LocationStrategy, 
            useClass: HashLocationStrategy
        },
        NgbModalConfig,
        AlertService,
        NavbarService,
        { provide: HTTP_INTERCEPTORS, useClass: ApiAuthInterceptor, multi: true },
        { provide: LOCALE_ID, useValue: "pt-PT" },
    ],
    bootstrap: [AppComponent],
    exports: [RouterModule]
})
export class AppModule { }
