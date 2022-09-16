import { isArray, isObject, isString } from 'lodash';

export class DateUtils {
    public static ToDateString(date: Date): string {
        return `${DateUtils.GetDateToString(date)}`;
    }

    private static GetDateToString(date: Date): string {
        return `${DateUtils.GetDateOfMonth(date)}.${DateUtils.GetMonth(date)}.${date.getFullYear().toString()}`
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
        let numberStr: string = '';
        if (number) {
            numberStr = number.toString();
        }
        if (number < 10 && !numberStr.startsWith('0')) {
            numberStr = '0' + numberStr;
        }
        return numberStr;
    }
}