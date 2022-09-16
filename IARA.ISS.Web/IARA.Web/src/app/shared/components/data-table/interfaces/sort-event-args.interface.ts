export interface ISortEventArgs {
    sorts: ISortedColumn[];
    column: string;
    prevValue: string;
    newValue: string;
}

export interface ISortedColumn {
    prop: string,
    dir: 'desc' | 'asc'
}