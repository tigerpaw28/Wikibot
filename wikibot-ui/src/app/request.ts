export class Request {
    id: number;
    statusName: string;
    comment: string;
    requestingUsername: string;
    submittedDateUTC: Date;
    timePreStartedUTC: Date;
    timeStartedUTC: Date;
    timePreFinishedUTC: Date;
    timeFinishedUTC: Date;
    rawRequest: string;
    notes: string;
    diffs: string[];
}