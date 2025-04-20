import path from 'node:path';
import type {IVideoService} from '../core/interfaces/IVideoService.ts';
import type {IWebTracerService} from '../core/interfaces/IWebTracerService.ts';
import puppeteer from 'puppeteer';

export class CreateTempVideoFileFromHlsUseCase {
  constructor(
    private readonly webTracerService: IWebTracerService,
    private readonly videoService: IVideoService,
  ) {}

  async execute(url: string) {

    const filename = `${crypto.randomUUID()}.mp4`;
    const tempFolderPath = `./temp/`;
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    const hlsMapUrl = await this.webTracerService.getHlsMapUrl(url, page)
    const outputPath = await this.videoService.saveVideoToFolder(hlsMapUrl, filename, tempFolderPath);
    const metadata = await this.videoService.getVideoMetadata(outputPath);

    return {
      filePath: path.join(tempFolderPath, filename),
      metadata,
    }
  }
}