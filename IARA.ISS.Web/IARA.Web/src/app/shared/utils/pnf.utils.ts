export class PnfUtils {
    private static WEIGHTS: number[] = [21, 19, 17, 13, 11, 9, 7, 3, 1];

    public static isPnfValid(value: string): boolean {
        if (![...value].every(x => x >= '0' && x <= '9')) {
            return false;
        }

        let sum: number = 0;
        for (let i = 0; i < 9; ++i) {
            sum += Number(value[i]) * this.WEIGHTS[i];
        }

        const checksum: number = Number(value.substr(9, 1));
        return checksum === sum % 10;
    }
}