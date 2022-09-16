const BASIC_TYPES = new Map<(new (...args: any) => any), string>([
    [Number, 'number'],
    [String, 'string'],
    [Boolean, 'boolean']
]);

export function StrictlyTyped(type: new (...args: any) => any): Function {
    return function (target: object, property: string): void {
        const newProperty: string = `${property[0].toUpperCase()}${property.substr(1)}`;

        Object.defineProperty(target, newProperty, {
            writable: true,

        });
        Object.defineProperty(target, property, {
            get: function (): any {
                return this[newProperty];
            },
            set: function (newValue: any): void {
                if (newValue !== null && newValue !== undefined) {
                    if (Array.isArray(newValue)) {
                        this[newProperty] = [];
                        for (const el of newValue) {
                            if (typeof el !== BASIC_TYPES.get(type) && !(el instanceof type)) {
                                this[newProperty].push(new type(el));
                            }
                            else {
                                this[newProperty].push(el);
                            }
                        }
                    }
                    else {
                        if (typeof newValue !== BASIC_TYPES.get(type) && !(newValue instanceof type)) {
                            this[newProperty] = new type(newValue);
                        }
                        else {
                            this[newProperty] = newValue;
                        }
                    }
                }
                else {
                    this[newProperty] = newValue;
                }
            }
        });
    }
}