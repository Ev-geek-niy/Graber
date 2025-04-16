import type {IDownloader} from '../../core/interfaces/IDownloader.ts';
import type {IAssembler} from '../../core/interfaces/IAssembler.ts';

export class getVideoBufferFromUrlUseCase {
  constructor(
    private readonly downloader: IDownloader,
    private readonly assembler: IAssembler,
  ) {
  }

  async execute(url: string): Promise<Uint8Array[]> {
    console.log('Find the HLS url for video...')
    const videoUrl = await this.downloader.getVideoHLCUrl(url)
    console.log(`Start assemble video: ${videoUrl}`);
    const videoBuffer = await this.assembler.getVideoBuffer(videoUrl)
    console.log('Done!')
    return videoBuffer;
  }
}