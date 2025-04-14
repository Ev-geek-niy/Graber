import type {IAssembler} from '../../core/interfaces/IAssembler.ts';
import { $ } from 'bun';

export class FfmpegAssembler implements IAssembler {
  constructor() {}

  async AssembleVideoByHLCUrl(url: string, outputPath: string, filename: string): Promise<void> {
    try {
      const proc = Bun.spawn(['ffmpeg', '-i', url, '-c', 'copy', `${filename}.mp4`]);
      await proc.exited
    }
    catch (error: any) {
      console.log(`Failed with code ${error.exitCode}`);
      console.log(error.stdout.toString());
      console.log(error.stderr.toString());
    }
  }
}
