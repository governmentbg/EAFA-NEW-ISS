﻿

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ApplicationsRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<ApplicationsRegisterFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public applicationId: number | undefined;
    public accessCode: string | undefined;
    public eventisNum: string | undefined;
    public applicationTypeId: number | undefined;
    public applicationStatusId: number | undefined;
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
    public submittedFor: string | undefined;
    public submittedForEgnLnc: string | undefined;
    public applicationSourceId: number | undefined;
    public showAssignedApplications: boolean | undefined;
    public assignedToUserId: number | undefined;
    public showOnlyNotFinished: boolean | undefined;
    public showOnlineApplications: boolean | undefined;
    public territoryUnitId: number | undefined;
    public submittedByPersonId: number | undefined;
    public submittedForPersonId: number | undefined;
    public submittedForLegalId: number | undefined;
    public shipId: number | undefined;
    public permitId: number | undefined;
    public permitLicenseId: number | undefined;
    public aquacultureFacilityId: number | undefined;
    public buyerId: number | undefined;
    public fishermanId: number | undefined;
}