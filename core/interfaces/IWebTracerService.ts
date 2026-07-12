import type {Page} from 'puppeteer';

export interface IWebTracerService {
  /**
   * Получает url-адрес HLS-потока (.m8u3 файл)
   * @param url Url-адрес страницы
   * @param page Загруженная страница в Puppeteer
   * @param timeout Время до сброса ожидания
   * @returns Promise<string> Строка ссылки на HLS-поток
   */
  getHlsMapUrl(url: string, page: Page, timeout?: number): Promise<string>
}