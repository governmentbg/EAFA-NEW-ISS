import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { error } from '@angular/compiler/src/util';
import { EventEmitter, Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { ErrorModel } from '@app/models/common/exception.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DateUtils } from '@app/shared/utils/date.utils';
import { Environment } from '@env/environment';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ErrorSnackbarComponent } from '../components/error-snackbar/error-snackbar.component';
import { SpinnerService } from '../components/spinner/spinner.service';
import { AreaTypes } from '../enums/area-type.enum';
import { IHttpOptions } from './http-options.interface';
import { RequestProperties } from './request-properties';
import { IRequestServiceParams } from './request-service-params.interface';

@Injectable({
    providedIn: 'root'
})
export class RequestService {
    public http: HttpClient;
    public snackbar: MatSnackBar;
    public translator: FuseTranslationLoaderService;
    public spinner: SpinnerService;

    private get baseUrl(): string {
        return Environment.Instance.apiBaseUrl as string;
    }

    public constructor(http: HttpClient, snackbar: MatSnackBar, translator: FuseTranslationLoaderService, spinner: SpinnerService) {
        this.http = http;
        this.snackbar = snackbar;
        this.translator = translator;
        this.spinner = spinner;
        Date.prototype.toJSON = function () {
            return DateUtils.ToDateTimeString((this as Date));
        }

        //Number.prototype.toString = function (radix?: number | undefined) {
        //    return this.toString();
        //}
    }

    public get<TResult>(area: AreaTypes, controller: string, service: string, params?: IRequestServiceParams): Observable<TResult> {
        const url: string = this.buildUrl(area, controller, service);
        const result = this.initializeRequest(params);

        switch (result.requestParams.responseType) {
            case 'json': {
                const observable = this.http.get<TResult>(url, {
                    params: result.httpOptions.params,
                    headers: result.httpOptions.headers,
                    responseType: 'json'
                });
                return this.pipeResult(observable, params);
            }
            case 'text': {
                const observable = this.http.get(url, {
                    params: result.httpOptions.params,
                    headers: result.httpOptions.headers,
                    responseType: 'text'
                });
                return this.pipeResult(observable, params) as unknown as Observable<TResult>;
            }
            default:
                Observable.throw('');
                break;
        }

        return Observable.create();
    }

    public post<TResult, TBody>(area: AreaTypes, controller: string, service: string, body?: TBody, params?: IRequestServiceParams): Observable<TResult> {
        const url: string = this.buildUrl(area, controller, service);
        const result = this.initializeRequest(params);

        const observable = this.http.post<TResult>(
            url,
            params?.properties?.asFormData ? this.buildFormData(body) : body,
            {
                params: result.httpOptions.params,
                headers: result.httpOptions.headers,
                responseType: 'json'
            }
        );
        return this.pipeResult(observable, params);
    }

    public put<TResult, TBody>(area: AreaTypes, controller: string, service: string, body?: TBody, params?: IRequestServiceParams): Observable<TResult> {
        const url: string = this.buildUrl(area, controller, service);
        const result = this.initializeRequest(params);

        const observable = this.http.put<TResult>(url, body, {
            params: result.httpOptions.params,
            headers: result.httpOptions.headers,
            responseType: 'json'
        });
        return this.pipeResult(observable, params);
    }

    public patch<TResult, TBody>(area: AreaTypes, controller: string, service: string, body?: TBody, params?: IRequestServiceParams): Observable<TResult> {
        const url: string = this.buildUrl(area, controller, service);
        const result = this.initializeRequest(params);

        const observable = this.http.patch<TResult>(url, body, {
            params: result.httpOptions.params,
            headers: result.httpOptions.headers,
            responseType: 'json'
        });
        return this.pipeResult(observable, params);
    }

    public delete<TResult>(area: AreaTypes, controller: string, service: string, params?: IRequestServiceParams): Observable<TResult> {
        const url: string = this.buildUrl(area, controller, service);
        const result = this.initializeRequest(params);

        const observable = this.http.delete<TResult>(url, {
            params: result.httpOptions.params,
            headers: result.httpOptions.headers,
            responseType: 'json'
        });
        return this.pipeResult(observable, params);
    }

