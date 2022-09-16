import { Component, OnInit, ViewChild } from '@angular/core';
import { INewsPublicService } from '@app/interfaces/public-app/news-public.interface';
import { NewsDTO } from '@app/models/generated/dtos/NewsDTO';
import { NewsPublicService } from '@app/services/public-app/news-public.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { NewsFilters } from '@app/models/generated/filters/NewsFilters';
import { FormControl, FormGroup } from '@angular/forms';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CommonUtils } from '../../../shared/utils/common.utils';
import { NomenclatureDTO } from '../../../models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '../../../shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '../../../enums/nomenclature.types';
import { CommonNomenclatures } from '../../../services/common-app/common-nomenclatures.service';
import { DateRangeData } from '../../../shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'news',
    templateUrl: './news.component.html',
    styleUrls: ['./news.component.scss']
})
export class NewsComponent implements OnInit {
    public translateService: FuseTranslationLoaderService;
    public arrayOfNews: NewsDTO[] = [];
    public page: number = 0;
    public size: number = 5;
    public newsCount: number = 1000;
    public pageSizeOptions: number[] = [2, 3, 4, 5];
    public formGroup: FormGroup;
    public districts!: NomenclatureDTO<number>[];

    @ViewChild(MatPaginator)
    public set Paginator(paginator: MatPaginator) {
        this.paginator = paginator;
    }

    private readonly newsPublicService: INewsPublicService;

    private paginator!: MatPaginator;

    constructor(newsPublicService: NewsPublicService,
        translateService: FuseTranslationLoaderService) {
        this.newsPublicService = newsPublicService;
        this.translateService = translateService;

        this.formGroup = new FormGroup({
            freeTextSearchControl: new FormControl(),
            dateRangeControl: new FormControl(),
            districtsControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.Districts, this.newsPublicService
            .getDistricts.bind(this.newsPublicService), false)
            .subscribe((result: NomenclatureDTO<number>[]) => {
                this.districts = result;
            });

        this.reloadData(true);
    }

    public loadNews(event?: PageEvent): void {
        if (event !== undefined) {
            this.page = event.pageIndex;
            this.size = event.pageSize;
            this.reloadData();
        }
    }

    public searchBtnClicked(): void {
        this.page = 0;
        this.paginator.pageIndex = 0;
        this.reloadData();
    }

    private reloadData(noFilters?: boolean): void {
        const gridRequestModel: GridRequestModel<NewsFilters> = new GridRequestModel<NewsFilters>();
        if (!noFilters) {
            const listOfIds: number[] = [];
            if (!CommonUtils.isNullOrEmpty(this.formGroup.controls.districtsControl.value)) {
                for (const district of this.formGroup.controls.districtsControl.value) {
                    listOfIds.push(district.value);
                }
            }

            const filters: NewsFilters = new NewsFilters({
                freeTextSearch: this.formGroup.controls.freeTextSearchControl.value,
                dateFrom: (this.formGroup.controls.dateRangeControl!.value as DateRangeData)?.start,
                dateTo: (this.formGroup.controls.dateRangeControl!.value as DateRangeData)?.end,
                districtsIds: listOfIds
            });

            gridRequestModel.filters = filters;
        }

        gridRequestModel.pageNumber = (this.page + 1);
        gridRequestModel.pageSize = this.size;

        this.newsPublicService.getAll(gridRequestModel).subscribe(
            (result: GridResultModel<NewsDTO>) => {
                this.arrayOfNews = result.records;
                this.newsCount = result.totalRecordsCount;
            });
    }
}