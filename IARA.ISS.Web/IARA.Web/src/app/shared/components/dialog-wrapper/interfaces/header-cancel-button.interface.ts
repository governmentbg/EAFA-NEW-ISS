export type HeaderCloseFunction = () => void;
export type HeaderCancelCallback = (closeFn: HeaderCloseFunction) => void;

export interface IHeaderCancelButton {
    tooltip?: string;
    cancelBtnClicked: HeaderCancelCallback;
}
