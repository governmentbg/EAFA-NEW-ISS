export class DashboardCardModel {

    public constructor(obj?: Partial<DashboardCardModel>) {
        Object.assign(this, obj);
    }

    public title: string | undefined;
    public icon: string | undefined;
    public content: string | undefined;
}