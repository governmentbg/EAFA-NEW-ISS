import { Environment } from '@env/environment';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private oidcSecurityService: OidcSecurityService;
    private authService: AuthService;

    constructor(private injector: Injector) {
        this.oidcSecurityService = this.injector.get(OidcSecurityService);
        this.authService = this.injector.get(AuthService);
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let requestToForward = req;
        const token = this.getToken();

        const headers = this.getHeaders(token, Environment.Instance.production);

        if (headers != undefined) {
            requestToForward = req.clone({ setHeaders: headers });
        } else {
            requestToForward = req;
        }

        return next.handle(requestToForward).pipe(catchError(error => { return this.errorHandler(error); }));
    }

    private getHeaders(token: string, isProduction: boolean): any {
        let headers: any = undefined;

        if (!isProduction) {
            if (headers == undefined) {
                headers = {};
            }

            headers['Access-Control-Allow-Origin'] = '*';
        }

        if (token != undefined && token != '') {

            if (headers == undefined) {
                headers = {};
            }

            headers['Authorization'] = `Bearer ${token}`;
        }

        if (this.authService.ImpersonationToken != undefined ) {

            if (headers == undefined) {
                headers = {};
            }

            headers['Impersonate'] = this.authService.ImpersonationToken;
        }

        return headers;
    }

    private getToken(): string {
        let token = '';
        if (this.oidcSecurityService !== undefined) {
            token = this.oidcSecurityService.getToken();
        }

        return token;
    }

    private errorHandler(error: HttpErrorResponse): Observable<any> {
        if (error.status === 401) {
            if (this.oidcSecurityService != null && this.oidcSecurityService != undefined) {
                this.oidcSecurityService.authorize();
            }
        }

        return throwError(error);
    }
}