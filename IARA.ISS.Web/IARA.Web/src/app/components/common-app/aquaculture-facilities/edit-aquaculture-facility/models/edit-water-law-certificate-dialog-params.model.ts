import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { AquacultureWaterLawCertificateDTO } from '@app/models/generated/dtos/AquacultureWaterLawCertificateDTO';

export class EditWaterLawCertificateDialogParams {
    public service!: IAquacultureFacilitiesService;
    public model: AquacultureWaterLawCertificateDTO | undefined;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<EditWaterLawCertificateDialogParams>) {
        Object.assign(this, obj);
    }
}