import type {FfmpegStreamJson} from './FfmpegStreamJson.ts';

export interface FfmpegHlsJson {
  streams: FfmpegStreamJson[];
  format: {
    filename: string;
    format_name: string;
    duration: string;
    size: string;
    bit_rate: number;
  }
}