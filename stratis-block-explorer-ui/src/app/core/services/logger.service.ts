import { Injectable, InjectionToken } from '@angular/core';
import { environment } from '@app/../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LogService {
  private env: string;

  constructor() {
    this.env = environment.production ? 'prod' : 'dev';
  }

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
    return `%c [${this.getDateStamp()}] Block Explorer:`;
  }
}
