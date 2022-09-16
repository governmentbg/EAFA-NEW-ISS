import { GenderEnum } from '@app/enums/gender.enum';

export class EgnUtils {
    private static WEIGHTS: number[] = [2, 4, 8, 5, 10, 9, 7, 3, 6];

    public static isEgnValid(value: string): boolean {
        const isDateValid = (year: number, month: number, day: number): boolean => {
            const date: Date = new Date(year, month, day);
            return !isNaN(date.getTime()) && date.getFullYear() === year && date.getMonth() === month && date.getDate() === day;
        };

        if (![...value].every(x => x >= '0' && x <= '9')) {
            return false;
        }

        let year: number = Number(value.substr(0, 2));
        let month: number = Number(value.substr(2, 2));
        const day: number = Number(value.substr(4, 2));

        if (month > 40) {
            year += 2000;
            month -= 41;

            if (!isDateValid(year, month, day)) {
                return false;
            }
        }
        else if (month > 20) {
            year += 1800;
            month -= 21;

            if (!isDateValid(year, month, day)) {
                return false;
            }
        }
        else {
            year += 1900;
            month -= 1;

            if (!isDateValid(year, month, day)) {
                return false;
            }
        }

        let sum: number = 0;
        for (let i = 0; i < 9; ++i) {
            sum += Number(value[i]) * EgnUtils.WEIGHTS[i];
        }

        let validChecksum: number = sum % 11;
        if (validChecksum === 10) {
            validChecksum = 0;
        }

        const checksum: number = Number(value.substr(9, 1));
        return checksum === validChecksum;
    }

    public static getDateOfBirthFromEgn(egn: string): Date {
        const year: number = Number(egn.substr(0, 2));
        const month: number = Number(egn.substr(2, 2));
        const day: number = Number(egn.substr(4, 2));

        if (month > 40) {
            return new Date(year + 2000, month - 40 - 1, day);
        }
        else if (month > 20) {
            return new Date(year + 1800, month - 20 - 1, day);
        }

        return new Date(year + 1900, month - 1, day);
    }

    public static getPersonSexFromEgn(egn: string): GenderEnum {
        const result: number = Number(egn.substr(8, 1)) % 2;
        return result === 0 ? GenderEnum.M : GenderEnum.F;
    }
}