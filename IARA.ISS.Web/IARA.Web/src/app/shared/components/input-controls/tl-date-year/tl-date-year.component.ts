import { Component, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { TLDateComponent } from '../tl-date/tl-date.component';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { TLDateAdapter } from '@app/shared/utils/date.adapter';

export const MY_FORMATS = {
    parse: {
        dateInput: { month: 'short', year: 'numeric', day: 'numeric' }
    },
    display: {
        dateInput: 'inputYear',
        monthYearLabel: 'inputYear',
        dateA11yLabel: { year: 'numeric', month: 'long', day: 'numeric' },
        monthYearA11yLabel: { year: 'numeric', month: 'long' },
    }
};

@Component({
    selector: 'tl-date-year',
    templateUrl: '../tl-date/tl-date.component.html',
    providers: [
        { provide: DateAdapter, useClass: TLDateAdapter },
        { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }
    ]
})
export class TLDateYearComponent extends TLDateComponent {
    public constructor(@Self() @Optional() ngControl: NgControl, fuseTranslateionService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, fuseTranslateionService, tlTranslatePipe);

        this.startView = 'multi-year';
    }

    public selectedYear<D>(date: Date, datepicker: MatDatepicker<D>): void {
        datepicker.close();

        date.setMonth(0);
        date.setDate(1);

        this.formControl!.setValue(date);
    }
}