    /**
     * Downloads a file from the requested path. Returns a boolean, indicating whether the file was successfully downloaded
     * @param area idicates public or administrative app
     * @param controller the end point in which the service is located
     * @param service the name of the service method to be called
     * @param params these are passed inside to the http query, EXCEPT the responseType - this param is set always to BLOB
     */
    public download(area: AreaTypes, controller: string, service: string, fileName: string, params?: IRequestServiceParams): Observable<boolean> {

        const fileDownloadedEvent: EventEmitter<boolean> = new EventEmitter<boolean>();
        const url = this.buildUrl(area, controller, service);

        if (params !== null && params !== undefined) {
            params.responseType = 'blob';
        } else {
            params = {
                responseType: 'blob'
            } as IRequestServiceParams;
        }

        const result = this.initializeRequest(params);

        this.pipeResult(this.http.get(url, {
            headers: result.httpOptions.headers,
            params: result.httpOptions.params,
            responseType: 'blob',
            observe: 'response'
        }), params).subscribe({
                next: (response: HttpResponse<Blob>) => {
                    this.handleDownloadResult(response, fileName, fileDownloadedEvent);
                },
                error: (errorResponse: HttpErrorResponse) => {
                    fileDownloadedEvent.error(errorResponse);
                }
            });

        return fileDownloadedEvent.asObservable();
    }

    /**
     * Downloads a file from the requested path. Returns a boolean, indicating whether the file was successfully downloaded
     * @param area idicates public or administrative app
     * @param controller the end point in which the service is located
     * @param service the name of the service method to be called
     * @param params these are passed inside to the http query, EXCEPT the responseType - this param is set always to BLOB
     */
    public downloadPost<TBody>(area: AreaTypes, controller: string, service: string, fileName: string, body: TBody, params?: IRequestServiceParams): Observable<boolean> {

        const fileDownloadedEvent: EventEmitter<boolean> = new EventEmitter<boolean>();
        const url = this.buildUrl(area, controller, service);

        if (params !== null && params !== undefined) {
            params.responseType = 'blob';
        } else {
            params = {
                responseType: 'blob'
            } as IRequestServiceParams;
        }

        const result = this.initializeRequest(params);

        this.pipeResult(this.http.post(
            url,
            params?.properties?.asFormData ? this.buildFormData(body) : body,
            {
                headers: result.httpOptions.headers,
                params: result.httpOptions.params,
                responseType: 'blob',
                observe: 'response'
            }),
            params).subscribe({
                next: (response: HttpResponse<Blob>) => {
                    this.handleDownloadResult(response, fileName, fileDownloadedEvent);
                },
                error: (errorResponse: HttpErrorResponse) => {
                    fileDownloadedEvent.error(errorResponse);
                }
            });

        return fileDownloadedEvent.asObservable();
    }

    private handleDownloadResult(response: HttpResponse<Blob>, fileName: string, fileDownloadedEvent: EventEmitter<boolean>): void {
        if (response?.headers !== null && response?.headers !== undefined) {
            const contentDisponsition = response.headers.get('content-disposition');
            if (contentDisponsition !== null && contentDisponsition !== undefined) {
                const dispositionParts = contentDisponsition.split(';');
                if (dispositionParts.length >= 3) {
                    fileName = decodeURIComponent(dispositionParts[2].replace('filename*=UTF-8\'\'', '').trim());
                } else if (dispositionParts.length === 2) {
                    fileName = dispositionParts[2].replace('filename=', '').trim();
                }
            }

            const url: string = window.URL.createObjectURL(response.body);
            const link: HTMLAnchorElement = document.createElement('a');
            link.href = url;
            link.download = fileName;
            link.click();

            fileDownloadedEvent.emit(true);
        }
        else {
            fileDownloadedEvent.emit(false);
        }

        fileDownloadedEvent.complete();
    }

