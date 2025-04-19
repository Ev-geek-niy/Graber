import type {IWebTracerService} from '../../core/interfaces/IWebTracerService.ts';
import type {Page} from 'puppeteer';
import {log, logError} from '../../shared/logger.ts';

export class PuppeteerWebTracerService implements IWebTracerService {
  async getHlsMapUrl(url: string, page: Page, timeout: number = 60000): Promise<string> {
    log(`Попытка получения HLS-потока со страницы: ${page.url()}`)
    try {
      await page.goto(url);

      const videoResponse = await page.waitForResponse(response => {
        log(`Получен ответ: ${response.url()}`);
        return response.url().includes('.m3u8') && response.status() === 200
      }, {timeout: timeout});

      return videoResponse.url();
    } catch (error) {
      if (error instanceof Error) {
        logError(`Ошибка при перехвате HLS-потока: ${error.message}`)
        throw error;
      }
    }
  }
}