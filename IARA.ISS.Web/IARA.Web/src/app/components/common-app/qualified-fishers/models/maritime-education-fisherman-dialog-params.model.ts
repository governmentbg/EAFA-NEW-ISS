import { EventEmitter } from '@angular/core';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';

export class MaritimeEducationFishermanDialogParamsModel {
    public id!: number;
    public applicationsService: IApplicationsService | undefined;
    public viewMode: boolean = false;
    public service?: unknown;
    public pageCode!: PageCodeEnum;
    public onRecordAddedOrEdittedEvent: EventEmitter<number> | undefined;
    public isThirdCountryFisherman: boolean = false;

    public constructor(params?: Partial<MaritimeEducationFishermanDialogParamsModel>) {
        Object.assign(this, params);
    }
}