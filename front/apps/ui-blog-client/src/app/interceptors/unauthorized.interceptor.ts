import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

@Injectable({
  providedIn: 'root'
})
export class UnauthorizedInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private toastService: ToastService,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError(err => {
        switch (err.status) {
          case 401: {
            // auto logout if 401 response returned from api
            this.authService.logout();
            location.reload();
            return next.handle(request);
          }
          case 400: {
            this.toastService.error(JSON.stringify(err.error));
            return next.handle(request);
          }
          case 404: {
            this.toastService.error(JSON.stringify(err.error));
            return next.handle(request);
          }
          default:
            return throwError(err);
        }
      })
    );
  }
}
