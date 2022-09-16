import { PageUtils } from './page-utils';
import {
    EveryPredicate,
    FilterPredicate,
    FindIndexPredicate,
    FindPredicate,
    ForeachPredicate,
    MapPredicate,
    ReducePredicate,
    SomePredicate
} from './paged-array-predicates';
import { PagedIndexIterator } from './paged-index-iterator';
import { PagedIterator } from './paged-iterator';

export class PagedArray<T> implements Array<T | undefined> {

    public fillPage(page: number, records: T[]): number {
        if (this.array.length < page) {

            for (let i = 0; i <= page; i++) {
                this.array.push(undefined);
            }

            return this.array.push(records);
        } else if (this.array.length == page) {
            return this.array.push(records);
        } else {
            return (this.array[page] as T[]).push(...records);
        }
    }

    public calculateLength(): number {

        if (this.array == undefined || this.array.length == 0) {
            return 0;
        } else {
            let counter: number = 0;
            for (let page of this.array) {
                if (page != undefined && page.length > 0) {
                    counter += page.length;
                }
            }

            return counter;
        }
    }

    private array: Array<Array<T> | undefined> = [];

    constructor(pageSize: number = 50, maxLength: number = 10000) {
        this.pageSize = pageSize;
        this.maxLength = maxLength;
    }

    [n: number]: T | undefined;

    private _length: number = 0;

    public get length(): number {
        if (this._length == 0) {
            this._length = this.calculateLength();
        }

        return this._length;
    }

    public set length(value: number) {
        this._length = value;
    }

    public pageSize: number;
    public maxLength: number;


    public toString(): string {
        return this.join();
    }

    public toLocaleString(): string {
        return this.join();
    }

    public pop(): T | undefined {
        if (this.array.length > 0 && this.array[0] != undefined) {
            return this.array[0].pop();
        }

        return undefined;
    }

    public push(...items: (T | undefined)[]): number {
        let newArray = items.map(x => x as T);
        let newItemsCounter: number = 0;

        if (this.array.length == 0) {
            newItemsCounter = this.array.push(newArray);
        } else if (this.array[0] == undefined || this.array[0].length == 0) {
            this.array[0] = newArray;
            newItemsCounter = newArray.length;
        } else {
            let array = this.array[0];

            for (let item of newArray) {

                if (array.length == this.pageSize) {
                    array.pop();
                } else {
                    newItemsCounter++;
                }

                array.push(item);
            }
        }

        this._length += newItemsCounter;
        return newItemsCounter;
    }

    public concat(...items: ConcatArray<T | undefined>[]): (T | undefined)[] {
        throw new Error('Method not implemented.');
    }

    public join(separator?: string): string {
        return this.array.map(value => {
            if (value != undefined) {
                return value.join(separator);
            } else {
                return "";
            }
        }).join(separator);
    }

    public reverse(): (T | undefined)[] {
        const tempArray: T[] = [];

        this.array.reverse().forEach(value => {
            if (value != undefined) {
                tempArray.push(...value.reverse());
            }
        });

        return tempArray;
    }

    public shift(): T | undefined {
        throw new Error('Method not implemented.');
    }

    public slice(start?: number, end?: number): (T | undefined)[] {
        if (start == undefined) {
            start = 0;
        }

        if (end == undefined) {
            end = this.array.length * this.pageSize;
        }

        const tempArray: T[] = [];
        const startPage = PageUtils.getPageForIndex(start, this.pageSize);
        const endPage = PageUtils.getPageForIndex(end - 1, this.pageSize);

        const startNumber = Math.ceil(((start / this.pageSize) - startPage) * this.pageSize);

        if (startPage == endPage) {

            let endNumber = Math.floor(((end / this.pageSize) - endPage) * this.pageSize);

            for (let j = startNumber; j < endNumber; j++) {
                if (this.array[startPage] != undefined) {
                    tempArray.push((this.array[startPage] as T[])[j]);
                }
            }
        } else {
            let endNumber = Math.floor(((end / this.pageSize) - endPage) * this.pageSize);

            for (let j = startNumber; j < this.pageSize; j++) {
                if (this.array[startPage] != undefined) {
                    tempArray.push((this.array[startPage] as T[])[j]);
                }
            }

            for (let i = (startPage + 1); i <= (endPage - 1); i++) {
                for (let j = 0; j < this.pageSize; j++) {
                    if (this.array[i] != undefined) {
                        tempArray.push((this.array[i] as T[])[j]);
                    }
                }
            }

            for (let j = 0; j < endNumber; j++) {
                if (this.array[endPage] != undefined) {
                    tempArray.push((this.array[endPage] as T[])[j]);
                }
            }
        }

        return tempArray;
    }

