import type {PostTypeEnum} from '../../shared/enums/postTypeEnum.ts';
import {Page} from 'puppeteer';

export interface IDownloader {
  getPostType(page: Page): Promise<PostTypeEnum>;
  getVideoHLCUrl(url: string): Promise<string>;
  getGifUrl(url: string): Promise<string>;
}