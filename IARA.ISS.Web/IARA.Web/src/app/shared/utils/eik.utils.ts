export class EikUtils {
    private static FIRST_SUM_WEIGHTS9: number[] = [1, 2, 3, 4, 5, 6, 7, 8];
    private static SECOND_SUM_WEIGHTS9: number[] = [3, 4, 5, 6, 7, 8, 9, 10];
    private static FIRST_SUM_WEIGHTS13: number[] = [2, 7, 3, 5];
    private static SECOND_SUM_WEIGHTS13: number[] = [4, 9, 5, 7];

    public static isEikValid(value: string): boolean {
        if (value?.length !== 9 && value?.length !== 13) {
            return false;
        }

        if (![...value].every(x => x >= '0' && x <= '9')) {
            return false;
        }

        return value.length === 9 ? this.isEik9Valid(value) : this.isEik13Valid(value);
    }

    private static isEik9Valid(value: string): boolean {
        const digits: number[] = this.stringToNumbers(value);
        return this.calcCheckSum9(digits) === digits[8];
    }

    private static isEik13Valid(value: string): boolean {
        try {
            const digits: number[] = this.stringToNumbers(value);
            return this.calcCheckSum13(digits) === digits[12];
        }
        catch {
            return false;
        }
    }

    private static calcCheckSum9(digits: number[]): number {
        let sum: number = 0;
        for (let i = 0; i < 8; ++i) {
            sum += digits[i] * this.FIRST_SUM_WEIGHTS9[i];
        }

        const rem: number = sum % 11;
        if (rem !== 10) {
            return rem;
        }

        let secondSum: number = 0;
        for (let i = 0; i < 8; ++i) {
            secondSum += digits[i] * this.SECOND_SUM_WEIGHTS9[i];
        }

        const secondRem: number = secondSum % 11;
        if (secondRem !== 10) {
            return secondRem;
        }

        return 0;
    }

    private static calcCheckSum13(digits: number[]): number {
        const checkSum9: number = this.calcCheckSum9(digits);
        if (checkSum9 !== digits[8]) {
            throw new Error('Invalid check sum for 9th digit in 13-digit EIK');
        }

        let sum: number = 0;
        for (let i = 8, j = 0; j < 4; ++i, ++j) {
            sum += digits[i] * this.FIRST_SUM_WEIGHTS13[j];
        }

        const rem: number = sum % 11;
        if (rem !== 10) {
            return rem;
        }

        let secondSum: number = 0;
        for (let i = 8, j = 0; j < 4; ++i, ++j) {
            secondSum += digits[i] * this.SECOND_SUM_WEIGHTS13[i];
        }

        const secondRem: number = secondSum % 11;
        if (secondRem !== 10) {
            return secondRem;
        }

        return 0;
    }

    private static stringToNumbers(value: string): number[] {
        const result: number[] = [];

        for (const ch of value) {
            result.push(Number(ch));
        }

        return result;
    }
}