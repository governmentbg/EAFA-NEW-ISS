import { NgxMatDateFormats } from '@angular-material-components/datetime-picker';
import { DateDifference } from '@app/models/common/date-difference.model';
import { CommonUtils } from './common.utils';

export class DateUtils {
    static readonly MIN_DATE: Date = new Date(1895, 0, 1);
    static readonly MAX_DATE: Date = new Date(9999, 11, 31);

    static readonly TL_DATE_FORMATS = {
        parse: {
            dateInput: { month: 'short', year: 'numeric', day: 'numeric' }
        },
        display: {
            dateInput: 'input',
            monthYearLabel: 'input',
            dateA11yLabel: { year: 'numeric', month: 'long', day: 'numeric' },
            monthYearA11yLabel: { year: 'numeric', month: 'long' },
        }
    };

    static readonly TL_NGX_DATE_FORMATS: NgxMatDateFormats = {
        parse: {
            dateInput: 'DD.MM.YYYY LT',
        },
        display: {
            dateInput: "DD.MM.YYYY LT",
            monthYearLabel: "MMM YYYY",
            dateA11yLabel: "LL",
            monthYearA11yLabel: "MMMM YYYY"
        }
    };

    public static ToDisplayDateTime(date: Date | undefined): string {
        if (date != undefined) {
            date = new Date(date);
            return `${DateUtils.GetDateOfMonth(date)}.${DateUtils.GetMonth(date)}.${date.getFullYear()} ${DateUtils.GetTimeToString(date)}`;
        } else {
            return '';
        }
    }

    public static ToDisplayDateString(date: Date): string {
        return `${DateUtils.GetDateToDisplayString(date)}`;
    }

    public static ToISODateString(date: Date): string {
        return `${DateUtils.GetDateToISOString(date)}`;
    }

    public static FromDateString(dateStr: string): Date {
        console.log(dateStr);
        throw new Error('Method not implemented');
    }

    public static ToDateTimeString(dateTime: Date): string {
        const offsetHours: number = (dateTime.getTimezoneOffset() * -1) / 60;
        const sign = DateUtils.GetOffsetSign(offsetHours);

        return `${DateUtils.GetDateToISOString(dateTime)}T${DateUtils.GetTimeToString(dateTime)}${sign}${DateUtils.GetTimeOffset(offsetHours)}`;
    }

    public static FromDateTimeString(dateTimeStr: string): Date {
        console.log(dateTimeStr);
        throw new Error('Method not implemented');
    }

    public static getDateDifference(startDate: Date, endDate: Date): DateDifference | undefined {
        if (!CommonUtils.isNullOrEmpty(startDate) && !CommonUtils.isNullOrEmpty(endDate) && !isNaN(startDate.getTime()) && !isNaN(endDate.getTime())) {
            let seconds: number = Math.floor((endDate.getTime() - (startDate.getTime())) / 1000);
            let minutes: number = Math.floor(seconds / 60);
            let hours: number = Math.floor(minutes / 60);
            const days: number = Math.floor(hours / 24);

            hours = hours - (days * 24);
            minutes = minutes - (days * 24 * 60) - (hours * 60);
            seconds = seconds - (days * 24 * 60 * 60) - (hours * 60 * 60) - (minutes * 60);

            return new DateDifference({ days: days, hours: hours, minutes: minutes, seconds: seconds });
        }
        else {
            return undefined;
        }
    }

    private static GetOffsetSign(offset: number): string {
        if (offset > 0)
            return '+';
        else
            return '-';
    }

    private static GetTimeOffset(offsetHours: number): string {
        const hoursPortion: number = Number.parseInt(offsetHours.toFixed(0));
        const minutesPortion: number = Number.parseInt(((offsetHours - hoursPortion) * 60).toFixed(0));

        return `${DateUtils.ConcatLeadingZero(hoursPortion)}:${DateUtils.ConcatLeadingZero(minutesPortion)}`
    }

    private static GetTimeToString(date: Date): string {
        return `${DateUtils.ConcatLeadingZero(date.getHours())}:${DateUtils.ConcatLeadingZero(date.getMinutes())}:${DateUtils.ConcatLeadingZero(date.getSeconds())}`;
    }

    private static GetDateToISOString(date: Date): string {
        return `${date.getFullYear().toString()}-${DateUtils.GetMonth(date)}-${DateUtils.GetDateOfMonth(date)}`;
    }

    private static GetDateToDisplayString(date: Date): string {
        return `${DateUtils.GetDateOfMonth(date)}.${DateUtils.GetMonth(date)}.${date.getFullYear().toString()}`;
    }

    private static GetMonth(date: Date): string {
        const month: number = date.getMonth() + 1;
        return DateUtils.ConcatLeadingZero(month);
    }

    private static GetDateOfMonth(date: Date): string {
        const dateNumber: number = date.getDate();
        return DateUtils.ConcatLeadingZero(dateNumber);
    }

    private static ConcatLeadingZero(number: number): string {
        let numberStr: string = number.toString();
        if (number < 10) {
            numberStr = '0' + numberStr;
        }
        return numberStr;
    }
}