    public sort(compareFn?: (a: T | undefined, b: T | undefined) => number): this {
        throw new Error('Method not implemented.');
    }

    public splice(start: number, deleteCount?: number): (T | undefined)[] {
        throw new Error('Method not implemented');
    }


    public unshift(...items: (T | undefined)[]): number {
        throw new Error('Method not implemented.');
    }

    public indexOf(searchElement: T, fromIndex?: number): number {
        const indexes = this.array.map(value => {
            if (value != undefined) {
                return value.indexOf(searchElement, fromIndex);
            } else {
                return -1;
            }
        }).filter(x => x >= 0);

        if (indexes.length == 0) {
            return -1;
        } else {
            return indexes[0];
        }
    }

    public lastIndexOf(searchElement: T, fromIndex?: number): number {
        const indexes = this.array.map(value => {
            if (value != undefined) {
                return value.indexOf(searchElement, fromIndex);
            } else {
                return -1;
            }
        }).filter(x => x >= 0);

        if (indexes.length == 0) {
            return -1;
        } else {
            return indexes[indexes.length - 1];
        }
    }

    public some(predicate: SomePredicate<T>, thisArg?: any): boolean {
        throw new Error('Method not implemented.');
    }

    public forEach(callbackfn: ForeachPredicate<T>, thisArg?: any): void {
        this.array.forEach(value => {
            if (value != undefined) {
                value.forEach(callbackfn, thisArg);
            }
        });
    }

    public map<U>(callbackfn: MapPredicate<T, U>, thisArg?: any): U[] {
        const tempArray: U[] = [];

        this.array.forEach(value => {
            if (value != undefined) {
                tempArray.push(...value.map(callbackfn));
            }
        });

        return tempArray;
    }

    public filter<S extends T | undefined>(predicate: FilterPredicate<T, S>, thisArg?: any): S[] {
        const tempArray: S[] = [];
        this.array.forEach(value => {
            if (value != undefined) {
                const temp = value.filter(predicate);
                for (const item of temp) {
                    tempArray.push(item as S);
                }
            }
        });

        return tempArray;
    }

    reduce(callbackfn: ReducePredicate<T>): T | undefined {
        throw new Error('Method not implemented.');
    }

    reduceRight(callbackfn: (previousValue: T | undefined, currentValue: T | undefined, currentIndex: number, array: (T | undefined)[]) => T | undefined): T | undefined {
        throw new Error('Method not implemented.');
    }

    public find<S extends T | undefined>(predicate: FindPredicate<T, S>, thisArg?: any): S | undefined {
        return this.array.map(value => {
            if (value != undefined) {
                return value.find(predicate) as S;
            } else {
                return undefined;
            }
        })[0];
    }

    findIndex(predicate: FindIndexPredicate<T>, thisArg?: any): number {
        throw new Error('Method not implemented.');
    }

    fill(value: T | undefined, start?: number, end?: number): this {
        throw new Error('Method not implemented.');
    }

    copyWithin(target: number, start: number, end?: number): this {
        throw new Error('Method not implemented.');
    }

    [Symbol.iterator](): IterableIterator<T | undefined> {
        return this.values();
    }

    entries(): IterableIterator<[number, T | undefined]> {
        return new PagedIndexIterator<T>(this.array, this.pageSize, this.maxLength);
    }

    keys(): IterableIterator<number> {
        throw new Error('Method not implemented.');
    }

    values(): IterableIterator<T | undefined> {
        return new PagedIterator<T>(this.array, this.pageSize, this.maxLength);
    }

    [Symbol.unscopables](): { copyWithin: boolean; entries: boolean; fill: boolean; find: boolean; findIndex: boolean; keys: boolean; values: boolean; } {
        throw new Error('Method not implemented.');
    }

    includes(searchElement: T | undefined, fromIndex?: number): boolean {
        throw new Error('Method not implemented.');
    }

    every<S extends T | undefined>(predicate: EveryPredicate<T, S>, thisArg?: any): this is S[] {
        throw new Error('Method not implemented.');
    }
}
