import {Proxy} from './Proxy.ts';
import type {Credentials} from 'puppeteer';

export class NullProxy extends Proxy {
  constructor() {
    super({address: '', port: ''});
  }

  public isActive() : boolean {
    return false;
  }

  public isUseCredentials() : boolean {
    return false;
  }

  public isHttp() : boolean {
    return false;
  }

  public getConnectionString() : string {
    return '';
  }

  public getAuthenticateData() : Credentials {
    return {
      username: '',
      password: '',
    };
  }
}