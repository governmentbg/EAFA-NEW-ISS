export class FilterEventArgs {
    public constructor(searchText: string, showInactiveRecords?: boolean) {
        this.searchText = searchText;
        if (showInactiveRecords != undefined) {
            this.showInactiveRecords = showInactiveRecords;
        } else {
            this.showInactiveRecords = false;
        }

        this._advancedFilters = new Map<string, any>();
    }

    public searchText: string;
    public showInactiveRecords: boolean;

    public get AdvancedFilters(): Map<string, any> {
        return this._advancedFilters;
    }

    private _advancedFilters: Map<string, any>;

    public getValue<T>(key: string): T | undefined {
        const value = this._advancedFilters.get(key);

        if (value === '') {
            return undefined;
        } else {
            return value as T;
        }
    }
}