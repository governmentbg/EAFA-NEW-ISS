

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ApplicationRegiXChecksFilters extends BaseRequestModel {

    constructor(obj?: Partial<ApplicationRegiXChecksFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public applicationId: string | undefined;
    public applicationTypeId: number | undefined;
    public webServiceName: string | undefined;
    public requestDateFrom: Date | undefined;
    public requestDateTo: Date | undefined;
    public responseDateFrom: Date | undefined;
    public responseDateTo: Date | undefined;
    public requestContent: string | undefined;
    public responseContent: string | undefined;
    public errorLevels: string[] | undefined;
}