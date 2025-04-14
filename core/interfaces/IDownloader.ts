export interface IDownloader {
  getVideoHLCUrl(url: string): Promise<string>;
}