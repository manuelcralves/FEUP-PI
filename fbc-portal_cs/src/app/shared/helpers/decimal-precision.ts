export class DecimalPrecision {
    private static powers: number[] = [1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19, 1e20, 1e21, 1e22]

    private static intpow10(power: number): number {
        if (power < 0 || power > 22) {
            return Math.pow(10, power);
        }
        return this.powers[power];
    };

    private static isRound(num: number, decimalPlaces: number): boolean {
        let p = this.intpow10(decimalPlaces);
        return Math.round(num * p) / p === num;
    };

    private static decimalAdjust(type: string, num: number, decimalPlaces: number): number {
        if (type !== 'round' && this.isRound(num, decimalPlaces || 0))
            return num;
        let p = this.intpow10(decimalPlaces || 0);
        let n = (num * p) * (1 + (Number.EPSILON ?? Math.pow(2, -52)));
        return Math[type](n) / p;
    };

    public static round(num: number, decimalPlaces: number): number {
        return this.decimalAdjust('round', num, decimalPlaces);
    };

    public static ceil(num: number, decimalPlaces: number): number {
        return this.decimalAdjust('ceil', num, decimalPlaces);
    };

    public static floor(num: number, decimalPlaces: number): number {
        return this.decimalAdjust('floor', num, decimalPlaces);
    };

    public static trunc(num: number, decimalPlaces: number): number {
        return this.decimalAdjust('trunc', num, decimalPlaces);
    };

    public static toFixed(num: number, decimalPlaces: number): string {
        return this.decimalAdjust('round', num, decimalPlaces).toFixed(decimalPlaces);
    };
};
