import { HttpErrorResponse } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';

@Injectable({
    providedIn: 'root'
})
export class SystemPropertiesService {
    protected readonly area: AreaTypes = AreaTypes.Nomenclatures;
    protected controller: string = 'Nomenclatures';

    public get properties(): Observable<SystemPropertiesDTO> {
        if (this.props !== null && this.props !== undefined) {
            return of(this.props);
        }
        else {
            const event: EventEmitter<SystemPropertiesDTO> = new EventEmitter<SystemPropertiesDTO>();
            this.getSystemProperties().subscribe({
                next: (props: SystemPropertiesDTO) => {
                    this.props = props;
                    event.emit(this.props);
                    event.complete();
                },
                error: (error: HttpErrorResponse) => {
                    event.error(error);
                }
            });

            return event;
        }
    }

    private http: RequestService;
    private props?: SystemPropertiesDTO;

    public constructor(http: RequestService) {
        this.http = http;
    }

    private getSystemProperties(): Observable<SystemPropertiesDTO> {
        return this.http.get(this.area, this.controller, 'GetSystemProperties', {
            responseTypeCtr: SystemPropertiesDTO
        });
    }
}