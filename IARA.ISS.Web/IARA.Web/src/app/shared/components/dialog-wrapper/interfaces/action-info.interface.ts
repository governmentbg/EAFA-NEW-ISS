export interface IActionInfo {
    id?: string;
    translateValue: string;
    customClass?: string;
    color: string;
    buttonData?: any;
    icon?: IIconInfo;
    disabled?: boolean;
    isVisibleInViewMode?: boolean;
    hidden?: boolean;
}

export interface IIconInfo {
    id: string;
    class?: string;
    size?: number;
}