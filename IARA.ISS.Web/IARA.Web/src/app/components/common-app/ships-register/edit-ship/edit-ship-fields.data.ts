import { ShipEventTypeEnum } from '@app/enums/ship-event-type.enum';

export const ENABLE_FIELD_CONTROLS = new Map<ShipEventTypeEnum, string[]>([
    [
        ShipEventTypeEnum.EDIT, [
            'forbiddenForRSRControl', 'forbiddenReasonControl', 'forbiddenStartDateControl', 'forbiddenEndDateControl',
            'regLicenceNumControl', 'regLicenceDateControl', 'regLicencePublisherControl', 'regLicencePublishVolumeControl',
            'regLicencePublishNumControl', 'regLicencePublishPageControl', 'adminDecisionNumControl', 'adminDecisionDateControl',
            'stayPortControl', 'sailAreaControl', 'mainEngineNumControl', 'fuelTypeControl', 'totalPassengerCapacityControl',
            'controlCardNumControl', 'controlCardDateControl', 'controlCardValidityCertificateNumControl', 'controlCardValidityCertificateDateControl',
            'controlCardDateOfLastAttestationControl', 'foodLawLicenceNumControl', 'foodLawLicenceDateControl', 'shipAssociationControl', 'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.CST, [
            'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.CEN, [
            'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.CHA, [
            'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.IMP, [
            'importCountryControl', 'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.MOD, [
            'shipNameControl', 'vesselTypeControl', 'externalMarkControl', 'regDateControl',
            'ircsCallSignControl', 'mmsiControl', 'uviControl', 'aisControl', 'ersControl', 'vmsControl', 'regNumberControl',
            'exploitationStartDateControl', 'buildYearControl', 'buildPlaceControl', 'publicAidCodeControl', 'portControl',
            'totalLengthControl', 'totalWidthControl', 'grossTonnageControl', 'netTonnageControl', 'otherTonnageControl',
            'mainEnginePowerControl', 'auxiliaryEnginePowerControl', 'mainEngineModelControl', 'mainFishingGearControl',
            'additionalFishingGearControl', 'boardHeightControl', 'draughtControl', 'lengthBetweenPerpendicularsControl',
            'hullMaterialControl', 'crewCountControl', 'fleetSegmentControl', 'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.EXP, [
            'exportCountryControl', 'exportTypeControl', 'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.RET, [
            'cancellationReasonControl', 'cancellationOrderNumControl', 'cancellationOrderDateControl', 'filesControl', 'commentsControl'
        ]
    ],
    [
        ShipEventTypeEnum.DES, [
            'destructionTypeControl', 'filesControl', 'commentsControl'
        ]
    ]
]);

export const DETAIL_FIELDS: string[] = [
    'forbiddenReasonControl', 'forbiddenStartDateControl', 'forbiddenEndDateControl'
];

export const ENABLE_OWNERS_EDIT = new Map<ShipEventTypeEnum, boolean>([
    [ShipEventTypeEnum.EDIT, false],
    [ShipEventTypeEnum.CST, false],
    [ShipEventTypeEnum.CEN, false],
    [ShipEventTypeEnum.CHA, false],
    [ShipEventTypeEnum.IMP, false],
    [ShipEventTypeEnum.MOD, true],
    [ShipEventTypeEnum.EXP, false],
    [ShipEventTypeEnum.RET, false],
    [ShipEventTypeEnum.DES, false]
]);