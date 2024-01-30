import { Component, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepicker } from '@angular/material/datepicker';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { TLDateAdapter } from '@app/shared/utils/date.adapter';
import { TLDateComponent } from '../tl-date/tl-date.component';

export const MY_FORMATS = {
    parse: {
        dateInput: { month: 'short', year: 'numeric', day: 'numeric' }
    },
    display: {
        dateInput: 'inputMonth',
        monthYearLabel: 'inputMonth',
        dateA11yLabel: { year: 'numeric', month: 'long', day: 'numeric' },
        monthYearA11yLabel: { year: 'numeric', month: 'long' },
    }
};

@Component({
    selector: 'tl-date-month',
    templateUrl: '../tl-date/tl-date.component.html',
    providers: [
        { provide: DateAdapter, useClass: TLDateAdapter },
        { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }
    ]
})
export class TLDateMonthComponent extends TLDateComponent {
    public constructor(@Self() @Optional() ngControl: NgControl, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, tlTranslatePipe);

        this.startView = 'multi-year';
    }

    public selectedMonth<D>(date: Date, datepicker: MatDatepicker<D>): void {
        datepicker.close();

        date.setDate(1);
        this.formControl!.setValue(date);
    }
}