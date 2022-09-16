import { CollectionViewer, DataSource, ListRange } from '@angular/cdk/collections';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { PageUtils } from './page-utils';
import { PagedArray } from './paged-array';
import { PagePredicate } from './paged-array-predicates';

export class PagedDatasource<T> extends DataSource<T | undefined> {

    public readonly pageSize: number;

    private readonly dataStream: BehaviorSubject<(T | undefined)[]>;
    private readonly fetchedPages: Set<number>;

    private subscription!: Subscription;
    private cachedData: PagedArray<T>;
    public readonly newDataReceived: Subject<GridResultModel<T>>;
    private readonly getPage: PagePredicate<T>;

    constructor(getPage: PagePredicate<T>, pageSize: number = 50, maxLength: number = 10000) {
        super();
        this.cachedData = new PagedArray<T>(pageSize, maxLength);
        this.newDataReceived = new Subject<GridResultModel<T>>()
        this.fetchedPages = new Set<number>();
        this.pageSize = pageSize;
        this.dataStream = new BehaviorSubject<(T | undefined)[]>(this.cachedData);
        this.getPage = getPage;
    }

    public connect(collectionViewer: CollectionViewer): Observable<(T | undefined)[]> {
        this.subscription = collectionViewer.viewChange.subscribe(this.viewChangeHandler.bind(this));
        return this.dataStream;
    }

    public disconnect(): void {
        this.subscription.unsubscribe();
    }

    public getFirstRecords(): void {
        this.fetchPage(0);
    }

    public push(...items: T[]): number {
        const count = this.cachedData.push(...items);
        this.dataStream.next(this.cachedData);
        return count;
    }

    private viewChangeHandler(range: ListRange) {
        const startPage = PageUtils.getPageForIndex(range.start, this.pageSize);
        const endPage = PageUtils.getPageForIndex(range.end - 1, this.pageSize);
        for (let i = startPage; i <= endPage; i++) {
            this.fetchPage(i);
        }
    }

    private fetchPage(page: number) {
        if (this.fetchedPages.has(page)) {
            return;
        } else {
            this.fetchedPages.add(page);
            this.getPageResult(page).then(result => {

                this.cachedData.fillPage(page, result.records);
                this.cachedData.length = result.totalRecordsCount;
                this.dataStream.next(this.cachedData);
                this.newDataReceived.next(result);
            }, (reason) => {
                //
            });
        }
    }

    private getPageResult(page: number, pageSize?: number): Promise<GridResultModel<T>> {

        if (pageSize == undefined) {
            pageSize = this.pageSize;
        }

        return this.getPage(page, pageSize);
    }
}