    private pipeResult<TResult>(result: Observable<TResult>, params?: IRequestServiceParams): Observable<TResult> {
        return result.pipe(
            map((data: TResult) => {
                if (params?.responseTypeCtr) {
                    if (Array.isArray(data)) {
                        for (let i = 0; i < data.length; ++i) {
                            data[i] = this.mapData(data[i], params.responseTypeCtr);
                        }
                    }
                    else {
                        data = this.mapData(data, params.responseTypeCtr);
                    }
                }
                return this.afterSuccessProcessing<TResult>(data, this.snackbar, params?.successMessage, params?.properties);
            }),
            catchError((err: HttpErrorResponse) => {
                if (params?.responseType === 'blob') {
                    return (err.error as Blob).text().then(json => {
                        err = new HttpErrorResponse({
                            headers: err.headers,
                            status: err.status,
                            statusText: err.statusText,
                            url: err.url as string,
                            error: JSON.parse(json)
                        });

                        return this.errorHandler(err, params?.properties);
                    });
                } else {
                    return this.errorHandler(err, params?.properties);
                }
            })
        );
    }

    private initializeRequest(requestParams?: IRequestServiceParams): { httpOptions: IHttpOptions, requestParams: IRequestServiceParams } {

        const httpOptions: IHttpOptions = {
            headers: new HttpHeaders({
                'Cache-control': 'no-cache, no-store',
                'Pragma': 'no-cache',
                'Expires': '0',
                //'Accept-Language': 'bg'
            }),
            params: new HttpParams()
        };

        if (!Environment.Instance.production) {
            httpOptions.headers?.append('Access-Control-Allow-Origin', '*');
        }

        if (requestParams?.properties?.asFormData) {
            //Няма нужда от header формата си слага сама
            //multipart/form-data; boundary=----WebKitFormBoundaryzuW5nPZQFQCwQtg4
            //httpOptions.headers = httpOptions.headers?.append('Content-Type', 'multipart/form-data');
        }
        else if (requestParams?.properties?.asText) {
            httpOptions.headers = httpOptions.headers?.append('Content-Type', 'text/plain');
        }
        else {
            httpOptions.headers = httpOptions.headers?.append('Content-Type', 'application/json');
        }

        if (requestParams === undefined) {
            requestParams = {
                httpParams: new HttpParams(),
                responseType: 'json',
                properties: RequestProperties.DEFAULT,
                successMessage: 'successful-message'
            } as IRequestServiceParams;
        }

        if (requestParams.httpParams !== undefined) {
            httpOptions.params = requestParams.httpParams;
        }

        if ((requestParams.properties === null || requestParams.properties === undefined)) {
            requestParams.properties = RequestProperties.DEFAULT;
        }

        if (requestParams.properties.showProgressSpinner) {
            this.spinner.show();
        }

        if (requestParams.responseType === undefined) {
            requestParams.responseType = 'json';
        }

        if (requestParams.headers !== undefined) {
            for (const key of requestParams.headers.keys()) {
                const value = requestParams.headers.get(key);
                if (value !== null && value !== undefined) {
                    httpOptions.headers = httpOptions.headers?.append(key, value);
                }
            }
        }

        return { httpOptions: httpOptions, requestParams: requestParams };
    }

