import { $ } from 'bun';
import type {IAssembler} from '../../core/interfaces/IAssembler.ts';
import {basePath} from '../../shared/helpers/pathHelper.ts';

export class FfmpegAssembler implements IAssembler {
  constructor() {}

  async AssembleVideoByHLCUrl(url: string, outputPath: string = basePath, filename: string = crypto.randomUUID()): Promise<void> {
      await $`ffmpeg -i ${url} -c copy ${outputPath}/downloads/${filename}.mp4`
  }
}
