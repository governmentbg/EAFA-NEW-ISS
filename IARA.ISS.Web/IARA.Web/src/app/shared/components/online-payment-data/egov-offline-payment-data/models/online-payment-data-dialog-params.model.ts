export class OnlinePaymentDataDialogParams {
    public applicationId!: number;
    public showTariffs: boolean = false;

    public constructor(obj?: Partial<OnlinePaymentDataDialogParams>) {
        Object.assign(this, obj);
    }

}