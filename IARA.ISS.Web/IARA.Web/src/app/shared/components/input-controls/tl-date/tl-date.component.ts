import { Component, EventEmitter, Input, Optional, Output, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { MatCalendarView, MatDatepicker } from '@angular/material/datepicker';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { DateUtils } from '@app/shared/utils/date.utils';
import { BaseTLControl } from '../base-tl-control';

@Component({
    selector: 'tl-date',
    templateUrl: './tl-date.component.html',
})
export class TLDateComponent extends BaseTLControl {
    @Input() public min: Date = DateUtils.MIN_DATE;
    @Input() public max: Date = DateUtils.MAX_DATE;

    @Input() public startView: MatCalendarView = 'month';

    @Output()
    public monthSelected: EventEmitter<unknown> = new EventEmitter<unknown>();

    @Output()
    public yearSelected: EventEmitter<unknown> = new EventEmitter<unknown>();

    public constructor(@Self() @Optional() ngControl: NgControl, fuseTranslateionService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, fuseTranslateionService, tlTranslatePipe);
    }

    public selectedMonth<D>(date: Date, datepicker: MatDatepicker<D>): void {
        // nothing to do
    }

    public selectedYear<D>(date: Date, datepicker: MatDatepicker<D>): void {
        // nothing to do
    }
}