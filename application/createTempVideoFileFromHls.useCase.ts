import type {IVideoService} from '../core/interfaces/IVideoService.ts';
import type {IWebTracerService} from '../core/interfaces/IWebTracerService.ts';
import puppeteer from 'puppeteer';

export class CreateTempVideoFileFromHlsUseCase {
  constructor(
    private readonly webTracerService: IWebTracerService,
    private readonly videoService: IVideoService,
  ) {}

  async execute(url: string) {
    const tempFilePath = `${crypto.randomUUID()}.mp4`;
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    const hlsMapUrl = await this.webTracerService.getHlsMapUrl(url, page)
    await this.videoService.saveVideoToFolder(hlsMapUrl, tempFilePath);
    const metadata = await this.videoService.getVideoMetadata(tempFilePath);

    return {
      filePath: tempFilePath,
      metadata,
    }
  }
}