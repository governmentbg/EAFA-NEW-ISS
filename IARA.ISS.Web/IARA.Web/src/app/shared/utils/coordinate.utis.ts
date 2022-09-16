export class CoordinateUtils {

    public static ConvertFromDMS(dms: string): number {
        const array: string[] = dms.split(' ');
        return CoordinateUtils.ConvertToDecimal(Number.parseInt(array[0]), Number.parseInt(array[1]), Number.parseInt(array[2]));
    }

    public static ConvertToDecimal(degrees: number, minutes: number, seconds: number): number {
        return (degrees + (minutes / 60) + (seconds / 3600));
    }

    public static ConvertToDMS(decimaDegrees: number): string {
        const degrees: number = Math.floor(decimaDegrees);
        const minutes: number = Math.floor((decimaDegrees - degrees) * 60);
        const seconds: number = Number(((decimaDegrees - degrees - (minutes / 60)) * 3600).toPrecision(2));

        return `${degrees} ${minutes} ${seconds}`;
    }
}