    private errorHandler(
        errorResponse: HttpErrorResponse,
        properties: Partial<RequestProperties> = RequestProperties.DEFAULT
    ): Observable<any> {
        let handledExceptionWithErrorCode: boolean = false;

        if (errorResponse.status !== 0 && errorResponse.error !== null) {
            const error: ErrorModel = errorResponse.error as ErrorModel;

            if ((error.code === null || error.code === undefined) && properties.showException) {
                this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                    data: error,
                    duration: properties.showExceptionDurationErr,
                    panelClass: properties.showExceptionColorClassErr
                });
            }
            else {
                handledExceptionWithErrorCode = true;
            }
        } else {
            if (properties.showException) {
                const error = new ErrorModel();

                error.messages = [this.translate('an-error-occurred-in-the-app')];
                this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                    data: error,
                    duration: properties.showExceptionDurationErr,
                    panelClass: properties.showExceptionColorClassErr
                });
            }
        }

        if (properties.showProgressSpinner) {
            this.spinner.hide();
        }

        if (!properties.rethrowException && !handledExceptionWithErrorCode) {
            return of();
        }
        else {
            throw errorResponse;
        }
    }

    private afterSuccessProcessing<T>(
        data: T,
        snackBarRef: MatSnackBar,
        successMessage?: string,
        properties: Partial<RequestProperties> = RequestProperties.DEFAULT
    ): T {
        if (!CommonUtils.isNullOrEmpty(successMessage)) {
            const config = new MatSnackBarConfig();
            config.horizontalPosition = properties.snackbarHorizontalPosition;
            config.verticalPosition = properties.snackbarVerticalPosition;
            config.duration = properties.showExceptionDurationSucc;
            config.panelClass = [properties.showExceptionColorClassSucc as string];

            snackBarRef.open(this.translate(successMessage as string), undefined, config);
        }

        if (properties.showProgressSpinner) {
            this.spinner.hide();
        }
        return data;
    }

    private translate(property: string): string {
        return this.translator.getValue('service.' + property);
    }

    private buildUrl(area: AreaTypes, controller: string, service: string): string {
        return `${this.baseUrl}/${area}/${controller}/${service}`;
    }

    private buildFormData<TBody>(body: TBody): FormData {
        const data: FormData = new FormData();
        this.buildFormDataHelper(data, body, '');
        return data;
    }

    private mapData<T>(data: T, constr: new (...args: any[]) => T): T | undefined {
        if (data === null || data === undefined) {
            return undefined;
        }

        const result: T = new constr();
        Object.assign(result, data);
        return result;
    }

    private buildFormDataHelper(data: FormData, body: any, prefix: string): void {
        const append = (prefix: string, property: string | null, value: string | Blob, filename?: string) => {
            if (prefix?.length > 0) {
                property = property ? `${prefix}.${property}` : `${prefix}`;
            }
            if (property) {
                if (filename) {
                    data.append(property, value, filename);
                }
                else {
                    data.append(property, value);
                }
            }
        };

        if (typeof body !== 'object') {
            if (body !== null && body !== undefined) {
                switch (typeof body) {
                    case 'number':
                        append(prefix, null, body.toString())
                        break;
                    case 'string':
                        append(prefix, null, body);
                        break;
                    case 'boolean':
                        append(prefix, null, (body === true ? 'true' : 'false'));
                        break;
                }
            }
        }
        else {
            const properties: Set<string> = CommonUtils.getProperties(body);
            for (const prop of properties) {
                if (body[prop] !== null && body[prop] !== undefined) {
                    switch (typeof body[prop]) {
                        case 'number':
                            append(prefix, prop, body[prop].toString())
                            break;
                        case 'string':
                            append(prefix, prop, body[prop]);
                            break;
                        case 'boolean':
                            append(prefix, prop, (body[prop] === true ? 'true' : 'false'));
                            break;
                        case 'object':
                            if (body[prop] instanceof Number) {
                                append(prefix, prop, body[prop].toString());
                            }
                            else if (body[prop] instanceof String) {
                                append(prefix, prop, body[prop]);
                            }
                            else if (body[prop] instanceof Boolean) {
                                append(prefix, prop, (body[prop] === true ? 'true' : 'false'));
                            }
                            else if (body[prop] instanceof Date) {
                                append(prefix, prop, (body[prop] as Date).toJSON());
                            }
                            else if (body[prop] instanceof File) {
                                append(prefix, prop, body[prop] as File, (body[prop] as File).name);
                            }
                            else if (Array.isArray(body[prop])) {
                                (body[prop] as any[]).forEach((value: any, index: number) => {
                                    this.buildFormDataHelper(data, value, (prefix?.length > 0 ? `${prefix}.` : '') + prop + `[${index}]`);
                                });
                            }
                            else {
                                this.buildFormDataHelper(data, body[prop], (prefix?.length > 0 ? `${prefix}.` : '') + prop);
                            }
                            break;
                    }
                }
            }
        }
    }
}