import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { Message } from '@app/models/common/message.model';

@Injectable({
    providedIn: 'root'
})
export class MessageService {
    private _subject = new BehaviorSubject<Message | null>(null);

    public sendMessage(message: string): void {
        this._subject.next(new Message(message));
    }

    public cleanMessage(): void {
        this._subject.next(new Message(''));
    }

    public getMessage(): Observable<Message | null> {
        return this._subject.asObservable();
    }

    public getMessageCurrentValue(): Message | null {
        return this._subject.value;
    }
}