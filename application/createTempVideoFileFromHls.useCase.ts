import path from 'node:path';
import type {IVideoService} from '../core/interfaces/IVideoService.ts';
import type {IWebTracerService} from '../core/interfaces/IWebTracerService.ts';
import puppeteer from 'puppeteer';
import type {Proxy} from '../core/models/Proxy.ts';

export class CreateTempVideoFileFromHlsUseCase {
  constructor(
    private readonly webTracerService: IWebTracerService,
    private readonly videoService: IVideoService,
    private readonly proxy: Proxy = new NullProxy()
  ) {}

  async execute(url: string) {
    const filename = `${crypto.randomUUID()}.mp4`;
    const tempFolderPath = `./temp/`;
    const browser = await puppeteer.launch({
      executablePath: process.env.PUPPETEER_EXECUTABLE_PATH,
      headless: true,
      args: [
        '--no-sandbox',
        '--disable-setuid-sandbox',
        this.proxy.isActive()
          ? `--proxy-server=${this.proxy.getConnectionString(true)}`
          : ''
      ]
    });
    const page = await browser.newPage();

    if (this.proxy.isActive() && this.proxy.isUseCredentials()){
      await page.authenticate(this.proxy.getAuthenticateData())
    }

    const hlsMapUrl = await this.webTracerService.getHlsMapUrl(url, page)
    const outputPath = await this.videoService.saveVideoToFolder(hlsMapUrl, filename, tempFolderPath);
    const metadata = await this.videoService.getVideoMetadata(outputPath);

    return {
      filePath: path.join(tempFolderPath, filename),
      metadata,
    }
  }
}