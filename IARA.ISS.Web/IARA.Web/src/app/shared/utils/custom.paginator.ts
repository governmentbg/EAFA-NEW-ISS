import { MatPaginatorIntl } from '@angular/material/paginator';
import { Injectable } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

@Injectable()
export class CustomMatPaginatorIntl extends MatPaginatorIntl {
    public itemsPerPageLabel: string;
    public nextPageLabel: string;
    public previousPageLabel: string;
    public getRangeLabel: (page: number, pageSize: number, length: number) => string;

    private translate: FuseTranslationLoaderService;

    public constructor(translate: FuseTranslationLoaderService) {
        super();
        this.translate = translate;

        this.itemsPerPageLabel = this.translate.getValue('common.paginator-per-page');
        this.nextPageLabel = this.translate.getValue('common.paginator-next-page');
        this.previousPageLabel = this.translate.getValue('common.paginator-prev-page');

        this.getRangeLabel = this.getRangeLabelMethod.bind(this);
    }

    private getRangeLabelMethod(page: number, pageSize: number, length: number): string {
        if (length === 0 || pageSize === 0) {
            return `0 ${this.translate.getValue('common.paginator-from')} ${length}`;
        }

        length = Math.max(length, 0);

        const startIndex: number = page * pageSize;

        const endIndex: number = startIndex < length
            ? Math.min(startIndex + pageSize, length)
            : startIndex + pageSize;

        return `${startIndex + 1} – ${endIndex} ${this.translate.getValue('common.paginator-from')} ${length}`;
    }
}