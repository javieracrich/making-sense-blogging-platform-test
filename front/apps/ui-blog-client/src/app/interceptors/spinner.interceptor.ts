import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';

import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { SpinnerService } from '../services/spinner.service';

@Injectable({
  providedIn: 'root'
})
export class SpinnerInterceptor implements HttpInterceptor {

  constructor(private spinnerService: SpinnerService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.spinnerService.display(true);

    return next.handle(request)
      .pipe(
        tap(event => {
          if (event instanceof HttpResponse) {
            this.spinnerService.display(false);
          }
        }, error => {
          // http response status code
          console.error(error.status);
          console.error(error.message);
          this.spinnerService.display(false);
        })
      );
  }
}
