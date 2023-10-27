export class HomePageVideoModel {
    public title: string = '';
    public url: string = '';
    public tooltipText: string = '';

    public constructor(obj?: Partial<HomePageVideoModel>) {
        Object.assign(this, obj);
    }
}