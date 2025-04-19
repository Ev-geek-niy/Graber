import {FileStreamHelper} from '../../shared/helpers/fileStreamHelper.ts';
import type {IVideoService} from '../../core/interfaces/IVideoService.ts';
import type {Video} from '../../core/types/Video.ts';
import type {VideoMetadata} from '../../core/types/VideoMetadata.ts';
import {log, logError} from '../../shared/logger.ts';
import type {FfmpegHlsJson} from '../../core/types/FfmpegHlsJson.ts';
import {TwitterCodecTypesEnum} from '../../core/enums/twitterCodecTypesEnum.ts';

export class FfmpegVideoService implements IVideoService {
  constructor() {
  }

  async getVideoMetadata(filePath: string): Promise<VideoMetadata> {
    log(`Получение мета-данных из HLS-потока: ${filePath}`)
    try {
      const {stdout} = Bun.spawn([
        'ffprobe',
        '-show_streams',
        '-show_format',
        '-print_format', 'json',
        filePath
      ]);

      const output_json_str = await new Response(stdout).text();
      const json = JSON.parse(output_json_str) as FfmpegHlsJson;
      log('Попытка найти данные о видео-потоке')
      const videoMetaData = json.streams.find(stream => {
        log(`Текущий поток: ${stream.index}, ${stream.codec_name}`)
        return stream.codec_type === TwitterCodecTypesEnum.Video
      })
      if (!videoMetaData) throw new Error('Не удалось получить мета-данные о видео-потоке')
      return {
        format: videoMetaData.codec_type,
        height: videoMetaData.height,
        width: videoMetaData.width,
        duration: json.format.duration
      }
    }
    catch(err) {
      const error = err instanceof Error ? err : new Error('Неизвестная ошибка при получении мета-данных');
      logError(`Ошибка при получении мета-данных из HLS-потока: ${error.message}`);
      throw error;
    }
  }

  async saveVideoToFolder(filePath: string, outputPath: string): Promise<void> {
    const proc = Bun.spawn([
      'ffmpeg',
      '-i', filePath,
      '-vcodec', 'libx264',
      '-crf', '24',
      '-preset', 'veryfast',
      '-acodec', 'aac',
      '-b:a', '128k',
      '-movflags', '+faststart',
      '-vf', 'scale=-2:480',
      outputPath
    ], {
      stderr: 'inherit'
    })

    const exitCode = await proc.exited
    if (exitCode !== 0) throw new Error(`ffmpeg exited with code ${exitCode}`)
  }

  async getVideoBuffer(url: string): Promise<Uint8Array[]> {
    const {stdout} = Bun.spawn([
      'ffmpeg',
      '-i', url,
      '-vcodec', 'libx264',
      '-pix_fmt', 'yuv420p',
      '-crf', '24',
      '-preset', 'medium',
      '-acodec', 'aac',
      '-b:a', '128k',
      '-movflags', 'frag_keyframe+empty_moov',
      '-f', 'mp4',
      'pipe:1'
    ], {
      stdout: 'pipe',
      stderr: 'inherit'
    })

    return await FileStreamHelper.readAllStream(stdout);
  }

  async prepareVideo(url: string): Promise<Video> {
    const metadata = await this.getVideoMetadata(url);
    const videoBuffer = await this.getVideoBuffer(url)

    return {
      Buffer: videoBuffer,
      metadata: metadata
    }
  }
}
