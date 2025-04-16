import type {IAssembler} from '../../core/interfaces/IAssembler.ts';
import {FileStreamHelper} from '../../shared/helpers/fileStreamHelper.ts';

export class FfmpegAssembler implements IAssembler {
  constructor() {
  }

  async getVideoBuffer(url: string): Promise<Uint8Array[]> {
    const {stdout} = Bun.spawn([
      'ffmpeg',
      '-i', url,
      '-f', 'mp4',
      '-movflags', 'frag_keyframe+empty_moov',
      'pipe:1'
    ], {
      stdout: 'pipe',
      stderr: 'inherit'
    })

    return await FileStreamHelper.readAllStream(stdout);
  }
}
