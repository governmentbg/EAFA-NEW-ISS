import { PageCodeEnum } from '@app/enums/page-code.enum';

export interface IApplicationRegister {
    id?: number;
    applicationId?: number | undefined;
    statusReason?: string;
    pageCode?: PageCodeEnum;
    isDraft?: boolean;
}