import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment, apiurl } from '../environments/environment';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { RoutingModule } from './routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { JwtModule } from '@auth0/angular-jwt';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';
import { AuthorizationInterceptor } from './interceptors/auth.interceptor';
import { SpinnerInterceptor } from './interceptors/spinner.interceptor';
export function nullTokenGetter() {
  return '';
}

@NgModule({
  declarations: [AppComponent, LoginComponent, HomeComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    HttpClientModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production
    }),
    JwtModule.forRoot({
      config: {
        tokenGetter: nullTokenGetter,
        // do not add any Authorization header. we use the specific http interceptors for that
        blacklistedRoutes: ['localhost:4200', 'localhost:4300']
      }
    }),
    RoutingModule
  ],
  providers: [
    {
      provide: 'apiurl',
      useValue: apiurl,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UnauthorizedInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizationInterceptor,
      multi: true
    },
    { provide: HTTP_INTERCEPTORS, useClass: SpinnerInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
