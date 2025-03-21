﻿

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class InspectionsFilters extends BaseRequestModel {

    constructor(obj?: Partial<InspectionsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipId: number | undefined;
    public territoryNode: number | undefined;
    public inspector: string | undefined;
    public inspectorId: number | undefined;
    public inspectionTypeId: number | undefined;
    public stateIds: number[] | undefined;
    public reportNumber: string | undefined;
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
    public year: number | undefined;
    public subjectIsLegal: boolean | undefined;
    public subjectName: string | undefined;
    public subjectEIK: string | undefined;
    public subjectEGN: string | undefined;
    public aquacultureId: number | undefined;
    public poundNetId: number | undefined;
    public unregisteredShipName: string | undefined;
    public fishermanName: string | undefined;
    public waterObjectName: string | undefined;
    public firstSaleCenterName: string | undefined;
    public tractorLicensePlateNumber: string | undefined;
    public trailerLicensePlateNumber: string | undefined;
    public showOnlyUserInspections: boolean | undefined;
    public inspectorUserId: number | undefined;
    public shipLogBookPageId: number | undefined;
    public addmissionLogBookPageId: number | undefined;
    public transportationLogBookPageId: number | undefined;
    public firstSaleLogBookPageId: number | undefined;
    public aquacultureLogBookPageId: number | undefined;
    public inspectedPersonId: number | undefined;
    public inspectedLegalId: number | undefined;
    public inspectionId: number | undefined;
    public updatedAfter: Date | undefined;
}