import { Component, OnInit } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { MessageService } from '@app/shared/services/message.service';
import { BasePageComponent } from '@app/components/common-app/base-page.component';

@Component({
    selector: 'recreational-fishing-association-tickets',
    templateUrl: './recreational-fishing-association-tickets.component.html'
})
export class RecreationalFishingAssociationTicketsComponent extends BasePageComponent implements OnInit {
    public service: RecreationalFishingPublicService;
    public showAssociationPicker: boolean = true;

    private translate: FuseTranslationLoaderService;

    public constructor(
        service: RecreationalFishingPublicService,
        translate: FuseTranslationLoaderService,
        messageService: MessageService
    ) {
        super(messageService);

        this.service = service;
        this.translate = translate;
    }

    public ngOnInit(): void {
        if (this.service.currentUserChosenAssociation !== undefined) {
            this.showAssociationPicker = false;
            this.setTitle();
        }
    }

    public associationChosen(assoc: NomenclatureDTO<number>): void {
        this.service.currentUserChosenAssociation = assoc;
        this.showAssociationPicker = false;

        this.setTitle();
    }

    private setTitle(): void {
        setTimeout(() => {
            this.messageService.sendMessage(`${this.translate.getValue('navigation.association-ticket-issuing')} — ${this.service.currentUserChosenAssociation!.displayName}`);
        });
    }
}