import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { Environment } from '@env/environment';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private securityService: ISecurityService;

    constructor(private injector: Injector) {
        this.securityService = this.injector.get(SECURITY_SERVICE_TOKEN);
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

        return headers;
    }

    private getToken(): string {
        let token = '';
        if (this.securityService !== undefined) {
            token = this.securityService.token ?? '';
        }

        return token;
    }

    private errorHandler(error: HttpErrorResponse): Observable<any> {
        if (error.status === 401) {
            if (this.securityService != null && this.securityService != undefined) {
                this.securityService.authorize();
            }
        }

        return throwError(error);
    }
}