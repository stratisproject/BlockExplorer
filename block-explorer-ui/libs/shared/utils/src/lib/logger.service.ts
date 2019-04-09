import { Injectable, InjectionToken, Inject } from '@angular/core';

export const ENVIRONMENT = new InjectionToken<string>('ENVIRONMENT');

@Injectable()
export class Log {
    public static Logger: Log = new Log('dev');

    static log(...args: any[]) {
        Log.Logger.log(...args);
    }

    static warn(...args: any[]) {
        Log.Logger.warn(...args);
    }

    static info(...args: any[]) {
        Log.Logger.info(...args);
    }

    static debug(...args: any[]) {
        Log.Logger.debug(...args);
    }

    static error(error: Error) {
        Log.Logger.error(error);
    }

    constructor(@Inject(ENVIRONMENT) private env: string) { }

    log(...args: any[]) {
        if (this.env !== 'prod') {
            console.log(this.getLogPrefix(), 'background: #444; color: #fff', ...args);
        }
    }

    warn(...args: any[]) {
        if (this.env !== 'prod') {
            args.push();
            console.warn(this.getLogPrefix(), 'background: #444; color: #fff', ...args);
        }
    }

    info(...args: any[]) {
        if (this.env !== 'prod') {
            args.push();
            // tslint:disable-next-line:no-console
            console.info(this.getLogPrefix(), 'background: #444; color: #fff', ...args);
        }
    }

    debug(...args: any[]) {
        if (this.env !== 'prod') {
            // tslint:disable-next-line:no-console
            console.debug(this.getLogPrefix(), 'background: #444; color: #fff', ...args);
        }
    }

    error(...args: any[]) {
        if (this.env !== 'prod') {
            console.error(this.getLogPrefix(), 'background: #444; color: #fff', ...args);
        }
    }

    private getDateStamp(): string {
        return new Date().toTimeString();
    }

    private getLogPrefix(): string {
        return `%c [${this.getDateStamp()}] Breeze wallet:`;
    }
}
