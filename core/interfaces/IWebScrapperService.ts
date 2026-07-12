export interface IWebScrapperService {
  getVideoUrl(document: Document): Promise<string>;
  getGifUrl(document: Document): Promise<string>;
}