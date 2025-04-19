import type {TwitterCodecTypesEnum} from '../enums/twitterCodecTypesEnum.ts';

export interface FfmpegStreamJson {
  index: number
  codec_name: string
  codec_long_name: string
  profile: string,
  codec_type: TwitterCodecTypesEnum,
  width: number,
  height: number,
  coded_width: number,
  coded_height: number,
  tags: object
}