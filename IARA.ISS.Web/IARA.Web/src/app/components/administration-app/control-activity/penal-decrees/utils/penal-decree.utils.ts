import { AuanInspectedEntityDTO } from '@app/models/generated/dtos/AuanInspectedEntityDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

export class PenalDecreeUtils {
    public static getInspectedEntityName(data: AuanInspectedEntityDTO | undefined): string | undefined {
        let inspectedEntityName: string = '';

        if (data !== undefined && data !== null) {
            if (data.isPerson) {
                if (data.person !== undefined && data.person !== null) {
                    if (data.person.firstName !== undefined && data.person.firstName !== null) {
                        inspectedEntityName += `${data.person.firstName}`;
                    }

                    if (data.person.middleName !== undefined && data.person.middleName !== null) {
                        inspectedEntityName += ` ${data.person.middleName}`;
                    }

                    if (data.person.lastName !== undefined && data.person.lastName !== null) {
                        inspectedEntityName += ` ${data.person.lastName}`;
                    }
                }
            }
            else {
                if (data.legal !== undefined && data.legal !== null && data.legal.name !== undefined && data.legal.name !== null) {
                    inspectedEntityName = `"${data.legal.name}"`;
                }
            }
        }

        return inspectedEntityName;
    }

    public static getViolatedRegulationsTitle(inspectedEntityName: string | undefined, translate: FuseTranslationLoaderService): string {
        const violatedRegulationsText: string = translate.getValue('penal-decrees.edit-violated-regulations');
        const violatedRegulationsText2: string = translate.getValue('penal-decrees.edit-violated-regulations-by-inspected-entity');

        if (inspectedEntityName !== undefined && inspectedEntityName !== null && inspectedEntityName.length > 0) {
            return `${violatedRegulationsText}, ${violatedRegulationsText2} ${inspectedEntityName}`
        }

        return violatedRegulationsText;
    }
}