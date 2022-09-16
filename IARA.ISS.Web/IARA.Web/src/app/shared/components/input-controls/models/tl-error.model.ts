export class TLError {
    public text: string = '';
    public type: 'error' | 'warn' = 'error';

    constructor(obj?: Partial<TLError>) {
        Object.assign(this, obj);
    }
}