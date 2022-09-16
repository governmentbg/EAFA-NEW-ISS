import { NativeDateAdapter } from '@angular/material/core';
import { Injectable } from '@angular/core';

@Injectable()
export class TLDateAdapter extends NativeDateAdapter {
    public parse(value: string | number): Date | null {
        let timestamp: number;

        if (typeof value === 'string') {
            if (value.indexOf('.') !== -1) {
                const parts: string[] = value.split('.');

                const year: number = Number(parts[2]);
                const month: number = Number(parts[1]) - 1;
                const date: number = Number(parts[0]);
                return new Date(year, month, date);
            }

            timestamp = Date.parse(value);
        }
        else {
            timestamp = value;
        }
        return isNaN(timestamp) ? null : new Date(timestamp);
    }

    public format(date: Date, displayFormat: string): string {
        if (displayFormat === 'input') {
            const day: number = date.getDate();
            const month: number = date.getMonth() + 1;
            const year: number = date.getFullYear();

            return `${this.to2digit(day)}.${this.to2digit(month)}.${year}`;
        }
        else if (displayFormat === 'inputMonth') {
            const month: number = date.getMonth() + 1;
            const year: number = date.getFullYear();

            return `${this.to2digit(month)}.${year}`;
        }
        else if (displayFormat === 'inputYear') {
            const year: number = date.getFullYear();

            return `${year}`;
        }
        else if (displayFormat === 'YYYYMMDD') {
            const day: number = date.getDate();
            const month: number = date.getMonth() + 1;
            const year: number = date.getFullYear();

            return `${year}-${this.to2digit(month)}-${this.to2digit(day)}`;
        }

        return date.toDateString();
    }

    public getFirstDayOfWeek(): number {
        return 1;
    }

    private to2digit(n: number) {
        return ('00' + n).slice(-2);
    }
}