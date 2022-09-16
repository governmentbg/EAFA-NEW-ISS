import { HttpErrorResponse } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { BaseGridRequestModel } from '../../models/common/base-grid-request.model';
import { ColumnSorting } from '../../models/common/grid-request.model';
import { GridResultModel } from '../../models/common/grid-result.model';
import { IPageEventArgs } from '../components/data-table/interfaces/page-event-args.interface';
import { ISortEventArgs } from '../components/data-table/interfaces/sort-event-args.interface';
import { IRemoteTLDatatableComponent } from '../components/data-table/interfaces/tl-remote-datatable.interface';
import { IBaseDatatableManagerParams } from '../interfaces/base-datatable-manager-params.interface';

export class BaseDataTableManager<TDataModel> {

    public constructor(params: IBaseDatatableManagerParams<TDataModel>, gridRequest?: BaseGridRequestModel) {
        this.tlDataTable = params.tlDataTable;
        this.requestServiceMethod = params.requestServiceMethod;

        if (gridRequest == undefined) {
            this.gridRequest = new BaseGridRequestModel();
        } else {
            this.gridRequest = gridRequest;
        }

        this.tlDataTable.sortingChanged.subscribe(this.onSortingChanged.bind(this));
        this.tlDataTable.pageChanged.subscribe(this.onPageChanged.bind(this));

        this.gridRequest.pageSize = this.tlDataTable.recordsPerPage;
        this.gridRequest.pageNumber = (this.tlDataTable.pageNumber + 1);

        this.onRequestServiceMethodCalled = new Subject<TDataModel[] | undefined>();
    }


    protected tlDataTable!: IRemoteTLDatatableComponent;
    protected requestServiceMethod: (request: BaseGridRequestModel) => Observable<GridResultModel<TDataModel>>;
    protected gridRequest!: BaseGridRequestModel;

    public onRequestServiceMethodCalled: Subject<TDataModel[] | undefined>;

    public get GridRequest(): BaseGridRequestModel {
        return this.gridRequest;
    }

    public refreshData(): void {
        if (this.tlDataTable) {
            this.tlDataTable!.showLoader();
            this.requestServiceMethod(this.gridRequest).subscribe({
                next: this.successResponseHandler.bind(this),
                error: this.errorResponseHandler.bind(this)
            });
        }
    }

    protected successResponseHandler(response: GridResultModel<TDataModel>): void {
        setTimeout(() => {
            this.tlDataTable!.setRows(response.records);
            this.tlDataTable!.totalRecordsCount = response.totalRecordsCount;
            this.tlDataTable!.hideLoader();

            this.onRequestServiceMethodCalled.next(response.records);
        });
    }

    protected errorResponseHandler(error: HttpErrorResponse): void {
        this.tlDataTable!.hideLoader();
        console.log(error);

        this.onRequestServiceMethodCalled.next(undefined);
    }

    public deleteRecord(record: TDataModel): void {
        //this.tlDataTable.rows = this.tlDataTable.rows.filter(item => item != record);
        this.refreshData();
    }

    public addRecord(record: TDataModel): void {
        //this.tlDataTable.rows.push(record);
        this.refreshData();
    }

    public editRecord(record: TDataModel): void {
        this.refreshData();
    }

    public undoDeleteRecord(record: TDataModel): void {
        this.refreshData();
    }

    protected onSortingChanged(event: ISortEventArgs): void {
        this.gridRequest.sortColumns = event.sorts.map(x => new ColumnSorting(x.prop, x.dir));
        this.refreshData();
    }

    protected onPageChanged(event: IPageEventArgs): void {
        this.gridRequest.pageNumber = (event.offset + 1);
        this.gridRequest.pageSize = event.pageSize;
        this.refreshData();
    }
}