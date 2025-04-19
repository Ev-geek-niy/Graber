import type {VideoMetadata} from '../types/VideoMetadata.ts';
import type {Video} from '../types/Video.ts';

export interface IVideoService {
  /**
   * Получение буфера с видео-потоком по указанному пути.
   * @param filePath Путь до файла, Url на .mp4 или .m8u3 файлы.
   * @returns Promise с буфером.
   * @Warning В данный момент лучше не использовать, так как при отправке буфера в телеграм бота
   * видео по какой-то причине не использует стриминг.
   */
  getVideoBuffer(filePath: string): Promise<Uint8Array[]>

  /**
   * Сохранить файл в указанный путь.
   * @param filePath Путь до файла, Url на .mp4 или .m8u3 файлы.
   * @param outputPath Путь к выходной папке формата './{folders}/{file}.mp4'.
   */
  saveVideoToFolder(filePath: string, outputPath: string): Promise<void>

  /**
   * Получение мета-данных из HLS-потока.
   * @param filePath Путь до .m8u3 файла.
   */
  getVideoMetadata(filePath: string): Promise<VideoMetadata>

  /**
   * Подготовить объект видео-файла из HLS-потока.
   * @param filePath Путь до .m8u3 файла.
   */
  prepareVideo(filePath: string): Promise<Video>
}