import type {IDownloader} from '../../core/interfaces/IDownloader.ts';
import type {IAssembler} from '../../core/interfaces/IAssembler.ts';

export class DownloadAndAssembleVideoUseCase {
  constructor(
    private readonly downloader: IDownloader,
    private readonly assembler: IAssembler,
  ) { }

  async execute(url: string): Promise<void> {
    console.log('Find the HLS url for video...')
    const videoUrl = await this.downloader.getVideoHLCUrl(url)
    console.log(`Start assemble video: ${videoUrl}`);
    await this.assembler.AssembleVideoByHLCUrl(videoUrl, '../downloads/', 'testVideo')
    console.log('Done!')
  }
}