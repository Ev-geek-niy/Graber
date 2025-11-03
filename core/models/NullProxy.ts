import {Proxy} from './Proxy.ts';

export class NullProxy extends Proxy {
  constructor() {
    super({address: '', port: ''});
  }

  public isActive() {
    return false;
  }

  public isUseCredentials() {
    return false;
  }

  public isHttp() {
    return false;
  }

  public getConnectionString() {
    return '';
  }

  public getAuthenticateData() {
    return {
      username: '',
      password: '',
    };
  }
}