export enum ShipNomenclatureChangeFlags {
    None = 0,
    Name = 1 << 0,
    Cfr = 1 << 1,
    ExternalMark = 1 << 2,
    TotalLength = 1 << 3,
    GrossTonnage = 1 << 4,
    MainEnginePower = 1 << 5
}