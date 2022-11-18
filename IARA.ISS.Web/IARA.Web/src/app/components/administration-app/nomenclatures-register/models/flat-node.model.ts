export class FlatNode {
    public id: number | undefined;
    public level: number = 0;
    public name: string = '';
    public expandable: boolean = false;

    public constructor(init?: Partial<FlatNode>) {
        Object.assign(this, init);
    }
}