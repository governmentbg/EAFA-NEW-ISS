export interface FuseNavigationItem {
    id: string;
    title: string;
    type: 'item' | 'group' | 'collapsable';
    translate?: string;
    icon?: string;
    iconType?: 'IC_ICON' | 'FA_ICON' | 'MAT_ICON';
    hidden?: boolean;
    url?: string;
    classes?: string;
    exactMatch?: boolean;
    externalUrl?: boolean;
    openInNewTab?: boolean;
    function?: any;
    badge?: {
        title?: string;
        translate?: string;
        bg?: string;
        fg?: string;
    };
    permissions?: string[];
    children?: FuseNavigationItem[];
}

export interface FuseNavigation extends FuseNavigationItem {
    children?: FuseNavigationItem[];
}