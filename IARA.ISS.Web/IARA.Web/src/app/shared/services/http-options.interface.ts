import { HttpHeaders, HttpParams } from "@angular/common/http";

export interface IHttpOptions {
    headers?: HttpHeaders;
    params: HttpParams;
}