import { HttpParams } from '@angular/common/http';
import { TranslateLoader } from '@ngx-translate/core';
import { Observable, of } from 'rxjs';
import { AreaTypes } from '../enums/area-type.enum';
import { RequestService } from '../services/request.service';

export class WebTranslateLoader implements TranslateLoader {
    public constructor(private http: RequestService) { }

    public getTranslation(lang: string): Observable<any> {
        const params = new HttpParams().append('language', lang);
        return this.http.get(AreaTypes.Common, 'Resources', 'GetWebResources', { httpParams: params });
    }
}