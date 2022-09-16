export class DateTime extends Date {
    public toJSON(): string {
        this.setHours(this.getHours() + this.getTimezoneOffset());
        const result: string = super.toJSON();
        this.setHours(this.getHours() - this.getTimezoneOffset());
        return result;
    }
}