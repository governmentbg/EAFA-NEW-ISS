import { floor } from 'lodash';


export class PagedIndexIterator<T> implements IterableIterator<[number, T | undefined]>, Iterator<[number, T | undefined]> {

    constructor(array: Array<Array<T> | undefined>, pageSize: number, totalLength: number) {
        this.array = array;
        this.pageSize = pageSize;
        this.totalLength = totalLength;
    }

    [Symbol.iterator](): IterableIterator<[number, T | undefined]> {
        return this;
    }

    private counter: number = 0;
    private array: Array<Array<T> | undefined>;
    private pageSize: number;
    private totalLength: number;

    public next(value?: any): IteratorReturnResult<[number, T | undefined]> {
        let item: T | undefined = undefined;
        const diff = this.counter / this.pageSize;
        const page = floor(diff);
        const index = (diff - page) * this.pageSize;

        if (page <= this.array.length) {
            if (this.array[page] != undefined) {
                item = (this.array[page] as T[])[index];
            }
        }

        this.counter++;

        return {
            done: this.counter == this.totalLength,
            value: [this.counter - 1, item]
        } as IteratorReturnResult<[number, T | undefined]>;
    }
}
