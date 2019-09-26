import { IReconnectPolicy } from "./IReconnectPolicy";
/** @private */
export declare class DefaultReconnectPolicy implements IReconnectPolicy {
    private readonly retryDelays;
    constructor(retryDelays?: number[]);
    nextRetryDelayInMilliseconds(previousRetryCount: number): number | null;
}
