import type {IProxySettings} from '../interfaces/IProxySettings.ts';
import type {Credentials} from 'puppeteer';

export class Proxy {
  private readonly protocol?: string;
  public readonly address: string;
  public readonly port: string;
  private readonly username?: string;
  private readonly password?: string;

  constructor(settings: IProxySettings) {
    this.protocol = settings.protocol ?? 'http';
    this.address = settings.address;
    this.port = settings.port;
    this.username = settings.username;
    this.password = settings.password;
  }

  public getConnectionString(withoutCredentials: boolean = false) {
    return this.username && this.password && !withoutCredentials
      ? `${this.protocol}://${this.username}:${this.password}@${this.address}:${this.port}`
      : `${this.protocol}://${this.address}:${this.port}`;
  }

  public getAuthenticateData(): Credentials {
    if (!this.username || !this.password) {
      throw new Error('No username or password provided');
    }

    return {
      username: this.username,
      password: this.password,
    }
  }

  public isActive() {
    return this.address && this.port;
  }

  public isUseCredentials() {
    return this.username && this.password;
  }

  public isHttp() {
    if (!this.isActive())
      return false;

    return this.protocol === 'http';
  }
}