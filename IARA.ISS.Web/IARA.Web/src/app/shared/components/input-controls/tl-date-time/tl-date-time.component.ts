import { Component, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { ThemePalette } from '@angular/material/core';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { DateUtils } from '@app/shared/utils/date.utils';
import { BaseTLControl } from '../base-tl-control';
import { TLError } from '../models/tl-error.model';


@Component({
    selector: 'tl-date-time',
    templateUrl: './tl-date-time.component.html',
})
export class TLDateTimeComponent extends BaseTLControl {
    /**
     * 	If true, the picker is readonly and can't be modified. `false` by default.
     * */
    @Input()
    public disabled: boolean = false;

    /**
     * If true, the spinners above and below input are visible. `false` by default.
     * */
    @Input()
    public showSpinners: boolean = false;

    /**
     * If true, it is not possible to select seconds. `false` by default.
     * */
    @Input()
    public showSeconds: boolean = false;

    /**
     * If true, the minute (and second) is readonly. `false` by default.
     * */
    @Input()
    public disableMinute: boolean = false;

    /**
     * An array [hour, minute, second] for default time when the date is not yet defined. By default is `current time`
     * */
    @Input()
    public defaultTime?: number[]; // TODO find out what type is this: [hour, minute, second]

    /**
     * The number of hours to add/substract when clicking hour spinners.`1` by default.
     * */
    @Input()
    public stepHour: number = 1;

    /**
     * The number of minutes to add/substract when clicking minute spinners. `1` by default.
     * */
    @Input()
    public stepMinute: number = 1;

    /**
     * The number of seconds to add/substract when clicking second spinners. `1` by default.
     * */
    @Input()
    public stepSecond: number = 1;

    /**
     * 	Color palette to use on the datepicker's calendar. `accent` by default. Can be:
     * 	`'primary' | 'accent' | 'warn' | undefined`
     * */
    @Input()
    public color: ThemePalette = 'accent';

    /**
     * Whether to display 12H or 24H mode. Default is `false`.
     * */
    @Input()
    public enableMeridian: boolean = false;

    /**
     * If true, the time is hidden. Default is `false`
     * */
    @Input()
    public hideTime: boolean = false;

    /**
     * Whether the calendar UI is in touch mode. In touch mode the calendar opens in a dialog
     * rather than a popup and elements have more padding to allow for bigger touch targets.
     * Default is `false`
     * */
    @Input()
    public touchUi: boolean = false;

    @Input()
    public min: Date = DateUtils.MIN_DATE;

    @Input()
    public max: Date = DateUtils.MAX_DATE;

    public errors: TLError[] = new Array<TLError>();

    public fieldIsRequired: boolean = false;

    constructor(@Self() @Optional() ngControl: NgControl, translatePipe: TLTranslatePipe) {
        super(ngControl, translatePipe)
    }
}