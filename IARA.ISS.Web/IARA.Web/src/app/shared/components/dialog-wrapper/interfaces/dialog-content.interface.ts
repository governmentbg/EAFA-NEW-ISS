import { DialogWrapperData } from '../models/dialog-action-buttons.model';
import { IActionInfo } from './action-info.interface';

export type DialogCloseCallback = (dialogResult?: unknown) => void;

export interface IDialogComponent {
    setData(data: unknown, wrapperData: DialogWrapperData): void;
    dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void;
    saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void;
    cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void;
    closeBtnClicked?(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void;
}