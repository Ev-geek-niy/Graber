import type {Video} from '../core/types/Video.ts';
import type {IVideoService} from '../core/interfaces/IVideoService.ts';
import type {IWebTracerService} from '../core/interfaces/IWebTracerService.ts';
import puppeteer from 'puppeteer';

export class GetVideoFromHls {
  constructor(
    private readonly webTracerService: IWebTracerService,
    private readonly videoService: IVideoService,
  ) {
  }

  async execute(url: string): Promise<Video> {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    const hlsUrl = await this.webTracerService.getHlsMapUrl(url, page);
    return await this.videoService.prepareVideo(hlsUrl)
  }
}