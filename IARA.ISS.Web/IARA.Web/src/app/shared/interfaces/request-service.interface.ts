import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IRequestServiceParams } from '../services/request-service-params.interface';

export interface IRequestService {
    newTokenSent: Observable<string>;
    errorEvent: Observable<HttpErrorResponse>;
    get<TResult>(baseRoute: string, controller: string, service: string, params?: IRequestServiceParams): Observable<TResult>;
    post<TResult, TBody>(baseRoute: string, controller: string, service: string, body?: TBody, params?: IRequestServiceParams): Observable<TResult>;
    put<TResult, TBody>(baseRoute: string, controller: string, service: string, body?: TBody, params?: IRequestServiceParams): Observable<TResult>;
    patch<TResult, TBody>(baseRoute: string, controller: string, service: string, body?: TBody, params?: IRequestServiceParams): Observable<TResult>;
    delete<TResult>(baseRoute: string, controller: string, service: string, params?: IRequestServiceParams): Observable<TResult>;
    download(baseRoute: string, controller: string, service: string, fileName: string, params?: IRequestServiceParams): Observable<boolean>;
    downloadPost<TBody>(baseRoute: string, controller: string, service: string, fileName: string, body: TBody, params?: IRequestServiceParams): Observable<boolean>;
}