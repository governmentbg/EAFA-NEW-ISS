export class HomePageCardModel {
    public title: string = '';
    public url: string = '';
    public tooltipText: string = '';
    public icon: string = 'fa-play-circle';
    public shouldDownload: boolean = false; 

    public constructor(obj?: Partial<HomePageCardModel>) {
        Object.assign(this, obj);
    }
}