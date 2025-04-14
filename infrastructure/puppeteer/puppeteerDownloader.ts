import type {IDownloader} from '../../core/interfaces/IDownloader.ts';
import puppeteer, {Page, ProtocolError, TimeoutError} from 'puppeteer';
import {InvalidUrlException} from '../../shared/errors/InvalidUrlException.ts';
import {PostTypeEnum} from '../../shared/enums/postTypeEnum.ts';

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

      const postType = await this.getPostType(page);
      console.log("Type of post is", postType);

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

  async getPostType(page: Page): Promise<PostTypeEnum> {
    const html = await page.waitForSelector('[data-testid="videoComponent"]', {timeout: 10000})
    const type = await html?.evaluate(() => {
      enum PostTypeEnum {
        Unknown = 'unknown',
        Video = 'video',
        Gif = 'gif',
      }

      return document.body.querySelector('video')?.src
        ? PostTypeEnum.Gif
        : PostTypeEnum.Video
    })

    return type ?? PostTypeEnum.Unknown
  }
}
