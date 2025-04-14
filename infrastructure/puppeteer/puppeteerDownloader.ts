import type {IDownloader} from '../../core/interfaces/IDownloader.ts';
import puppeteer, {ProtocolError, TimeoutError} from 'puppeteer';
import {InvalidUrlException} from '../../shared/errors/InvalidUrlException.ts';

export class PuppeteerDownloader implements IDownloader {
  private readonly timeout: number = 30000
  constructor() {}

  async getVideoHLCUrl(url: string): Promise<string> {
    if (!url.includes('x.com') && !url.includes('status'))
      throw new InvalidUrlException('The specified URL is not a link to x.com');

    const browser = await puppeteer.launch({timeout: this.timeout});
    const page = await browser.newPage();

    try {
      await page.goto(url);

      const videoResponse = await page.waitForResponse(response =>
        response.url().includes('.m3u8') && response.status() === 200
      );
      return videoResponse.url();
    }
    catch (err){
      if (err instanceof ProtocolError) {
        err.message = 'Incorrect URL is specified';
      }
      if (err instanceof TimeoutError) {
        err.message = `Couldn't receive HLS request for ${this.timeout}ms}`
      }
      throw err;
    }
    finally {
      await browser.close();
    }
  }
}
