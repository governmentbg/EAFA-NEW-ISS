

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class TranslationManagementFilters extends BaseRequestModel {

    constructor(obj?: Partial<TranslationManagementFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public code: string | undefined;
    public groupCode: string | undefined;
    public translationType: string | undefined;
    public translationValueBG: string | undefined;
    public translationValueEN: string | undefined;
}