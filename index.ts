import {GetVideoFromHls} from './application/getVideoFromHls.useCase.ts';
import {PuppeteerWebTracerService} from './infrastructure/services/puppeteerWebTracerService.ts';
import {FfmpegVideoService} from './infrastructure/services/ffmpegVideoService.ts';

const useCase = new GetVideoFromHls(new PuppeteerWebTracerService(), new FfmpegVideoService())
await useCase.execute('https://x.com/wildtiktokss/status/1913226620262826149')