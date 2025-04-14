import type {IDownloader} from '../../core/interfaces/IDownloader.ts';
import puppeteer from 'puppeteer';

export class PuppeteerDownloader implements IDownloader {
  private readonly videoUrlRegex = new RegExp(/.*.(m3u8)\??.*/);
  constructor() {}

  async getVideoHLCUrl(url: string): Promise<string> {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    await page.goto(url);
    const videoResponse = await page.waitForResponse(response =>
      response.url().includes('.m3u8') && response.status() === 200
    );
    await browser.close();

    return videoResponse.url();
  }
}
