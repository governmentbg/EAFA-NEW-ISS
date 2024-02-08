import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { IActionInfo } from './action-info.interface';
import { IHeaderAuditButton } from './header-audit-button.interface';
import { IHeaderCancelButton } from './header-cancel-button.interface';

export interface IDialogData<T> {
    TCtor: new (...args: any[]) => T;
    title: string;
    componentData?: any;
    disableDialogClose?: boolean;
    headerCancelButton?: IHeaderCancelButton;
    headerAuditButton?: IHeaderAuditButton;
    viewMode?: boolean;
    defaultFullscreen?: boolean;

    saveBtn?: IActionInfo;
    cancelBtn?: IActionInfo;
    closeBtn?: IActionInfo;
    leftSideActionsCollection?: Array<IActionInfo>;
    rightSideActionsCollection?: Array<IActionInfo>;
    translteService: ITranslationService;
}