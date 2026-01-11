import {Proxy} from '../core/models/Proxy.ts';
import {NullProxy} from '../core/models/NullProxy.ts';
import {PuppeteerWebTracerService} from './services/puppeteerWebTracerService.ts';
import {FfmpegVideoService} from './services/ffmpegVideoService.ts';
import {CreateTempVideoFileFromHlsUseCase} from '../application/createTempVideoFileFromHls.useCase.ts';
import type {IWebTracerService} from '../core/interfaces/IWebTracerService.ts';
import type {IVideoService} from '../core/interfaces/IVideoService.ts';

function createProxyFromEnv(): Proxy {
  if (process.env.PROXY_ADDRESS && process.env.PROXY_PORT) {
    return new Proxy({
      address: process.env.PROXY_ADDRESS,
      port: process.env.PROXY_PORT,
      username: process.env.PROXY_USERNAME,
      password: process.env.PROXY_PASSWORD,
    });
  }
  return new NullProxy();
}

export interface AppServices {
  proxy: Proxy;
  videoService: IVideoService;
  webTracerService: IWebTracerService;
}

export function createServices(): AppServices {
  const proxy = createProxyFromEnv();

  return {
    proxy,
    videoService: new FfmpegVideoService(proxy),
    webTracerService: new PuppeteerWebTracerService(),
  };
}

export function createVideoCreatorUseCase(): CreateTempVideoFileFromHlsUseCase {
  const {webTracerService, videoService, proxy} = createServices();
  return new CreateTempVideoFileFromHlsUseCase(webTracerService, videoService, proxy);
}
