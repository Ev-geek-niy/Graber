import type {PostTypeEnum} from '../../shared/enums/postTypeEnum.ts';

export interface IDownloader {
  getPostType(url: string): Promise<PostTypeEnum>;
  getVideoHLCUrl(url: string): Promise<string>;
  getGifUrl(url: string): Promise<string>;
}