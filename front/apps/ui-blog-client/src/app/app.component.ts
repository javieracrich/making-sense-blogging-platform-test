import { TokenService } from './services/token.service';
import { Component, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { environment } from '../environments/environment';
import { Subscription } from 'rxjs';
import { Router, NavigationStart } from '@angular/router';
import { SwUpdate } from '@angular/service-worker';
import { ToastService } from './services/toast.service';
import { AuthService } from './services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { SpinnerService } from './services/spinner.service';

@Component({
  selector: 'blogging-client-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'home';
  production = environment.production;
  private subscriptions: Subscription[] = [];
  message: any;

  public username = '';
  public showLoader = false;

  constructor(
    private tokenService: TokenService,
    private authService: AuthService,
    private toastService: ToastService,
    private helper: JwtHelperService,
    private router: Router,
    private cdRef: ChangeDetectorRef,
    private spinnerService: SpinnerService,
    updates: SwUpdate
  ) {
    updates.available.subscribe(ev => {
      updates.activateUpdate().then(() => document.location.reload());
    });

    router.events.subscribe(evt => {
      if (evt instanceof NavigationStart) {
        // -----------------------
        // ROUTER INTERCEPTOR
        // -----------------------
      }
    });
  }

  ngOnInit() {

    this.subscriptions.push(
      this.toastService.getMessage().subscribe(message => {
        this.message = message;
      })
    );
    this.username = this.tokenService.getUsername();
    this.subscriptions.push(
      this.spinnerService.status.subscribe((val: boolean) => {
        this.showLoader = val;
        this.cdRef.detectChanges();
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  userIsLoggedIn() {
    const token = this.tokenService.getToken();
    if (token == null || this.helper.isTokenExpired(token)) {
      return false;
    }
    return true;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  removeMessage() {
    this.message = null;
  }
}
