import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { AquacultureInstallationDTO } from '@app/models/generated/dtos/AquacultureInstallationDTO';

export class EditAquacultureInstallationDialogParams {
    public service!: IAquacultureFacilitiesService;
    public model: AquacultureInstallationDTO | undefined;
    public isReadOnly: boolean = false;
    public isDraft: boolean = false;

    public constructor(
        service: IAquacultureFacilitiesService,
        model: AquacultureInstallationDTO | undefined,
        isReadOnly: boolean = false,
        isDraft: boolean = false
    ) {
        this.service = service;
        this.model = model;
        this.isReadOnly = isReadOnly;
        this.isDraft = isDraft;
    }
}