import { CommandTypes } from '../enums/command-type.enum';

export class RecordChangedEventArgs<T> {
    private record: T;
    private command: CommandTypes;

    constructor(record: T, command: CommandTypes) {
        this.record = record;
        this.command = command;
    }

    public get Record(): T {
        return this.record;
    }

    public get Command(): CommandTypes {
        return this.command;
    }
}