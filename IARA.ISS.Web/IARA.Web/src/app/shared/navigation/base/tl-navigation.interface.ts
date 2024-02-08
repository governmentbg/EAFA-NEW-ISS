export interface ITLNavigation {
    id: string;
    title: string;
    translate: string;
    type: 'item' | 'group' | 'collapsable',
    classes?: string;
    url?: string;
    icon?: string;
    component?: any;
    permissions?: string[];
    exceptPermissions?: string[],
    children?: ITLNavigation[];
    canLoad?: any[] | undefined;
    canActivate?: any[] | undefined;
    hideInMenu?: boolean;
    isPublic?: boolean;
}

export class TLNavigation implements ITLNavigation {

    constructor(public id: string,
        public title: string,
        public translate: string,
        public type: "item" | "group" | "collapsable",
        public isPublic: boolean = false,
        public classes?: string | undefined,
        public url?: string | undefined,
        public icon?: string | undefined,
        public component?: any,
        public permissions?: string[] | undefined,
        public exceptPermissions?: string[] | undefined,
        public children?: ITLNavigation[] | undefined,
        public canLoad?: any[] | undefined,
        public canActivate?: any[] | undefined,
        public hideInMenu?: boolean | undefined) {

    }



}