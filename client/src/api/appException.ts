export class ServerBadRequestError extends Error{
    statusCode: number|string;

    constructor(statusCode: number|string, message: string) {
        super(message);
        this.name = "ServerBadRequestError";
        this.statusCode = statusCode;
    }
    log() {
        console.log(`Http error ${this.statusCode}: ${this.message}`);
    }
}
export class HttpError extends Error {
    statusCode: number;

    constructor(statusCode: number, message: string) {
        super(message);
        this.name = "HttpError";
        this.statusCode = statusCode;
    }

    log() {
        console.log(`Http error ${this.statusCode}: ${this.message}`);
    }
}