export interface IConfirmDialogData {
    message?: string;
    title?: string;
    okBtnLabel?: string;
    okBtnColor?: 'accent' | 'primary' | 'warn',
    cancelBtnLabel?: string;
    hasCancelButton?: boolean;
}