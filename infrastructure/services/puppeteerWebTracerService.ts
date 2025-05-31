import type {IWebTracerService} from '../../core/interfaces/IWebTracerService.ts';
import {type Page } from 'puppeteer';
import {log, logError} from '../../shared/logger.ts';

export class PuppeteerWebTracerService implements IWebTracerService {
  async getHlsMapUrl(url: string, page: Page, timeout: number = 60000): Promise<string> {
    try {
      await page.goto(url, {waitUntil: 'domcontentloaded'});

      log(`Попытка получения HLS-потока со страницы: ${page.url()}`)

      const videoResponse = await page.waitForResponse(response => {
        log(`Получен ответ: ${response.url()}`);
        return response.url().includes('.m3u8') && response.status() === 200
      }, {timeout: timeout});

      log(`Поток найден: ${videoResponse.url()}`);

      return videoResponse.url();
    } catch (err) {
      const error = err instanceof Error ? err : new Error('Неизвестная ошибка при получении мета-данных');
      logError(`Ошибка при получении HLS-потока: ${error.message}`);
      throw error;
    }
  }
}