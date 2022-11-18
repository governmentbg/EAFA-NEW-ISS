import { EventEmitter } from '@angular/core';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';

export class DialogParamsModel {
    public id!: number;
    public applicationId: number | undefined;
    public applicationsService: IApplicationsService | undefined;
    public isApplication: boolean = false;
    public isReadonly: boolean = false;
    public isApplicationHistoryMode: boolean = false;
    public viewMode: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public service?: unknown;
    public isThirdCountry?: boolean;
    public pageCode!: PageCodeEnum;
    public onRecordAddedOrEdittedEvent: EventEmitter<number> | undefined;
    public loadRegisterFromApplication: boolean = false;
    public model: IApplicationRegister | undefined;

    public constructor(params?: Partial<DialogParamsModel>) {
        Object.assign(this, params);
    }
}
