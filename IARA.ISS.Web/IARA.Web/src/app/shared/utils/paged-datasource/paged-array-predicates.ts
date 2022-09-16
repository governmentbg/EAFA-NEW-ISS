import { GridResultModel } from '../../../models/common/grid-result.model';

export type EveryPredicate<T, S extends T | undefined> = (value: T | undefined, index: number, array: (T | undefined)[]) => value is S;
export type FindIndexPredicate<T> = (value: T | undefined, index: number, obj: (T | undefined)[]) => unknown;
export type FindPredicate<T, S extends T | undefined> = (this: void, value: T | undefined, index: number, obj: (T | undefined)[]) => value is S;
export type ReducePredicate<T> = (previousValue: T | undefined, currentValue: T | undefined, currentIndex: number, array: (T | undefined)[]) => T | undefined;
export type FilterPredicate<T, S extends T | undefined> = (value: T | undefined, index: number, array: (T | undefined)[]) => value is S;
export type MapPredicate<T, U> = (value: T | undefined, index: number, array: (T | undefined)[]) => U;
export type ForeachPredicate<T> = (value: T | undefined, index: number, array: (T | undefined)[]) => void;
export type SomePredicate<T> = (value: T | undefined, index: number, array: (T | undefined)[]) => unknown;
export type PagePredicate<T> = (page: number, pageSize?: number) => Promise<GridResultModel<T>>;