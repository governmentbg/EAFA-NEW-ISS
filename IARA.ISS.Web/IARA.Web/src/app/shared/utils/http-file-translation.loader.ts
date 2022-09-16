import { HttpClient, HttpParams } from '@angular/common/http';
import { TranslateLoader } from '@ngx-translate/core';
import { Observable } from 'rxjs';

export class FileTranslateLoader implements TranslateLoader {
    public constructor(private http: HttpClient) { }

    public getTranslation(lang: string): Observable<any> {
        if (lang === 'bg') {
            return this.http.get("assets/translations/bg.json");
        }
        else {
            return this.http.get("assets/translations/en.json");
        }
    }
}