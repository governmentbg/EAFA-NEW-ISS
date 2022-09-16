import { Component, OnDestroy } from '@angular/core';
import { MessageService } from '@app/shared/services/message.service';

@Component({
    selector: 'base-page',
    template: ''
})
export abstract class BasePageComponent implements OnDestroy {
    protected messageService: MessageService;

    public constructor(messageService: MessageService) {
        this.messageService = messageService;
    }

    public ngOnDestroy(): void {
        this.messageService.cleanMessage();
    }
}