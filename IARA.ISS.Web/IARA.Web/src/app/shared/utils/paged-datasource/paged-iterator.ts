import { PagedIndexIterator } from './paged-index-iterator';

export class PagedIterator<T> implements IterableIterator<T | undefined>, Iterator<T | undefined> {

    private pageIndexIterator: PagedIndexIterator<T>;

    constructor(array: Array<Array<T> | undefined>, pageSize: number, totalLength: number) {
        this.pageIndexIterator = new PagedIndexIterator(array, pageSize, totalLength);
    }

    [Symbol.iterator](): IterableIterator<T | undefined> {
        return this;
    }

    public next(value?: any): IteratorReturnResult<T | undefined> {
        return {
            done: this.pageIndexIterator.next(value).done,
            value: this.pageIndexIterator.next(value).value?.[1]
        } as IteratorReturnResult<T>;
    }
}
