import { DateDifference } from '@app/models/common/date-difference.model';
import { LogBookPageEditExceptionDTO } from '../../../../models/generated/dtos/LogBookPageEditExceptionDTO';

export class CatchesAndSalesUtils {
    public static pageHasLogBookPageDateLockedViaDaysAfterMonth(pageFillDate: Date, now: Date, numberOfDaysAfterMonth: number): boolean {
        const pageFillDateNoTimeAndDay: Date = new Date(pageFillDate.getFullYear(), pageFillDate.getMonth(), 1, 0, 0, 0, 0);
        const nowNoTimeAndDay: Date = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0, 0);
        const dateToCompare: Date = new Date(pageFillDateNoTimeAndDay.getFullYear(), pageFillDateNoTimeAndDay.getMonth() + 1, 1, 0, 0, 0); // Добавяме 1 месец към датата, за да сравним дали е страница от предишния месец

        if (dateToCompare.getTime() < nowNoTimeAndDay.getTime()) {
            return true;
        }

        const today: number = now.getDate();

        if (dateToCompare.getTime() === nowNoTimeAndDay.getTime() && today >= numberOfDaysAfterMonth) {
            return true;
        }

        return false;
    }

    public static convertDateDifferenceToHours(difference: DateDifference): number {
        return (difference.days! * 24) + difference.hours! + (difference.minutes! / 60);
    }

    public static checkIfPageDateIsUnlocked(
        logBookPageEditExceptions: LogBookPageEditExceptionDTO[],
        currentUserId: number,
        logBookTypeId: number,
        logBookId: number,
        date: Date,
        now: Date
    ): boolean {
        if (logBookPageEditExceptions.some
            (x =>
                x.exceptionActiveFrom!.getTime() <= now.getTime()
                && x.exceptionActiveTo!.getTime() > now.getTime()

                && x.editPageFrom!.getTime() <= date.getTime()
                && x.editPageTo!.getTime() > date.getTime()

                && (
                    (x.userId === currentUserId
                        && (x.logBookTypeId === logBookTypeId || x.logBookTypeId === null || x.logBookTypeId === undefined)
                        && (x.logBookId === logBookId || x.logBookId === null || x.logBookId === undefined))

                    || ((x.userId === null || x.userId === undefined)
                        && x.logBookTypeId === logBookTypeId
                        && (x.logBookId === logBookId || x.logBookId === null || x.logBookId === undefined))

                    || ((x.userId === null || x.userId === undefined)
                        && (x.logBookTypeId === null || x.logBookTypeId === undefined)
                        && x.logBookId === logBookId)
                )
            )
        ) { // този дневник го има в изключение за избраната дата за потребител и/или тип дневник, и/или дневник
            return true;
        }
        else {
            return false;
        }
    }
}