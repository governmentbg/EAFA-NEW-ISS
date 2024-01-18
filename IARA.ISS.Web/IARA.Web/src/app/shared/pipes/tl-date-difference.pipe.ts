import { Pipe, PipeTransform } from '@angular/core';
import { DateDifference } from '@app/models/common/date-difference.model';

@Pipe({ name: 'tlDateDifference' })
export class TLDateDifferencePipe implements PipeTransform {
    transform(dateDifference: DateDifference | undefined): string {
        if (dateDifference !== null && dateDifference !== undefined) {
            let difference: string = '';
            let hasDays: boolean = false;
            let hasHours: boolean = false;

            if (dateDifference.days !== 0 && dateDifference.days !== null && dateDifference.days !== undefined) {
                if (dateDifference.days === 1) {
                    difference = `${dateDifference.days} ден `;
                }
                else {
                    difference = `${dateDifference.days} дни `;
                }

                hasDays = true;
            }

            if (dateDifference.hours !== 0 && dateDifference.hours !== null && dateDifference.hours !== undefined) {
                if (hasDays && (dateDifference.minutes === 0 || dateDifference.minutes === null || dateDifference.minutes === undefined)) {
                    difference = `${difference}и `;
                }

                if (dateDifference.hours === 1) {
                    difference = `${difference}${dateDifference.hours} час `;
                }
                else {
                    difference = `${difference}${dateDifference.hours} часа `;
                }

                hasHours = true;
            }

            if (dateDifference.minutes !== 0 && dateDifference.minutes !== null && dateDifference.minutes !== undefined) {
                if (hasDays === true || hasHours === true) {
                    difference = `${difference}и `;
                }

                difference = `${difference}${dateDifference.minutes} минути`;
            }

            return difference.trim();
        }
        else {
            return '';
        }
    }
}