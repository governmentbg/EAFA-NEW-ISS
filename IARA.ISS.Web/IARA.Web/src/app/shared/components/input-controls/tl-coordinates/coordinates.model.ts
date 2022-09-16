
export class Coordinates {
    constructor(degrees: string, minutes: string, seconds: string) {
        this.degrees = degrees;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public degrees: string;
    public minutes: string;
    public seconds: string;

    public isEmpty(): boolean {
        return this.degrees == '' && this.minutes == '' && this.seconds == '';
    }

    public equals(coordinates: Coordinates | undefined): boolean {
        if (coordinates != undefined) {
            return this.degrees == coordinates.degrees
                && this.minutes == coordinates.minutes
                && this.seconds == coordinates.seconds;
        } else {
            return false;
        }
    }

    public toString(): string {
        return `${this.degrees} ${this.minutes} ${this.seconds}`;
    }
}