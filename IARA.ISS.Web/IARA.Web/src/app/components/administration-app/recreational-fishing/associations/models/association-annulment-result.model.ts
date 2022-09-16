export class AssociationAnnulmentResult {
    public canceled: boolean;
    public date: Date;
    public reason: string;

    public constructor(canceled: boolean, date: Date, reason: string) {
        this.canceled = canceled;
        this.date = date;
        this.reason = reason;
    }
}